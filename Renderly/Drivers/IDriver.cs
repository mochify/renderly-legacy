using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renderly.Drivers
{
    /// <summary>
    /// This interface spcifies a hierarchy of Renderly 'engine' drivers for classes that
    /// use the MVC componentry to do (stuff)
    /// </summary>
    public interface IDriver
    {
        void Execute();
    }
}
