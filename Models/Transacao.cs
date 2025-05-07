namespace CarteiraDigitalAPI.Models
{
    public class Transacao
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; } = DateTime.UtcNow;

        public int CarteiraOrigemId { get; set; }
        public required Carteira CarteiraOrigem { get; set; }

        public int CarteiraDestinoId { get; set; }
        public required Carteira CarteiraDestino { get; set; }

        public Transacao(){ }
    }
}
