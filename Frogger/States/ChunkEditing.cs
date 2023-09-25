using System.Collections.Generic;
using System.IO;
using System;
using Microsoft.Xna.Framework;
using Frogger.GameManagement;
using Frogger.GameManagement.LevelGenerator;
using Frogger.GameManagement.GameObj;
using Microsoft.Xna.Framework.Graphics;
using Frogger.Menu;

namespace Frogger.States
{
    /// <summary>
    /// the gamestate that handles all buttons for the chunkeditor
    /// </summary>
    class ChunkEditing : GameObjectList
    {
        protected Camera cam;
        protected ChunkEditor editor;

        SpriteGameObject selectedTile;
        private Button exit, save, background, deadly, road, transition, shop, obstacle, cloud, platform, shield, life, duskflower, tadpole, biomeSwitch, biomeSwitchBack, biomeButton;
        private Button[] spawnerActive, spawnerDirection, spawnerType, spawnerNext, spawnerPrev;
        private SpriteGameObject activeSign, DirectionSign, ObstaclePlatformSign;

        private string biome = "Forest";

        private List<string> possibleBiomes = new List<string>();

        private Dictionary<bool, List<string>> possibleSpawners = new Dictionary<bool, List<string>>();

        private bool[] isDeadlySpawner, active;
        private string[] chosenSpawners, toRight;

        public ChunkEditing()
        {
            editor = new ChunkEditor();
            cam = new Camera();

            activeSign = new SpriteGameObject("Menu/Vertical_Sign Active");
            DirectionSign = new SpriteGameObject("Menu/Vertical_Sign Direction");
            ObstaclePlatformSign = new SpriteGameObject("Menu/Vertical_Sign ObstacleOrPlatform");

            activeSign.Position = new Vector2((Level.ChunkWidth) * Level.tileWidth, 0);
            DirectionSign.Position = new Vector2(activeSign.Position.X + Level.tileWidth, 0);
            ObstaclePlatformSign.Position = new Vector2(DirectionSign.Position.X + Level.tileWidth, 0);

            Add(activeSign);
            Add(DirectionSign);
            Add(ObstaclePlatformSign);

            exit = new Button("Menu/Button Return to Menu", 0, 2);
            Add(exit);

            save = new Button("Menu/Button Save Chunk", 0, 2);
            save.Position = new Vector2(GameEnvironment.Screen.X - save.BoundingBox.Width, GameEnvironment.Screen.Y - save.BoundingBox.Height);
            Add(save);

            possibleBiomes.Add("Forest");
            possibleBiomes.Add("Cave");
            possibleBiomes.Add("Sky");
            possibleBiomes.Add("Volcanic");

            possibleSpawners[true] = new List<string>();//deadly spawner list
            possibleSpawners[false] = new List<string>();//safe spawner list

            //arrays for a lot of buttons and some spawner info
            isDeadlySpawner = new bool[Level.ChunkLength - 1];
            active = new bool[Level.ChunkLength - 1];
            chosenSpawners = new string[Level.ChunkLength - 1];
            toRight = new string[Level.ChunkLength - 1];
            spawnerActive = new Button[Level.ChunkLength - 1]; 
            spawnerDirection = new Button[Level.ChunkLength - 1];
            spawnerType = new Button[Level.ChunkLength - 1];
            spawnerNext = new Button[Level.ChunkLength - 1];
            spawnerPrev = new Button[Level.ChunkLength - 1];

            for (int i = 0; i < Level.ChunkLength - 1; i++)
            {
                //make all spawners go left, be deadly but also start inactive
                toRight[i] = "";
                isDeadlySpawner[i] = true;
                active[i] = false;
            }

            SetupButtons();
            SetupSpawnerButtons();
            
            GetSpawners();

            //add the cursor at the very end so it's drawn on top of everything
            Add(new Cursor());
        }

