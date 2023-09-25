using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frogger.GameManagement.GameObj;
using Frogger.GameObjects;

namespace Frogger.GameManagement.LevelGenerator
{
    /// <summary>
    /// The class that contains the specific objects that make the world
    /// </summary>
    class Chunk : GameObject
    {
        public Tile[,] grid;
        public int counter;
        public GameObjectList spawners = new GameObjectList();
        public God god;
        public GameObjectList laneObjects;
        public GameObjectList specialObjects;
        public bool active;

        public Chunk(int width, int length, int chunkCounter, God God = null)
        {
            position = new Vector2(0, -chunkCounter * Level.ChunkLength * Level.tileHeight);
            grid = new Tile[width, length];
            counter = chunkCounter;
            god = God;
            laneObjects = new GameObjectList();
            specialObjects = new GameObjectList();
            active = true;
        }

        /// <summary>
        /// Places a tile in the grid and gives it the correct position
        /// </summary>
        public void SetTile(int x, int y, Tile tile)
        {
            grid[x, y] = tile;
            float posX = position.X + x * Level.tileWidth;
            float posY = position.Y - y * Level.tileHeight;
            tile.Position = new Vector2(posX, posY);
        }

        /// <summary>
        /// Applies the god modifier difficulty to all spawners in the chunk
        /// </summary>
        public void ApplyDifficulty()
        {
            foreach (Spawner spawner in spawners.Children)
            {
                spawner.SetModifiers(god);
            }
        }

        public override void Update(GameTime gameTime)
        {
            spawners.Update(gameTime);
            laneObjects.Update(gameTime);
            specialObjects.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach(Tile t in grid)
            {
                t.Draw(gameTime, spriteBatch);
            }
            specialObjects.Draw(gameTime, spriteBatch);
            laneObjects.Draw(gameTime, spriteBatch);
        }
    }
}
