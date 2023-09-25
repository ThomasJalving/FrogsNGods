using Frogger.GameManagement.GameObj;
using Frogger.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frogger.GameManagement.LevelGenerator;
using Frogger.GameManagement;
using Frogger.Menu;
using System;

namespace Frogger.States
{
    /// <summary>
    /// Create the shop UI
    /// </summary>
    class Shop : GameObjectList
    {
        protected Button buy, previousItem, nextItem, exitShop, descriptionBox;
        protected SpecialGameObject merch;
        private SpriteGameObject book;
        private TextGameObject shopName;
        protected string description;
        protected int currentItem, shopItemCount = 3, Score, itemCost, notEnoughTimer;
        protected double bought = 1;
        protected Camera cam;

        public Shop()
        {
            book = new SpriteGameObject("UI/Book");
            book.Position = new Vector2(1, 1);
            Add(book);

            shopName = new TextGameObject("-Shop-", new Vector2(book.Position.X + 120, 100), centered: false, color: Color.Black);
            Add(shopName);

            //Create buttons and such for the shop
            buy = new Button("Menu/Button Buy", 0, 2)
            {
                Position = new Vector2(Level.tileWidth * 6, Level.tileHeight * 11)
            };
            Add(buy);

            previousItem = new Button("Menu/Button Back", 0, 2)
            {
                Position = new Vector2(Level.tileWidth * 2, Level.tileHeight * 7)
            };
            Add(previousItem);

            merch = new SpecialGameObject(false, "SpecialObjects/Shield", position)
            {
                Position = new Vector2(Level.tileWidth * 6 + 16, Level.tileHeight * 7)
            };
            Add(merch);

            nextItem = new Button("Menu/Button Next", 0, 2)
            {
                Position = new Vector2(Level.tileWidth * 11, Level.tileHeight * 7)
            };
            Add(nextItem);

            descriptionBox = new Button("Menu/Textbox 13x2", 0, 1, text: description)
            {
                Position = new Vector2(Level.tileWidth / 2, Level.tileHeight * 9)
            };
            Add(descriptionBox);

            exitShop = new Button("Menu/Button Exit", 0, 2)
            {
                Position = new Vector2(Level.tileWidth * 5, Level.tileHeight * 15)
            };
            Add(exitShop);

            //Load animations for different shop items
            LoadAnimations("SpecialObjects/Shield", "Shield");
            LoadAnimations("SpecialObjects/duskFlower", "duskFlower");
            LoadAnimations("SpecialObjects/Life", "Life");

            //Create camera and mouse
            cam = new Camera();
            Add(new Cursor());
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            if (inputHelper.MouseLeftButtonPressed() && buy.IsOnButton)
            {
                if (GameEnvironment.GameStateManager.GetGameState("gamePlaying") is GamePlaying gamePlaying)
                {
                    if (Score >= itemCost)
                    {
                        //Stop buying if already fully immune to duskflower
                        if (!ImmunityCheck())
                        {
                            gamePlaying.Player.ShopItemBought(currentItem, itemCost);
                            bought++;
                        } else
                        {
                            ImmunityMessage();
                        }
                    }
                    else
                    {
                        NotEnoughScore();
                    }
                }
            }
            if (inputHelper.MouseLeftButtonPressed() && previousItem.IsOnButton)
            {
                currentItem--;
            }
            else if (inputHelper.MouseLeftButtonPressed() && nextItem.IsOnButton)
            {
                currentItem++;
            }

            if (inputHelper.MouseLeftButtonPressed() && exitShop.IsOnButton)
            {
                if (GameEnvironment.GameStateManager.GetGameState("gamePlaying") is GamePlaying gamePlaying)
                    gamePlaying.Level.PauseGodTimers(true);
                GameEnvironment.GameStateManager.SwitchTo("gamePlaying");
            }
        }

        public override void Update(GameTime gameTime)
        {
            GetScore();
            if (currentItem >= shopItemCount)
            {
                currentItem = 0;
            } else if (currentItem < 0)
            {
                currentItem = shopItemCount - 1;
            }
            switch (currentItem)
            {
                case 0:
                    merch.StartAnimation("Shield");
                    itemCost = (int)(bought * 1500);
                    description = "A nice shield that will protect you from 2 obstacles. \n It costs: " + itemCost + " points. You have " + Score;
                    break;
                case 1:
                    merch.StartAnimation("duskFlower");
                    itemCost = (int)(bought * 2500);
                    description = "Decreases the duration of the duskflower's pollen effect. \n It costs: " + itemCost + " points. You have " + Score;
                    break;
                case 2:
                    merch.StartAnimation("Life");
                    itemCost = (int)(bought * 5000);
                    description = "Grants you an extra life. \n It costs: " + itemCost + " points.  You have " + Score;
                    break;
            }
            if (notEnoughTimer >= 0)
            {
                notEnoughTimer--;
            } else
            {
                descriptionBox.ChangeText(description);
            }
            base.Update(gameTime);
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GameEnvironment.GameStateManager.GetGameState("gamePlaying").Draw(gameTime, spriteBatch);

            spriteBatch.Begin(transformMatrix: cam.Transform);
            base.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }

        /// <summary>
        /// Loads the animations for the shop item on display
        /// </summary>
        /// <param name="sprite">The sprite of the item</param>
        /// <param name="id">What the animation will be called</param>
        public void LoadAnimations(string sprite, string id)
        {
            Texture2D measurement = AssetManager.GetSprite(sprite);
            int frames = measurement.Height / measurement.Width;
            merch.LoadAnimation(id, sprite, frames, 0.2f, looping: true);
        }

        /// <summary>
        /// Gets the current score from player
        /// </summary>
        /// <returns>An int containing the current score</returns>
        public int GetScore()
        {
            if (GameEnvironment.GameStateManager.GetGameState("gamePlaying") is GamePlaying playing)
            {
                Score = playing.Player.Score;
            }
            return Score;
        }

        /// <summary>
        /// Gets called when the player tries to buy something without having a high enough score
        /// </summary>
        public void NotEnoughScore()
        {
            descriptionBox.ChangeText("You do not have enough points to buy this...");
            notEnoughTimer = 120;
        }

        public bool ImmunityCheck()
        {
            if (currentItem != 1)
            {
                return false;
            } else
            {
                if (GameEnvironment.GameStateManager.GetGameState("gamePlaying") is GamePlaying playing)
                {
                    if (playing.Player.Immunity >= 10)
                    {
                        return true;
                    }
                }
            } return false;
        }

        public void ImmunityMessage()
        {
            descriptionBox.ChangeText("You're already fully immune to the flower!");
            notEnoughTimer = 120;
        }
    }
}
