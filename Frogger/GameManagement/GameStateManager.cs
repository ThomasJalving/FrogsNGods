using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frogger.GameManagement.GameObj;

namespace Frogger.GameManagement
{
    /// <summary>
    /// The class that keeps track of the different gamestates and which one is active
    /// </summary>
    public class GameStateManager : IGameLoopObject
    {
        protected bool wipeInput;
        protected Dictionary<string, GameObjectList> gameStates;
        protected GameObjectList currentGameState;

        public GameStateManager()
        {
            gameStates = new Dictionary<string, GameObjectList>();
            currentGameState = null;
        }

        /// <summary>
        /// Adds a gamestate to the dictionary of gamestats
        /// </summary>
        public void AddGameState(string name, GameObjectList state)
        {
            gameStates[name] = state;
        }

        /// <summary>
        /// Returns a specific gamestate
        /// </summary>
        public GameObjectList GetGameState(string name)
        {
            return gameStates[name];
        }

        /// <summary>
        /// Switches the active gamestate to a different gamestate, as long as it exists
        /// </summary>
        public void SwitchTo(string name)
        {
            if (gameStates.ContainsKey(name))
            {
                wipeInput = true;
                GameObjectList lastState = currentGameState;
                currentGameState = gameStates[name];
                AddGameState("last", lastState);
            }
            else
            {
                throw new KeyNotFoundException("Could not find game state: " + name);
            }
        }

        /// <summary>
        /// Returns the currently active gamestate
        /// </summary>
        public GameObjectList CurrentGameState
        {
            get
            {
                return currentGameState;
            }
        }

        public void HandleInput(InputHelper inputHelper)
        {
            if (wipeInput)
            {
                wipeInput = false;
                inputHelper.speechPrompts.Clear();
            }
            if (currentGameState != null)
            {
                currentGameState.HandleInput(inputHelper);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (currentGameState != null)
            {
                currentGameState.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (currentGameState != null)
            {
                currentGameState.Draw(gameTime, spriteBatch);
            }
        }

        public void Reset()
        {
            if (currentGameState != null)
            {
                currentGameState.Reset();
            }
        }
    }
}
