using Application.Dto;
using Application.Interfaces;
using Application.UseCases;
using Domain.Interfaces;
using Domain.Models;
using Moq;

namespace Tests
{
    public class GetEmployeeUseCaseTests
    {
        private Mock<IDbRepository<Employee>> _employeeRepositoryMock;
        private Mock<IWeekService> _weekServiceMock;
        private GetEmployeesUseCase _getEmployeesUseCase;

        [SetUp]
        public void SetUp()
        {
            _employeeRepositoryMock = new Mock<IDbRepository<Employee>>();
            _weekServiceMock = new Mock<IWeekService>();

            _getEmployeesUseCase = new GetEmployeesUseCase(
                _employeeRepositoryMock.Object,
                _weekServiceMock.Object
            );
        }

        [Test]
        public async Task ExecuteAsync_ShouldReturnEmptyList_WhenNoEmployeesExist()
        {
            // Arrange
            _weekServiceMock.Setup(service => service.GetStartOfWeek())
                .Returns(new DateTime(2025, 1, 13));

            _weekServiceMock.Setup(service => service.GetEndOfWeek())
                .Returns(new DateTime(2025, 1, 19));

            _employeeRepositoryMock.Setup(repo => repo.QueryAsync(It.IsAny<Func<IQueryable<Employee>, IQueryable<EmployeeDto>>>()))
                .ReturnsAsync(new List<EmployeeDto>());

            // Act
            var result = await _getEmployeesUseCase.ExecuteAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task ExecuteAsync_ShouldHandleEmployeesWithoutRoles()
        {
            // Arrange
            _weekServiceMock.Setup(service => service.GetStartOfWeek())
                .Returns(new DateTime(2025, 1, 12));

            _weekServiceMock.Setup(service => service.GetEndOfWeek())
                .Returns(new DateTime(2025, 1, 18));

            var employees = new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    Name = "Jane Doe",
                    Roles = null,
                    Shifts = new List<Shift>
                    {
                        new Shift { Id = 1, EmployeeId = 1, RoleId = 1, Date = new DateTime(2025, 1, 14), StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(17) }
                    }
                }
            };

            _employeeRepositoryMock.Setup(repo => repo.QueryAsync(It.IsAny<Func<IQueryable<Employee>, IQueryable<EmployeeDto>>>()))
                .ReturnsAsync(employees.Select(emp => new EmployeeDto
                {
                    Id = emp.Id,
                    Name = emp.Name,
                    Roles = emp.Roles,
                    Shifts = emp.Shifts.Select(s => new ShiftDto
                    {
                        Id = s.Id,
                        EmployeeId = s.EmployeeId,
                        RoleName = s.Role?.Name ?? "Unknown",
                        RoleId = s.RoleId,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        Date = s.Date
                    }).ToList()
                }).ToList());

            // Act
            var result = await _getEmployeesUseCase.ExecuteAsync();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Jane Doe", result[0].Name);
            Assert.AreEqual(1, result[0].Shifts.Count);
        }


