using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zelda
{
    class tree
    {
        public Point p;
        public string move;
        
        public int gn, totalcost;
        public tree parent;
        
        public tree(tree parent, int gn, int totalcost, Point p, string move)
        {
            this.p = p;
            this.gn = gn;
            this.move = move;
            this.parent = parent;
            this.totalcost = totalcost;
        }
    }
}
