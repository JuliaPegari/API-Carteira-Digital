namespace CarteiraDigitalAPI.Configurations
{
    public class ConfiguracaoJwt
    {
        public required string ChaveSecreta { get; set; }
        public required string Emissor { get; set; }
        public required string Audiencia { get; set; }
    }
}