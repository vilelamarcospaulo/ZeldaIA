using System.Drawing;

namespace Zelda
{
    public abstract class ground
    {
        public int cost { get; set; }
        public Image image { get; set; }
        public string tag = "";

        public ground(int cost, Image image)
        {
            this.cost = cost;
            this.image = image;
        }
    }

    public class dungeonYES : ground
    {
        public dungeonYES() : base(10, Zelda.Properties.Resources.dungeonYES)
        {
        }
    }

    public class dungeonNO : ground
    {
        public dungeonNO() : base(0, Zelda.Properties.Resources.dungeonNO)
        {
        }
    }

    public class grass : ground
    {
        public grass() : base(10, Zelda.Properties.Resources.grass)
        {
        }
    }

    public class jungle : ground
    {   
        public jungle() : base(100, Zelda.Properties.Resources.jungle)
        {
        }
    }

    public class mountain : ground
    {
        public mountain() : base(150, Zelda.Properties.Resources.mountain)
        {
        }
    }

    public class sand : ground
    {
        public sand() : base(20, Zelda.Properties.Resources.sand)
        {
        }
    }

    public class water : ground
    {
        public water() : base(180, Zelda.Properties.Resources.water)
        {
        }
    }
}
