using JsonConsole.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonConsole.Interfaces
{
    public interface IStorage<T>
    {
        public OperationResult<T> Add(T entity);

        public OperationResult<T> Update(T entity);

        public OperationResult<T> Get(int id);

        public OperationResult<T> Delete(int id);

        public OperationResult<T> GetAll();

        public OperationResult<T> Save();
    }
}
