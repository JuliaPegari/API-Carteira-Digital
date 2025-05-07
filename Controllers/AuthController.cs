using CarteiraDigitalAPI.Models;
using CarteiraDigitalAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarteiraDigitalAPI.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("registrar")]
        public IActionResult Registrar([FromBody] Usuario usuario)
        {
            try
            {
                var mensagem = _authService.Registrar(usuario);
                return Ok(mensagem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            try
            {
                var token = _authService.Login(login);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult UsuarioLogado()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value; // Pegando o email do token

            if (email == null)
            {
                return Unauthorized("Usuário não encontrado.");
            }

            var usuario = _authService.ObterUsuarioPorEmail(email); // Método no AuthService para buscar o usuário

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Email
            });
        }
    }
}
