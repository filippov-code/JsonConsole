using JsonConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonConsole
{
    public class Employee : IHaveId, ICloneable<Employee>
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal SalaryPerHour { get; set; }

        public override string ToString() 
            => $"Id = {Id}, FirstName = {FirstName}, LastName = {LastName}, SalaryPerHour = {SalaryPerHour}";

        public override bool Equals(object? employee)
        { 
            Employee? employee2 = employee as Employee;
            return employee2 != null && Id == employee2.Id && FirstName == employee2.FirstName && LastName == employee2.LastName;
        }

        public Employee Clone()
            => new Employee { Id = Id, FirstName = FirstName, LastName = LastName, SalaryPerHour = SalaryPerHour };
    }
}
