using JsonConsole.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace JsonConsole.Tests
{
    public class JsonStorageTests
    {
        private readonly string Filepath = "employees.json";

        [Fact]
        public void AddEmployee_Success()
        {
            //Arrange
            DeleteJson();
            var storage = new JsonStorage<Employee>(Filepath);
            Employee employee = new Employee()
            {
                FirstName = "Stepan",
                LastName = "Filippov",
                SalaryPerHour = 442
            };
            //Act
            var result = storage.Add(employee);
            //Assert
            Assert.True(result.Success);
        }
        [Fact]
        public void AddEmployee_Fail()
        {
            //Arrange
            DeleteJson();
            var storage = new JsonStorage<Employee>(Filepath);
            //Act
            var result = storage.Add(null);
            storage.Save();
            //Assert
            Assert.False(result.Success);
        }
        [Fact]
        public void GetEmployee_Success()
        {
            //Arrange
            DeleteJson();
            var storage = new JsonStorage<Employee>(Filepath);
            Employee employee = new Employee()
            {
                FirstName = "Stepan",
                LastName = "Filippov",
                SalaryPerHour = 442
            };
            //Act
            storage.Add(employee);
            var result = storage.Get(1);
            //Assert
            Assert.True(result.Success);
        }
        [Fact]
        public void GetEmployee_Fail()
        {
            //Arrange
            DeleteJson();
            var storage = new JsonStorage<Employee>(Filepath);
            Employee employee = new Employee()
            {
                FirstName = "Stepan",
                LastName = "Filippov",
                SalaryPerHour = 442
            };
            //Act
            var result = storage.Get(1111111);
            //Assert
            Assert.False(result.Success);
        }
        [Fact]
        public void AutoIdGenerated_Success()
        {
            //Arrange
            DeleteJson();
            var storage = new JsonStorage<Employee>(Filepath);
            Employee employee = new Employee()
            {
                FirstName = "Stepan",
                LastName = "Filippov",
                SalaryPerHour = 444
            };
            //Act
            storage.Add(employee);
            var result = storage.Get(1);
            //Assert
            Assert.True(result.Success);
        }
        [Fact]
        public void UpdateEmployee_Success()
        {
            //Arrange
            DeleteJson();
            var storage = new JsonStorage<Employee>(Filepath);
            Employee employee = new Employee()
            {
                FirstName = "Stepan",
                LastName = "Filippov",
                SalaryPerHour = 444
            };
            storage.Add(employee);
            employee.Id = 1;
            employee.SalaryPerHour = 1000;
            //Act
            var result = storage.Update(employee);
            //Assert
            Assert.True(result.Success);
        }
        [Fact]
        public void UpdateEmployee_Fail()
        {
            //Arrange
            DeleteJson();
            var storage = new JsonStorage<Employee>(Filepath);
            Employee employee = new Employee()
            {
                FirstName = "Stepan",
                LastName = "Filippov",
                SalaryPerHour = 444
            };
            storage.Add(employee);
            employee.Id = 20;
            //Act
            var result = storage.Update(employee);
            //Assert
            Assert.False(result.Success);
        }
        [Fact]
        public void DeleteEmployee_Success()
        {
            //Arrange
            DeleteJson();
            var storage = new JsonStorage<Employee>(Filepath);
            Employee employee = new Employee()
            {
                FirstName = "Stepan",
                LastName = "Filippov",
                SalaryPerHour = 444
            };
            //Act
            storage.Add(employee);
            var result = storage.Delete(1);
            //Assert
            Assert.True(result.Success);
        }
        [Fact]
        public void DeleteEmployee_Fail()
        {
            //Arrange
            DeleteJson();
            var storage = new JsonStorage<Employee>(Filepath);
            Employee employee = new Employee()
            {
                FirstName = "Stepan",
                LastName = "Filippov",
                SalaryPerHour = 444
            };
            //Act
            storage.Add(employee);
            var result = storage.Delete(11111);
            //Assert
            Assert.False(result.Success);
        }

        [Fact]
        public void GetAllEmployee_Succeess()
        {
            //Arrange
            DeleteJson();
            var storage = new JsonStorage<Employee>(Filepath);
            Employee employee1 = new Employee()
            {
                FirstName = "Stepan",
                LastName = "Filippov",
                SalaryPerHour = 111
            };
            Employee employee2 = new Employee()
            {
                FirstName = "Stepan",
                LastName = "Filippov",
                SalaryPerHour = 222
            };
            Employee employee3 = new Employee()
            {
                FirstName = "Stepan",
                LastName = "Filippov",
                SalaryPerHour = 333
            };
            //Act
            storage.Add(employee1);
            storage.Add(employee2);
            storage.Add(employee3);
            var result = storage.GetAll();
            //Assert
            Assert.True(result.Result.Length == 3);
        }

        private void DeleteJson()
        {
            if (File.Exists(Filepath))
                File.Delete(Filepath);
        }
    }
}
