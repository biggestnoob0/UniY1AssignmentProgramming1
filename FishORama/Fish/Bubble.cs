using FishORamaEngineLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishORama.Fish
{
    class Bubble : IDraw
    {
        private string textureID;
        private Vector2 position;
        private Screen screen;
        public bool removeFlag = false;
        const float SpeedY = 2.5f;

        public static readonly Vector2 imageAssetBounds = new Vector2(32, 32);

        public Bubble(string textureID, Vector2 position, Screen screen)
        {
            this.textureID = textureID;
            this.position = position;
            this.screen = screen;
        }
        public void Update()
        {
            // moves up and checks if off screen. if off screen lets parent know by updaing the remove flag.
            position.Y += SpeedY;
            if(position.Y > screen.height / 2 + (imageAssetBounds.Y / 2))
            {
                removeFlag = true;
            }
        }

        public void Draw(IGetAsset pAssetManager, SpriteBatch pSpriteBatch)
        {
            Asset currentAsset = pAssetManager.GetAssetByID(textureID); // Get this token's asset from the AssetManager

            // Draw an image centered at the token's position, using the associated texture / position
            pSpriteBatch.Draw(currentAsset.Texture,                                             // Texture
                              new Vector2(position.X, position.Y * -1),                                // Position
                              null,                                                             // Source rectangle (null)
                              Color.White,                                                      // Background colour
                              0f,                                                               // Rotation (radians)
                              new Vector2(currentAsset.Size.X / 2, currentAsset.Size.Y / 2),    // Origin (places token position at centre of sprite)
                              new Vector2(1, 1),                                                // scale (resizes sprite)
                              SpriteEffects.None,                                              // Sprite effect (used to reverse image - see above)
                              1);                                                               // Layer depth
        }
    }
}
