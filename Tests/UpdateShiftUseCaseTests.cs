using Application.Contracts;
using Application.Dto;
using Application.Interfaces;
using Application.UseCases;
using Domain.Interfaces;
using Domain.Models;
using Moq;

namespace Application.Tests
{
    public class UpdateShiftUseCaseTests
    {
        private Mock<IDbRepository<Shift>> _shiftRepositoryMock;
        private Mock<IShiftValidationService> _shiftValidationServiceMock;
        private UpdateShiftUseCase _updateShiftUseCase;

        [SetUp]
        public void SetUp()
        {
            _shiftRepositoryMock = new Mock<IDbRepository<Shift>>();
            _shiftValidationServiceMock = new Mock<IShiftValidationService>();

            _updateShiftUseCase = new UpdateShiftUseCase(
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
                Id = 1,
                EmployeeId = 1,
                RoleId = 1,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(12, 0, 0),
                Date = DateTime.Today
            };

            var existingShift = new Shift
            {
                Id = 1,
                RoleId = 1,
                EmployeeId = 1,
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

            _shiftRepositoryMock.Setup(service =>
                service.GetByIdAsync(It.IsAny<int>())
            ).ReturnsAsync(existingShift);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _updateShiftUseCase.ExecuteAsync(shiftDto));

            Assert.That(ex.Message, Does.Contain("Invalid Role."));
        }

        [Test]
        public async Task ExecuteAsync_ShouldUpdateShift_WhenValidationSucceeds()
        {
            // Arrange
            var shiftDto = new ShiftDto
            {
                Id = 1,
                EmployeeId = 1,
                RoleId = 1,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(12, 0, 0),
                Date = DateTime.Today
            };

            var validationResult = new ValidationResult { IsValid = true };

            _shiftValidationServiceMock.Setup(service =>
                service.ShiftIsValid(It.IsAny<TimeSpan>()
                ,It.IsAny<TimeSpan>()
                ,It.IsAny<int>()
                ,It.IsAny<int>()
                ,It.IsAny<List<ShiftDto>>())
            ).ReturnsAsync(validationResult);

            var existingShift = new Shift
            {
                Id = 1,
                RoleId = 1,
                EmployeeId = 1,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(12, 0, 0),
                Date = DateTime.Today
            };

            _shiftRepositoryMock.Setup(repo =>
                repo.GetByIdAsync(It.IsAny<int>())
            ).ReturnsAsync(existingShift);

            // Act
            await _updateShiftUseCase.ExecuteAsync(shiftDto);

            // Assert
            _shiftRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Shift>()), Times.Once);
            _shiftRepositoryMock.Verify(repo => repo.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ExecuteAsync_ShouldThrowException_WhenShiftNotFound()
        {
            // Arrange
            var shiftDto = new ShiftDto { Id = 999 };
            _shiftRepositoryMock.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((Shift)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _updateShiftUseCase.ExecuteAsync(shiftDto));
            Assert.That(ex.Message, Does.Contain("Shift not found"));
        }

        [Test]
        public async Task ExecuteAsync_ShouldThrowException_WhenShiftTimesConflict()
        {
            // Arrange
            var shiftDto = new ShiftDto
            {
                Id = 1,
                EmployeeId = 1,
                RoleId = 1,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(12, 0, 0),
                Date = DateTime.Today
            };

            var existingShift = new Shift
            {
                Id = 2,
                EmployeeId = 1,
                RoleId = 1,
                StartTime = new TimeSpan(9, 30, 0),
                EndTime = new TimeSpan(11, 30, 0),
                Date = DateTime.Today
            };

            _shiftRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingShift);

            var validationResult = new ValidationResult { IsValid = false, Errors = new List<string> { "Overlap schedule." } };
            _shiftValidationServiceMock.Setup(service => service.ShiftIsValid(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<ShiftDto>>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _updateShiftUseCase.ExecuteAsync(shiftDto));
            Assert.That(ex.Message, Does.Contain("Overlap schedule."));
        }

        [Test]
        public async Task ExecuteAsync_ShouldUpdateShiftWithCorrectValues()
        {
            // Arrange
            var shiftDto = new ShiftDto { Id = 1, EmployeeId = 1, RoleId = 2, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(16, 0, 0), Date = DateTime.Today };
            var existingShift = new Shift { Id = 1, RoleId = 1, EmployeeId = 1, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(12, 0, 0), Date = DateTime.Today };
            _shiftRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingShift);

            var validationResult = new ValidationResult { IsValid = true };
            _shiftValidationServiceMock.Setup(service => service.ShiftIsValid(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<ShiftDto>>()))
                .ReturnsAsync(validationResult);

            // Act
            await _updateShiftUseCase.ExecuteAsync(shiftDto);

            // Assert
            _shiftRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Shift>(s => s.RoleId == 2 && s.StartTime == new TimeSpan(14, 0, 0) && s.EndTime == new TimeSpan(16, 0, 0))), Times.Once);
        }
    }
}
