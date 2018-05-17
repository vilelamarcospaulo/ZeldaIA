using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zelda
{
    public class link : agent
    {
        public GUI screen;
        public Point pos;
        public link() : base(new List<actuator> { new moveUp(), new moveDown(), new moveLeft(), new moveRight() },
                             new List<sensor> { new lookAround() })
        {
            foreach (sensor s in this.sensors)
                s.agent = this;

            foreach (actuator a in this.actuators)
                a.agent = this;
            
            this.pos = new Point(27, 24); //Set link at home
        }

        public override void action(object Specification)
        {
            int actuatorIndex = -1;
            
            if (((string)Specification).ToLower() == "u")
                actuatorIndex = 0;
            else if (((string)Specification).ToLower() == "d")
                actuatorIndex = 1;
            else if(((string)Specification).ToLower() == "l")
                actuatorIndex = 2;
            else if(((string)Specification).ToLower() == "r")
                actuatorIndex = 3;

            if(actuatorIndex != -1)
                this.actuators[actuatorIndex].actuate();
        }

        public override object sense(Point? p)
        {
            return this.sensors[0].getInformation(p.HasValue ? p.Value : this.pos);
        }

        public Dictionary<Point, int> heuristic(Point goal)
        {
            Dictionary<Point, int> h = new Dictionary<Point, int>();

            for (int i = 0; i < this.environment.map.GetLength(0); i++)
                for (int j = 0; j < this.environment.map.GetLength(1); j++)
                    h.Add(new Point(i, j), (int)Math.Sqrt(Math.Pow(goal.X - i, 2) + Math.Pow(goal.Y - j, 2)));

            return h;
        }

        public Tuple<string, int> Astar(Point position, Point goal) //A star algorithm, to move link
        {
            int total_cost = 0;
            string way = "";

            Dictionary<Point, int> h = heuristic(goal);
           
            tree node = new tree(null, 0, 0, position, "");

            List<tree> tree = new List<tree>() { node };

            List<Point> visited = new List<Point>();
            List<Point> open = new List<Point>();

            Dictionary<Point, int> totalCost = new Dictionary<Point, int>();

            while (node.p != goal)
            {
                node = tree[0];                
                tree.RemoveAt(0);
                open.Remove(node.p);

                if (!visited.Contains(node.p))
                    visited.Add(node.p);
                
                List<object[]> sense = (List<object[]>)this.sense(node.p); //list of possibles moves
                sense = sense.Where((vector) => vector[1].GetType() != typeof(dungeonNO)).ToList();
                foreach (object[] t in sense)
                {
                    int gn = node.gn + ((ground)t[1]).cost;
                    Point p = (Point)t[2];
                    //if (!less.ContainsKey(p)) less.Add(p, gn + 1);
                    //if(less[p] > gn)
                    if (visited.Contains(p))
                        continue;

                    if (open.Contains(p))
                    { 
                        if(totalCost[p] > gn)
                        {
                            for(int i = 0; i < tree.Count; i++)
                            {
                                if(tree[i].p == p)
                                {
                                    tree.RemoveAt(i);
                                    tree.Add(new tree(node, gn, h[p], p, (string)t[0]));
                                    totalCost[p] = gn;
                                    break;
                                }
                            }           
                        }
                    }
                    else
                    {
                        open.Add(p);
                        tree.Add(new tree(node, gn, gn + h[p], p, (string)t[0]));
                        totalCost.Add(p, gn);
                    }
                }

                tree = tree.AsEnumerable().OrderBy((t) => t.totalcost).ToList();
            }

            total_cost = node.gn;
            while(node != null)
            {
                way = node.move + way;
                node = node.parent;
            }
            
            return new Tuple<string, int>(way, total_cost);
        }
        
        private int[,] randomHamiltonian(int[,] graph)
        {
            int n = graph.GetLength(0);
            int[,] Hamiltonian = new int[n, n];
            List<int> Cvetex = new List<int> { };

            int v1 = (new Random()).Next(n);
            int v2 = (new Random()).Next(n);
            while (v1 == v2)
                v2 = (new Random()).Next(n);

            Cvetex.Add(v1);
            Cvetex.Add(v2);
            Hamiltonian[v1, v2] = graph[v1, v2];
            Hamiltonian[v2, v1] = graph[v2, v1];

            for (int k = 0; k < n - 2; k++) //chose 2 randoms edges to build a starter way
            {
                for (int i = 0; i < n; i++)
                {
                    if (!Cvetex.Contains(i) && graph[v2, i] != 0)
                    {
                        v1 = v2;
                        v2 = i;
                        Cvetex.Add(v2);
                        i = n;

                        Hamiltonian[v1, v2] = graph[v1, v2];
                        Hamiltonian[v2, v1] = graph[v2, v1];
                    }
                }
            }

            Hamiltonian[v2, Cvetex[0]] = graph[v2, Cvetex[0]];
            Hamiltonian[Cvetex[0], v2] = graph[Cvetex[0], v2];

            return Hamiltonian;
        }

        private List<string> getWay(int[,] hamiltonian)
        {
            int[,] h = (int[,])hamiltonian.Clone();

            List<string> Cvetex = new List<string> { };
            int k = 0;
            for (int j = 0; j < h.GetLength(0); j++)
            {
                if (h[k, j] != 0 && !Cvetex.Contains(k == 0 ? "H" : "D" + k.ToString()))
                {
                    h[j, k] = 0;
                    Cvetex.Add(k == 0 ? "H" : "D" + k.ToString());
                    k = j;
                    j = 0;

                    if (Cvetex.Count == 3)
                    {
                        Cvetex.Add(k == 0 ? "H" : "D" + k.ToString());
                        break;
                    }
                }
            }

            Cvetex.Add(Cvetex[0]);
            return Cvetex;
        }

        public List<string> bestWay() //Uses iterative search
        {
            screen.Console("---------------------------------------------------");
            screen.Console("Iniciando escolha melhor caminho");

            //V0 = Link's house, Vn = Dungeon N
            int n = 4;
            int[,] graph = new int[n, n];

            for(int i = 0; i < n; i++)
            {
                string s = i == 0 ? "H" : "D" + i.ToString();
                for (int j = i; j < n; j++)
                {
                    string goal = j == 0 ? "H" : "D" + j.ToString();

                    graph[i, j] = Astar(environment.GoalPositions[s], environment.GoalPositions[goal]).Item2;
                    graph[j, i] = graph[i, j];
                }
            }

            screen.Console("Sorteando caminho aletorio ...");
            int[,] Hamiltonian = randomHamiltonian(graph);
            
            List<int[]> UsedEdges = new List<int[]>();
            int CurrentCost = 0;

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (i != j && Hamiltonian[i, j] == graph[i, j])
                        UsedEdges.Add(new int[] { i, j });
            
            while (UsedEdges.Count > 0)
            {
                screen.Console("---------------------------------------------------");
                CurrentCost = 0;
                int FutureCost = 0;

                int[] e1 = UsedEdges[0], e2 = null;
                UsedEdges.Remove(e1);
                UsedEdges = UsedEdges.Where((e) => e[0] != e1[1] || e[1] != e1[0]).ToList();

                for (int i = 0; i < n; i++)
                    for (int j = i; j < n; j++)
                        if (graph[i, j] == Hamiltonian[i, j])
                            CurrentCost += graph[i, j];

                
                screen.Console(string.Format("Caminho atual: {0} custo = {1}", string.Join("-", getWay(Hamiltonian)), CurrentCost.ToString()));
                
                for (int i = 0; i < UsedEdges.Count; i++)
                {
                    int[] aux = UsedEdges[i];

                    if (aux[0] != e1[0] && aux[1] != e1[1] && aux[0] != e1[1] && aux[1] != e1[0])
                    {
                        e2 = aux;
                        UsedEdges.Remove(e2);
                        UsedEdges = UsedEdges.Where((e) => e[0] != e2[1] || e[1] != e2[0]).ToList();
                        break;
                    }
                }

                int[] e3 = new int[] { e1[0], e2[1] };
                int[] e4 = new int[] { e2[0], e1[1] };

                if(Hamiltonian[e3[0], e3[1]] != 0)
                {
                    e3 = new int[] { e1[0], e2[0] };
                    e4 = new int[] { e2[1], e1[1] };
                }


                FutureCost = CurrentCost;
                FutureCost -= graph[e1[0], e1[1]];
                FutureCost -= graph[e2[0], e2[1]];

                FutureCost += graph[e3[0], e3[1]];
                FutureCost += graph[e4[0], e4[1]];
    
                screen.Console(string.Format("Tentativa de troca das arestas ({0}) por ({1}) e ({2}) por ({3})", string.Join(",", e1).Replace("0", "H").Replace("1", "D1").Replace("2", "D2").Replace("3", "D3"), 
                                                                                                                    string.Join(",", e3).Replace("0", "H").Replace("1", "D1").Replace("2", "D2").Replace("3", "D3"),
                                                                                                                    string.Join(",", e2).Replace("0", "H").Replace("1", "D1").Replace("2", "D2").Replace("3", "D3"),
                                                                                                                    string.Join(",", e4).Replace("0", "H").Replace("1", "D1").Replace("2", "D2").Replace("3", "D3")));

                screen.Console(string.Format("Custo futuro: {0}", FutureCost));

                if (FutureCost >= CurrentCost)
                {
                    screen.Console("Custo atual melhor");
                    continue;
                }

                UsedEdges.Add(e3);
                UsedEdges.Add(e4);
                
                Hamiltonian[e1[0], e1[1]] = 0;
                Hamiltonian[e1[1], e1[0]] = 0;

                Hamiltonian[e2[0], e2[1]] = 0;
                Hamiltonian[e2[1], e2[0]] = 0;

                Hamiltonian[e3[0], e3[1]] = graph[e3[0], e3[1]];
                Hamiltonian[e3[1], e3[0]] = graph[e3[0], e3[1]];

                Hamiltonian[e4[0], e4[1]] = graph[e4[0], e4[1]];
                Hamiltonian[e4[1], e4[0]] = graph[e4[0], e4[1]];

                screen.Console("Troca efetuada");
            }

            List<string> way = getWay(Hamiltonian);

            screen.Console(string.Format("Caminho final: {0} custo = {1}", string.Join("-", way), CurrentCost.ToString()));
            way.Add("LW");
            return way;
        }
      
        public PictureBox getPictureBox()
        {
            return new PictureBox()
            {
                Size = new Size(17,17),
                Name = "pctLink",
                BackColor = Color.Transparent,
                Image = Properties.Resources.link,
                SizeMode = PictureBoxSizeMode.StretchImage
                
            };
        }
    }
}