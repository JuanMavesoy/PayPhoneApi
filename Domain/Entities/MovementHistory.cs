using Domain.Enums;

namespace Domain.Entities
{
    public class MovementHistory
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public MovementType Type { get; set; } 
        public DateTime CreatedAt { get; set; }

        public Wallet Wallet { get; set; }
    }
}
