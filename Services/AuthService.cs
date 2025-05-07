using CarteiraDigitalAPI.Data;
using CarteiraDigitalAPI.Models;
using CarteiraDigitalAPI.Configurations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace CarteiraDigitalAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ConfiguracaoJwt _jwtConfig;

        public AuthService(ApplicationDbContext context, IOptions<ConfiguracaoJwt> jwtOptions)
        {
            _context = context;
            _jwtConfig = jwtOptions.Value;
        }

        public string Registrar(Usuario usuario)
        {
            if (_context.Usuarios.Any(u => u.Email == usuario.Email))
                throw new Exception("Email já está em uso.");

            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            var carteira = new Carteira
            {
                UsuarioId = usuario.Id,
                Usuario = usuario,
                Saldo = 0
            };
            _context.Carteiras.Add(carteira);
            _context.SaveChanges();

            return "Usuário registrado com sucesso.";
        }
        public string Login(LoginRequest login)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == login.Email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(login.Senha, usuario.Senha))
                throw new Exception("Credenciais inválidas.");

            var token = GerarToken(usuario);
            return token;
        }

        private string GerarToken(Usuario usuario)
        {
            var chave = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfig.ChaveSecreta));
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nome)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Emissor,
                audience: _jwtConfig.Audiencia,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Usuario ObterUsuarioPorEmail(string email)
        {
            return _context.Usuarios.FirstOrDefault(u => u.Email == email) 
                   ?? throw new Exception("Usuário não encontrado.");
        }
    }
}
