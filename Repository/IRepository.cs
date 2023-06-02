using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonConsole.Repository
{
    internal interface IRepository
    {
        void Add(Employee employee);
        void Update();
        void Delete(int id);
        Employee? Get(int id);
        IEnumerable<Employee> GetAll();
    }
}
