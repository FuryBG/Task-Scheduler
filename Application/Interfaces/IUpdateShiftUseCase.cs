using Application.Dto;

namespace Application.Interfaces
{
    public interface IUpdateShiftUseCase
    {
        Task ExecuteAsync(ShiftDto shift);
    }
}
