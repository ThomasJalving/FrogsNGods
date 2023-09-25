using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Frogger.GameManagement.GameObj;
using Frogger.GameObjects;
using Microsoft.Xna.Framework;

namespace Frogger.GameManagement.LevelGenerator
{
    partial class Level
    {
        public const int ChunkWidth = 14;
        public const int ChunkLength = 13;

        public const int tileHeight = 32;
        public const int tileWidth = 32;

        Dictionary<int, int> nextChunkCounter = new Dictionary<int, int>(); //chunkcounter per layer

        /// <summary>
        /// creates the next chunk and fills it with tiles, spawners and special objects
        /// </summary>
        private void PopulateNextChunk()
        {
            int differentChunks = Directory.GetFiles("Content/Realms/" + nextBiome + "/Chunks").Length;
            int chosenChunkFile = ran.Next(1, differentChunks + 1);

            List<string> textLines = new List<string>();

            StreamReader fileReader = new StreamReader("Content/Realms/" + nextBiome + "/Chunks/" + chosenChunkFile + ".txt"); //chooses a random file based on the amount of available files
            string line = fileReader.ReadLine();

            while (line != null)
            {
                textLines.Add(line);
                line = fileReader.ReadLine();
            }
            fileReader.Close();
            if (textLines.Count != ChunkLength)
            {
                Console.WriteLine("Chunkfile" + chosenChunkFile + ".txt in " + nextBiome + " is not valid!"); //TODO show which chunk and start again
                PopulateNextChunk();
                return;
            }

            Chunk nextChunk = new Chunk(ChunkWidth, ChunkLength, nextChunkCounter[activeLayer], godList[nextBiome]);

            //split off spawner and tile info and add all spawners to the chunk
            for (int i = 0; i < textLines.Count; i++)
            {
                string tileLine = textLines[i];
                if (tileLine.Contains(":"))
                {
                    string spawnertype = tileLine.Split(':')[0];
                    string newTileLine = tileLine.Split(':')[1];
                    int number = textLines.Count - 1 - textLines.IndexOf(tileLine);

                    nextChunk.spawners.Add(CreateSpawner(spawnertype, (int)nextChunk.Position.Y - number * tileHeight, nextChunk));

                    if (spawnertype.Contains('R') && newTileLine.Contains('R'))
                    {
                        textLines[i] = newTileLine + "1";//used to get the left to right tiles from the spritesheets
                    }
                    else
                    {
                        textLines[i] = newTileLine;
                    }
                }
            }

            //apply the difficulty modifiers on the spawners
            nextChunk.ApplyDifficulty();

            //populate all the tiles
            for (int y = ChunkLength - 1; y > -1; y--) //file needs to be read from the bottom line up, otherwise the chunks will be flipped
            {
                int oddTile = 0;

                if (textLines[y].Contains('1'))
                {
                    oddTile = 1;
                }

                for (int x = 0; x < ChunkWidth; x++)
                {
                    char[,] surroundings = new char[3, 3];

                    if (x == 0)
                    {
                        surroundings[0, 0] = '-';
                        surroundings[0, 1] = '-';
                        surroundings[0, 2] = '-';
                    }
                    if (y == 0)
                    {
                        surroundings[0, 0] = '-';
                        surroundings[1, 0] = '-';
                        surroundings[2, 0] = '-';
                    }
                    if (x == ChunkWidth - 1)
                    {
                        surroundings[2, 0] = '-';
                        surroundings[2, 1] = '-';
                        surroundings[2, 2] = '-';
                    }
                    if (y == ChunkLength - 1)
                    {
                        surroundings[0, 2] = '-';
                        surroundings[1, 2] = '-';
                        surroundings[2, 2] = '-';
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (surroundings[j, i] != '-')
                                surroundings[j, i] = textLines[y - (1 - i)][x - (1 - j)];
                        }
                    }

                    nextChunk.SetTile(x, -y + ChunkLength - 1, LoadTile(textLines[y][x], surroundings, oddTile, nextBiome)); //TODO implement random biomes

                    //spawn a stationary platform or special object on top of the tile if needed
                    switch (textLines[y][x])
                    {
                        case 'P':
                            nextChunk.laneObjects.Add(new LaneObject(nextChunk.grid[x, -y + ChunkLength - 1].Position, "Realms/" + nextBiome + "/Platform_Stationary 1x1"));
                            break;
                        case 'E':
                            nextChunk.specialObjects.Add(CreateSpecialObject('E', nextChunk.grid[x, -y + ChunkLength - 1].Position));
                            break;
                        case 'D':
                            nextChunk.specialObjects.Add(CreateSpecialObject('D', nextChunk.grid[x, -y + ChunkLength - 1].Position));
                            break;
                        case 'S':
                            nextChunk.specialObjects.Add(CreateSpecialObject('S', nextChunk.grid[x, -y + ChunkLength - 1].Position));
                            break;
                        case 'H':
                            nextChunk.specialObjects.Add(CreateSpecialObject('H', nextChunk.grid[x, -y + ChunkLength - 1].Position));
                            break;
                        case 'L':
                            nextChunk.specialObjects.Add(CreateSpecialObject('L', nextChunk.grid[x, -y + ChunkLength - 1].Position));
                            break;
                    }
                }
            }
            ActiveChunkLayer().Add(nextChunk);
            nextChunkCounter[activeLayer] = nextChunkCounter[activeLayer] + 1;
            activeChunks.Add(nextChunk);
            RandomizeBiome();
        }

