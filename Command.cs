/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;*/

namespace Spider_Solitaire
{
    public abstract class Command
    {
        public abstract void execute();
        public abstract void undo();
    }
}
