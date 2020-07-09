using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace CSharpKatas
{
    public class PersonConverter
    {
        public IEnumerable<Person> ConvertFromJson(string salariedPeople, string hourlyPeople)
        {
            string[] people = { salariedPeople, hourlyPeople };
            Type[] types = { typeof(IEnumerable<SalariedEmployee>), typeof(IEnumerable<HourlyEmployee>) };
            return people
                .Select(p => p ?? "")
                .Zip(types, (s, t) => JsonConvert.DeserializeObject(s, t) as IEnumerable<Person>)
                .Where(list => list != null)
                .SelectMany(a => a);
        }
    }

    public class Person
    {
        public string Name { get; set; }
    }

    public class SalariedEmployee : Person
    {
        public decimal Salary { get; set; }
    }

    public class HourlyEmployee : Person
    {
        public decimal RatePerHour { get; set; }
    }

    [TestClass]
    public class TestGenerics
    {
        [TestMethod]
        public void ConvertFromJson_EmptyString_ReturnsZeroResults()
        {
            var convert = new PersonConverter();
            var people = convert.ConvertFromJson("", "");
            Assert.AreEqual(0, people.Count());
        }

        [TestMethod]
        public void ConvertFromJson_NullParameters_ReturnsZeroResults()
        {
            var convert = new PersonConverter();
            var people = convert.ConvertFromJson(null, null);
            Assert.AreEqual(0, people.Count());
        }

        [TestMethod]
        public void ConvertFromJson_SalariedEmployee_ReturnsStronglyTypedEmployee()
        {
            var convert = new PersonConverter();
            var people = convert.ConvertFromJson("[ { Name: 'Bob', Salary: 10 } ]", null).ToList();
            Assert.AreEqual(1, people.Count());
            var bob = people[0] as SalariedEmployee;
            Assert.IsNotNull(bob, "Expected a SalariedEmployee, but got some other type (Person?)");
            Assert.AreEqual("Bob", bob.Name);
            Assert.AreEqual(10, bob.Salary);
        }

        [TestMethod]
        public void ConvertFromJson_HourlyEmployee_ReturnsStronglyTypedHourlyEmployee()
        {
            var convert = new PersonConverter();
            var people = convert.ConvertFromJson(null, "[ { Name: 'Bob', RatePerHour: 5.50 } ]").ToList();
            Assert.AreEqual(1, people.Count());
            var bob = people[0] as HourlyEmployee;
            Assert.IsNotNull(bob, "Expected a SalariedEmployee, but got a Person");
            Assert.AreEqual("Bob", bob.Name);
            Assert.AreEqual(5.5M, bob.RatePerHour);
        }

        [TestMethod]
        public void ConvertFromJson_HourlyAndSalariedEmployees_GetMerged()
        {
            var convert = new PersonConverter();
            var people = convert.ConvertFromJson("[ { Name: 'Sam', Salary: 15 } ]", "[ { Name: 'Bob', RatePerHour: 5.50 } ]").ToList();
            Assert.AreEqual(2, people.Count());
            Assert.AreEqual(1, people.Count(i => i is SalariedEmployee));
            Assert.AreEqual(1, people.Count(i => i is HourlyEmployee));
        }
    }
}
