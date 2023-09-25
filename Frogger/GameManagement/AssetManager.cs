using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Frogger.GameManagement
{
    /// <summary>
    /// Manages the assets
    /// </summary>
    class AssetManager
    {
        public static ContentManager contentManager;

        public static Texture2D GetSprite(string spriteName)
        {
            if(spriteName == null || spriteName == "")
            {
                return null;
            }
            return contentManager.Load<Texture2D>(spriteName);
        }

        public static SpriteFont GetSpriteFont(string fontName)
        {
            if (fontName == null || fontName == "")
            {
                return null;
            }
            return contentManager.Load<SpriteFont>(fontName);
        }

        public static void PlaySound(string soundName)
        {
            SoundEffect soundEffect = contentManager.Load<SoundEffect>(soundName);
            soundEffect.Play();
        }

        public static void PlayMusic(string musicName, bool repeat = true)
        {
            MediaPlayer.Stop();
            MediaPlayer.IsRepeating = repeat;
            MediaPlayer.Play(contentManager.Load<Song>(musicName));
        }
    }
}
