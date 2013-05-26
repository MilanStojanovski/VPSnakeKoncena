using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Runtime;

namespace SnakeVP
{
    [Serializable]    
    public class SnakeFood
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Radius { get; set; }//golemina na strana ili radius
        public bool isEaten { get; set; }
        Random x;
        Random y;
        Color brush;
        public bool Special { get; set; }
        Random a;

        public SnakeFood(int Rad)
        {
            x = new Random();
            y = new Random();
            Radius = Rad;
            isEaten = false;
            a = new Random();
            brush = (Color.Red);
        }

        public void CheckIfEaten(SnakeDoc snake)
        {
            if (snake.head.X == X && snake.head.Y == Y)
                isEaten = true;
        }

        public void Draw(Graphics g)
        {
            Brush brushB = new SolidBrush(brush) ;
            if (Special == true)
            {
                brushB = new SolidBrush(Color.FromArgb(a.Next(0,255),a.Next(0,255),a.Next(0,255)));
            }
            g.FillEllipse(brushB, X * Radius, Y * Radius, Radius, Radius);
        }
    }
}
