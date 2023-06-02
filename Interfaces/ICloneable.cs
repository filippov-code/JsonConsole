using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonConsole.Interfaces
{
    public interface ICloneable<T>
    {
        T Clone();
    }
}
