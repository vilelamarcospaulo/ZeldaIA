using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zelda
{
    public class moveUp : actuator 
    {
        public override void actuate()
        {
            ((link)this.agent).pos.X--;
        }   
    }

    public class moveDown : actuator
    {
        public override void actuate()
        {
            ((link)this.agent).pos.X++;
        }
    }

    public class moveLeft : actuator
    {
        public override void actuate()
        {
            ((link)this.agent).pos.Y--;
        }
    }

    public class moveRight : actuator
    {
        public override void actuate()
        {
            ((link)this.agent).pos.Y++;
        }
    }
}
