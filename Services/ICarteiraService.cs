using CarteiraDigitalAPI.Models;

namespace CarteiraDigitalAPI.Services
{
    public interface ICarteiraService
    {
        decimal ObterSaldo(int usuarioId);
        decimal AdicionarSaldo(int usuarioId, decimal valor);
    }
}