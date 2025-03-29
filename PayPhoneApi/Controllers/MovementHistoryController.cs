using Application.DTOs.MovementHistory;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PayPhoneApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovementHistoryController : ControllerBase
    {
        private readonly IMovementHistoryService _movementService;
        private readonly ILogger<MovementHistoryController> _logger;

        public MovementHistoryController(IMovementHistoryService movementService, ILogger<MovementHistoryController> logger)
        {
            _movementService = movementService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Consultando todos los movimientos");
            var result = await _movementService.GetAllAsync();
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("wallet/{walletId:int}")]
        public async Task<IActionResult> GetByWalletId(int walletId)
        {
            _logger.LogInformation("Consultando movimientos para billetera ID: {WalletId}", walletId);
            var result = await _movementService.GetByWalletIdAsync(walletId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MovementHistoryCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                _logger.LogWarning("Solicitud inválida al crear movimiento. Errores: {Errores}", errores);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creando movimiento tipo {Type} por valor {Amount} en billetera ID: {WalletId}",
                    dto.Type, dto.Amount, dto.WalletId);

                var created = await _movementService.CreateAsync(dto);

                _logger.LogInformation("Movimiento creado con ID: {Id}", created.Id);
                return CreatedAtAction(nameof(GetByWalletId), new { walletId = created.WalletId }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear movimiento para billetera ID: {WalletId}", dto.WalletId);
                throw; 
            }
        }
    }
}