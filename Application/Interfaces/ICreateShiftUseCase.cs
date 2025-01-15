using Application.Dto;

namespace Application.Interfaces
{
    public interface ICreateShiftUseCase
    {
        Task ExecuteAsync(ShiftDto shift);
    }
}
