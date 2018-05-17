using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zelda
{
    public abstract class sensor
    {
        public agent agent { get; set; }
        public abstract object getInformation(Point? p);
    }
}
