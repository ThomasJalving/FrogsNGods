using System;
using Frogger.GameManagement.GameObj;


namespace Frogger.GameManagement.LevelGenerator
{
    /// <summary>
    /// The class out of which the ground of the world is made, uses different sprites out of a spritesheet
    /// </summary>
    class Tile : SpriteSheetGameObject
    {
        public enum TileType { Deadly,Background,Obstacle,Road,TransitionUp,TransitionDown, Gate, Shop, Thundercloud};
        public TileType type;

        public Tile(TileType Type, string biome, char[,] surroundings = null, int OddTile = 0, string spriteName = "") 
            :base(spriteName = "Realms/" + biome + "/Tiles", elementHeigth: Level.tileHeight, elementWidth: Level.tileWidth)
        {
            type = Type;
            switch (type)
            {
                case TileType.Background:
                    spriteSheet.Variation = 0;
                    break;
                case TileType.Obstacle:
                    //uses all possible stationary obstacle sprites in the spritesheet randomly
                    spriteSheet.Variation = 11 + GameEnvironment.Random.Next(spriteSheet.SheetColumns - 11);
                    break;
                case TileType.Deadly:
                    //Decides what tile sprite to use, depending on the tile above and below it
                    if (surroundings[1,0] == 'W' || surroundings[1, 0] == 'P')
                    {
                        if(surroundings[1, 2] == 'W' || surroundings[1, 2] == 'P')
                        {
                            spriteSheet.Variation = 3;
                        }
                        else
                        {
                            spriteSheet.Variation = 4;
                        }
                    }
                    else
                    {
                        if (surroundings[1, 2] == 'W' || surroundings[1, 2] == 'P')
                        {
                            spriteSheet.Variation = 5;
                        }
                        else
                        {
                            spriteSheet.Variation = 6;
                        }
                    }
                    break;
                case TileType.Road:
                    spriteSheet.Variation = 1 + OddTile;
                    break;
                case TileType.TransitionUp:
                    spriteSheet.Variation = 7 + OddTile;
                    break;
                case TileType.TransitionDown:
                    spriteSheet.Variation = 8 + OddTile;
                    break;
                case TileType.Gate:
                    spriteSheet.Variation = 9;
                    break;
                case TileType.Shop:
                    spriteSheet.Variation = 10+ OddTile;
                    break;
                case TileType.Thundercloud:
                    //Decides what tile sprite to use, depending on the surrounding tiles
                    string name = "";
                    if (surroundings[1, 0] == 'H')
                        name += 'a';
                    if (surroundings[2, 1] == 'H')
                        name += 'b';
                    if (surroundings[1, 2] == 'H')
                        name += 'c';
                    if (surroundings[0, 1] == 'H')
                        name += 'd';
                    if (surroundings[0, 0] == 'H' && name.Contains("a") && name.Contains("d"))
                        name += 'e';
                    if (surroundings[2, 0] == 'H' && name.Contains("a") && name.Contains("b"))
                        name += 'f';
                    if (surroundings[2, 2] == 'H' && name.Contains("b") && name.Contains("c"))
                        name += 'g';
                    if (surroundings[0, 2] == 'H' && name.Contains("c") && name.Contains("d"))
                        name += 'h';
                    if (name == "")
                        name = "0";
                    spriteName = "Realms/Sky/Thundercloud/" + name;
                    ChangeSpriteSheet(spriteName, 0, 1);
                    break;
            }
        }
    }
}
