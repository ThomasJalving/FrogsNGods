using System;
using Frogger.GameManagement.GameObj;
using Frogger.GameManagement;
using Microsoft.Xna.Framework;
using Frogger.GameManagement.LevelGenerator;
using Frogger.States;

namespace Frogger.GameObjects
{
    /// <summary>
    /// The class that handles everything to do with the player
    /// </summary>
    class Player : AnimatedGameObject
    {
        Vector2 StartPos = new Vector2((Level.ChunkWidth / 2) * Level.tileWidth, 0);
        public Level Level { get; set; }
        protected int lives = 3;
        private float maxHeight;
        protected int score;
        protected int shieldCount, invControlsCounter, flowerImmunity;
        protected bool onLog, previousOnLog;
        protected const float jumpLength = 0.1f;
        protected bool jumping, invincible;
        protected float jumpLock;

        public Player(Level level) : base("Player/Frog Up")
        {
            position = StartPos;
            this.Level = level;
            //setting up all animations
            LoadAnimation("up", "Player/Frog Up", 20, jumpLength / 20);
            LoadAnimation("down", "Player/Frog Down", 20, jumpLength / 20);
            LoadAnimation("left", "Player/Frog Side", 20, jumpLength / 20, true);
            animations["left"].Origin = new Vector2(Level.tileWidth / 2, 0);
            LoadAnimation("right", "Player/Frog Side", 20, jumpLength / 20);
            animations["right"].Origin = new Vector2(Level.tileWidth / 2, 0);

            LoadAnimation("shield up", "Player/Frog Up_Shield", 20, jumpLength / 20);
            LoadAnimation("shield down", "Player/Frog Down_Shield", 20, jumpLength / 20);
            LoadAnimation("shield left", "Player/Frog Side_Shield", 20, jumpLength / 20, true);
            animations["shield left"].Origin = new Vector2(16, 0);
            LoadAnimation("shield right", "Player/Frog Side_Shield", 20, jumpLength / 20);
            animations["shield right"].Origin = new Vector2(16, 0);

            LoadAnimation("death", "Player/Frog Death", 5, 0.3f);
        }

        public override void Update(GameTime gameTime)
        {
            previousOnLog = onLog;
            base.Update(gameTime);
            CheckIfJumping(gameTime);

            CheckCollision();

            if (onLog && (position.X < 0 || position.X > Level.tileWidth * (Level.ChunkWidth - 1))) 
            {
                Kill();
            }
            if (!onLog)
            {
                if (Level.GetTileFromPosition(GridPosition).type == Tile.TileType.Deadly && !jumping)
                { //player is standing on a deadly tile so they should die
                    Kill();
                }
                if (previousOnLog) //player needs to snap back to grid
                {
                    position.X = GridPosition.X;
                }
            }

            try
            {
                CheckTransitionTiles();
            }
            catch(NullReferenceException)
            {//this happens when the player goes outside of the screen while invincible, otherwise the player wouldve died before any problem occured
                position.X = StartPos.X/2;
            }

            Level.CheckBiomeInChunk(position);

            if(currentAnimationName == "death" && !animation.Animating)
            {
                DeathReset();
            }
        }

