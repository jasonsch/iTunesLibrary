using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTunesShell.Interfaces
{
    public interface IMetaData
    {
        string ShortDescription { get; }
        string LongDescription { get; }
        string FileName { get; }
        //
        // TODO -- Get rid of LongDescription and add timestamp and size
        //
    }
}
