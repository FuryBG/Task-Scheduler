using Application.Dto;

namespace Application.Interfaces
{
    public interface IDeleteShiftUseCase
    {
        Task ExecuteAsync(ShiftDto shift);
    }
}
