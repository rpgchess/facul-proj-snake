using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public partial class frmSnake : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();
        public frmSnake()
        {
            InitializeComponent();

            new Settings();

            gameTimer.Interval = 1000 / Settings.Speed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();

            StartGamer();
        }

        private void StartGamer()
        {
            lblGameOver.Visible = false;
            new Settings();
            Snake.Clear();
            Circle head = new Circle { x = 15, y = 5 };
            Snake.Add(head);

            txtScore.Text = Settings.Score.ToString();
            GenerateFood();
        }

        private void GenerateFood()
        {
            int masXPos = pbCanvas.Size.Width / Settings.Width;
            int masYPos = pbCanvas.Size.Height / Settings.Height;

            Random random = new Random();
            food = new Circle { x = random.Next(0,masXPos), y = random.Next(0,masYPos) };
        }

        private void UpdateScreen(object sender, EventArgs e)
        {
            if (Settings.GameOver)
            {
                if (Input.KeyPressed(Keys.Enter))
                    StartGamer();
            }
            else
            {
                if (Input.KeyPressed(Keys.Right) && Settings.direction != Direction.Left)
                    Settings.direction = Direction.Right;
                else if (Input.KeyPressed(Keys.Left) && Settings.direction != Direction.Right)
                    Settings.direction = Direction.Left;
                else if (Input.KeyPressed(Keys.Up) && Settings.direction != Direction.Down)
                    Settings.direction = Direction.Up;
                else if (Input.KeyPressed(Keys.Down) && Settings.direction != Direction.Up)
                    Settings.direction = Direction.Down;
                MovePlayer();
            }
            pbCanvas.Invalidate();
        }

        private void MovePlayer()
        {
            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Direction.Right: Snake[i].x++; break;
                        case Direction.Left: Snake[i].x--; break;
                        case Direction.Up: Snake[i].y--; break;
                        case Direction.Down: Snake[i].y++; break;
                    }

                    // Obtem o tamanho da tela
                    int maxXPos = pbCanvas.Size.Width / Settings.Width;
                    int maxYPos = pbCanvas.Size.Height / Settings.Height;

                    // Detecta colisão com as bordas
                    if (Snake[i].x < 0 || Snake[i].y < 0 || Snake[i].x >= maxXPos || Snake[i].y >= maxYPos)
                        Die();

                    // Detecta colisão com o corpo
                    for (int j = 1; j < Snake.Count; j++)
                        if (Snake[i].x == Snake[j].x && Snake[i].y == Snake[j].y)
                            Die();

                    // Detecta colisão com a comida
                    if (Snake[0].x == food.x && Snake[0].y == food.y)
                        Eat();
                }
                else
                {
                    Snake[i].x = Snake[i - 1].x;
                    Snake[i].y = Snake[i - 1].y;
                }
            }
        }

        private void Die()
        {
            Settings.GameOver = true;
        }

        private void Eat()
        {
            Circle circle = new Circle { x = Snake[Snake.Count - 1].x, y = Snake[Snake.Count - 1].y };
            Snake.Add(circle);

            Settings.Score += Settings.Points;
            txtScore.Text = Settings.Score.ToString();

            GenerateFood();
        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            if (!Settings.GameOver)
            {
                for (int i = 0; i < Snake.Count; i++)
                {
                    Brush snakeColor;
                    if (i == 0)
                        snakeColor = Brushes.Black;
                    else
                        snakeColor = Brushes.Green;
                    canvas.FillEllipse(snakeColor, new Rectangle(Snake[i].x * Settings.Width, Snake[i].y * Settings.Height, Settings.Width, Settings.Height));
                    canvas.FillEllipse(Brushes.Red, new Rectangle(food.x * Settings.Width, food.y * Settings.Height, Settings.Width, Settings.Height));
                }
                canvas.DrawRectangle(Pens.Black, new Rectangle(0, 0, pbCanvas.Size.Width-1, pbCanvas.Size.Height-1));
            }
            else
            {
                string gameOver = "Game Over! \nSeu placar final é: " + Settings.Score + "\nPressione Enter para tentar novamente.";
                lblGameOver.Text = gameOver;
                lblGameOver.Visible = true;

            }
        }

        private void frmSnake_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
        }

        private void frmSnake_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }
    }
}