        /// <summary>
        /// moves the player based on the input, input can be a voice command or a simple key press. Also makes sure that the player can move in the desired direction
        /// </summary>
        public override void HandleInput(InputHelper inputHelper)
        {
            if (!jumping && currentAnimationName != "death")
            {
                if ((inputHelper.KeyPressed(Frogger.SettingsManager.GetKey("Up")) || inputHelper.KeyPressed(Frogger.SettingsManager.GetKey("UpSecondary"))) && IsMovePossible(1)) 
                {
                    IsNewLine();
                    jumping = true;
                    jumpLock = jumpLength;
                    velocity.Y -= Level.tileHeight / jumpLength;

                    if (invControlsCounter <= 0)
                    {
                        PlayPlayerAnimation("up");
                    }
                    else
                    {//invert, not *-1 because of platform momentum
                        velocity.Y += 2 * Level.tileHeight / jumpLength;
                        PlayPlayerAnimation("down");
                        invControlsCounter--;
                    }
                    return;
                }
                if ((inputHelper.KeyPressed(Frogger.SettingsManager.GetKey("Down")) || inputHelper.KeyPressed(Frogger.SettingsManager.GetKey("DownSecondary"))) && IsMovePossible(3))
                {
                    jumping = true;
                    jumpLock = jumpLength;
                    velocity.Y += Level.tileHeight / jumpLength;

                    if (invControlsCounter <= 0)
                    {
                        PlayPlayerAnimation("down");
                    }
                    else
                    {//invert
                        velocity.Y -= 2 * Level.tileHeight / jumpLength;
                        PlayPlayerAnimation("up");
                        invControlsCounter--;
                    }
                    return;
                }
                if ((inputHelper.KeyPressed(Frogger.SettingsManager.GetKey("Left")) || inputHelper.KeyPressed(Frogger.SettingsManager.GetKey("LeftSecondary"))) && IsMovePossible(4))
                {
                    jumping = true;
                    jumpLock = jumpLength;
                    velocity.X -= Level.tileWidth / jumpLength;

                    if (invControlsCounter <= 0)
                    {
                        PlayPlayerAnimation("left");
                    }
                    else
                    {//invert
                        velocity.X += 2 * Level.tileWidth / jumpLength;
                        PlayPlayerAnimation("right");
                        invControlsCounter--;
                    }
                    return;
                }
                if ((inputHelper.KeyPressed(Frogger.SettingsManager.GetKey("Rigth")) || inputHelper.KeyPressed(Frogger.SettingsManager.GetKey("RigthSecondary"))) && IsMovePossible(2))
                {
                    jumping = true;
                    jumpLock = jumpLength;
                    velocity.X += Level.tileWidth / jumpLength;

                    if (invControlsCounter <= 0)
                    {
                        PlayPlayerAnimation("right");
                    }
                    else
                    {//invert
                        velocity.X -= 2 * Level.tileWidth / jumpLength;
                        StartAnimation("left", jumpLength);
                        invControlsCounter--;
                    }
                    return;
                }
                             
                if (inputHelper.KeyPressed(GameEnvironment.SettingsManager.GetKey("DebugRealmDown"))) //just for testing
                {//moves you down by 1 layer if you're not in the cave realm
                    Level.GoDown();
                    if (Level.GetChunkFromPosition(GridPosition) == Level.activeChunks[0])
                        SpawnPlayerInChunk(Level.activeChunks[0]);
                    else
                        SpawnPlayerInChunk(Level.activeChunks[1]);
                }

                if (inputHelper.KeyPressed(GameEnvironment.SettingsManager.GetKey("DebugRealmUp"))) //just for testing
                {//moves you up by 1 layer if you're not in the sky realm
                    Level.GoUp();
                    if(Level.GetChunkFromPosition(GridPosition) == Level.activeChunks[0])
                        SpawnPlayerInChunk(Level.activeChunks[0]);
                    else
                        SpawnPlayerInChunk(Level.activeChunks[1]);
                }

                if (inputHelper.KeyPressed(GameEnvironment.SettingsManager.GetKey("DebugTeleport"))) //just for testing
                {//teleports you one chunk ahead
                    if(Level.GetChunkFromPosition(Position) == Level.activeChunks[0] )
                        SpawnPlayerInChunk(Level.activeChunks[1]);
                    else
                        SpawnPlayerInChunk(Level.activeChunks[2]);
                }

                if (inputHelper.KeyPressed(GameEnvironment.SettingsManager.GetKey("DebugInvincible"))) //just for testing
                {//makes you immune to all deaths, having a platform move you off screen is not recommended
                    if (invincible)
                        invincible = false;
                    else
                        invincible = true;
                }

                if (inputHelper.KeyPressed(GameEnvironment.SettingsManager.GetKey("DebugShield"))) //just for testing
                {//gives you 2 shield charges extra, can keep using for semi-invincibility
                    shieldCount += 2;
                    CheckShieldAnimation();
                }
            }
        }