        /// <summary>
        /// creates and positions all the different buttons that don't handle spawners or switching of gamestates
        /// </summary>
        private void SetupButtons()
        {
            //setup for all the tile chosing buttons
            selectedTile = new SpriteGameObject("Menu/Selected Tile");
            background = new Button(TileString(), 0, 13);
            deadly = new Button(TileString(), 3, 13);
            road = new Button(TileString(), 1, 13);
            transition = new Button(TileString(), 7, 13);
            shop = new Button(TileString(), 10, 13);
            obstacle = new Button(TileString(), 11, 13);
            cloud = new Button("Realms/Sky/Thundercloud/abcdefgh", 0,1);
            platform = new Button("Realms/" + biome + "/Platform_Stationary 1x1", 0, 1);

            //move them all to the right positions
            background.Position = new Vector2(GameEnvironment.Screen.X / 1.25f, Level.tileHeight * 3);
            deadly.Position = new Vector2(0, Level.tileHeight * 1.5f) + background.Position;
            road.Position = new Vector2(0, Level.tileHeight * 1.5f) + deadly.Position;
            transition.Position = new Vector2(0, Level.tileHeight * 1.5f) + road.Position;
            shop.Position = new Vector2(0, Level.tileHeight * 1.5f) + transition.Position;
            obstacle.Position = new Vector2(0, Level.tileHeight * 1.5f) + shop.Position;
            cloud.Position = obstacle.Position;
            cloud.Visible = false;
            platform.Position = new Vector2(0, Level.tileHeight * 1.5f) + obstacle.Position;

            selectedTile.Position = background.Position - new Vector2(1);

            //add them all to the children list
            Add(selectedTile);
            Add(background);
            Add(deadly);
            Add(road);
            Add(transition);
            Add(shop);
            Add(obstacle);
            Add(cloud);
            Add(platform);

            //setup for all the special objects
            shield = new Button("SpecialObjects/shield 1", 0, 1);
            life = new Button("SpecialObjects/Life 1", 0, 1);
            duskflower = new Button("SpecialObjects/Duskflower 1", 0, 1);
            tadpole = new Button("SpecialObjects/tadpole 1", 0, 1);

            shield.Position = new Vector2(Level.tileHeight * 1.5f, 0) + background.Position;
            life.Position = new Vector2(Level.tileHeight * 1.5f, 0) + deadly.Position;
            duskflower.Position = new Vector2(Level.tileHeight * 1.5f, 0) + road.Position;
            tadpole.Position = new Vector2(Level.tileHeight * 1.5f, 0) + transition.Position;

            Add(shield);
            Add(life);
            Add(duskflower);
            Add(tadpole);

            //setup for the biome switching
            biomeButton = new Button("Menu/TextBox 5x1", 0, 1, biome);
            biomeButton.Position = new Vector2((Level.ChunkWidth * Level.tileWidth - biomeButton.BoundingBox.Width)/2, Level.tileHeight * 2);
            Add(biomeButton);

            biomeSwitch = new Button("Menu/Button Back", 0, 2, mirrored: true);
            biomeSwitch.Position = new Vector2(biomeButton.BoundingBox.Width, 0) + biomeButton.Position;
            Add(biomeSwitch);

            biomeSwitchBack = new Button("Menu/Button Back", 0, 2);
            biomeSwitchBack.Position = new Vector2(-biomeSwitchBack.BoundingBox.Width, 0) + biomeButton.Position;
            Add(biomeSwitchBack);
        }

