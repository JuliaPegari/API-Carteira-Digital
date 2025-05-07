using CarteiraDigitalAPI.Models;

namespace CarteiraDigitalAPI.Data
{
    public static class DbSeeder
    {
        public static void Popular(ApplicationDbContext context)
        {
            if (!context.Usuarios.Any())
            {
                var usuario1 = new Usuario { Nome = "alice", Senha = BCrypt.Net.BCrypt.HashPassword("123456") };
                var usuario2 = new Usuario { Nome = "joao", Senha = BCrypt.Net.BCrypt.HashPassword("123456") };

                context.Usuarios.AddRange(usuario1, usuario2);
                context.SaveChanges();

                var carteira1 = new Carteira { UsuarioId = usuario1.Id, Usuario = usuario1, Saldo = 1000 };
                var carteira2 = new Carteira { UsuarioId = usuario2.Id, Usuario = usuario2, Saldo = 500 };

                context.Carteiras.AddRange(carteira1, carteira2);
                context.SaveChanges();

                var transacao = new Transacao
                {
                    CarteiraOrigemId = carteira1.Id,
                    CarteiraDestinoId = carteira2.Id,
                    CarteiraOrigem = carteira1,
                    CarteiraDestino = carteira2,
                    Valor = 200
                };

                context.Transacoes.Add(transacao);
                context.SaveChanges();
            }
        }
    }
}