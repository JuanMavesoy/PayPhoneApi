using Application.DTOs.Transfer;

namespace Application.Interfaces
{
    public interface ITransferService
    {
        Task TransferAsync(TransferRequestDto dto);
    }
}