        /// <summary>
        /// creates and positions all the buttons needed for the spawners of every lane
        /// </summary>
        private void SetupSpawnerButtons()
        {
            for (int i = 0; i < spawnerActive.Length; i++)
            {
                Button B = new Button("Menu/TextBox 1x1", 0, 1);
                B.Position = new Vector2(editor.Chunk.Position.X + Level.ChunkWidth * Level.tileWidth, editor.Chunk.Position.Y -(1 + i) * Level.tileHeight);
                spawnerActive[i] = B;
                Add(B);
            }

            for (int i = 0; i < spawnerDirection.Length; i++)
            {
                Button B = new Button("Menu/TextBox 1x1", 0, 1, "l");
                B.Position = spawnerActive[i].Position + new Vector2(spawnerActive[i].BoundingBox.Width, 0);
                spawnerDirection[i] = B;
                Add(B);
            }

            for (int i = 0; i < spawnerType.Length; i++)
            {
                Button B = new Button("Menu/TextBox 3x1", 0, 1);
                B.Position = spawnerDirection[i].Position + new Vector2(spawnerDirection[i].BoundingBox.Width, 0);
                spawnerType[i] = B;
                Add(B);
            }

            for (int i = 0; i < spawnerPrev.Length; i++)
            {
                Button B = new Button("Menu/Button Back");
                B.Position = spawnerType[i].Position + new Vector2(spawnerType[i].BoundingBox.Width, 0);
                spawnerPrev[i] = B;
                Add(B);
            }

            for (int i = 0; i < spawnerNext.Length; i++)
            {
                Button B = new Button("Menu/Button Back", mirrored:true);
                B.Position = spawnerPrev[i].Position + new Vector2(spawnerPrev[i].BoundingBox.Width, 0);
                spawnerNext[i] = B;
                Add(B);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            editor.Update(gameTime);
            cam.Follow();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, cam.Transform);
            editor.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }

        public override void Reset()
        {

        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            editor.HandleInput(inputHelper);

            CheckBasicButtons(inputHelper.MouseLeftButtonPressed());

            CheckSpawnerButtons(inputHelper.MouseLeftButtonPressed());
        }

        /// <summary>
        /// checks all buttons unrelated to spawners whether they've been clicked and then performs actions based on which one was clicked
        /// </summary>
        private void CheckBasicButtons(bool clicked)
        {
            if (clicked && background.IsOnButton)
            {
                TileButton(Tile.TileType.Background, "B", null, background.Position);
            }

            if (clicked && deadly.IsOnButton)
            {
                TileButton(Tile.TileType.Deadly, "W", null, deadly.Position);
            }

            if (clicked && road.IsOnButton)
            {
                TileButton(Tile.TileType.Road, "R", null, road.Position);
            }

            if (clicked && transition.IsOnButton)
            {
                TileButton(Tile.TileType.TransitionUp, "T", null, transition.Position);
            }

            if (clicked && shop.IsOnButton)
            {
                TileButton(Tile.TileType.Shop, "Q", null, shop.Position);
            }

            if (clicked && obstacle.IsOnButton && obstacle.Visible)
            {
                TileButton(Tile.TileType.Obstacle, "O", null, obstacle.Position);
            }

            if (clicked && cloud.IsOnButton && cloud.Visible)
            {
                TileButton(Tile.TileType.Thundercloud, "H", "SpecialObjects/Lightning", cloud.Position);
            }

            if (clicked && platform.IsOnButton && platform.Visible)
            {
                TileButton(Tile.TileType.Deadly, "P", "Realms/" + biome + "/Platform_Stationary 1x1", platform.Position);
            }

            if (clicked && shield.IsOnButton)
            {
                TileButton(Tile.TileType.Background, "S", "SpecialObjects/Shield", shield.Position);
            }

            if (clicked && life.IsOnButton)
            {
                TileButton(Tile.TileType.Background, "L", "SpecialObjects/Life", life.Position);
            }

            if (clicked && duskflower.IsOnButton)
            {
                TileButton(Tile.TileType.Background, "D", "SpecialObjects/Duskflower", duskflower.Position);
            }

            if (clicked && tadpole.IsOnButton)
            {
                TileButton(Tile.TileType.Background, "E", "SpecialObjects/tadpole", tadpole.Position);
            }

            if (clicked && biomeSwitch.IsOnButton)
            {
                ChangeBiome();
            }

            if (clicked && biomeSwitchBack.IsOnButton)
            {
                ChangeBiome(true);
            }

            if (clicked && exit.IsOnButton)
            {
                GameEnvironment.GameStateManager.SwitchTo("menuScreen");
            }

            if (clicked && save.IsOnButton)
            {
                editor.TryWriteFile(active);
            }
        }

