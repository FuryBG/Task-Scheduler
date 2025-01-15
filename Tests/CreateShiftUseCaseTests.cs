using Application.Contracts;
using Application.Dto;
using Application.Interfaces;
using Application.UseCases;
using Domain.Interfaces;
using Domain.Models;
using Moq;

namespace Tests
{
    public class CreateShiftUseCaseTests
    {
        private Mock<IDbRepository<Shift>> _shiftRepositoryMock;
        private Mock<IShiftValidationService> _shiftValidationServiceMock;
        private CreateShiftUseCase _createShiftUseCase;

        [SetUp]
        public void SetUp()
        {
            _shiftRepositoryMock = new Mock<IDbRepository<Shift>>();
            _shiftValidationServiceMock = new Mock<IShiftValidationService>();

            _createShiftUseCase = new CreateShiftUseCase(
                _shiftRepositoryMock.Object,
                _shiftValidationServiceMock.Object
            );
        }

        [Test]
        public async Task ExecuteAsync_ShouldThrowException_WhenValidationFails()
        {
            // Arrange
            var shiftDto = new ShiftDto
            {
                Id = 0,
                EmployeeId = 1,
                RoleId = 1,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(12, 0, 0),
                Date = DateTime.Today
            };

            var validationResult = new ValidationResult
            {
                IsValid = false,
                Errors = new List<string> { "Invalid Role." }
            };

            _shiftValidationServiceMock.Setup(service =>
                service.ShiftIsValid(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<ShiftDto>>())
            ).ReturnsAsync(validationResult);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _createShiftUseCase.ExecuteAsync(shiftDto));

            Assert.That(ex.Message, Does.Contain("Invalid Role."));
        }

        [Test]
        public async Task ExecuteAsync_ShouldCreateShift_WhenValidationSucceeds()
        {
            // Arrange
            var shiftDto = new ShiftDto
            {
                Id = 0,
                EmployeeId = 1,
                RoleId = 1,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(12, 0, 0),
                Date = DateTime.Today
            };

            var validationResult = new ValidationResult { IsValid = true };

            _shiftValidationServiceMock.Setup(service =>
                service.ShiftIsValid(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<ShiftDto>>())
            ).ReturnsAsync(validationResult);

            _shiftRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Shift>())).Returns(Task.CompletedTask);

            // Act
            await _createShiftUseCase.ExecuteAsync(shiftDto);

            // Assert
            _shiftRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Shift>(s =>
                s.EmployeeId == shiftDto.EmployeeId &&
                s.RoleId == shiftDto.RoleId &&
                s.StartTime == shiftDto.StartTime &&
                s.EndTime == shiftDto.EndTime &&
                s.Date == shiftDto.Date
            )), Times.Once);

            _shiftRepositoryMock.Verify(repo => repo.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
