using CarteiraDigitalAPI.Data;
using CarteiraDigitalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarteiraDigitalAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("transacao")]
    public class TransacaoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransacaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int ObterUsuarioId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        }

        [HttpPost("transferir")]
        public IActionResult Transferir([FromBody] TransferenciaRequest request)
        {
            var carteiraOrigemId = ObterUsuarioId();

            if (request.Valor <= 0)
                return BadRequest("Valor inválido.");

            if (request.CarteiraDestinoId == carteiraOrigemId)
                return BadRequest("Não é possível transferir para si mesmo.");

            var carteiraOrigem = _context.Carteiras.FirstOrDefault(c => c.UsuarioId == carteiraOrigemId);
            var carteiraDestino = _context.Carteiras.FirstOrDefault(c => c.UsuarioId == request.CarteiraDestinoId);

            if (carteiraOrigem == null || carteiraDestino == null)
                return NotFound("Carteira não encontrada.");

            if (carteiraDestino.Saldo < request.Valor)
                return BadRequest("Saldo insuficiente.");

            carteiraOrigem.Saldo -= request.Valor;
            carteiraDestino.Saldo += request.Valor;

            var transacao = new Transacao
            {
                CarteiraOrigemId = carteiraOrigemId,
                CarteiraDestinoId = request.CarteiraDestinoId,
                CarteiraOrigem = carteiraOrigem,
                CarteiraDestino = carteiraDestino,
                Valor = request.Valor,
                Data = DateTime.UtcNow
            };

            _context.Transacoes.Add(transacao);
            _context.SaveChanges();

            return Ok("Transferência realizada com sucesso.");
        }

        [HttpGet("listar")]
        public IActionResult ListarTransacoes([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
        {
            var usuarioId = ObterUsuarioId();

            var carteiraUsuario = _context.Carteiras
                .Include(c => c.Usuario)
                .FirstOrDefault(c => c.UsuarioId == usuarioId);

            if (carteiraUsuario == null)
                return NotFound("Carteira do usuário não encontrada.");

            var carteiraId = carteiraUsuario.Id;

            var query = _context.Transacoes
                .Include(t => t.CarteiraOrigem).ThenInclude(c => c.Usuario)
                .Include(t => t.CarteiraDestino).ThenInclude(c => c.Usuario)
                .Where(t => t.CarteiraOrigemId == carteiraId || t.CarteiraDestinoId == carteiraId);

            if (inicio.HasValue)
                query = query.Where(t => t.Data >= inicio.Value);

            if (fim.HasValue)
                query = query.Where(t => t.Data <= fim.Value);

            var resultado = query
                .OrderByDescending(t => t.Data)
                .Select(t => new
                {
                    Tipo = t.CarteiraOrigemId == carteiraId ? "Enviada" : "Recebida",
                    De = t.CarteiraOrigem.Usuario.Nome,
                    Para = t.CarteiraDestino.Usuario.Nome,
                    t.Valor,
                    Data = t.Data.ToString("dd/MM/yyyy HH:mm")
                })
                .ToList();

            return Ok(resultado);
        }
    }

    public class TransferenciaRequest
    {
        public int CarteiraDestinoId { get; set; }
        public decimal Valor { get; set; }
    }
}
