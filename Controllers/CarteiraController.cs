using CarteiraDigitalAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static CarteiraDigitalAPI.Services.CarteiraService;

namespace CarteiraDigitalAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("carteira")]
    public class CarteiraController : ControllerBase
    {
        private readonly ICarteiraService _carteiraService;

        public CarteiraController(ICarteiraService carteiraService)
        {
            _carteiraService = carteiraService;
        }

        [HttpGet("saldo")]
        public IActionResult ObterSaldo()
        {
            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(usuarioIdClaim))
                return BadRequest("O identificador do usuário está faltando.");

            var usuarioId = int.Parse(usuarioIdClaim);

            try
            {
                var saldo = _carteiraService.ObterSaldo(usuarioId);
                return Ok(new { Saldo = saldo });
            }
            catch (CarteiraNaoEncontradaException ex)
            {
                return NotFound(new { Mensagem = ex.Message });
            }
        }

        [HttpPost("adicionar-saldo")]
        public IActionResult AdicionarSaldo([FromBody] AdicionarSaldoRequest request)
        {
            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(usuarioIdClaim))
            {
                return BadRequest("O identificador do usuário está faltando.");
            }
            var usuarioId = int.Parse(usuarioIdClaim);
            var novoSaldo = _carteiraService.AdicionarSaldo(usuarioId, request.Valor);
            return Ok(new { Saldo = novoSaldo });
        }

        public class AdicionarSaldoRequest
        {
            public decimal Valor { get; set; }
        }
    }
}