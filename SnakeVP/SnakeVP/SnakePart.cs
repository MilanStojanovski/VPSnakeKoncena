using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SnakeVP
{
    [Serializable]
    public enum Direction {UP,DOWN,LEFT,RIGHT}
    [Serializable]
    public class SnakePart
    {
        public int X { get; set; }  
        public int Y { get; set; }
        public int side { get; set; }//golemina na strana ili radius
        public Direction direction { get; set; }
        public bool isHead { get; set; }
        public Color brush { get; set; }
        public Color color{ get; set; }
         
        public SnakePart(int x, int y, Direction d,bool h,int s)
        {
            X = x;
            Y = y;
            side = 10;//default
            direction = d;
            isHead = h;
            color=(Color.GreenYellow);
            if (isHead == true)
                brush = (Color.Green);
            else
                brush = (Color.Black);
            side = s;
        }
        
        public void Draw(Graphics g, float dx, float dy, float dw, float dh, bool help = false, int degree = 0)
        {
            if (isHead == true)
            {
                g.FillPie(new SolidBrush(brush), X * side + dx, Y * side + dy, dw, dh, degree, 270);
            }
            else
            {
                if (help)
                {
                    g.FillEllipse(new SolidBrush(brush), dx, dy, dw, dh);
                    g.DrawEllipse(new Pen(color), dx, dy, dw, dh);
                }
                else
                {
                    g.FillEllipse(new SolidBrush(brush), X * side + dx, Y * side + dy, dw, dh);
                    g.DrawEllipse(new Pen(color), X * side + dx, Y * side + dy, dw, dh);
                }
            }
        }
    }
}
