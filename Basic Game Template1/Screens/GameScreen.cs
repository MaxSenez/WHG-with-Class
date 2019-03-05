using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameSystemServices;
using System.Threading;

namespace WHGwoClass
{
    public partial class GameScreen : UserControl
    {
        //player1 button control keys 
        Boolean leftPressed, downPressed, rightPressed, upPressed;

        //used to draw all objects. Color will be changed depending on object
        SolidBrush drawBrush = new SolidBrush(Color.Black);

        List<GameArea> gameAreas = new List<GameArea>();   //playable areas
        List<GameArea> outAreas = new List<GameArea>();    //out of bounds areas
        List<Character> obstacles = new List<Character>(); //game obstacles

        //hero values
        int startX = 90, startY = 350;
        Character hero;

        public GameScreen()
        {
            InitializeComponent();
            InitializeGameValues();
        }

        public void InitializeGameValues()
        {
            //hero creation and start position
            hero = new Character(startX, startY, 20, 20, 5, Color.Black);

            //out of bounds areas
            outAreas.Add(new GameArea(0, 0, 50, 500, "out"));
            outAreas.Add(new GameArea(50, 0, 150, 250, "out"));
            outAreas.Add(new GameArea(150, 250, 50, 150, "out"));
            outAreas.Add(new GameArea(200, 0, 550, 50, "out"));
            outAreas.Add(new GameArea(550, 100, 50, 150, "out"));
            outAreas.Add(new GameArea(50, 450, 500, 50, "out"));
            outAreas.Add(new GameArea(550, 250, 150, 250, "out"));
            outAreas.Add(new GameArea(700, 50, 50, 450, "out"));

            //obstacles on screen
            obstacles.Add(new Character(262, 62, 24, 24, 6, "down", Color.Black));
            obstacles.Add(new Character(362, 62, 24, 24, 6, "down", Color.Black));
            obstacles.Add(new Character(462, 62, 24, 24, 6, "down", Color.Black));
            obstacles.Add(new Character(312, 412, 24, 24, 6, "up", Color.Black));
            obstacles.Add(new Character(412, 412, 24, 24, 6, "up", Color.Black));

            //in bounds areas (keep order of start, entry, end, exit, and play in same
            //order for any new levels).
            gameAreas.Add(new GameArea(50, 250, 100, 200, "start", Color.LightGreen));
            gameAreas.Add(new GameArea(150, 400, 50, 50, "entry", Color.White));
            gameAreas.Add(new GameArea(600, 50, 100, 200, "end", Color.LightGreen));            
            gameAreas.Add(new GameArea(550, 50, 50, 50, "exit", Color.White));
            gameAreas.Add(new GameArea(200, 50, 350, 400, "play", Color.White));
        }

        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // opens a pause screen is escape is pressed. Depending on what is pressed
            // on pause screen the program will either continue or exit to main menu
            if (e.KeyCode == Keys.Escape && gameTimer.Enabled)
            {
                gameTimer.Enabled = false;
                rightPressed = leftPressed = upPressed = downPressed = false;

                DialogResult result = PauseForm.Show();

                if (result == DialogResult.Cancel)
                {
                    gameTimer.Enabled = true;
                }
                else if (result == DialogResult.Abort)
                {
                    MainForm.ChangeScreen(this, "MenuScreen");
                }
            }

            //player 1 button presses
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftPressed = true;
                    break;
                case Keys.Down:
                    downPressed = true;
                    break;
                case Keys.Right:
                    rightPressed = true;
                    break;
                case Keys.Up:
                    upPressed = true;
                    break;
            }
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            //player 1 button releases
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftPressed = false;
                    break;
                case Keys.Down:
                    downPressed = false;
                    break;
                case Keys.Right:
                    rightPressed = false;
                    break;
                case Keys.Up:
                    upPressed = false;
                    break;
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //store current position of hero before moves are processed in case there is need to revert
            int tempX = hero.x;
            int tempY = hero.y;

            // move main character 
            if (leftPressed == true) { hero.Move("left"); }
            if (downPressed == true) { hero.Move("down"); }
            if (rightPressed == true) { hero.Move("right"); }
            if (upPressed == true) { hero.Move("up"); }

            // check if hero has gone out of bounds and reverse the move if they have
            foreach(GameArea g in outAreas)
            {
                if (hero.Collision(g))
                {
                    hero.x = tempX;
                    hero.y = tempY;
                }
            }

            // change the direction of obstacles when it's time
            foreach(Character c in obstacles)
            {
                c.Move(c.direction);

                if (c.direction == "down" && c.y >=412)
                {
                    c.direction = "up";
                }
                else if (c.direction == "up" && c.y <=62)
                {
                    c.direction = "down";
                }

            }

            // if the hero collides with an obstacle reset them to the start area
            foreach(Character c in obstacles)
            {
                if (hero.Collision(c))
                {
                    hero.x = startX;
                    hero.y = startY;
                }
            }

            // check for end of level, must be completely in end area, thus
            // colliding with end(index 2) and not colliding with exit(index 3)
            if (hero.Collision(gameAreas[2]) && !hero.Collision(gameAreas[3]))
            {
                hero.x = startX;
                hero.y = startY;
            }

            Refresh();
        }

        private void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            // draw all game areas
            foreach (GameArea g in gameAreas)
            {
                drawBrush.Color = g.color;
                e.Graphics.FillRectangle(drawBrush, g.x, g.y, g.width, g.height);
            }

            // draw hero
            drawBrush.Color = hero.color;
            e.Graphics.FillRectangle(drawBrush, hero.x, hero.y, hero.width, hero.height);

            //draw all obstacles
            foreach (Character c in obstacles)
            {
                drawBrush.Color = c.color;
                e.Graphics.FillEllipse(drawBrush, c.x, c.y, c.width, c.height);
            }
        }
    }
}
