using Application.DTOs.MovementHistory;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public static class MovementHistoryMapper
    {
        public static MovementHistoryDto ToDto(MovementHistory m) => new MovementHistoryDto
        {
            Id = m.Id,
            WalletId = m.WalletId,
            Amount = m.Amount,
            Type = m.Type.ToString(),
            CreatedAt = m.CreatedAt
        };
    }
}
