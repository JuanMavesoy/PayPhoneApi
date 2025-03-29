using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Transfer
{
    public class TransferRequestDto
    {
        [Required]
        public int FromWalletId { get; set; }

        [Required]
        public int ToWalletId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto de la transferencia debe ser mayor que cero.")]
        public decimal Amount { get; set; }
    }
}
