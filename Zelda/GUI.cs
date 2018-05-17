using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zelda
{
    public partial class GUI : Form
    {
        WMPLib.WindowsMediaPlayer sound = new WMPLib.WindowsMediaPlayer();
        int SQM_SIZE = 17;
        public link link;

        public GUI()
        {
            InitializeComponent();

            link = new link();
            link.environment = new kingdom();
            link.screen = this;
        }

        private void removeLinkMap(link link)
        {
            PictureBox pnl = (PictureBox)pnl_world.Controls["P" + link.pos.X.ToString() + "|" + link.pos.Y.ToString()];
            if (pnl.Controls.ContainsKey("pctLink"))
                pnl.Controls.RemoveByKey("pctLink");
            else
                pnl.Controls[0].Controls.RemoveByKey("pctLink");
        }

        private void putLinkMap(link link)
        {
            PictureBox pct = (PictureBox)pnl_world.Controls["P" + link.pos.X.ToString() + "|" + link.pos.Y.ToString()];
            if (pct.Controls.Count > 0)
                pct.Controls[0].Controls.Add(this.link.getPictureBox());
            else
                pct.Controls.Add(this.link.getPictureBox());
        }

        private void drawMap(environment e)
        {
            int N_SQM = e.map.GetLength(0);
            pnl_world.Controls.Clear();

            for (int i = 0; i < N_SQM; i++)
            {
                for (int j = 0; j < N_SQM; j++)
                {
                    PictureBox p = (new PictureBox()
                    {
                        Name = "P" + i.ToString() + "|" + j.ToString(),
                        Size = new Size(SQM_SIZE, SQM_SIZE),
                        Location = new Point((j * SQM_SIZE), (i * SQM_SIZE)),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Image = e.map[i, j].image
                    });
                    pnl_world.Controls.Add(p);

                    if (e.map[i, j].tag != "")
                    {
                        Image img = null;

                        switch (e.map[i, j].tag.Substring(0, 1))
                        {
                            case "D":
                                img = Zelda.Properties.Resources.dg;
                                break;

                            case "L":
                                img = Zelda.Properties.Resources.dg;
                                break;

                            case "M":
                                img = Zelda.Properties.Resources.ms;
                                break;
                        }

                        if (img != null) {
                            PictureBox p1 = (new PictureBox
                            {
                                Parent = p,
                                Name = "P" + i.ToString() + "|" + j.ToString() + "Object",
                                Size = new Size(SQM_SIZE, SQM_SIZE),
                                BackColor = Color.Transparent,
                                SizeMode = PictureBoxSizeMode.StretchImage,
                                Image = img
                            });

                            p.Controls.Add(p1);
                            p1.BringToFront();
                        }
                    }
                }
            }

            putLinkMap(this.link);
        }
        
        private void redrawMap(link link)
        {
            environment e = link.environment;

            int N_SQM = e.map.GetLength(0);
            
            for (int i = 0; i < 42; i++)
            {
                for (int j = 0; j < 42; j++)
                {
                    PictureBox p = (PictureBox)pnl_world.Controls["P" + i.ToString() + "|" + j.ToString()];
                    p.Controls.Clear();

                    if (i >= N_SQM || j >= N_SQM)
                    {
                        p.Image = null;
                        continue;
                    }

                    p.Image = e.map[i, j].image;

                    if (e.map[i, j].tag != "")
                    {
                        Image img = null;

                        switch (e.map[i, j].tag)
                        {
                            case "D":
                            case "D1":
                            case "D2":
                            case "D3":
                                img = Zelda.Properties.Resources.dg;
                                break;

                            case "PP":
                                img = Zelda.Properties.Resources.pendant_of_power;
                                break;

                            case "PC":
                                img = Zelda.Properties.Resources.pendant_of_courage;
                                break;

                            case "PW":
                                img = Zelda.Properties.Resources.pendant_of_wisdom;
                                break;

                            case "LW":
                                img = Zelda.Properties.Resources.lw;
                                break;

                            case "MS":
                                img = Zelda.Properties.Resources.ms;
                                break;
                        }

                        if (img != null)
                        {
                            PictureBox p1 = (new PictureBox
                            {
                                Parent = p,
                                Name = "P" + i.ToString() + "|" + j.ToString() + "Object",
                                Size = new Size(SQM_SIZE, SQM_SIZE),
                                BackColor = Color.Transparent,
                                SizeMode = PictureBoxSizeMode.StretchImage,
                                Image = img
                            });

                            p.Controls.Add(p1);
                            p1.BringToFront();
                        }
                    }
                }
            }

            putLinkMap(link);
            Application.DoEvents();
        }

        public void Console(string s)
        {
            txt_Console.AppendText(s + "\n");
            txt_Console.SelectionStart = txt_Console.Text.Length;
            txt_Console.ScrollToCaret();

            Application.DoEvents();
            Thread.Sleep(100);
        }

        private void moveLink(link link, string action)
        {
            removeLinkMap(link);

            link.action(action.ToLower());

            putLinkMap(link);

            Application.DoEvents();
            Thread.Sleep(200);
        }

        private void txt_Command_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string command = txt_Command.Text.ToUpper();

                txt_Command.Text = "";
                this.Console(command);

                if (command.Contains("LOAD"))
                {
                    sound.URL = @"music/default.mp3";
                    sound.controls.play();

                    drawMap(link.environment);
                }

                
                else if (command.Contains("GOTO"))
                {
                    command = command.Remove(0, command.IndexOf("("));
                    //
                    string goal = command.Substring(1, command.IndexOf(")") - 1);
                    Tuple<string, int> way = link.Astar(link.pos, link.environment.GoalPositions[goal]);
                    foreach (char c in way.Item1)
                        moveLink(this.link, c.ToString());

                    this.Console("-----------------------\nCaminho: " + way.Item1 + "\nCusto: " + way.Item2.ToString() + "\n-----------------------");
                }

                else if (command.Contains("SAVETHEKINGDOM"))
                {
                    int totalCost = 0;
                    List<string> bestWay = link.bestWay();
                    foreach(string s in bestWay)
                    {
                        Tuple<string, int> way = link.Astar(link.pos, link.environment.GoalPositions[s.ToUpper()]);
                        totalCost += way.Item2;
                        Console("---------------------------------------------------");
                        this.Console(string.Format("Indo para: {0}", s));
                        this.Console(string.Format("Caminho: {0}", way.Item1));
                        this.Console(string.Format("Custo do caminho: {0}", way.Item2));
                        this.Console(string.Format("Custo acumulado da viagem: {0}", totalCost));

                        foreach (char c in way.Item1)
                        {
                            moveLink(this.link, c.ToString());

                            if (link.environment.map[link.pos.X, link.pos.Y].tag.Contains("D")) //Inside the dungeon
                            {
                                sound.controls.stop();
                                sound.URL = @"music/dungeon.mp3";
                                sound.controls.play();

                                link lDungeon = new link();
                                lDungeon.environment = new dungeon(int.Parse(link.environment.map[link.pos.X, link.pos.Y].tag.Substring(1, 1)));
                                lDungeon.pos = lDungeon.environment.GoalPositions["D"];

                                redrawMap(lDungeon);

                                string goal = "";
                                foreach (string k in lDungeon.environment.GoalPositions.Keys)
                                {
                                    if (k.Substring(0, 1) == "P")
                                    {
                                        goal = k;
                                        break;
                                    }
                                }

                                this.Console("Localizando o amuleto...");
                                Tuple<string, int> way2 = lDungeon.Astar(lDungeon.pos, lDungeon.environment.GoalPositions[goal]);

                                Console("---------------------------------------------------");
                                totalCost += way2.Item2;
                                this.Console(string.Format("Indo para: {0}", goal));
                                this.Console(string.Format("Caminho: {0}", way2.Item1));
                                this.Console(string.Format("Custo: {0}", way2.Item2));
                                this.Console(string.Format("Custo acumulado da viagem: {0}", totalCost));

                                

                                foreach (char c2 in way2.Item1)
                                    moveLink(lDungeon, c2.ToString());
                                
                                this.Console("Amuleto capturado...");

                                ((PictureBox)pnl_world.Controls["P" + lDungeon.pos.X.ToString() + "|" + lDungeon.pos.Y.ToString()].Controls["P" + lDungeon.pos.X.ToString() + "|" + lDungeon.pos.Y.ToString() + "Object"]).Image = null;


                                Application.DoEvents();

                                way2 = lDungeon.Astar(lDungeon.pos, lDungeon.environment.GoalPositions["D"]);

                                Console("---------------------------------------------------");
                                totalCost += way2.Item2;
                                this.Console(string.Format("Indo para a saida"));
                                this.Console(string.Format("Caminho: {0}", way2.Item1));
                                this.Console(string.Format("Custo: {0}", way2.Item2));
                                this.Console(string.Format("Custo acumulado da viagem: {0}", totalCost));
                                
                                foreach (char c2 in way2.Item1)
                                    moveLink(lDungeon, c2.ToString());

                                redrawMap(link);
                                sound.controls.stop();
                                sound.URL = @"music/default.mp3";
                                sound.controls.play();
                            }
                        }
                    }

                    sound.controls.stop();
                    Console("---------------------------------------------------");
                    this.Console("Chegada ao objetivo");
                    this.Console(string.Format("Custo total da viagem: {0}", totalCost));
                    Console("---------------------------------------------------");
                }
            }
        }
    }
}