        /// <summary>
        /// checks all buttons related to only the spawners whether they've been clicked and then performs their respective actions
        /// </summary>
        private void CheckSpawnerButtons(bool clicked)
        {

            //button that changes whether the spawner in a lane should be added to the textfile when saved
            foreach (Button B in spawnerActive)
            {
                if (clicked && B.IsOnButton)
                {
                    int index = Array.FindIndex(spawnerActive, x => x == B);

                    if (B.Sprite.Name == "Menu/TextBox 1x1")
                    {
                        B.ChangeSpriteSheet("Menu/TextBox_filled 1x1", 0, 1);
                        active[index] = true;
                    }
                    else
                    {
                        B.ChangeSpriteSheet("Menu/TextBox 1x1", 0, 1);
                        active[index] = false;
                    }
                }
            }

            //button that handles the direction of the spawner in a lane
            foreach (Button B in spawnerDirection)
            {
                if (clicked && B.IsOnButton)
                {
                    int index = Array.FindIndex(spawnerDirection, x => x == B);
                    //change the sprite and the corresponding string
                    if (B.GetText == "l")
                    {
                        B.ChangeText("r");//lowercase because it fits better in the button

                        toRight[index] = "R";
                    }
                    else
                    {
                        B.ChangeText("l");

                        toRight[index] = "";//to the left uses no letter
                    }
                    ChangeSpawnerInfo(index);
                }
            }

            //button that handles the changing of the type (deadly/safe) of a spawner in a lane
            foreach (Button B in spawnerType)
            {
                if (clicked && B.IsOnButton)
                {
                    int index = Array.FindIndex(spawnerType, x => x == B);

                    isDeadlySpawner[index] = !isDeadlySpawner[index];//change from deadly to safe or the other way around

                    chosenSpawners[index] = possibleSpawners[isDeadlySpawner[index]][0];//reset to the first possible spawner
                    
                    ChangeSpawnerInfo(index);
                }
            }

            foreach (Button B in spawnerNext)
            {
                if (clicked && B.IsOnButton)
                {
                    ChangeChosenSpawner(Array.FindIndex(spawnerNext, x => x == B));
                }
            }

            foreach (Button B in spawnerPrev)
            {
                if (clicked && B.IsOnButton)
                {
                    ChangeChosenSpawner(Array.FindIndex(spawnerPrev, x => x == B), true);
                }
            }
        }

        /// <summary>
        /// applies the tileType, textType, special object type to the chunkeditor and moves the SelectedTile sprite to te right position
        /// </summary>
        private void TileButton(Tile.TileType type, string textType, string special, Vector2 position)
        {
            editor.type = type;
            editor.textType = textType;
            editor.chosenSpecial = special;
            selectedTile.Position = position - new Vector2(1);
        }

        /// <summary>
        /// changes the chosen biome to the next or previous one based on the given bool.
        /// also changes the different button sprites, resets the chunk and disables the obstacle button depending on the biome
        /// </summary>
        private void ChangeBiome(bool backwards = false)
        {
            int BiomeIndex = possibleBiomes.FindIndex(0, x => x == biome);
            if (backwards)
            {
                if(BiomeIndex == 0)
                {
                    biome = possibleBiomes[possibleBiomes.Count - 1];
                }
                else
                {
                    biome = possibleBiomes[BiomeIndex - 1];
                }
            }
            else
            {
                if (BiomeIndex == possibleBiomes.Count-1)
                {
                    biome = possibleBiomes[0];
                }
                else
                {
                    biome = possibleBiomes[BiomeIndex + 1];
                }
            }

            //change the different button sprites
            if(biome != "Sky")
            {
                background.ChangeSpriteSheet(TileString(), 0, 13);
                deadly.ChangeSpriteSheet(TileString(), 3, 13);
                road.ChangeSpriteSheet(TileString(), 1, 13);
                transition.ChangeSpriteSheet(TileString(), 7, 13);
                shop.ChangeSpriteSheet(TileString(), 10, 13);
                obstacle.ChangeSpriteSheet(TileString(), 11, 13);
                obstacle.Visible = true;
                cloud.Visible = false;
            }
            else
            {//only the sky biome doesnt have obstacles
                background.ChangeSpriteSheet(TileString(), 0, 11);
                deadly.ChangeSpriteSheet(TileString(), 3, 11);
                road.ChangeSpriteSheet(TileString(), 1, 11);
                transition.ChangeSpriteSheet(TileString(), 7, 11);
                shop.ChangeSpriteSheet(TileString(), 10, 11);
                obstacle.Visible = false;
                cloud.Visible = true;
            }

            //only the forest and sky biome have stationary platforms
            if(biome == "Forest" || biome == "Sky")
            {
                platform.ChangeSpriteSheet("Realms/" + biome + "/Platform_Stationary 1x1", 0, 1);
                platform.Visible = true;
            }
            else
            {
                platform.Visible = false;
            }

            //reset the chosen button to basic tile
            editor.type = Tile.TileType.Background;
            editor.textType = "B";
            selectedTile.Position = background.Position - new Vector2(1);
            //change the biome and the text on the biomebutton
            editor.chosenBiome = biome;
            biomeButton.ChangeText(biome);
            //reset the chunk and get the spawners of the new biome
            GetSpawners();
            editor.Fillchunk();
        }

