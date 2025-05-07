using CarteiraDigitalAPI.Models;

namespace CarteiraDigitalAPI.Services
{
    public interface ITransacaoService
    {
        void RealizarTransferencia(int usuarioOrigemId, string emailDestino, decimal valor);
        List<Transacao> ListarTransacoes(int usuarioId, DateTime? inicio = null, DateTime? fim = null);
    }
}