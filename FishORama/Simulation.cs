using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FishORamaEngineLibrary;
using FishORama.Fish;

namespace FishORama
{
    /// CLASS: Simulation class - the main class users code in to set up a FishORama simulation
    /// All tokens to be displayed in the scene are added here
    public class Simulation : IUpdate, ILoadContent
    {
        // CLASS VARIABLES
        // Variables store the information for the class
        private IKernel kernel;                 // Holds a reference to the game engine kernel which calls the draw method for every token you add to it
        private Screen screen;                  // Holds a reference to the screeen dimensions (width, height)
        private ITokenManager tokenManager;     // Holds a reference to the TokenManager - for access to ChickenLeg variable

        /// PROPERTIES
        public ITokenManager TokenManager      // Property to access chickenLeg variable
        {
            set { tokenManager = value; }
        }

        // *** ADD YOUR CLASS VARIABLES HERE ***
        // Variables to hold fish will be declared here

        OrangeFish orangeFish;

        Urchin[] urchins;

        Seahorse[] seaHorses;

        Piranha piranha;

        const string orangeFishAssetID = "OrangeFish";
        const string urchinAssetID = "Urchin";
        const string SeahorseAssetID = "Seahorse";
        const string piranhaAssetID1 = "Piranha1";

        Random random = new();

        /// CONSTRUCTOR - for the Simulation class - run once only when an object of the Simulation class is INSTANTIATED (created)
        /// Use constructors to set up the state of a class
        public Simulation(IKernel pKernel)
        {
            kernel = pKernel;                   // Stores the game engine kernel which is passed to the constructor when this class is created
            screen = kernel.Screen;             // Sets the screen variable in Simulation so the screen dimensions are accessible

            // *** ADD OTHER INITIALISATION (class setup) CODE HERE ***




        }

        /// METHOD: LoadContent - called once at start of program
        /// Create all token objects and 'insert' them into the FishORama engine
        public void LoadContent(IGetAsset pAssetManager)
        {
            // *** ADD YOUR NEW TOKEN CREATION CODE HERE ***
            // Code to create fish tokens and assign to thier variables goes here
            // Remember to insert each token into the kernel

            // places the orange fish in a random position on the screen factoring in sprite size
            Vector2 offsetMax = new Vector2(screen.width / 2, screen.height / 2);

            // getting the asset bounds to factor into the initial location
            Vector2 orangeFishOffset = new(OrangeFish.imageAssetBounds.X / 2, OrangeFish.imageAssetBounds.Y / 2);
            Vector2 urchinOffset = new(Urchin.imageAssetBounds.X / 2, Urchin.imageAssetBounds.Y / 2);
            Vector2 seahorseOffset = new(Seahorse.imageAssetBounds.X / 2, Seahorse.imageAssetBounds.Y / 2);
            Vector2 piranha1Offset = new(Piranha.imageAssetBounds.X / 2, Piranha.imageAssetBounds.Y / 2);

            orangeFish = new OrangeFish(orangeFishAssetID,
                // random position (includes texture size)
                random.Next((int)(-offsetMax.X + orangeFishOffset.X), (int)(offsetMax.X - orangeFishOffset.X)),
                random.Next((int)(-offsetMax.Y + orangeFishOffset.Y), (int)(offsetMax.Y - orangeFishOffset.Y)),

                screen, tokenManager);

            kernel.InsertToken(orangeFish);

            urchins = new Urchin[3];
            for (int i = 0; i < urchins.Length; i++)
            {
                urchins[i] = new Urchin(urchinAssetID,
                    //random position (includes texture size)
                    random.Next((int)(-offsetMax.X + urchinOffset.X), (int)(offsetMax.X - urchinOffset.X)),
                    random.Next((int)(-offsetMax.Y + urchinOffset.Y), (int)((-offsetMax.Y / 2) - urchinOffset.Y)),
                    screen, tokenManager);

                kernel.InsertToken(urchins[i]);
            }

            seaHorses = new Seahorse[5];
            for (int i = 0; i < seaHorses.Length; i++)
            {
                seaHorses[i] = new Seahorse(SeahorseAssetID,
                    //random position (includes texture size)
                    random.Next((int)(-offsetMax.X + seahorseOffset.X), (int)(offsetMax.X - seahorseOffset.X)),
                    random.Next((int)(-offsetMax.Y + seahorseOffset.Y), (int)(offsetMax.Y - seahorseOffset.Y)),
                    screen, tokenManager);

                kernel.InsertToken(seaHorses[i]);
            }

            piranha = new Piranha(piranhaAssetID1,
               // random position (includes texture size)
               random.Next((int)(-offsetMax.X + piranha1Offset.X), (int)(offsetMax.X - piranha1Offset.X)),
               // makes the fish spawn in the top 2/3 of the screen by adding 2/3 of the positive offset to the negative,
               // resulting in the fish being spawnable in 4/6 or 2/3 of the screen
               random.Next((int)(-offsetMax.Y + ((offsetMax.Y * 2) / 3) + piranha1Offset.Y), (int)(offsetMax.Y - piranha1Offset.Y)),
               screen, tokenManager);

            kernel.InsertToken(piranha);
        }

        /// METHOD: Update - called 60 times a second by the FishORama engine when the program is running
        /// Add all tokens so Update is called on them regularly
        public void Update(GameTime gameTime)
        {

            // *** ADD YOUR UPDATE CODE HERE ***
            // Each fish object (sitting in a variable) must have Update() called on it here

            orangeFish.Update();
            foreach(Urchin urchin in urchins)
            {
                urchin.Update();
            }
            foreach(Seahorse seahorse in seaHorses)
            {
                seahorse.Update();
            }
            piranha.Update();

        }
    }
}
