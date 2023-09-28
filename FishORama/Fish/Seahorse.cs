using FishORamaEngineLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FishORama.Fish
{
    class Seahorse : IDraw
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

        public static readonly Vector2 imageAssetBounds = new Vector2(74, 128);
        const int minFramesBetweenSinkOrRise = 90;
        const int maxFramesBetweenSinkOrRise = 300;


        float speedX;
        float speedY;
        Random random = new Random();

        float minY;
        float maxY;

        float minSinkY;
        float maxFloatY;

        bool isSinkingOrRising;
        bool sinkingMode;

        int framesBetweenSinkOrRise;
        int frameCount = 0;

        bool hitBoundry;


        /// CONSTRUCTOR: OrangeFish Constructor
        /// The elements in the brackets are PARAMETERS, which will be covered later in the course
        public Seahorse(string pTextureID, float pXpos, float pYpos, Screen pScreen, ITokenManager pTokenManager)
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

            // max is 51 not 50 as random upper bound is exclusive (doesnt include the max value)
            // i am passing in 20-51 instead of 2-6 due to 20-51 / 10 giving more accuracy and therefore more variety of speeds
            speedX = random.Next(20, 51) / 10;
            speedY = random.Next(20, 51) / 10;
            minY = yPosition - 50;
            maxY = yPosition + 50;


            framesBetweenSinkOrRise = random.Next(minFramesBetweenSinkOrRise, maxFramesBetweenSinkOrRise);
        }

        /// METHOD: Update - will be called repeatedly by the Update loop in Simulation
        /// Write the movement control code here
        public void Update()
        {
            // *** ADD YOUR MOVEMENT/BEHAVIOUR CODE HERE ***
            hitBoundry = false;

            //reverse direction if edge of screen reached (factors in image width)
            if (xPosition >= (screen.width / 2) - (imageAssetBounds.X / 2))
            {
                //handles speed
                speedX = -Math.Abs(speedX);
                xDirection = -1;
                hitBoundry = true;
            }
            else if (xPosition <= -screen.width / 2 + (imageAssetBounds.X / 2))
            {
                speedX = Math.Abs(speedX);
                xDirection = 1;
                hitBoundry = true;
            }
            else if (yPosition >= (screen.height / 2) - (imageAssetBounds.Y / 2))
            {
                speedY = -Math.Abs(speedY);
                hitBoundry = true;
                yDirection = -1;
            }
            else if (yPosition <= -screen.height / 2 + (imageAssetBounds.Y / 2))
            {
                speedY = Math.Abs(speedY);
                hitBoundry = true;
                yDirection = 1;
            }


            if (!isSinkingOrRising)
            {
                HandleMovement(hitBoundry);
                frameCount++;
                if (frameCount >= framesBetweenSinkOrRise)
                {
                    framesBetweenSinkOrRise = random.Next(minFramesBetweenSinkOrRise, maxFramesBetweenSinkOrRise);
                    frameCount = 0;
                    isSinkingOrRising = true;
                    int randomNum = random.Next(2);
                    // if num is 0 set sinking
                    if (randomNum == 0)
                    {
                        // abs gets the positive version of the number that is invereted so it is certain that the speed is negative
                        speedY = -Math.Abs(speedY);
                        sinkingMode = true;
                        minSinkY = yPosition - 100;
                    }
                    else
                    {
                        speedY = Math.Abs(speedY);
                        maxFloatY = yPosition + 100;
                    }
                }
            }
            else
            {
                if(hitBoundry)
                {
                    SetMinMaxY();
                    sinkingMode = false;
                    isSinkingOrRising = false;
                    return;
                }
                if (sinkingMode && yPosition <= minSinkY)
                {
                    SetMinMaxY();
                    sinkingMode = false;
                    isSinkingOrRising = false;
                    return;
                }
                else if (!sinkingMode && yPosition >= maxFloatY)
                {
                    SetMinMaxY();
                    isSinkingOrRising = false;
                    return;
                }
                yPosition += speedY;
            }



        }
        void SetMinMaxY()
        {
            if (sinkingMode)
            {
                sinkingMode = false;
                minY = yPosition;
                maxY = yPosition + 100;
            }
            else
            {
                maxY = yPosition;
                minY = yPosition - 100;
            }
        }

        private void HandleMovement(bool hitBoundry)
        {
            if (yPosition >= maxY)
            {
                speedY = -Math.Abs(speedY);
            }
            else if (yPosition <= minY)
            {
                speedY = Math.Abs(speedY);
            }

            // adds speed to the overall position
            xPosition += speedX;
            yPosition += speedY;
        }

        /// METHOD: Draw - Called repeatedly by FishORama engine to draw token on screen
        /// DO NOT ALTER, and ensure this Draw method is in each token (fish) class
        /// Comments explain the code - read and try and understand each lines purpose
        public void Draw(IGetAsset pAssetManager, SpriteBatch pSpriteBatch)
        {
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
