using CarteiraDigitalAPI.Data;
using CarteiraDigitalAPI.Models;

namespace CarteiraDigitalAPI.Services
{
    public class CarteiraService : ICarteiraService
    {
        private readonly ApplicationDbContext _context;

        public CarteiraService(ApplicationDbContext context)
        {
            _context = context;
        }

        public decimal ObterSaldo(int usuarioId)
        {
            var carteira = _context.Carteiras.FirstOrDefault(c => c.UsuarioId == usuarioId)
                ?? throw new Exception("Carteira não encontrada.");

            return carteira.Saldo;
        }

        public decimal AdicionarSaldo(int usuarioId, decimal valor)
        {
            var carteira = _context.Carteiras.FirstOrDefault(c => c.UsuarioId == usuarioId)
                ?? throw new Exception("Carteira não encontrada.");

            carteira.Saldo += valor;
            _context.SaveChanges();

            return carteira.Saldo;
        }

        public class CarteiraNaoEncontradaException : Exception
        {
            public CarteiraNaoEncontradaException() : base("Carteira não encontrada.") { }
        }
    }
}