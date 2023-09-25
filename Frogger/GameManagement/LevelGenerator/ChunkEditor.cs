using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Frogger.GameManagement.GameObj;
using Microsoft.Xna.Framework.Graphics;

namespace Frogger.GameManagement.LevelGenerator
{
    /// <summary>
    /// Used in the ChunkEditing gameState to draw chunks
    /// </summary>
    class ChunkEditor : IGameLoopObject
    {
        public Chunk Chunk { get; private set; }
        public string[,] TextGrid { get; private set; }
        public string[] spawnerinfo;
        private Dictionary<Point, SpecialGameObject> SpecialGrid = new Dictionary<Point, SpecialGameObject>();
        public string chosenBiome = "Forest";
        public string chosenSpecial;
        public Tile.TileType type = Tile.TileType.Background;
        public string textType = "B";
        
        /// <summary>
        /// A class to edit chunks and write them to file
        /// </summary>
        public ChunkEditor()
        {
            //the chunk here is only used for drawing, the textgrid and spawnerinfo is what will end up in the actual files when saved
            Chunk = new Chunk(Level.ChunkWidth, Level.ChunkLength,-1);
            Chunk.Position += new Vector2(0, 2 * Level.tileHeight);
            TextGrid = new string[Level.ChunkWidth, Level.ChunkLength];
            spawnerinfo = new string[Level.ChunkLength - 1];
            Fillchunk(true);
        }

        /// <summary>
        /// fills the entire chunk with basic tiles
        /// </summary>
        public void Fillchunk(bool first = false)
        {
            for (int y = 0; y < Level.ChunkLength; y++)
            {
                for (int x = 0; x < Level.ChunkWidth; x++)
                {
                    Chunk.SetTile(x, y, new Tile(type, chosenBiome));
                    TextGrid[x, y] = "B";
                    //set up the specialobjects grid
                    if (first)
                        SpecialGrid.Add(new Point(x, y), null);
                }
            }

            foreach(SpecialGameObject s in Chunk.specialObjects.Children)
            {
                s.Deletable = true;
            }
        }

        /// <summary>
        /// returns true if the given position is within the grid
        /// </summary>
        private bool MouseInGrid(Vector2 position)
        {//there is a small offset to correctly work with the drawn cursor
            if (position.X > Chunk.Position.X && position.X < (Chunk.Position.X + Level.ChunkWidth * Level.tileWidth) && 
                position.Y - Level.tileHeight < Chunk.Position.Y && position.Y > (Chunk.Position.Y - (Level.ChunkLength-1) * Level.tileHeight))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// returns a vector2 with the grid position of a certain tile in the chunk
        /// </summary>
        private Point MouseGridPosition(Vector2 position)
        {
            Vector2 chunkpos = position - Chunk.Position - new Vector2(0,Level.tileHeight);//the offset is applied here as well
            chunkpos.Y *= -1;

            Vector2 snappedpos = Vector2.Zero;
            float rest = chunkpos.X % 32;
            snappedpos.X = chunkpos.X - rest;

            float rest2 = chunkpos.Y % 32;
            snappedpos.Y = chunkpos.Y - rest2;
            
            //divides the snapped positions to become values that fit the grid array and converts it to a 2D point
            return (snappedpos / new Vector2(Level.tileWidth, Level.tileHeight)).ToPoint();
        }

        public void Update(GameTime gametime)
        {
            Chunk.Update(gametime);
        }

        public void Draw(GameTime gametime, SpriteBatch spriteBatch)
        {
            Chunk.Draw(gametime, spriteBatch);
        }

        public void HandleInput(InputHelper inputhelper)
        {
            if (inputhelper.MouseLeftButtonDown() && MouseInGrid(inputhelper.MousePosition))
            {
                TryPlaceInGrid(inputhelper.MousePosition);
                
            }
        }

        /// <summary>
        /// attempts to place any selected object into the grid at the given position while also removing any pre-exisiting object if needed
        /// </summary>
        private void TryPlaceInGrid(Vector2 Mousepos)
        {
            Point gridpos = MouseGridPosition(Mousepos);//get the grid position of the clicked tile

            //only tries to place a tile if its a new type or if you have a powerup selected
            if (Chunk.grid[gridpos.X, gridpos.Y].type != type || chosenSpecial != null || SpecialGrid[gridpos] != null)
            {
                //set the tile into the chunk and the textgrid
                char[,] surroundings = new char[3,3];
                surroundings[1, 0] = 'W';
                surroundings[1, 2] = 'W';
                Chunk.SetTile(gridpos.X, gridpos.Y, new Tile(type, chosenBiome, surroundings));
                TextGrid[gridpos.X, gridpos.Y] = textType;

                //remove a special object if there is one
                if (SpecialGrid[new Point(gridpos.X, gridpos.Y)] != null)
                {
                    SpecialGrid[new Point(gridpos.X, gridpos.Y)].Deletable = true;
                }

                //if a special object is selected, place it
                if (chosenSpecial != null)
                {
                    Vector2 SpecialPos = new Vector2(gridpos.X * Level.tileWidth, gridpos.Y * Level.tileHeight);
                    SpecialGameObject s = new SpecialGameObject(false, chosenSpecial, new Vector2(SpecialPos.X, Chunk.Position.Y - SpecialPos.Y));
                    Chunk.specialObjects.Add(s);
                    SpecialGrid[new Point(gridpos.X, gridpos.Y)] = s;
                }
            }
        }

        /// <summary>
        /// tries to write the current chunk out as a file, but checks if the file is theoretically possible as well, does not cover all situations
        /// </summary>
        public void TryWriteFile(bool[] active)
        {
            string[] chunkfile = new string[Level.ChunkLength];

            for (int y = 0; y < Level.ChunkLength; y++)
            {
                chunkfile[y] = "";

                if(y != 0 && active[y-1])
                {
                    chunkfile[y] += spawnerinfo[y - 1] + ":";
                }
                for (int x = 0; x < Level.ChunkWidth; x++)
                {
                    chunkfile[y] += TextGrid[x, y];
                }
            }

            foreach (string line in chunkfile)
            {
                //check if a tadpole is somewhere in the chunk
                if (line.Contains("E") && Array.FindIndex(chunkfile, x => x == line) != chunkfile.Length - 1)
                {//make the last line a gate unless it contains the tadpole in which case you can pass it and be a bad parent, this also removes the spawner on that line
                    chunkfile[chunkfile.Length - 1] = "GGGGGGGGGGGGGG";
                }
            }

            bool possible = true;
            foreach(string s in chunkfile)
            {
                //check if all rows are actually possible
                if(!s.Contains("P") && !s.Contains("R") && !s.Contains("B") && !s.Contains("L") && !s.Contains("E") && !s.Contains("S") && !s.Contains("D") && !s.Contains("Q") && !s.Contains("G") && !s.Contains("H"))
                {
                    possible = false;
                }
            }

            if (possible)
            {
                int differentChunks = Directory.GetFiles("Content/Realms/" + chosenBiome + "/Chunks").Length;

                FileStream FS = new FileStream("Content/Realms/" + chosenBiome + "/Chunks/" + (differentChunks + 1) + ".txt", FileMode.CreateNew);
                
                StreamWriter SW = new StreamWriter(FS);
                
                for (int i = Level.ChunkLength - 1; i > -1; i--)
                {
                    SW.WriteLine(chunkfile[i]);
                }

                SW.Close();

                GameEnvironment.GameStateManager.SwitchTo("menuScreen");
            }
            else
            {
                return;
            }
        } 

        public void Reset()
        {
            Fillchunk();
        }
    }
}
