using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frogger.GameManagement.LevelGenerator;
using Frogger.GameManagement.GameObj;
using Frogger.GameManagement;
using Frogger.Menu;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Xna.Framework.Input;

namespace Frogger.States
{
    /// <summary>
    /// A gamestate for displaying, loading and adding highscores
    /// </summary>
    class HighScoreScreen : GameObjectList
    {
        protected Button button1;
        private SpriteGameObject book, background, ragemeter, slider, submitBack;
        protected Camera cam;
        protected List<string> nameList;
        protected List<int> scoreList;
        protected int newScore,  replacableElement;
        protected bool scoreChecked, waitingForInput;
        protected TextGameObject message;
        protected Vector2 messagePosition;
        protected string input;

        public HighScoreScreen()
        {
            book = new SpriteGameObject("Menu/Book");
            book.Position = new Vector2(1, 1);
            Add(book);

            background = new SpriteGameObject("Menu/Background");
            background.Position = new Vector2(book.Position.X + book.Sprite.Width, book.Position.Y - 16);
            Add(background);

            ragemeter = new SpriteGameObject("Menu/Ragemeter");
            ragemeter.Position = new Vector2(background.Position.X + background.Sprite.Width, background.Position.Y + 16);
            Add(ragemeter);

            slider = new SpriteGameObject("Menu/Slider");
            slider.Position = new Vector2(ragemeter.Position.X + 3, ragemeter.Sprite.Height / 2);
            Add(slider);

            button1 = new Button("Menu/Button Return to Menu", 0, 2, "");
            button1.Position = new Vector2(background.Sprite.Width - button1.BoundingBox.Width + background.Position.X, 540- button1.BoundingBox.Height);
            Add(button1);

            cam = new Camera();
            Add(new Cursor());

            messagePosition = new Vector2(background.Position.X + 64, 416);
            message = new TextGameObject("", messagePosition, color: Color.Black);

            nameList = new List<string>();
            scoreList = new List<int>();
            input = string.Empty;
            ScoreReader();
            AddScoreList(true);
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            if (inputHelper.MouseLeftButtonPressed() && button1.IsOnButton)
            {
                GameEnvironment.GameStateManager.SwitchTo("menuScreen");
            }

            if(waitingForInput)
            {
                if(inputHelper.KeyDown(Keys.Enter) && input != string.Empty)
                {
                    waitingForInput = false;
                    AddHighScore();
                }
                char inputChar;
                if(inputHelper.TryConvertKeyboardInput(out inputChar, true))
                    input += inputChar;
                if (inputHelper.KeyPressed(Keys.Back) && input.Count() > 0)
                    input = input.Remove(input.Count()-1, 1);
                if(input.Count()>3)
                    input = input.Remove(input.Count() - 1, 1);
            }
        }


        public override void Update(GameTime gameTime)
        {
            if(!waitingForInput)
                base.Update(gameTime);
            if (scoreChecked || waitingForInput)
                AddHighScore();
            if (submitBack != null)
                submitBack.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: cam.Transform);
            base.Draw(gameTime, spriteBatch);
            if (submitBack != null)
                submitBack.Draw(gameTime, spriteBatch);
            message.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }

        public void CheckHighScore()
        {
            if (!scoreChecked && !waitingForInput)
            {
                int i = 0;
                foreach (int score in scoreList)
                {
                    if (newScore > score)
                    {
                        waitingForInput = true;
                        scoreChecked = true;
                        replacableElement = i;
                        return;
                    }
                    i++;
                }
            }
            newScore = 0;
        }

        public void AddHighScore()
        {
            if (submitBack == null)
                submitBack = new SpriteGameObject("Menu/Textbox 7x1");
            submitBack.Position = messagePosition + new Vector2(-16, -4);
            if (scoreChecked)
            {
                int x = 9;
                while (x > replacableElement)
                {
                    scoreList[x] = scoreList[x - 1];
                    nameList[x] = nameList[x - 1];
                    x--;
                }
                scoreList[replacableElement] = newScore;
                nameList[replacableElement] = "CRASHED";
                scoreChecked = false;
            }

            if(waitingForInput)
            {
                message.Text = ("Your name: "+ input);
            }
            else if(input != string.Empty)
            {
                nameList[replacableElement] = input;
                ScoreWriter();
                AddScoreList();
                input = string.Empty;
                message.Text = "";
                submitBack = null;
                newScore = 0;
            }
        }

        public void AddScoreList(bool newList = false)
        {
            if(newList)
            {
                SpriteGameObject addingObject;
                for (int i = 0; i < 10; i++)
                {
                    addingObject = new SpriteGameObject("Menu/Textbox 7x1");
                    addingObject.Position = new Vector2(128 + background.Position.X - 16, i * 32 + 28);
                    Add(addingObject);
                }
            }
            if(children != null)
            {
                List<TextGameObject> removable = children.OfType<TextGameObject>().ToList();

                foreach (TextGameObject item in removable)
                    DeleteChild(item);
            }

            string spaces = "   ";
            for (int i = 0; i < 10; i++)
            {
                if (i == 9)
                    spaces = "  ";
                Add(new TextGameObject(i + 1 + spaces + nameList[i] + "  " + scoreList[i], new Vector2(128 + background.Position.X, i * 32 + 32), color: Color.Black));
            }
        }

        public void NoSubmit()
        {
            waitingForInput = false;
            scoreChecked = false;
        }

        public void ScoreReader()
        {
            StreamReader fileReader;
            string[] highScores;
            int scoreLines;

            scoreLines = File.ReadLines("content/HighScores.txt").Count();
            fileReader = new StreamReader("content/HighScores.txt");
            highScores = new string[scoreLines];

            for (int i = 0; i < scoreLines; i++)
            {
                highScores[i] = fileReader.ReadLine();
            }

            foreach (string highScore in highScores)
            {
                nameList.Add(highScore.Split(',')[0]);
                scoreList.Add(int.Parse(highScore.Split(',')[1]));
            }
            fileReader.Close();
        }

        public void ScoreWriter()
        {
            StreamWriter fileWriter;
            string name;
            int score;

            fileWriter = new StreamWriter("content/HighScores.txt");

            for (int i = 0; i < 10; i++)
            {
                name = nameList[i];
                score = scoreList[i];
                fileWriter.WriteLine(name + "," + score);
            }
            fileWriter.Close();
        }

        public int NewScore
        {
            set { newScore = value; }
        }

    }
}
