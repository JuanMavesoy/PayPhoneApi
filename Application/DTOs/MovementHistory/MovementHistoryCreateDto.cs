using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.MovementHistory
{
    public class MovementHistoryCreateDto
    {
        [Required]
        public int WalletId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero")]
        public decimal Amount { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty; 
    }
}
