using System;
using System.Collections.Generic;
using System.Drawing;

namespace Zelda
{
    public abstract class agent
    {
        public environment environment { get; set; }
        public List<actuator> actuators { get; set;  }
        public List<sensor> sensors { get; set; }
        
        public agent(List<actuator> a, List<sensor> s)
        {
            this.actuators = a;
            this.sensors = s;
        }

        public abstract object sense(Point? p);
        public abstract void action(object Specification);
    }
}
