using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zelda
{
    public class lookAround : sensor
    {
        public override object getInformation(Point? p) //Return a list of tuples where first element of tuple is the side and the second is what has in the side 
        {
            Point pos = (p.HasValue ? p.Value : ((link)this.agent).pos);
            int M = this.agent.environment.map.GetLength(0);
            int N = this.agent.environment.map.GetLength(1);

            List<object[]> around = new List<object[]>() {};

            if (pos.X > 0) around.Add(new object[] { "U", this.agent.environment.map[pos.X - 1, pos.Y], new Point(pos.X - 1, pos.Y) }); //if link are not  in the left border add position at left
            if (pos.X < M - 1) around.Add(new object[] { "D", this.agent.environment.map[pos.X + 1, pos.Y], new Point(pos.X + 1, pos.Y) }); //if link are not  in the right border add position at right
            if (pos.Y > 0) around.Add(new object[] { "L", this.agent.environment.map[pos.X, pos.Y - 1], new Point(pos.X, pos.Y - 1) }); //if link are not  in the bottom border add position at bottom
            if (pos.Y < N - 1) around.Add(new object[] { "R", this.agent.environment.map[pos.X, pos.Y + 1], new Point(pos.X, pos.Y + 1) }); //if link are not  in the top border add position at top
            
            
            return around; 
        }   
    }
}
