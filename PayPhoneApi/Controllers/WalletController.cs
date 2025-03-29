using Application.DTOs.Wallet;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PayPhoneApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ILogger<WalletController> _logger;

        public WalletController(IWalletService walletService, ILogger<WalletController> logger)
        {
            _walletService = walletService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Consultando todas las billeteras");
            var wallets = await _walletService.GetAllAsync();
            return Ok(wallets);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Consultando billetera con ID: {Id}", id);
            var wallet = await _walletService.GetByIdAsync(id);

            if (wallet is null)
            {
                _logger.LogWarning("No se encontró billetera con ID: {Id}", id);
                return NotFound();
            }

            return Ok(wallet);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}/with-movements")]
        public async Task<IActionResult> GetWithMovements(int id)
        {
            _logger.LogInformation("Consultando billetera con movimientos, ID: {Id}", id);
            var result = await _walletService.GetWithMovementsAsync(id);

            if (result is null)
            {
                _logger.LogWarning("No se encontró la billetera con ID: {Id}", id);
                return NotFound();
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WalletCreateUpdateDto walletDto)
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                _logger.LogWarning("Solicitud inválida al crear billetera. Errores: {Errores}", errores);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creando billetera para documento: {DocumentId}", walletDto.DocumentId);
                var created = await _walletService.CreateAsync(walletDto);
                _logger.LogInformation("Billetera creada con ID: {Id}", created.Id);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear billetera");
                throw;
            }
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] WalletCreateUpdateDto walletDto)
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                _logger.LogWarning("Solicitud inválida al actualizar billetera con ID: {Id}. Errores: {Errores}", id, errores);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Actualizando billetera con ID: {Id}", id);
            var updated = await _walletService.UpdateAsync(id, walletDto);

            if (updated is null)
            {
                _logger.LogWarning("No se encontró billetera con ID: {Id} para actualizar", id);
                return NotFound();
            }

            _logger.LogInformation("Billetera actualizada con ID: {Id}", id);
            return Ok(updated);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Eliminando billetera con ID: {Id}", id);
            var deleted = await _walletService.DeleteAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("No se encontró billetera con ID: {Id} para eliminar", id);
                return NotFound();
            }

            _logger.LogInformation("Billetera eliminada con ID: {Id}", id);
            return NoContent();
        }
    }
}