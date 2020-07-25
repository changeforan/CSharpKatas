using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace CSharpKatas
{
    public class Employee
    {
        public int? EmployeeId { get; set; }
    }

    public class Sale
    {
        public int? SaleId { get; set; }
    }

    public class MockDatabase
    {
        
        private List<Employee> employees;
        private List<Sale> sales;

        public MockDatabase()
        {
            this.employees = new List<Employee>();
            this.sales = new List<Sale>();
        }
        public void InsertEmployee(Employee employee)
        {
            // todo #1: Remove the ignore attribute and get all the tests to pass any way you can
            // todo #2: Refactor out common auto increment logic from InsertEmployee and InsertSale into a GetNextId function (you'll need System.Func a good grasp of generics)
            // todo #3: Refactor GetNextId so it has no if statements and no ternary operators (hint: use the ?? operator)
            // todo #4: Refactor out *all* common logic so InsertEmployee and InsertSale are a single line of code (lots of possible solutions). Was this an improvement?
            if (this.GetEmployeeById(employee.EmployeeId ?? -1) == null)
            {
                employee.EmployeeId = employee.EmployeeId ?? this.GetNextId(employees, e => e.EmployeeId);
                this.employees.Add(employee);
            }
        }

        public void InsertSale(Sale sale)
        {
            this.Insert(this.sales, sale, s => s.SaleId, (s, id) => s.SaleId = id, this.GetSaleById, s => s.SaleId);
        }

        public Employee GetEmployeeById(int employeeId)
        {
            var ret = this.employees
                .FirstOrDefault(e => e.EmployeeId == employeeId);
            return ret;
        }
        
        public Sale GetSaleById(int saleId)
        {
            var ret = this.sales
                .FirstOrDefault(s => s.SaleId == saleId);
            return ret;
        }

        private void Insert<T>(List<T> list, 
                               T item, 
                               Func<T, int?> getId,
                               Action<T, int> setId, 
                               Func<int, T> getter, 
                               Func<T, int?> selector)
        {
            if (getter(getId(item) ?? -1) == null)
            {
                if (getId(item) == null)
                {
                    setId(item, this.GetNextId(list, selector));
                }

                list.Add(item);
            }
        }

        private int GetNextId<T>(IEnumerable<T> list, Func<T, int?> selector)
        {
            var maxId = list
                .Select(selector)
                .Max();
            
            return (maxId ?? 0) + 1;
        }
    }

    [TestFixture]
    public class TestMockDatabase
    {
        [Test]
        public void GetEmployeeById_IfYouInsertAnEmployee_YouCanGetItBack()
        {
            var mockDatabase = new MockDatabase();
            var employee = new Employee {EmployeeId = 20202};
            mockDatabase.InsertEmployee(employee);
            var actual = mockDatabase.GetEmployeeById(20202);
            Assert.IsNotNull(actual);
            Assert.AreEqual(20202, actual.EmployeeId);
        }

        [Test]
        public void GetSaleById_IfYouInsertASale_YouCanGetItBack()
        {
            var mockDatabase = new MockDatabase();
            var sale = new Sale {SaleId = 20202};
            mockDatabase.InsertSale(sale);
            var actual = mockDatabase.GetSaleById(20202);
            Assert.IsNotNull(actual);
            Assert.AreEqual(20202, actual.SaleId);
        }

        [Test]
        public void InsertEmployee_IfYouInsertAnEmpoyeeWithNullIdAndNoExistingEmployees_IdStartsAtOne()
        {
            var mockDatabase = new MockDatabase();
            var employee = new Employee();
            mockDatabase.InsertEmployee(employee);
            Assert.AreEqual(1, employee.EmployeeId);
        }

        [Test]
        public void InsertEmployee_IfYouInsertAnEmployeeWithNonNullId_ExistingIdIsNotOverwritten()
        {
            var mockDatabase = new MockDatabase();
            var employee = new Employee
            {
                EmployeeId = 7
            };
            mockDatabase.InsertEmployee(employee);
            Assert.AreEqual(7, employee.EmployeeId);
        }

        [Test]
        public void InsertEmployee_IfYouInsertAnEmployeeWithNullIdAndAnEmployeeExists_IdIsMaxPlusOne()
        {
            var mockDatabase = new MockDatabase();
            var initialEmployee = new Employee { EmployeeId = 7 };
            mockDatabase.InsertEmployee(initialEmployee);
            var secondEmployee = new Employee();
            mockDatabase.InsertEmployee(secondEmployee);
            Assert.AreEqual(8, secondEmployee.EmployeeId);
        }
        
        [Test]
        public void InsertSale_IfYouInsertASaleWithNullIdAndNoExistingSales_IdStartsAtOne()
        {
            var mockDatabase = new MockDatabase();
            var sale = new Sale();
            mockDatabase.InsertSale(sale);
            Assert.AreEqual(1, sale.SaleId);
        }

        [Test]
        public void InsertSale_IfYouInsertANonNullId_TheExistingIdIsNotOverwritten()
        {
            var mockDatabase = new MockDatabase();
            var sale = new Sale
            {
                SaleId = 7
            };
            mockDatabase.InsertSale(sale);
            Assert.AreEqual(7, sale.SaleId);
        }

        [Test]
        public void InsertSale_IfYouInsertInsertASaleWithNullIdAndAPreviousSaleExists_IdIsMaxPlusOne()
        {
            var mockDatabase = new MockDatabase();
            var initialSale = new Sale { SaleId = 7 };
            mockDatabase.InsertSale(initialSale);
            var secondSale = new Sale();
            mockDatabase.InsertSale(secondSale);
            Assert.AreEqual(8, secondSale.SaleId);
        }
    }
}
