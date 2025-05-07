using CarteiraDigitalAPI.Models;

namespace CarteiraDigitalAPI.Services
{
    public interface IAuthService
    {
        string Registrar(Usuario usuario);
        string Login(LoginRequest login);
        Usuario ObterUsuarioPorEmail(string email);
    }
}