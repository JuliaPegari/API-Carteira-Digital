namespace CarteiraDigitalAPI.Models
{
    public class Carteira
    {
        public int Id { get; set; }
        public decimal Saldo { get; set; } = 0;

        public int UsuarioId { get; set; }
        public required Usuario Usuario { get; set; }

        public ICollection<Transacao> TransferenciasEnviadas { get; set; } = new List<Transacao>();
        public ICollection<Transacao> TransferenciasRecebidas { get; set; } = new List<Transacao>();
    }
}