
namespace Application.DTOs.MovementHistory
{
    public class MovementHistoryDto
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; }
    }
}
