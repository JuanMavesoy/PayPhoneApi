using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Wallet
{
    public class WalletCreateUpdateDto
    {
        [Required(ErrorMessage = "El documento es requerido")]
        [StringLength(20, ErrorMessage = "El documento no debe superar los 20 caracteres")]
        public string DocumentId { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres")]
        [StringLength(100, ErrorMessage = "El nombre no debe superar los 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "El saldo no puede ser negativo")]
        public decimal Balance { get; set; }
    }
}
