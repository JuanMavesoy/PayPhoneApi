using Application.DTOs.Transfer;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PayPhoneApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransferController : ControllerBase
    {
        private readonly ITransferService _transferService;
        private readonly ILogger<TransferController> _logger;

        public TransferController(ITransferService transferService, ILogger<TransferController> logger)
        {
            _transferService = transferService;
            _logger = logger;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Transfer([FromBody] TransferRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                _logger.LogWarning("Solicitud inválida al realizar transferencia. Errores: {Errores}", errores);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Iniciando transferencia de {Amount} desde billetera {FromWalletId} hacia {ToWalletId}",
                    dto.Amount, dto.FromWalletId, dto.ToWalletId);

                await _transferService.TransferAsync(dto);

                _logger.LogInformation("Transferencia completada exitosamente.");
                return NoContent(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar la transferencia");
                throw; 
            }
        }
    }
}