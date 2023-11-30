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
    class BlueFish : IDraw
    {
        // CLASS VARIABLES
        // Variables hold the information for the class
        // NOTE - these variables must be present for the class to act as a TOKEN for the FishORama engine
        private string textureID;               // Holds a string to identify asset used for this token
        private float xPosition;                // Holds the X coordinate for token position on screen
        private float yPosition;                // Holds the Y coordinate for token position on screen
        private int xDirection;                 // Holds the direction the token is currently moving - X value should be either -1 (left) or 1 (right)
        private int yDirection;                 // Holds the direction the token is currently moving - Y value should be either -1 (down) or 1 (up)
        private Screen screen;                  // Holds a reference to the screen dimansions (width and height)
        private ITokenManager tokenManager;     // Holds a reference to the TokenManager - for access to ChickenLeg variable

        // *** ADD YOUR CLASS VARIABLES HERE *** 

        public static readonly Vector2 imageAssetBounds = new Vector2(128, 86);

        const string bubbleTextureID = "Bubble";

        float standardSpeed;
        Vector2 targetPosition;
        Random random = new Random();
        public List<Bubble> bubbles = new List<Bubble>();

        const int maxTargetDistance = 50;
        const int minimumTargetCoordChange = 300;


        /// CONSTRUCTOR: OrangeFish Constructor
        /// The elements in the brackets are PARAMETERS, which will be covered later in the course
        public BlueFish(string pTextureID, float pXpos, float pYpos, Screen pScreen, ITokenManager pTokenManager)
        {
            // State initialisation (setup) for the object
            textureID = pTextureID;
            xPosition = pXpos;
            yPosition = pYpos;
            xDirection = 1;
            yDirection = 1;
            screen = pScreen;
            tokenManager = pTokenManager;

            // *** ADD OTHER INITIALISATION (class setup) CODE HERE ***

            // max is 41 not 40 as random upper bound is exclusive (doesnt include the max value)
            // i am passing in 30-41 instead of 3-5 due to 30-41 / 10 giving more accuracy and therefore more variety of speeds
            standardSpeed = random.Next(30, 41) / 10f;

            GenRandomTarget();

        }

        /// METHOD: Update - will be called repeatedly by the Update loop in Simulation
        /// Write the movement control code here
        public void Update()
        {
            // *** ADD YOUR MOVEMENT/BEHAVIOUR CODE HERE ***

            float coordinateDiff = Math.Abs(xPosition - targetPosition.X) + Math.Abs(yPosition - targetPosition.Y);

            // if at target make new one
            if (coordinateDiff <= maxTargetDistance)
            {
                GenRandomTarget();
                // spawns a bubble when the target is hit
                SpawnBubble();
            }

            // vector movement
            Vector2 position = new Vector2(xPosition, yPosition);

            Vector2 direction = Vector2.Normalize(targetPosition - position);
            if(direction.X >= 0)
            {
                xDirection = 1;
            }
            else
            {
                xDirection = -1;
            }

            xPosition += direction.X * standardSpeed;
            yPosition += direction.Y * standardSpeed;

            // updates all bubbles and checks if they need removing
            for(int i = 0; i < bubbles.Count;)
            {
                bubbles[i].Update();
                if (bubbles[i].removeFlag)
                {
                    bubbles.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

        }
        private void GenRandomTarget()
        {
            float randomX, randomY;
            float diff;
            // loops until the random coordinates are at far enough away from the current coordinates. far enough is determined by the minimumTargetCoordChange variable
            do {
                randomX = random.Next((int)(-screen.width / 2 + (imageAssetBounds.X / 2)), (int)(screen.width / 2 - (imageAssetBounds.X / 2)));
                randomY = random.Next((int)(-screen.height / 2 + (imageAssetBounds.Y / 2)), (int)(screen.height / 2 - (imageAssetBounds.Y / 2)));
                // takes random coordinates away from current ones and gets the positive difference of the 2
                diff = Math.Abs(randomX - xPosition) + Math.Abs(randomY - yPosition);

            } while(diff < minimumTargetCoordChange);
            // assigns new target
            targetPosition = new Vector2(randomX, randomY);
        }
        private void SpawnBubble()
        {
            // adds a new instance of a bubble with its starting psoition slightly above the fish
            bubbles.Add(new Bubble(bubbleTextureID, new Vector2(xPosition, yPosition + imageAssetBounds.Y), screen));
        }

        /// METHOD: Draw - Called repeatedly by FishORama engine to draw token on screen
        /// DO NOT ALTER, and ensure this Draw method is in each token (fish) class
        /// Comments explain the code - read and try and understand each lines purpose
        public void Draw(IGetAsset pAssetManager, SpriteBatch pSpriteBatch)
        {
            // draws the bubbles to the screen
            foreach(Bubble bubble in bubbles)
            {
                bubble.Draw(pAssetManager, pSpriteBatch);
            }

            Asset currentAsset = pAssetManager.GetAssetByID(textureID); // Get this token's asset from the AssetManager

            SpriteEffects horizontalDirection; // Stores whether the texture should be flipped horizontally

            if (xDirection < 0)
            {
                // If the token's horizontal direction is negative, draw it reversed
                horizontalDirection = SpriteEffects.FlipHorizontally;
            }
            else
            {
                // If the token's horizontal direction is positive, draw it regularly
                horizontalDirection = SpriteEffects.None;
            }

            // Draw an image centered at the token's position, using the associated texture / position
            pSpriteBatch.Draw(currentAsset.Texture,                                             // Texture
                              new Vector2(xPosition, yPosition * -1),                                // Position
                              null,                                                             // Source rectangle (null)
                              Color.White,                                                      // Background colour
                              0f,                                                               // Rotation (radians)
                              new Vector2(currentAsset.Size.X / 2, currentAsset.Size.Y / 2),    // Origin (places token position at centre of sprite)
                              new Vector2(1, 1),                                                // scale (resizes sprite)
                              horizontalDirection,                                              // Sprite effect (used to reverse image - see above)
                              1);                                                               // Layer depth
        }
    }
}
