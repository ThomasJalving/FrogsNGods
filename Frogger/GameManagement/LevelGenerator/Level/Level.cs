using Frogger.GameManagement.GameObj;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Frogger.GameManagement.LevelGenerator
{
    /// <summary>
    /// the class that builds and keeps track of the chunks that make up the world
    /// </summary>
    partial class Level
    {
        private int activeLayer = 1; //0 = underground, 1 = surface, 2 = sky
        private Dictionary<int,List<Chunk>> chunks;
        public List<Chunk> activeChunks;
        private string currentBiome = "Forest"; //value for keeping track of the current biome, with Forest beign the default
        private string nextBiome = "Forest"; //value for keeping track of the next biome, needed to switch to a different active god
        private string middleBiome = "Forest"; //value for keeping track of the biome in the middle layer, useful for when we want to return to it from cave/sky
        private string nextMiddleBiome = "forest"; //value for keeping track of the next biome in the middle layer,

        private List<string> surfaceBiomes = new List<string>();

        public Dictionary<string, God> godList = new Dictionary<string, God>();
        public God activeGod;

        InputHelper inputHelper;

        Random ran;

        public Level(InputHelper inputHelper)
        {
            this.inputHelper = inputHelper;
            ran = GameEnvironment.Random;

            surfaceBiomes.Add("Forest");
            surfaceBiomes.Add("Volcanic");

            godList["Forest"] = new God("Forest", inputHelper);
            godList["Cave"] = new God("Cave", inputHelper);
            godList["Sky"] = new God("Sky", inputHelper);
            godList["Volcanic"] = new God("Volcanic", inputHelper);



            activeGod = godList[currentBiome];
            activeChunks = new List<Chunk>();
            chunks = new Dictionary<int, List<Chunk>>();

            //add counter and list for each layer to the dictionaries
            for (int i = 0; i < 3; i++) 
            {
                chunks.Add(i, new List<Chunk>());
                nextChunkCounter.Add(i, 0);
            }

            //make the first 3 chunks at the beginning
            for (int i = nextChunkCounter[activeLayer]; i < 3; i++)
            {
                PopulateNextChunk();
            }
            activeGod.OnPlayerEnterBiome();
        }

        /// <summary>
        /// moves the player down by 1 layer, if possible.
        /// it populates, unloads and/or loads chunks if needed
        /// </summary>
        public void GoDown()
        {
            if (activeLayer != 0)
            {
                int count = nextChunkCounter[activeLayer];
                activeLayer--;
                if(activeLayer == 1)
                {
                    currentBiome = middleBiome;
                    nextBiome = nextMiddleBiome;
                    AssetManager.PlayMusic("Music/" + currentBiome);
                }
                else //moving down to the cave, saving current biome
                {
                    AssetManager.PlayMusic("Music/Cave");
                    middleBiome = currentBiome;
                    nextMiddleBiome = nextBiome;
                    currentBiome = "Cave";
                    nextBiome = "Cave";
                }

                UnloadAllChunks();
                LoadThreeChunks(count);

                PauseGodTimers();
                activeGod = godList[currentBiome];
                PauseGodTimers(true);
                activeGod.OnPlayerEnterBiome();
            }
        }

        /// <summary>
        /// moves the player up by 1 layer, if possible.
        /// it populates, unloads and/or loads chunks if needed
        /// </summary>
        public void GoUp() //goes up a layer and populates any missing/needed chunks
        {
            if (activeLayer != 2)
            {
                int count = nextChunkCounter[activeLayer];
                activeLayer++;
                if (activeLayer == 1)
                {
                    currentBiome = middleBiome;
                    nextBiome = nextMiddleBiome;
                    AssetManager.PlayMusic("Music/" + currentBiome);
                }
                else //moving up to the sky, save current middle biome
                {
                    AssetManager.PlayMusic("Music/Sky");
                    middleBiome = currentBiome;
                    nextMiddleBiome = nextBiome;
                    currentBiome = "Sky";
                    nextBiome = "Sky";
                }

                UnloadAllChunks();
                LoadThreeChunks(count);

                PauseGodTimers();
                activeGod = godList[currentBiome];
                PauseGodTimers(true);
                activeGod.OnPlayerEnterBiome();
            }
        }

        /// <summary>
        /// Pauses or resumes the timers of the current god; necessary when switching gameStates or realms
        /// </summary>
        public void PauseGodTimers(bool resume = false)
        {
            if (!resume && activeGod.eventTime.IsRunning)
                activeGod.eventTime.Stop();
            else if (activeGod.eventTime.ElapsedMilliseconds > 0)
                activeGod.eventTime.Start();

            if (!resume && activeGod.questionTime.IsRunning)
                activeGod.questionTime.Stop();
            else if (activeGod.questionTime.ElapsedMilliseconds > 0)
                activeGod.questionTime.Start();

            if (!resume && activeGod.questionDelay.IsRunning)
                activeGod.questionDelay.Stop();
            else if (activeGod.questionDelay.ElapsedMilliseconds > 0)
                activeGod.questionDelay.Start();

            if (resume && activeGod.eventTime.ElapsedMilliseconds == 0 && activeGod.questionTime.ElapsedMilliseconds == 0 && activeGod.questionDelay.ElapsedMilliseconds == 0)
                activeGod.questionDelay.Start();
        }

        /// <summary>
        /// checks if a new chunk should be generated and the oldest unloaded
        /// <summary>
        public void CheckActiveChunks(Vector2 position)
        {
            if (GetChunkFromPosition(position) == activeChunks[2])
            {
                PopulateNextChunk();
                activeChunks[0].active = false;
                activeChunks.Remove(activeChunks[0]);
            }
        }

        /// <summary>
        /// Tries to randomize the biome if the current layer is the surface layer
        /// </summary>
        private void RandomizeBiome()
        {
            if(activeLayer != 1)
            {//doesn't randomize when in the sky or cave biomes
                return;
            }

            if(ran.Next(10) < 2)
            {//20% to get a random biome
                nextBiome = surfaceBiomes[ran.Next(surfaceBiomes.Count)];
            }
        }

        public void CheckBiomeInChunk(Vector2 position)
        {
            Chunk c = GetChunkFromPosition(position);
            if (c.god.BiomeName != currentBiome)
            {
                PauseGodTimers();
                currentBiome = c.god.BiomeName;
                activeGod = godList[c.god.BiomeName];
                PauseGodTimers(true);
                AssetManager.PlayMusic("Music/" + c.god.BiomeName);
                if(c != activeChunks[0])
                {
                    c.god.OnPlayerEnterBiome();
                }
            }
        }

        /// <summary>
        ///returns a list of all the chunks in the active layer
        /// <summary>
        public List<Chunk> ActiveChunkLayer() 
        {
            return chunks[activeLayer];
        }

        /// <summary>
        /// returns the god that rules over the given position
        /// <summary>
        public God GetGodFromPosition(Vector2 position)
        {
            position.X = MathHelper.Clamp(position.X, 0f, 14f); //some objects outside the grid also use this method, it will return null if this isn't used
            return GetChunkFromPosition(position).god;
            
        }

        /// <summary>
        /// returns the chunk at a given position, useable for checking biomes
        /// <summary>
        public Chunk GetChunkFromPosition(Vector2 position) 
        {
            int y = (int) -position.Y;
            int chunk = (y - (y % Level.tileHeight)) / Level.tileHeight / Level.ChunkLength;
            if(ActiveChunkLayer().Count - 1 < chunk)
            {
                return null;
            }
            try
            {
                if (ActiveChunkLayer()[chunk].active)
                {
                    return ActiveChunkLayer()[chunk];
                }
            }
            catch (ArgumentOutOfRangeException)
            {

            }
            return null;
        }

        /// <summary>
        /// get the tile at position, used for checking collisions
        /// <summary>
        public Tile GetTileFromPosition(Vector2 position)
        {
            Chunk chunk = GetChunkFromPosition(position);
            if(chunk == null)
            {
                return null;
            }
            Vector2 tilePosition = WorldPositionToChunkGrid(position, chunk);
            try
            {
                return chunk.grid[(int)tilePosition.X, (int)tilePosition.Y];
            } catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        /// <summary>
        /// translates world position into the x and y of a grid position, used for getting a certain tile from a chunk
        /// <summary>
        public Vector2 WorldPositionToChunkGrid(Vector2 position, Chunk chunk)
        {
            float tileX = (position.X - (position.X % Level.tileWidth))/Level.tileWidth;
            float tileY = -(position.Y - (position.X % Level.tileHeight)) / Level.tileHeight - chunk.counter * Level.ChunkLength;
            return new Vector2(tileX, tileY);
        }
        
        /// <summary>
        /// unloads all currently active chunks
        /// </summary>
        private void UnloadAllChunks()
        {
            while (activeChunks.Count > 0)
            {
                UnloadOldestChunk();
            }
        }
        /// <summary>
        /// unloads the oldest active chunk
        /// </summary>
        private void UnloadOldestChunk()
        {
            activeChunks[0].active = false;
            activeChunks.RemoveAt(0);
        }
        /// <summary>
        /// makes sure only 3 chunks are loaded, creates any that are missing
        /// </summary>
        private void LoadThreeChunks(int count)
        {
            //if the player moved any chunks before switching realms, all missing chunks are made 
            for (int i = nextChunkCounter[activeLayer]; i < count; i++)
            {
                PopulateNextChunk();
            }
            //if there were more than 3 missing chunks, the oldest ones get unloaded
            while (activeChunks.Count > 3)
            {
                UnloadOldestChunk();
            }
            //if less than 3 chunks were made, the newest unused chunk is loaded in
            for (int i = 3 - activeChunks.Count; i > 0; i--)
            {
                activeChunks.Insert(0, ActiveChunkLayer()[ActiveChunkLayer().Count - 4 + i]);
                ActiveChunkLayer()[ActiveChunkLayer().Count - 4 + i].active = true;
            }
        }

        public void HandleInput(InputHelper inputHelper)
        {
            activeGod.HandleInput(inputHelper);
        }

        public void Update(GameTime gameTime)
        {
            activeGod.Update(gameTime);
            foreach (Chunk c in activeChunks)
            {
                c.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Chunk c in activeChunks)
            {
                c.Draw(gameTime, spriteBatch);
            }
        }
    }
}
