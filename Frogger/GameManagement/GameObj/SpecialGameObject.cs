using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frogger.GameObjects;
using System;
using Frogger.GameManagement.LevelGenerator;

namespace Frogger.GameManagement.GameObj
{
    /// <summary>
    /// The class for GameObjects that have to be able to cause a certain effect
    /// </summary>
    class SpecialGameObject : AnimatedGameObject
    {
        public bool Pickupable;
        public bool ActiveShield, Life, InverseControls, Electrocuted, tadpole;
        public string objectType;

        public SpecialGameObject(bool pickupable, string spritename, Vector2 Position) 
            :base(spritename)
        {
            Texture2D measurement = AssetManager.GetSprite(spritename);
            int frames = measurement.Height / measurement.Width;
            LoadAnimation("animation", spritename, frames, 0.2f, looping: true);
            Pickupable = pickupable;
            objectType = spritename.Split('/')[1];
            position = Position;
        }

        /// <summary>
        /// Decides what effect to call
        /// </summary>
        public void Effect() 
        {
            switch (objectType) 
            {
                case "Tadpole":
                    Tadpole();
                    break;
                case "Shield":
                    Shield();
                    break;
                case "Life":
                    ExtraLife();
                    break;
                case "Duskflower":
                    FlowerPollen();
                    break;
                case "Lightning":
                    Lightning();
                    break;
                case null:
                    break;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

        //the different SpecialGameObject effects
        public void Tadpole()
        {
            tadpole = true;
        }
        public void Shield()
        {
            ActiveShield = true;
        }
        public void ExtraLife()
        {
            Life = true;
        }
        public void FlowerPollen()
        {
            InverseControls = true;
        }
        public void Lightning()
        {
            if (CurrentAnimation.Step >= 7)
                Electrocuted = true;
            else
                Electrocuted = false;
        }

        /// <summary>
        /// Removes the impassable row at the end of a chunk
        /// </summary>
        public void RemoveGate(Chunk c, string realm)
        {
            for (int x = 0; x < Level.ChunkWidth; x++)
            {
                Tile backgroundTile = new Tile(Tile.TileType.Background, realm);
                int y = Level.ChunkLength - 1;
                c.SetTile(x, y, backgroundTile);
            }
        }
    }
}