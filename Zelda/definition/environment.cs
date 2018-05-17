using System.IO;
using System.Collections.Generic;
using System.Drawing;

namespace Zelda
{
    public abstract class environment
    {
        public Dictionary<string, Point> GoalPositions;
        public ground[,] map { get; set; }
        
        private Point PosGoal(string goal)
        {
            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(1); j++)
                    if (map[i, j].tag == goal)
                        return new Point(i, j);

            return new Point(-1, -1);
        }

        public void loadFile(string file)
        {
            GoalPositions = new Dictionary<string, Point>();
            Dictionary<string, ground> ground = new Dictionary<string, ground>() { { "G", new grass() }, { "J", new jungle()},{ "M", new mountain()},{ "S", new sand()},{ "W", new water()}};
            StreamReader r = new StreamReader(file);
            
            for(int i = 0; i < this.map.GetLength(0); i++)
            {
                string[] buffer = r.ReadLine().Split('|');
                for (int j = 0; j < this.map.GetLength(1); j++)
                {
                    this.map[i, j] = (buffer[j] == "G") ? (ground)new grass() :
                                     (buffer[j] == "J") ? (ground)new jungle() :
                                     (buffer[j] == "M") ? (ground)new mountain() :
                                     (buffer[j] == "S") ? (ground)new sand() :
                                     (buffer[j] == "W") ? (ground)new water() :
                                     (buffer[j] == "Y") ? (ground)new dungeonYES() :
                                     (buffer[j] == "N") ? (ground)new dungeonNO() :
                                     null;
                }
            }

            //Last line goals
            var goals = r.ReadLine();
            goals = goals.Replace("{", "").Replace("}", "");

            foreach (string s in goals.Split(';'))
            {
                string tag = s.Substring(0, s.IndexOf("="));
                int x = int.Parse(s.Substring(s.IndexOf("(") + 1, s.IndexOf(",") - (s.IndexOf("(") + 1))) - 1;
                int y = int.Parse(s.Substring(s.IndexOf(",") + 1, s.IndexOf(")") - (s.IndexOf(",") + 1))) - 1;

                this.map[x, y].tag = tag.Trim();
                GoalPositions.Add(tag.ToUpper().Trim(), new Point(x, y));
            }


        }
    }
}