using Application.DTOs.MovementHistory;


namespace Application.DTOs.Wallet
{
    public class WalletWithMovementsDto
    {
        public int Id { get; set; }
        public string DocumentId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<MovementHistoryDto> Movements { get; set; } = new();
    }
}