        [Test]
        public async Task ExecuteAsync_ShouldReturnEmployeeDtosWithShiftsInCurrentWeek()
        {
            // Arrange
            var startOfWeek = new DateTime(2025, 1, 13);
            var endOfWeek = new DateTime(2025, 1, 19);

            _weekServiceMock.Setup(service => service.GetStartOfWeek())
                .Returns(startOfWeek);

            _weekServiceMock.Setup(service => service.GetEndOfWeek())
                .Returns(endOfWeek);

            var employees = new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    Name = "Jane Doe",
                    Roles = new List<Role> { new Role { Id = 1, Name = "Manager" } },
                    Shifts = new List<Shift>
                    {
                        new Shift { Id = 1, EmployeeId = 1, RoleId = 1, Date = new DateTime(2025, 1, 14), StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(17) },
                        new Shift { Id = 2, EmployeeId = 1, RoleId = 1, Date = new DateTime(2025, 1, 16), StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(17) }
                    }
                },
                new Employee
                {
                    Id = 3,
                    Name = "Mary Johnson",
                    Roles = new List<Role> { new Role { Id = 3, Name = "HR" } },
                    Shifts = new List<Shift>()
                }
            };

            _employeeRepositoryMock.Setup(repo => repo.QueryAsync(It.IsAny<Func<IQueryable<Employee>, IQueryable<EmployeeDto>>>()))
             .ReturnsAsync(employees.Select(emp => new EmployeeDto
             {
                 Id = emp.Id,
                 Name = emp.Name,
                 Roles = emp.Roles,
                 Shifts = emp.Shifts.Where(s => s.Date >= startOfWeek && s.Date <= endOfWeek)
                     .Select(s => new ShiftDto
                     {
                         Id = s.Id,
                         EmployeeId = s.EmployeeId,
                         RoleName = s.Role != null ? s.Role.Name : "Unknown",
                         RoleId = s.RoleId,
                         StartTime = s.StartTime,
                         EndTime = s.EndTime,
                         Date = s.Date
                     }).ToList()
             }).ToList());


            // Act
            var result = await _getEmployeesUseCase.ExecuteAsync();

            // Assert
            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(2, result[0].Shifts.Count);

            Assert.AreEqual(0, result[1].Shifts.Count);
        }

        [Test]
        public async Task ExecuteAsync_ShouldReturnEmployeeDtosWithShiftsNotInCurrentWeek()
        {
            // Arrange
            var startOfWeek = new DateTime(2025, 1, 13);
            var endOfWeek = new DateTime(2025, 1, 19);

            _weekServiceMock.Setup(service => service.GetStartOfWeek())
                .Returns(startOfWeek);

            _weekServiceMock.Setup(service => service.GetEndOfWeek())
                .Returns(endOfWeek);

            var employees = new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    Name = "Jane Doe",
                    Roles = new List<Role> { new Role { Id = 1, Name = "Manager" } },
                    Shifts = new List<Shift>
                    {
                        new Shift { Id = 1, EmployeeId = 1, RoleId = 1, Date = new DateTime(2024, 1, 24), StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(17) },
                        new Shift { Id = 2, EmployeeId = 1, RoleId = 1, Date = new DateTime(2024, 1, 26), StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(17) }
                    }
                },
                new Employee
                {
                    Id = 2,
                    Name = "John Smith",
                    Roles = new List<Role> { new Role { Id = 2, Name = "Developer" } },
                    Shifts = new List<Shift>
                    {
                        new Shift { Id = 3, EmployeeId = 2, RoleId = 2, Date = new DateTime(2025, 1, 20), StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(17) }
                    }
                },
                new Employee
                {
                    Id = 3,
                    Name = "Mary Johnson",
                    Roles = new List<Role> { new Role { Id = 3, Name = "HR" } },
                    Shifts = new List<Shift>()
                }
            };

            _employeeRepositoryMock.Setup(repo => repo.QueryAsync(It.IsAny<Func<IQueryable<Employee>, IQueryable<EmployeeDto>>>()))
             .ReturnsAsync(employees.Select(emp => new EmployeeDto
             {
                 Id = emp.Id,
                 Name = emp.Name,
                 Roles = emp.Roles,
                 Shifts = emp.Shifts.Where(s => s.Date >= startOfWeek && s.Date <= endOfWeek)
                     .Select(s => new ShiftDto
                     {
                         Id = s.Id,
                         EmployeeId = s.EmployeeId,
                         RoleName = s.Role != null ? s.Role.Name : "Unknown",
                         RoleId = s.RoleId,
                         StartTime = s.StartTime,
                         EndTime = s.EndTime,
                         Date = s.Date
                     }).ToList()
             }).ToList());


            // Act
            var result = await _getEmployeesUseCase.ExecuteAsync();

            // Assert
            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(0, result[0].Shifts.Count);

            Assert.AreEqual(0, result[1].Shifts.Count);
        }
    }
}
