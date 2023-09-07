using AutoMapper;
using Domain.Repositories;
using Domain.Services;
using Service.Services;
using Moq;
using System.Text;
using Domain.Models;
using Repository.Mapping_Profile;
using Service.ServiceModels;

namespace TaskApplication.Test.Services
{
    [TestClass]
    public class EmployeeServiceTest
    {
        private Mock<IEmployeeRepository> _mockEmployeeRepository;
        private Mock<IMapper> _mockMapper;
        private Stream _rightStream, _wrongStream;
        private StringWriter consoleOutput;
        private IMapper _mapper;
        private IEmployeeService _employeeService;

        [TestInitialize]
        public void TaskSetUp()
        {
            string csvData = "Personnel_Records.Payroll_Number,Personnel_Records.Forenames,Personnel_Records.Surname,Personnel_Records.Date_of_Birth,Personnel_Records.Telephone,Personnel_Records.Mobile,Personnel_Records.Address,Personnel_Records.Address_2,Personnel_Records.Postcode,Personnel_Records.EMail_Home,Personnel_Records.Start_Date\nCOOP08,John ,William,26/01/1955,12345678,987654231,12 Foreman road,London,GU12 6JW,nomadic20@hotmail.co.uk,18/04/2013";
            _rightStream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));
            _mockEmployeeRepository = new Mock<IEmployeeRepository>();
            _mockMapper = new Mock<IMapper>();//create actual mapper object 
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.AddProfile<MappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            //instantiate employeeService with actual mapper object 
            _employeeService = new EmployeeService(_mockEmployeeRepository.Object, _mapper);

            consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
        }

        [TestMethod]
        public async Task GivenWrongCsv_ReturnsError()
        {
            var result = await _employeeService.ImportEmployeesFromCsvAsync(_wrongStream);
            var consoleResult = consoleOutput.ToString();

            Assert.IsNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(consoleResult));
            Assert.IsTrue(consoleResult.StartsWith("Value cannot be null."));
        }

        [TestMethod]
        public async Task GivenWrongMapping_ReturnsError()
        {
            var exception = new Exception("Mapper goes wrong");
            _mockMapper.Setup(c => c.Map<List<IEmployee>>(It.IsAny<IEnumerable<IEmployee>>())).Throws(exception);
            _employeeService = new EmployeeService(_mockEmployeeRepository.Object, _mockMapper.Object);

            var result = await _employeeService.ImportEmployeesFromCsvAsync(_rightStream);
            var consoleResult = consoleOutput.ToString();

            Assert.IsNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(consoleResult));
            Assert.IsTrue(consoleResult.StartsWith("Mapper goes wrong"));
        }


        [TestMethod]
        public async Task GivenWrongAddEmployee_ReturnsError()
        {
            var exception = new Exception("Error while adding object to database");
            _mockEmployeeRepository.Setup(c => c.AddEmployeesAsync(It.IsAny<List<IEmployee>>())).Throws(exception);

            var result = await _employeeService.ImportEmployeesFromCsvAsync(_rightStream);
            var consoleResult = consoleOutput.ToString();

            Assert.IsNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(consoleResult));
            Assert.IsTrue(consoleResult.StartsWith("Error while adding object to database"));
        }

        [TestMethod]
        public async Task GivenWrongImportedEmployees_ReturnsError()
        {
            var exception = new Exception("Users has not been returned");
            _mockEmployeeRepository.Setup(c => c.GetImportedEmployeesAsync(It.IsAny<int>())).Throws(exception);

            var result = await _employeeService.ImportEmployeesFromCsvAsync(_rightStream);
            var consoleResult = consoleOutput.ToString();

            Assert.IsNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(consoleResult));
            Assert.IsTrue(consoleResult.StartsWith("Users has not been returned"));
        }

        [TestMethod]
        public async Task HappyPath()
        {
            _mockEmployeeRepository.Setup(c => c.AddEmployeesAsync(It.IsAny<List<IEmployee>>()));
            _mockEmployeeRepository.Setup(c => c.GetImportedEmployeesAsync(1)).ReturnsAsync(new List<IEmployee> { 
                                                                                            new EmployeeServiceModel {
                                                                                                Id = 0,
                                                                                                Forenames = "John",
                                                                                                Surname = "William",
                                                                                                Date_of_Birth = new DateTime(1955,01,26),
                                                                                                Telephone = "12345678",
                                                                                                Mobile = "987654231",
                                                                                                Address = "12 Foreman road,London",
                                                                                                Address_2 = "",
                                                                                                Postcode = "GU12 6JW",
                                                                                                EMail_Home = "nomadic20@hotmail.co.uk",
                                                                                                Start_Date = new DateTime(2013, 04,18),
                                                                                                Payroll_Number = "COOP08"
                                                                                            }});

            var result = await _employeeService.ImportEmployeesFromCsvAsync(_rightStream);
            var consoleResult = consoleOutput.ToString();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 1);
            Assert.AreEqual(result.FirstOrDefault(c => c.Payroll_Number == "COOP08")?.Payroll_Number, "COOP08");
            Assert.AreEqual(result.FirstOrDefault(c => c.Mobile == "987654231")?.Mobile, "987654231");
            Assert.IsTrue(result is IEnumerable<IEmployee>);
        }

    }
}