        ///B background
        ///R road/track
        ///W water
        ///P platform
        ///O obstacle
        ///T transition
        ///H thundercloud
        ///L life
        ///S shield
        ///D duskflower
        ///E tadpole
        ///Q shop
        
        /// <summary>
        /// Returns a specific type of tile based on a given character and biome
        /// </summary>
        Tile LoadTile(char s, char[,] surroundings, int oddTile, string biome) //load the right sprite for the tile type and biome
        {
            Tile t;
            switch (s)
            {
                case 'B':
                case 'L':
                case 'S':
                case 'E':
                case 'D':
                    t = new Tile(Tile.TileType.Background, biome);
                    break;
                case 'H':
                    t = new Tile(Tile.TileType.Thundercloud, biome, surroundings);
                    break;
                case 'P':
                case 'W':
                    t = new Tile(Tile.TileType.Deadly, biome, surroundings);
                    break;
                case 'R':
                    t = new Tile(Tile.TileType.Road, biome);
                    break;
                case 'O':
                    t = new Tile(Tile.TileType.Obstacle, biome);
                    break;
                case 'G':
                    t = new Tile(Tile.TileType.Gate, biome);
                    break;
                case 'T':
                    //randomly spawn transition tiles to get to different layers
                    if (GameEnvironment.Random.Next(10) < 3)
                    {
                        //if the current layer is the surface, it will choose one or the other, if it isnt the surface, it will always send you to the surface
                        if(activeLayer == 1)
                        {
                            if(GameEnvironment.Random.Next(2) == 0)
                            {
                                t = new Tile(Tile.TileType.TransitionUp, biome);
                            }
                            else
                            {
                                t = new Tile(Tile.TileType.TransitionDown, biome);
                            }
                        }
                        else if(activeLayer == 0)//it will always be a transition up from the cave
                        {
                            if (GameEnvironment.Random.Next(2) == 0)
                            {
                                t = new Tile(Tile.TileType.TransitionUp, biome);
                            }
                            else
                            {
                                t = new Tile(Tile.TileType.TransitionUp, biome, OddTile: 1);
                            }

                        }
                        else//it will always be a transition down from the sky
                        {
                            if (GameEnvironment.Random.Next(2) == 0)
                            {
                                t = new Tile(Tile.TileType.TransitionDown, biome);
                            }
                            else
                            {
                                t = new Tile(Tile.TileType.TransitionDown, biome, OddTile: -1);
                            }
                        }
                    }
                    else
                    {
                        t = new Tile(Tile.TileType.Background, biome);
                    }
                    break;
                case 'Q':
                    t = new Tile(Tile.TileType.Shop, biome);
                    break;
                default:
                    t = new Tile(Tile.TileType.Background, biome);
                    Console.WriteLine("found an unrecognized letter");//this should only ever happen if any unknown letter is used in the chunkfile
                    break;
            }
            return t;
        }

        /// <summary>
        /// returns a spawner made from info read out of a textfile
        /// </summary>
        Spawner CreateSpawner(string spawnertype, int vertpos, Chunk nextChunk)//check which specific values to give the spawner
        {
            StreamReader fileReader;
            string[] spawnerstats;
            string spawnerInfo;
            int spawnerLine;
            bool deadly = false;
            bool toRight = false;
            string startcd = "0";

            //looking for specific letters to assign specific values to the spawner
            if (spawnertype.Contains('O'))
            {
                deadly = true;
                fileReader = new StreamReader("Content/Spawners/DeadlySpawners.txt");
                spawnerInfo = spawnertype.Split('O')[1];
            }
            else
            {
                fileReader = new StreamReader("Content/Spawners/SafeSpawners.txt");
                spawnerInfo = spawnertype.Split('P')[1];
            }

            if (spawnerInfo.Contains('-'))
            {
                startcd = spawnerInfo.Split('-')[1];
                spawnerLine = int.Parse(spawnerInfo.Split('-')[0]);
            }
            else
            {
                spawnerLine = int.Parse(spawnerInfo);
            }

            //loop through to get the correct line, it gets +1 because of header info
            for (int i = 0; i < spawnerLine + 1; i++)
            {
                fileReader.ReadLine();
            }

            spawnerstats = fileReader.ReadLine().Split(';');

            fileReader.Close();

            spawnerstats[0] = "Realms/" + nextBiome + "/" + spawnerstats[0];
            if (spawnertype.Contains('R'))
                toRight = true;
            //check whether to use turtlespawner for special dangerous turtle platforms, or the normal spawner
            if (spawnerstats[0].Contains("1x1") || spawnerstats[0].Contains("2x2") && spawnerstats[5] != "0")
            {
                return new TurtleSpawner(vertpos, deadly, toRight, startcd, nextChunk, spawnerstats);
            }
            else
            {
                return new Spawner(vertpos, deadly, toRight, startcd, nextChunk, spawnerstats);
            }
        }

        public SpecialGameObject CreateSpecialObject(char type, Vector2 position)
        {
            //Spawn a specified special object in a specified place
            SpecialGameObject s;
            switch (type)
            {
                case 'E':
                    s = new SpecialGameObject(true, "SpecialObjects/Tadpole", position);
                    break;
                case 'D':
                    s = new DuskFlower(false, "SpecialObjects/DuskFlower", position);
                    break;
                case 'S':
                    s = new SpecialGameObject(true, "SpecialObjects/Shield", position);
                    break;
                case 'H':
                    s = new Thundercloud(false, "SpecialObjects/Lightning", position);
                    break;
                case 'L':
                default:
                    s = new SpecialGameObject(true, "SpecialObjects/Life", position);
                    break;
            }
            return s;
        }
    }
}