        /// <summary>
        /// starts the animation for a specific direction, made to efficiently differentiate between the shielded and non-shielded animations
        /// </summary>
        private void PlayPlayerAnimation(string direction)
        {
            if (shieldCount > 0)
            {
                StartAnimation("shield " + direction, jumpLock);
            }
            else
            {
                StartAnimation(direction, jumpLength);
            }
        }

        private void CheckIfJumping(GameTime gameTime)
        {
            if (!jumping)
            {
                return;
            }

            jumpLock -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (currentAnimationName != "left" && currentAnimationName != "right" && currentAnimationName != "shield left" && currentAnimationName != "shield right")
            {
                velocity.X = 0;
            }
            if (jumpLock <= 0)
            {
                position += jumpLock * velocity;
                position.X = (float)Math.Round(position.X);
                position.Y = (float)Math.Round(position.Y);
                jumping = false;
            }
        }

        private void CheckTransitionTiles()
        {
            if (Level.GetTileFromPosition(GridPosition).type == Tile.TileType.TransitionDown && !jumping)
            {
                Level.GoDown();
                if (Level.GetChunkFromPosition(GridPosition) == Level.activeChunks[0])
                    SpawnPlayerInChunk(Level.activeChunks[0]);
                else
                    SpawnPlayerInChunk(Level.activeChunks[1]);
            }
            else if (Level.GetTileFromPosition(GridPosition).type == Tile.TileType.TransitionUp && !jumping)
            {
                Level.GoUp();
                if (Level.GetChunkFromPosition(GridPosition) == Level.activeChunks[0])
                    SpawnPlayerInChunk(Level.activeChunks[0]);
                else
                    SpawnPlayerInChunk(Level.activeChunks[1]);
            }
            else if (Level.GetTileFromPosition(GridPosition).type == Tile.TileType.Shop && !jumping)
            {
                Point Tileposition = new Point((int)position.X / Level.tileWidth, (-(int)position.Y / Level.tileHeight) % Level.ChunkLength);
                Level.GetChunkFromPosition(position).SetTile(Tileposition.X, Tileposition.Y, new Tile(Tile.TileType.Background, Level.activeGod.BiomeName));
                Level.PauseGodTimers();
                Frogger.GameStateManager.SwitchTo("shop");//open shop
                //pause game (automatically done when overlay is triggered)
            }
        }

        private void CheckShieldAnimation()
        {
            if (shieldCount <= 0 && currentAnimationName.Contains("shield"))
            {
                StartAnimation(currentAnimationName.Split(' ')[1]);
            }
            else if(!currentAnimationName.Contains("shield"))
            {
                StartAnimation("shield " + currentAnimationName);
            }
        }

        /// <summary>
        /// checks if the player can move in the desired direction, 1 is up, 2 is right, 3 is down, 4 is left
        /// </summary>
        public bool IsMovePossible(int direction)
        {
            Vector2 addition = Vector2.Zero;
            if (invControlsCounter > 0 && direction < 3) //reverse direction when controls are reversed
            {
                direction += 2;
            } else if (invControlsCounter > 0)
            {
                direction -= 2;
            }

            switch (direction)
            {
                case 1:
                    addition = new Vector2(0, -Level.tileHeight);
                    break;
                case 2:
                    addition = new Vector2(Level.tileWidth, 0);
                    break;
                case 3:
                    addition = new Vector2(0, Level.tileHeight);
                    break;
                case 4:
                    addition = new Vector2(-Level.tileWidth, 0);
                    break;
            }

            if (Level.GetTileFromPosition(GridPosition + addition) != null && Level.GetTileFromPosition(GridPosition + addition).type != Tile.TileType.Obstacle && Level.GetTileFromPosition(GridPosition + addition).type != Tile.TileType.Gate)
            {
                Level.CheckActiveChunks(position);
                return true;
            }

            return false;
        }
        /// <summary>
        /// checks if the player is colliding with an obstacle, the method first uses a more efficient but less precise method and then if it detects a hit.
        /// it uses a pixel perfect (less efficient but more precise) method to make sure it wasn't a false positive
        /// </summary>
        public void CheckCollision()
        {
            foreach(Chunk c in Level.activeChunks)
            {
                foreach (SpecialGameObject other in c.specialObjects.Children)
                {
                    if (!other.Visible)
                    {
                        return;
                    }

                    Rectangle otherBox = other.BoundingBox;

                    if (otherBox.Intersects(BoundingBox) && PerPixelCollision(this, other))
                    {
                        other.Effect();
                        CheckPowerUp(other);
                        if (other.Pickupable)
                        {
                            if (other.tadpole)
                            {
                                score += 500;
                                other.RemoveGate(Level.GetChunkFromPosition(Position), Level.activeGod.BiomeName);
                            }
                            c.specialObjects.Children.Remove(other);
                        }
                        break;
                    }
                    
                }
            }

            if (!jumping)
            {
                onLog = false;
                velocity = Vector2.Zero;
            }

            foreach (Chunk c in Level.activeChunks)
            {
                foreach (LaneObject other in c.laneObjects.Children)
                {
                    LaneObjectCollision(other);
                }
            }
        }

