using CarteiraDigitalAPI.Data;
using CarteiraDigitalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CarteiraDigitalAPI.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly ApplicationDbContext _context;

        public TransacaoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void RealizarTransferencia(int usuarioOrigemId, string emailDestino, decimal valor)
        {
            if (valor <= 0) throw new Exception("O valor da transferência deve ser positivo.");

            var carteiraOrigem = _context.Carteiras
                .Include(c => c.Usuario)
                .FirstOrDefault(c => c.UsuarioId == usuarioOrigemId)
                ?? throw new Exception("Carteira de origem não encontrada.");

            var carteiraDestino = _context.Carteiras
                .Include(c => c.Usuario)
                .FirstOrDefault(c => c.Usuario.Email == emailDestino)
                ?? throw new Exception("Carteira de destino não encontrada.");

            if (carteiraOrigem.Saldo < valor)
                throw new Exception("Saldo insuficiente.");

            carteiraOrigem.Saldo -= valor;
            carteiraDestino.Saldo += valor;

            var transacao = new Transacao
            {
                CarteiraOrigemId = carteiraOrigem.Id,
                CarteiraDestinoId = carteiraDestino.Id,
                CarteiraOrigem = carteiraOrigem,
                CarteiraDestino = carteiraDestino,
                Valor = valor,
                Data = DateTime.UtcNow
            };

            _context.Transacoes.Add(transacao);
            _context.SaveChanges();
        }

        public List<Transacao> ListarTransacoes(int usuarioId, DateTime? inicio = null, DateTime? fim = null)
        {
            var carteira = _context.Carteiras.FirstOrDefault(c => c.UsuarioId == usuarioId)
                ?? throw new Exception("Carteira não encontrada.");

            var query = _context.Transacoes
                .Include(t => t.CarteiraDestino).ThenInclude(c => c.Usuario)
                .Include(t => t.CarteiraOrigem).ThenInclude(c => c.Usuario)
                .Where(t => t.CarteiraOrigemId == carteira.Id || t.CarteiraDestinoId == carteira.Id);

            if (inicio.HasValue)
                query = query.Where(t => t.Data >= inicio.Value);

            if (fim.HasValue)
                query = query.Where(t => t.Data <= fim.Value);

            return query.OrderByDescending(t => t.Data).ToList();
        }
    }
}
