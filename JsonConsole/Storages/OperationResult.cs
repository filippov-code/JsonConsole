using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonConsole.Storages
{
    public class OperationResult<T>
    {
        public bool Success { get; init; }
        public string Message { get; init; }
        public T[]? Result { get; init; }
        public static OperationResult<T> EmptyOk => new OperationResult<T>(true, "Ok", default);

        public OperationResult(bool succeded, string message, params T[]? result)
        {
            Success = succeded; 
            Message = message; 
            Result = result; 
        }
    }
}
