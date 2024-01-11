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

        // dont need this for X as the sink and rise states dont altar the speedX variable
        float standardSpeedY;
        const float sinkRiseSpeed = 1;
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
            // i am passing in 20-51 instead of 2-6 / 2 due to 20-51 / 20 giving more accuracy and therefore more variety of speeds
            // divding by 20 as standard movement is 45 degrees rather than straight, so the total speed will be roughly speedX + speedY or roughly between 2 and 5
            speedX = random.Next(20, 51) / 20;
            speedY = random.Next(20, 51) / 20;
            minY = yPosition - 50;
            maxY = yPosition + 50;

            standardSpeedY = speedY;

            // generates the amount of frames needed to sink or float randomly between 2 constant bounds
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
                xDirection = -1;
                hitBoundry = true;
            }
            else if (xPosition <= -screen.width / 2 + (imageAssetBounds.X / 2))
            {
                xDirection = 1;
                hitBoundry = true;
            }
            else if (yPosition >= (screen.height / 2) - (imageAssetBounds.Y / 2))
            {
                hitBoundry = true;
                yDirection = -1;
            }
            else if (yPosition <= -screen.height / 2 + (imageAssetBounds.Y / 2))
            {
                hitBoundry = true;
                yDirection = 1;
            }

            // if not sinking or rising aka normal behaviour
            if (!isSinkingOrRising)
            {
                HandleStandardMovement(hitBoundry);
                frameCount++;
                // if there has been sufficient frames since the last sink or rise
                if (frameCount >= framesBetweenSinkOrRise)
                {
                    // sets frames needed for next sink or rise to a random amount of frames between 2 constants
                    framesBetweenSinkOrRise = random.Next(minFramesBetweenSinkOrRise, maxFramesBetweenSinkOrRise);
                    frameCount = 0;
                    isSinkingOrRising = true;
                    int randomNum = random.Next(2);
                    // if num is 0 set sinking
                    if (randomNum == 0)
                    {
                        // abs gets the positive version of the number that is invereted so it is certain that the speed is negative
                        speedY = sinkRiseSpeed;
                        yDirection = -1;
                        sinkingMode = true;
                        minSinkY = yPosition - 100;
                    }
                    // else set rising
                    else
                    {
                        speedY = sinkRiseSpeed;
                        yDirection = 1;
                        maxFloatY = yPosition + 100;
                    }
                }
            }
            else
            {
                // if a boundry is hit min and max Y is reset and sinking/rising mode is stopped
                if(hitBoundry)
                {
                    SwitchToNormState();
                    // stops method execution
                    return;
                }
                // if sinking mode and already sunk 100 units
                if (sinkingMode && yPosition <= minSinkY)
                {
                    SwitchToNormState();
                    return;
                }
                // if rising mode and already risen 100 units
                else if (!sinkingMode && yPosition >= maxFloatY)
                {
                    SwitchToNormState();
                    return;
                }
                yPosition += speedY * yDirection;
            }



        }
        void SwitchToNormState()
        {
            // if sinking mode set the new zig zag boundry max to 100 above the current value
            if (sinkingMode)
            {
                sinkingMode = false;
                minY = yPosition;
                maxY = yPosition + 100;
                speedY = standardSpeedY;
            }
            // else if rising set min to 100 below
            else
            {
                maxY = yPosition;
                minY = yPosition - 100;
                speedY = standardSpeedY;
            }
            // go back to normal state
            isSinkingOrRising = false;
        }
        /// <summary>
        /// handles the standard (zigzag) movement including adding speed to position
        /// </summary>
        /// <param name="hitBoundry"></param>
        private void HandleStandardMovement(bool hitBoundry)
        {
            // changes Y direction when zig-zag Y threshold met
            if (yPosition >= maxY)
            {
                yDirection = -1;
            }
            else if (yPosition <= minY)
            {
                yDirection = 1;
            }

            // adds speed to the overall position
            xPosition += speedX * xDirection;
            yPosition += speedY * yDirection;
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
