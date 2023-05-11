using EfCoreApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAddEmployee()
        {
            // Arrange

            int expectedId; // мы ожидаем, что добавленный сотрудник будет иметь идентификатор равный самому большому значению столбца Id + 1

            using (var db = new AppDbContext())
            {
                expectedId = db.Employees.Max(p => p.Id) + 1;
            }

            // Act
            Commands.addEmployee();

            // Assert
            using (var db = new AppDbContext())
            {
                Employee addedEmployee = db.Employees.FirstOrDefault(p => p.Id == expectedId);
                Assert.IsNotNull(addedEmployee); // проверяем, что сотрудник был добавлен
                Assert.AreEqual("John", addedEmployee.LastName); // проверяем, что фамилия равна "John"
                Assert.AreEqual("Doe", addedEmployee.FirstName); // проверяем, что имя равно "Doe"
                Assert.AreEqual((decimal)100.50, addedEmployee.SalaryPerHour); // проверяем, что зарплата равна 100.50

                db.Employees.Remove(addedEmployee);
                db.SaveChanges();
            }
        }
    }
}
