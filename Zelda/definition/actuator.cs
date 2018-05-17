using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zelda
{
    public abstract class actuator
    {
        public agent agent { get; set; }
        public abstract void actuate();
    }
}