        /// <summary>
        /// changes the chosen spawner to the next or previous possible spawner
        /// </summary>
        private void ChangeChosenSpawner(int index, bool backwards = false)
        {
            int possibleSpawnerIndex = possibleSpawners[isDeadlySpawner[index]].FindIndex(0, x => x == chosenSpawners[index]);

            if(possibleSpawnerIndex == 0 && backwards)
            {//change it to the last spawner
                chosenSpawners[index] = possibleSpawners[isDeadlySpawner[index]][possibleSpawners[isDeadlySpawner[index]].Count -1];
            }
            else if (possibleSpawnerIndex == possibleSpawners[isDeadlySpawner[index]].Count - 1 && !backwards)
            {//change it to the first spawner
                chosenSpawners[index] = possibleSpawners[isDeadlySpawner[index]][0];
            }
            else
            {
                if (backwards)
                {//change it to the previous spawner
                    chosenSpawners[index] = possibleSpawners[isDeadlySpawner[index]][possibleSpawnerIndex - 1];
                }
                else
                {//change it to the next spawner
                    chosenSpawners[index] = possibleSpawners[isDeadlySpawner[index]][possibleSpawnerIndex + 1];
                }
            }

            ChangeSpawnerInfo(index);
        }

        /// <summary>
        /// gets all possible spawners from the current biome and resets all spawners to the first possible spawner
        /// </summary>
        private void GetSpawners()
        {
            StreamReader filereader = new StreamReader("Content/Realms/" + biome + "/PossibleSpawners.txt");
            //first line is all deadly spawners, second line is safe spawners
            string deadlyspawners = filereader.ReadLine();
            string safespawners = filereader.ReadLine();

            //reset both lists of possible spawners if they are not empty
            if(possibleSpawners[true].Count != 0)
            {
                possibleSpawners[true].RemoveRange(0, possibleSpawners[true].Count);
            }

            if (possibleSpawners[false].Count != 0)
            {
                possibleSpawners[false].RemoveRange(0, possibleSpawners[false].Count);
            }

            //split up all the spawnerinfo from the file
            foreach (string s in deadlyspawners.Split(';'))
            {
                possibleSpawners[true].Add(s);
            }

            foreach (string s in safespawners.Split(';'))
            {
                possibleSpawners[false].Add(s);
            }

            for (int i = 0; i < editor.spawnerinfo.Length; i++)
            {//sets all spawners to the first possible spawner
                chosenSpawners[i] = possibleSpawners[isDeadlySpawner[i]][0];
                ChangeSpawnerInfo(i);
            }
        }

        /// <summary>
        /// changes the spawner info in the editor and changes the spawnertype button text to show the right text
        /// </summary>
        /// <param name="index"></param>
        private void ChangeSpawnerInfo(int index)
        {
            editor.spawnerinfo[index] = toRight[index] + chosenSpawners[index];
            spawnerType[index].ChangeText(editor.spawnerinfo[index]);
        }

        /// <summary>
        /// returns the string required to get the current tilesheet
        /// </summary>
        private string TileString()
        {
            return "Realms/" + biome + "/Tiles";
        }
    }
}