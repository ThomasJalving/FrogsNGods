using Frogger.GameManagement.GameObj;
using Frogger.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace Frogger.GameManagement
{
    /// <summary>
    /// Handles the complete UI of the game, as well as the difficulty values;
    /// </summary>
    class God : GameObjectList
    {
        private int previousLives;
        public Stopwatch eventTime, questionTime, questionDelay;
        public enum ModifierType {dangerChance, cooldown, burstcooldown, speed};
        private int rageCounter, chanceGood, chanceBad, effectGood, effectBad, scaleGood, scaleBad;

        private TextGameObject godName, godReaction, godQuestion;
        List<Question> questions = new List<Question>();
        Question activeQuestion;
        int lastQuestion = -1;

        Dictionary<ModifierType, float> modifiers;

        public string BiomeName { get; private set; }

        private List<string> influencers;
        private List<string> events;
        private List<string> difficultyValues;
        private List<string> goodWords;
        private List<string> badWords;
        private InputHelper inputHelper;

        Random ran = new Random();

        SpriteGameObject book, ragemeter, slider, life;

        /// <summary>
        /// Class that handles UI and difficulty
        /// </summary>
        public God(string biomeName, InputHelper inputHelper)
        {
            this.inputHelper = inputHelper;
            this.BiomeName = biomeName;
            eventTime = new Stopwatch();
            questionDelay = new Stopwatch();
            questionTime = new Stopwatch();
            modifiers = new Dictionary<ModifierType, float>();
            
            //file reading
            List<string> textLines = new List<string>();
            StreamReader fileReader = new StreamReader("Content/Gods/" + biomeName + ".txt");          

            string line = fileReader.ReadLine();

            while (line != null)
            {
                textLines.Add(line);
                line = fileReader.ReadLine();       
            }

            influencers = textLines[1].Split(',').ToList(); //Influencers are the things that will get modified by this god
            for(int i = 0; i < influencers.Count; i++)
            {
                modifiers.Add((ModifierType)Enum.Parse(typeof(ModifierType), influencers[i]), 1f); //Add the influencers to the modifiers list so that the spawners can use it to change the difficulty                
            }         
            events = textLines[2].Split(',').ToList(); //Events are the things the god will respond to, such as playerDeath and entering a biome
            difficultyValues = textLines[3].Split(',').ToList(); //Difficulty values are the values that determine how events, answers and good/bad words will affect the difficulty
            SetValues();

            LoadQuestions();

            //The good and bad words a god will react to
            goodWords = LoadWordList("Content/Realms/" + biomeName + "/GoodWords.txt");
            badWords = LoadWordList("Content/Realms/" + biomeName + "/BadWords.txt");

            //UI elements
            book = new SpriteGameObject("Realms/" + biomeName + "/UI/Book");
            book.Position = new Vector2(1, 1);
            Add(book);

            ragemeter = new SpriteGameObject("Realms/" + biomeName + "/UI/Ragemeter");
            ragemeter.Position = new Vector2(GameEnvironment.Screen.X - ragemeter.Sprite.Width + 1, 1);
            Add(ragemeter);

            slider = new SpriteGameObject("Realms/" + biomeName + "/UI/Slider");
            slider.Position = new Vector2(ragemeter.Position.X + 3, 0);
            Add(slider);

            godName = new TextGameObject(textLines[0], new Vector2(book.Position.X + 120, 100), centered: false, color: Color.Black);
            Add(godName);

            godReaction = new TextGameObject("", new Vector2(book.Position.X + 70, 150), centered: false, color: Color.Black);
            Add(godReaction);

            godQuestion = new TextGameObject("", new Vector2(book.Position.X + 70, 300), centered: false, color: Color.Black);
            Add(godQuestion);
        }

        /// <summary>
        /// Sets the god values used for the rageCounter
        /// </summary>
        void SetValues()
        {
            try
            {
                rageCounter = int.Parse(difficultyValues[0]);
                chanceGood = int.Parse(difficultyValues[1]);
                chanceBad = int.Parse(difficultyValues[2]);
                effectGood = int.Parse(difficultyValues[3]);
                effectBad = int.Parse(difficultyValues[4]);
                scaleGood = int.Parse(difficultyValues[5]);
                scaleBad = int.Parse(difficultyValues[6]);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e + ": " + godName + "has incorrect difficulty values!");
                GameEnvironment.ExitGame = true;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e + ": " + godName + "doesn't have difficulty values!");
                GameEnvironment.ExitGame = true;
            }
        }

        /// <summary>
        /// Loads al the questions of a god, listing them for easy access 
        /// </summary>
        void LoadQuestions()
        {
            List<string> textLines = new List<string>();
            StreamReader fileReader = new StreamReader("Content/Realms/" + BiomeName + "/Questions.txt");
            string line = fileReader.ReadLine();

            while (line != null)
            {
                textLines.Add(line);
                line = fileReader.ReadLine();
            }

            for(int i = 0; i < textLines.Count / 7; i++) 
            {
                List<string> questionLines = new List<string>();
                for(int j = 0; j < 7; j++)
                {
                    questionLines.Add(textLines[i * 7 + j]);
                }
                questions.Add(new Question(questionLines.ToArray()));
            }
        }

        List<string> LoadWordList(string path)
        {
            StreamReader fileReader = new StreamReader(path);
            string words = fileReader.ReadToEnd();

            List<string> list = words.Split(',').ToList<string>();
            return list;
        }

        /// <summary>
        /// Adds or removes words from the speechlibrary
        /// </summary>
        public void UpdateWords(List<string> list, bool addRemoveSwitch = true)
        {
            foreach (string word in list)
            {
                if (addRemoveSwitch)
                    inputHelper.Speech.AddSentence(word);
                else
                    inputHelper.Speech.RemoveSentence(word);
            }
        }

        /// <summary>
        /// Makes sure a question is displayed and its answers are accepted by the speechlibrary 
        /// </summary>
        public void AskQuestion()
        {
            int count = questions.Count();
            int choice = ran.Next(count);
            if(choice == lastQuestion && count > 1)
            {
                choice = (choice + 1) % (count - 1);
            }
            activeQuestion = questions[choice];
            lastQuestion = choice;
            godQuestion.BoxText(activeQuestion.questionLine, book.Position.X + 378);

            foreach (string answer in activeQuestion.answerLines)
            {
                inputHelper.Speech.AddSentence(answer);
                godQuestion.BoxText($@"
{answer}", book.Position.X + 378, true);
            }
        }

        /// <summary>
        /// Reverts the questiontext back to null and removes the answers from the speechlibrary 
        /// </summary>
        public void UnloadQuestion()
        {
            foreach (string answer in activeQuestion.answerLines)
            {
                inputHelper.Speech.RemoveSentence(answer);
            }
            activeQuestion = null;
            questionTime.Reset();
            questionDelay.Start();
        }

        /// <summary>
        /// Event methods for all the events a god can react to 
        /// </summary>
        void StartEvent(string eventType, bool writeEventInQuestionBox = false)
        {
            StreamReader fileReader = new StreamReader("Content/Realms/" + BiomeName + "/Events/" + eventType + ".txt");

            if (writeEventInQuestionBox)
            {
                godQuestion.BoxText(fileReader.ReadToEnd(), book.Position.X + 378);
            }
            else
            {
                godReaction.BoxText(fileReader.ReadToEnd(), book.Position.X + 378);
                eventTime.Start();
            }
        }

        public void OnGoodAnswer()
        {
            if (events.Contains("goodAnswer"))
            {
                ModifyRageCounter(effectGood, true);
                godQuestion.BoxText(activeQuestion.responseLineRight, book.Position.X + 378);
            }
        }

        public void OnBadAnswer()
        {
            if (events.Contains("badAnswer"))
            {
                ModifyRageCounter(effectBad, true);
                godQuestion.BoxText(activeQuestion.responseLineWrong, book.Position.X + 378);
            }
        }

        public void OnQuestionTimeout()
        {
            if (events.Contains("questionTimeout"))
            {
                ModifyRageCounter(effectBad, true);
                StartEvent("QuestionTimeout", true);
            }
        }

        public void OnPlayerDeath()
        {
            if (events.Contains("playerDeath"))
            {
                ModifyRageCounter(effectGood, true);
                StartEvent("PlayerDeath");
            }
        }

        public void OnPlayerEnterBiome()
        {
            if (events.Contains("enterBiome"))
            {
                UpdateWords(goodWords);
                UpdateWords(badWords);
                StartEvent("EnterBiome");
            }
        }

        public void OnPlayerExitBiome()
        {
            if (events.Contains("exitBiome"))
            {
                UpdateWords(goodWords, false);
                UpdateWords(badWords, false);
                StartEvent("ExitBiome");
            }
        }

        public void OnDetectedGoodWord()
        {
            if (events.Contains("goodWord"))
            {
                ModifyRageCounter(effectGood);
                StartEvent("GoodWord");
            }
        }

        public void OnDetectedBadWord()
        {
            if (events.Contains("badWord"))
            {
                ModifyRageCounter(effectBad);
                StartEvent("BadWord");
            }
        }

        /// <summary>
        /// Calculates the rageCounter using a variety of variables
        /// </summary>
        public void ModifyRageCounter(int points, bool alwaysSucceeds = false)
        {
            int decreaseChance = chanceGood + (rageCounter - 14) * scaleGood;
            int increaseChance = chanceBad - (rageCounter - 14) * scaleBad;
            int nr = ran.Next(100) + 1;

            if ((nr <= decreaseChance && points < 0) || (nr <= increaseChance && points >= 0) || alwaysSucceeds)
            {
                if (rageCounter + points >= -1 && rageCounter + points < 30)
                    rageCounter += points;
                else if (rageCounter + points < -1)
                    rageCounter = -1;
                else
                    rageCounter = 29;
            }
        }

        /// <summary>
        /// returns the difficulty value if the modifier is in the dictionary, if it isn't in the dictionary it should return a neutral value of 5f 
        /// </summary>
        public float GetModifier(ModifierType type) 
        {
            if (!modifiers.ContainsKey(type))
            {
                return 5f;
            }
            return ((float)Math.Floor((double)(rageCounter / 3)) + 1f);
        }

        SpriteFont questionFont = AssetManager.GetSpriteFont("UI/QuestionFont");

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            
            //Draws the player's score
            if (GameEnvironment.GameStateManager.GetGameState("gamePlaying") is GamePlaying gamePlaying)
                spriteBatch.DrawString(questionFont, "" + gamePlaying.Player.Score, new Vector2(book.Position.X + book.Sprite.Width + 5, 5), Color.Black);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //Displays the slider at the correct position on the ragemeter
            slider.Position = new Vector2(slider.Position.X, 508 - (rageCounter * 17));
            
            //Responsible for displaying the player's lives
            if (GameEnvironment.GameStateManager.GetGameState("gamePlaying") is GamePlaying gamePlaying)
            {
                for (int i = previousLives; i < gamePlaying.Player.Lives; i++)
                {
                    life = new SpriteGameObject("UI/Life");
                    life.Position = new Vector2(book.Position.X + 474, 31 + (i * 19));
                    Add(life);
                }

                if (previousLives > gamePlaying.Player.Lives)
                    DeleteChild(children[children.Count - 1]);
                previousLives = gamePlaying.Player.Lives;
            }

            //Determines how long eventtext is displayed
            if (eventTime.ElapsedMilliseconds >= 10000)
            {
                godReaction.Text = "";
                eventTime.Reset();
            }

            //Determines the time a player has to answer a question
            if (questionTime.ElapsedMilliseconds >= 30000)
            {
                OnQuestionTimeout();
                UnloadQuestion();
            }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            foreach (string word in goodWords)
            {
                if (inputHelper.CheckPrompt(word))
                {
                    OnDetectedGoodWord();
                    break;
                }
            }

            foreach (string word in badWords)
            {
                if (inputHelper.CheckPrompt(word))
                {
                    OnDetectedBadWord();
                    break;
                }
            }

            //Decides if the player has given an answer and if that answer is the correct answer to the question
            if (activeQuestion != null)
            {
                string answered = "";
                foreach (string answer in activeQuestion.answerLines)
                {
                    if (inputHelper.CheckPrompt(answer))
                    {
                        answered = answer;
                        int wrongAnswers = 0;

                        foreach (string s in activeQuestion.goodAnswer)
                        {
                            if (answered == s)
                            {
                                OnGoodAnswer();
                                break;
                            }
                            else
                                wrongAnswers++;
                        }

                        if (wrongAnswers > 3)
                            OnBadAnswer();
                        break;
                    }
                }
                if (answered != "")
                {
                    Console.WriteLine(answered);
                    UnloadQuestion();
                }
            }

            //Determines the time interval between 2 questions
            if (questionDelay.ElapsedMilliseconds >= 20000 && activeQuestion == null)
            {
                AskQuestion();
                questionDelay.Reset();
                questionTime.Start();
            }
            else if (questionDelay.ElapsedMilliseconds >= 5000) //Empties the displayed text after a certain amount of time (needed for questionTimeOutEvent)
                godQuestion.Text = "";
        }
    }
}
