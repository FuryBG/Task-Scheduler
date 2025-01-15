using Application.Dto;
using Application.UseCases;
using Domain.Interfaces;
using Domain.Models;
using Moq;

public class DeleteShiftUseCaseTests
{
    private Mock<IDbRepository<Shift>> _shiftRepositoryMock;
    private DeleteShiftUseCase _deleteShiftUseCase;

    [SetUp]
    public void SetUp()
    {
        _shiftRepositoryMock = new Mock<IDbRepository<Shift>>();
        _deleteShiftUseCase = new DeleteShiftUseCase(_shiftRepositoryMock.Object);
    }

    [Test]
    public async Task ExecuteAsync_ShouldDeleteShift_WhenShiftExists()
    {
        // Arrange
        var shiftDto = new ShiftDto { Id = 1, RoleId = 1 };
        var shift = new Shift { Id = 1, RoleId = 1 };

        _shiftRepositoryMock.Setup(repo => repo.GetByIdAsync(shiftDto.Id))
            .ReturnsAsync(shift);

        _shiftRepositoryMock.Setup(repo => repo.HardDeleteAsync(It.IsAny<Shift>()))
            .Returns(Task.CompletedTask);

        _shiftRepositoryMock.Setup(repo => repo.SaveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Task.FromResult(1).Result);

        // Act
        await _deleteShiftUseCase.ExecuteAsync(shiftDto);

        // Assert
        _shiftRepositoryMock.Verify(repo => repo.GetByIdAsync(shiftDto.Id), Times.Once);
        _shiftRepositoryMock.Verify(repo => repo.HardDeleteAsync(It.Is<Shift>(s => s.Id == shiftDto.Id)), Times.Once);
        _shiftRepositoryMock.Verify(repo => repo.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void ExecuteAsync_ShouldThrowException_WhenShiftNotFound()
    {
        // Arrange
        var shiftDto = new ShiftDto { Id = 1 };

        _shiftRepositoryMock.Setup(repo => repo.GetByIdAsync(shiftDto.Id))
            .ReturnsAsync((Shift)null);

        // Act & Assert
        Assert.ThrowsAsync<Exception>(() => _deleteShiftUseCase.ExecuteAsync(shiftDto));
        _shiftRepositoryMock.Verify(repo => repo.GetByIdAsync(shiftDto.Id), Times.Once);
        _shiftRepositoryMock.Verify(repo => repo.HardDeleteAsync(It.IsAny<Shift>()), Times.Never);
        _shiftRepositoryMock.Verify(repo => repo.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
