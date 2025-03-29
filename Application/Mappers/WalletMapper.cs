using Application.DTOs.Wallet;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public static class WalletMapper
    {
        public static WalletDto ToDto(Wallet wallet) => new WalletDto
        {
            Id = wallet.Id,
            DocumentId = wallet.DocumentId,
            Name = wallet.Name,
            Balance = wallet.Balance,
            CreatedAt = wallet.CreatedAt,
            UpdatedAt = wallet.UpdatedAt
        };

        public static WalletWithMovementsDto ToDtoWithMovements(Wallet wallet)
        {
            return new WalletWithMovementsDto
            {
                Id = wallet.Id,
                DocumentId = wallet.DocumentId,
                Name = wallet.Name,
                Balance = wallet.Balance,
                CreatedAt = wallet.CreatedAt,
                UpdatedAt = wallet.UpdatedAt,
                Movements = wallet.Movements.Select(MovementHistoryMapper.ToDto).ToList()
            };
        }
    }
}