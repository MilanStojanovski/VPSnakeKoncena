using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SnakeVP
{
    public partial class Form1 : Form
    {
        string FileName;
        public SnakeDoc snake;
        public static readonly int TIMER = 10;//for special food
        public SnakeFood food;
        public SnakeFood foodSpec;
        public bool [][] foods;//matrix of foods 
        Timer timer;//timer1
        Timer timerSpec;//timer for spec food
        bool pause=false;//pause
        int secondsLeft = TIMER;//time remaining for spec food
        int secondsElapsed = 0;//time elapsed
        int minutesElapsed = 0;//
        int sec = 10;
        public static readonly int WIDTH=30, HEIGHT=25;
        Random x;
        Color bodyColor=Color.Black;
        public int SIZE { get; set; }
        public static readonly int EASY=1;
        public static readonly int MEDIUM = 2;
        public static readonly int HARD = 5;
     
        public Form1()
        {
            InitializeComponent();
            x = new Random();
            DoubleBuffered = true;
            NewGame();
        }

        public void NewGame()
        {
            SIZE = 20;
            sec = 10;
            secondsLeft = 10;
            pause = false;
            snake = new SnakeDoc(6, 8,SIZE);//snake init
            menuStrip1.AutoSize = false;
            menuStrip1.Height = SIZE;
            this.ClientSize = new Size((WIDTH+1) * snake.size, (HEIGHT+1) * (snake.size)+toolStrip1.Height);

            bodyColor = Color.Black;
            snake.Width = WIDTH;
            snake.Height = HEIGHT;

            timer = new Timer();//timer
            timerSpec = new Timer();//timerSpec

            food = new SnakeFood(SIZE);//food
            foodSpec = null;
            foods = new bool[HEIGHT+1][];//matrix
            secondsElapsed = 0;
            minutesElapsed = 0;
            
            timer.Interval = (100);//NE GO MENVAJ TIMEROT zaradi vremetraenje ke se izmeni
            timer.Start();
            timer.Tick += timer_Tick;               

            toolStripLabel2.Text = "0";
            toolStripLabel3.Text = secondsLeft.ToString();
          
            for (int i = 1; i < HEIGHT+1; ++i)
            {
                foods[i] = new bool[WIDTH+1];
                for (int j = 0; j < WIDTH+1; ++j)
                {
                    foods[i][j] = false;
                }
            }
            foods[8][6] = true;//head
            foods[8][7] = true;
            GenerateFood();
            timerSpec.Interval = 1000;
            timerSpec.Stop();
            timerSpec.Tick += timerSpec_Tick;
            toolStripLabel3.Text = secondsLeft.ToString();
        }

        void timerSpec_Tick(object sender, EventArgs e)
        {
            secondsLeft--;
            if (secondsLeft == 0)
            {
                timerSpec.Stop();
                foodSpec = null;
                secondsLeft = 10;
            }
            toolStripLabel3.Text = secondsLeft.ToString();
        }

        public void GenerateFood()
        {
            foreach (SnakePart part in snake.body)
            {
                foods[part.Y][part.X] = true;
            }

            int coorX, coorY, coorSpecX, coorSpecY;
            coorX = x.Next(0,WIDTH);
            coorY = x.Next(1,HEIGHT);
           
            /*while (foods[coorY][coorX] != false)
            {
                coorX = x.Next(0, WIDTH);
                coorY = y.Next(1, HEIGHT);               
            }*/

            if (foods[coorY][coorX] == true)
            {
                GenerateFood();
            }

            food.X = coorX;
            food.Y = coorY;
            foods[food.Y][food.X] = true;
            if(snake.body.Count%5==0)
            {
                coorSpecX=x.Next(0,WIDTH);
                coorSpecY=x.Next(1,HEIGHT);
                while (foods[coorSpecY][coorSpecX] != false)
                {
                    coorSpecX = x.Next(0,WIDTH);
                    coorSpecY = x.Next(1,HEIGHT);
                }
                timerSpec.Start();
                foodSpec=new SnakeFood(SIZE);
                foodSpec.Special = true;
                foodSpec.X=coorSpecX;
                foodSpec.Y=coorSpecY;
                foods[foodSpec.Y][foodSpec.X]=true;
            }

            foreach (SnakePart part in snake.body)
            {
                foods[part.Y][part.X] = false;
            }
        }
       
        void timer_Tick(object sender, EventArgs e)
        {
            //game over
            if (snake.IsDead() == true)
            {
                timer.Stop();
                timerSpec.Stop();
                MessageBox.Show("Your score is "+  snake.score,"Elapsed time : "+string.Format("{0:00}:{1:00}",minutesElapsed,secondsElapsed));
                DialogResult result=MessageBox.Show("New game?", "You lost.", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    NewGame();
                }
                else
                    this.Close();
            }
            //elapsed time
            sec--;
            if (sec == 0)
            {
                sec = 10;
                secondsElapsed+=1;
                if(secondsElapsed%60==0)
                {
                    secondsElapsed=0;
                    minutesElapsed+=1;
                }
            }
            //snake movement and food status
           
            snake.Move();
           
            food.CheckIfEaten(snake);
            if (foodSpec != null)
            {
                foodSpec.CheckIfEaten(snake);
                if (foodSpec.isEaten == true)
                {
                    timerSpec.Stop();
                    snake.score += secondsLeft * 3;
                    secondsLeft = 10;
                    toolStripLabel3.Text = secondsLeft.ToString();
                    foods[foodSpec.Y][foodSpec.X] = false;
                    foodSpec = null;
                }
            }
            if (food.isEaten == true )
            {
                snake.AddPart(bodyColor);
                food = new SnakeFood(SIZE);
                GenerateFood();
                snake.foodIsEaten = true;
            }
            Invalidate(true);
        }
            
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            snake.Draw(e.Graphics);
            food.Draw(e.Graphics);
            if (foodSpec != null && foodSpec.isEaten != true)
                foodSpec.Draw(e.Graphics);
            toolStripLabel2.Text = snake.score.ToString();
            toolStripLabel8.Text = snake.body.Count.ToString();
            toolStripLabel5.Text = string.Format("{0:00}:{1:00}", minutesElapsed, secondsElapsed);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Up)
                snake.ChangeDirection(Direction.UP);
            else if (e.KeyCode == Keys.Down)
                 snake.ChangeDirection(Direction.DOWN);
            else if (e.KeyCode == Keys.Left )
                snake.ChangeDirection(Direction.LEFT);
            else if (e.KeyCode == Keys.Right)
                snake.ChangeDirection(Direction.RIGHT);
            else if (e.KeyCode == Keys.Space)
            {
                pause =!pause;
                if (pause == true)
                {
                    timerSpec.Stop();
                    timer.Stop();
                }
                else
                {
                    timer.Start();
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void changeHeadColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dial = new ColorDialog();
            DialogResult res = dial.ShowDialog();
            if (res == DialogResult.OK)
            {
                snake.head.brush = (dial.Color);
                Invalidate(true);
            }
        }
        
        private void changeBodyColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dial = new ColorDialog();
            DialogResult res = dial.ShowDialog();
            if (res == DialogResult.OK)
            {
                foreach (SnakePart part in snake.body)
                {
                    if (part != snake.head)
                    {
                        part.brush =(dial.Color);
                    }
                }
                bodyColor = dial.Color;
                Invalidate(true);
            }
        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void easyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void saveFile()
        {
            if (foodSpec == null)
            {
                if (FileName == null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Snake doc file (*.snk)|*.snk";
                    saveFileDialog.Title = "Save snake doc";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        FileName = saveFileDialog.FileName;
                    }
                }
                if (FileName != null)
                {
                    using (FileStream fileStream = new FileStream(FileName, FileMode.Create))
                    {
                        timer.Stop();
                      
                        IFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(fileStream, snake);
                        formatter.Serialize(fileStream, food);
                        formatter.Serialize(fileStream, minutesElapsed);
                        formatter.Serialize(fileStream, secondsElapsed);
                        formatter.Serialize(fileStream, bodyColor);
                    }
                }
            }
            else
            {
                MessageBox.Show("Cannot save during special food is on field.", "Error");
            }
        }
        private void openFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Snake file (*.snk)|*.snk";
            openFileDialog.Title = "Open snake doc file";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileName = openFileDialog.FileName;
                try
                {
                    using (FileStream fileStream = new FileStream(FileName, FileMode.Open))
                    {
                        IFormatter formater = new BinaryFormatter();
                        snake = (SnakeDoc)formater.Deserialize(fileStream);
                        food = (SnakeFood)formater.Deserialize(fileStream);
                       
                        minutesElapsed = (int)formater.Deserialize(fileStream);
                        secondsElapsed = (int)formater.Deserialize(fileStream);
                        bodyColor = (Color)formater.Deserialize(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not read file: " + FileName);
                    FileName = null;
                    return;
                }
                Invalidate(true);
            }
        }

        private void saveSaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFile();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFile();
        }
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Stop();
            timerSpec.Stop();
            pause = true;
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Stop();
            timerSpec.Stop();
            pause = true;
        }
    }
}
