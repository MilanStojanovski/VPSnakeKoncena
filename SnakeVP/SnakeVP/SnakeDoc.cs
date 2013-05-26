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
    public class SnakeDoc
    {
        public const int SIZE = 30;
        public List<SnakePart> body;
        public SnakePart head;
        public int  Width{ get; set; }
        public int  Height { get; set; }
        public int score { get; set; }
        public int size { get; set; }
        public int difficulty { get; set; }
        public bool IsOutOfBounds { get; set; }
        public bool foodIsEaten { get; set; }

        public SnakeDoc(int x, int y,int s)
        {
            body = new List<SnakePart>();
            size = s;
            head = new SnakePart(x, y, Direction.LEFT, true,size);
            body.Add(head);
            SnakePart tail = new SnakePart(x + 1, y, Direction.LEFT, false, size);
            body.Add(tail);
            score = 0;
            IsOutOfBounds = false;
            foodIsEaten = false;
        }

        public void Move()
        {
            int prevX = head.X;
            int prevY = head.Y;
            Direction prevD = head.direction;
            if (head.direction == Direction.UP)
            {
                head.Y -= 1;
                if (head.Y < 1)
                    IsOutOfBounds = true;
                    //head.Y = Height;
            }
            else if (head.direction == Direction.DOWN)
            {
                head.Y += 1;
                if (head.Y > Height)
                    IsOutOfBounds = true;
                    //head.Y = 1;
            }
            else if (head.direction == Direction.LEFT)
            {
                head.X -= 1;
                if (head.X < 0)
                    IsOutOfBounds = true;
                    //head.X = Width;
            }
            else if (head.direction == Direction.RIGHT)
            {
                head.X += 1;
                if (head.X > Width)
                    IsOutOfBounds = true;
                    //head.X = 0;
            }
            foreach (SnakePart p in body)
            {
                int x, y;
                Direction d;
                if (p != head)
                {
                    x = p.X;
                    y = p.Y;
                    d = p.direction;

                    p.X = prevX;
                    p.Y = prevY;
                    p.direction = prevD;

                    prevX = x;
                    prevY = y;
                    prevD = d;
                }
            }
        }

        public void ChangeDirection(Direction newD)
        {
            SnakePart part = body.ElementAt(1);

            if ((head.X == part.X && (newD == Direction.UP || newD == Direction.DOWN)) ||
                (head.Y == part.Y && (newD == Direction.LEFT || newD == Direction.RIGHT)))
                return;

            head.direction = newD;
        }
      
        public void Draw(Graphics g)
        {
            float halfSide = size / 2;
            float quarterSide = size / 4;
            float digonal = halfSide * 1.4142f;
            Direction HeadDirection = body.ElementAt(0).direction;

            // iscrtuvanje na glavata
            if (body.ElementAt(0).Y == body.ElementAt(1).Y)
            {
                if (HeadDirection == Direction.LEFT)
                    body.ElementAt(0).Draw(g, 0, quarterSide, size, halfSide, false, 225);
                else
                    body.ElementAt(0).Draw(g, 0, quarterSide, size, halfSide, false, 45);//g.FillPie(Cetka, body.ElementAt(0).X * StranaKvadrat, body.ElementAt(0).Y * StranaKvadrat + CetStr, StranaKvadrat, PolStr, 45, 270);
            }
            else
            {
                if (HeadDirection == Direction.UP)
                    body.ElementAt(0).Draw(g, quarterSide, 0, halfSide, size, false, 315);
                else
                    body.ElementAt(0).Draw(g, quarterSide, 0, halfSide, size, false, 135);
            }

            for (int i = 1; i < body.Count-1; i++)
            {
                // vertikala
                if (body.ElementAt(i - 1).X == body.ElementAt(i + 1).X)
                {
                    body.ElementAt(i).Draw(g, quarterSide, 0, halfSide, size);
                }
                // horizontala
                else if (body.ElementAt(i - 1).Y == body.ElementAt(i + 1).Y)
                {
                    body.ElementAt(i).Draw(g, 0, quarterSide, size, halfSide);
                }
                // pod 45 stepeni cetiri slucai
                else
                {
                    g.TranslateTransform(halfSide + body.ElementAt(i).X * size, size * body.ElementAt(i).Y);

                    if (body.ElementAt(i - 1).X > body.ElementAt(i + 1).X &&
                        body.ElementAt(i - 1).Y > body.ElementAt(i + 1).Y ||
                        body.ElementAt(i + 1).X > body.ElementAt(i - 1).X &&
                        body.ElementAt(i + 1).Y > body.ElementAt(i - 1).Y)
                    {
                        g.RotateTransform(-45);
                        if (body.ElementAt(i + 1).X == body.ElementAt(i).X && body.ElementAt(i + 1).Y < body.ElementAt(i).Y
                            || body.ElementAt(i - 1).X == body.ElementAt(i).X && body.ElementAt(i - 1).Y < body.ElementAt(i).Y)
                        {
                            // gore desno
                            body.ElementAt(i).Draw(g, -quarterSide, 0, halfSide, digonal, true);
                        }
                        else
                        {
                            // dolu levo
                            body.ElementAt(i).Draw(g, -size, 0, halfSide, digonal, true);
                        }
                        g.RotateTransform(45);
                    }
                    else
                    {
                        g.RotateTransform(+45);
                        if (body.ElementAt(i + 1).X == body.ElementAt(i).X && body.ElementAt(i + 1).Y < body.ElementAt(i).Y
                            || body.ElementAt(i - 1).X == body.ElementAt(i).X && body.ElementAt(i - 1).Y < body.ElementAt(i).Y)
                        {
                            // gore levo
                            body.ElementAt(i).Draw(g, -quarterSide, 0, halfSide, digonal, true);
                        }
                        else
                        {
                            // dolu desno
                            body.ElementAt(i).Draw(g, halfSide, 0, halfSide, digonal, true);
                        }
                        g.RotateTransform(-45);
                    }

                    g.TranslateTransform(-halfSide - body.ElementAt(i).X * size, -size * body.ElementAt(i).Y);
                }
            }

            // opaska
            if (!foodIsEaten)
            {
                int sizeBody = body.Count;
                if (body.ElementAt(sizeBody - 1).Y == body.ElementAt(sizeBody - 2).Y)
                    body.ElementAt(sizeBody - 1).Draw(g, 0, quarterSide, size, halfSide);
                else
                    body.ElementAt(sizeBody - 1).Draw(g, quarterSide, 0, halfSide, size);
            }
            else
                foodIsEaten = false;
        }

        public void AddPart(Color col)
        {
            SnakePart prevTail = body.ElementAt(body.Count - 1);
            SnakePart tail = new SnakePart(prevTail.X,prevTail.Y,prevTail.direction,false,size);
            tail.brush = col;
            body.Add(tail);
            score += 3;
        }

        public bool IsDead()
        {
            foreach (SnakePart part in body)
            {
                if (part != head)
                {
                    if (head.X == part.X && head.Y == part.Y)
                        return true;
                }
            }

            // dokolku zmijata izlezi od granicite
            if (IsOutOfBounds)
                return true;

            return false;
        }
    }
}
