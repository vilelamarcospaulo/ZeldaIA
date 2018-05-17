using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zelda
{
    public class kingdom : environment
    {
        public kingdom()
        {
            this.map = new ground[42, 42];
            this.loadFile("kingdom.map");
        }
    }

    public class dungeon : environment
    {
        public dungeon(int nDungeon)
        {
            this.map = new ground[28, 28];
            this.loadFile(string.Format("dungeon{0}.map", nDungeon));
        }
    }
}