        private void IsNewLine()
        {
            if (position.Y < maxHeight)
            {
                maxHeight = position.Y;
                score += 10;
            }
        }

        /// <summary>
        /// specifically handles collisions with LaneObjects
        /// </summary>
        private void LaneObjectCollision(LaneObject other)
        {
            if (!other.Visible) //checks if the object is visible to make sure collision is actually possible
            {
                return;
            }

            Rectangle otherBox = other.BoundingBox;

            if (otherBox.Intersects(BoundingBox) && PerPixelCollision(this, other))
            {
                if (other.Deadly)
                {
                    if (shieldCount <= 0) //When not shielded, frog is dead
                    {
                        Kill();                    
                    }
                    else //lose a shield charge and removed the collided object to give the player some safety
                    {
                        shieldCount--;
                        other.Visible = false;//doesnt directly delete because we are looping with a foreach
                        CheckShieldAnimation();
                    }
                }
                else if (!jumping && !other.Fake && currentAnimationName !="death")
                {
                    onLog = true;
                    velocity = other.Velocity;
                }
            }
            
        }

        /// <summary>
        /// Returns a left to rightmirrored version of an array with given width and height
        /// </summary>
        public Color[] MirroredArray(Color[] original, int width, int height)
        {
            Color[] result = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    result[y * width + x] = original[y * width + width - 1 - x];
                }
            }
            return result;
        }

        /// <summary>
        ///used to see if the objects are colliding at a pixel perfect level, changes it behaviour depending on what subclass the other object is
        /// </summary>
        public bool PerPixelCollision(Player player, SpriteGameObject other)
        {
            Color[] pixelsA = new Color[player.animation.Source.Width * player.animation.Source.Height];
            player.animation.Sheet.GetData(0,player.animation.Source,pixelsA,0,player.animation.Source.Width*player.animation.Source.Height);
            if (player.animation.Mirror)
                pixelsA = MirroredArray(pixelsA, player.animation.Source.Width, player.animation.Source.Height);

            Color[] pixelsB = new Color[other.Sprite.Width * other.Sprite.Height];

            if(other is AnimatedGameObject otherAnimated)
            {
                otherAnimated.CurrentAnimation.Sheet.GetData(0, otherAnimated.CurrentAnimation.Source, pixelsB, 0, otherAnimated.CurrentAnimation.Source.Width * otherAnimated.CurrentAnimation.Source.Height);
                if (otherAnimated.CurrentAnimation.Mirror)
                    pixelsB = MirroredArray(pixelsB, other.Sprite.Width, other.Sprite.Height);
            } else if (other is SpriteSheetGameObject otherSheet)
            {
                otherSheet.Sheet.Sheet.GetData(0, otherSheet.Sheet.Source, pixelsB, 0, otherSheet.Sheet.Source.Width * otherSheet.Sheet.Source.Height);
                if (otherSheet.Sheet.Mirror)
                    pixelsB = MirroredArray(pixelsB, other.Sprite.Width, other.Sprite.Height);
            } else
            {
                other.Sprite.GetData(pixelsB);
            }

            int x1 = Math.Max(player.BoundingBox.X, other.BoundingBox.X);
            int x2 = Math.Min(player.BoundingBox.X + player.BoundingBox.Width, other.BoundingBox.X + other.BoundingBox.Width);

            int y1 = Math.Max(player.BoundingBox.Y, other.BoundingBox.Y);
            int y2 = Math.Min(player.BoundingBox.Y + player.BoundingBox.Height, other.BoundingBox.Y + other.BoundingBox.Height);

            //loop through pixels
            for (int y = y1; y < y2; ++y)
            {
                for (int x = x1; x < x2; ++x)
                {
                    // Get the colour value of the pixel
                    Color a = pixelsA[(x - player.BoundingBox.X) + (y - player.BoundingBox.Y) * player.BoundingBox.Width];
                    Color b = pixelsB[(x - other.BoundingBox.X) + (y - other.BoundingBox.Y) * other.BoundingBox.Width];

                    if (a.A != 0 && b.A != 0) // if the value isn't zero (pixel isn't transparent), they collide
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks which type of SpecialGameObject the player collided with to give them buffs or debuffs
        /// </summary>
        public void CheckPowerUp(SpecialGameObject s)
        {
            if (s.ActiveShield)
            {
                shieldCount = 2;
                CheckShieldAnimation();
            }

            if (s.Life && lives < 25)
            {
                lives++;
            }

            if (s.InverseControls)
            {
                invControlsCounter = 10 - flowerImmunity;
                flowerImmunity = 0;
            }

            if (s.Electrocuted)
            {
                Kill();
            }
        }

        /// <summary>
        /// Checks what is bought in the shop and activates the right effect 
        /// </summary>
        /// <param name="id">A number corresponding to a shop item</param>
        /// <param name="itemCost">The cost of the item</param>
        public void ShopItemBought(int id, int itemCost)
        {
            switch (id)
            {
                //shield
                case 0:
                    shieldCount += 2;
                    CheckShieldAnimation();
                    break;
                //DuskFlower immunity
                case 1:
                    flowerImmunity += 5;
                    break;
                //life
                case 2:
                    lives++;
                    break;
            }
            //Decreases score by item price
            score -= itemCost;
        }


        /// <summary>
        /// stops the player and starts playing the death animation
        /// </summary>
        public void Kill()
        {
            if (currentAnimationName != "death" && !invincible)
            {
                StartAnimation("death", 1.5f);
                if (Lives <= 0)
                {
                    if (Frogger.GameStateManager.GetGameState("gameOver") is GameOver gameOver)
                        gameOver.SubmittableScore = score;
                    Frogger.GameStateManager.SwitchTo("gameOver");
                    return;
                }
                shieldCount = 0;
                velocity = Vector2.Zero;
                maxHeight = 0;
                jumping = false;
                onLog = false;
                previousOnLog = false;
                lives--;
            }
        }

        /// <summary>
        /// teleports the player back to the oldest active chunk
        /// </summary>
        private void DeathReset()
        {
            StartAnimation("up");
            SpawnPlayerInChunk(Level.activeChunks[0]);
            Level.activeGod.OnPlayerDeath();
        }

        /// <summary>
        /// Spawns the player in the given chunk
        /// </summary>
        public void SpawnPlayerInChunk(Chunk c)
        {
            position = c.Position + new Vector2(GridPosition.X,0); ;
            for (int i = 1; i < Level.activeChunks.Count; i++)
            {
                Level.activeChunks[i].ApplyDifficulty();
            }
        }

        /// <summary>
        /// returns a Vector2 containing the precise player location on the grid
        /// </summary>
        public Vector2 GridPosition
        {
            get
            {
                Vector2 snappedpos = Vector2.Zero;
                float rest = position.X % 32;
                if (rest >= 16)
                {
                    snappedpos.X = position.X + Level.tileWidth - rest;
                }
                else
                {
                    snappedpos.X = position.X - rest;
                }

                float rest2 = position.Y % 32;
                if (rest2 >= 16)
                {
                    snappedpos.Y = position.Y + Level.tileHeight - rest2;
                }
                else
                {
                    snappedpos.Y = position.Y - rest2;
                }

                return snappedpos;
            }
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        public int Lives
        {
            get { return lives; }
        }

        public int Immunity
        {
            get { return flowerImmunity; }
        }
    }
}
