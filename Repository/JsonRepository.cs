using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonConsole.Repository
{
    internal class JsonRepository
    {
        private readonly List<Employee> employees;
        public JsonRepository()
        {
            using (StreamReader reader = File.OpenText("employees.json"))
            {
                string json = reader.ReadToEnd();
                employees = JsonConvert.DeserializeObject<List<Employee>>(json) ?? new();
            }
        }

        public void Add(Employee employee)
        {
            employee.Id = GetFreeId();
            employees.Add(employee);
        }

        public void Delete(int id)
        {
            
        }

        public Employee? Get(int id)
        {
            return employees.FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<Employee> GetAll()
        {
            return employees.ToArray();
        }

        public void Update(Employee employee)
        {
            
        }

        public void SaveChanges()
        {

        }

        private int GetFreeId()
        {
            if (employees.Count > 0)
                return employees.Last().Id + 1;

            return 1;
        }

    }
}
