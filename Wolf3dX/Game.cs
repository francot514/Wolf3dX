/* Copyright (c) 2009, www.NexxtStudios.com
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
* 
* WOLFENSTEIN 3D is a trademark of ID Sofware, this is an open source fan remake, you are
* not authorized to sell this game without explicit authorization of ID Software and Nexxt Studios 
* 
* NOTE: If you use this code as BASE/FRAMEWORK or any portions of it for 
* writing commercial or non Open Source games you must the include the "Nexxt" Logo on a splash screen for 5 seconds
* 
* Thanks.
* 
* Coders:
* Ivan Fernandez
* Alberto Xavier Guerra 
* Hugo Munoz 
*/

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
using Nexxt.Engine.Sound;
using Wolf3d.StateManagement; 
#endregion


namespace Wolf3d
{
    /// <summary>
    /// Wolfenstein 3DX, entry point
    /// </summary>
    public class WW2Game : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        #endregion

        #region Initialization


        /// <summary>
        /// The main game constructor.
        /// </summary>
        public WW2Game()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);

            //no fixed time step and no vertical sync. by turning these
            //off, the game can run a little faster.
            IsFixedTimeStep = true;
            graphics.SynchronizeWithVerticalRetrace = true;
            //we can also use multisampling on Windows to make it look nicer
            graphics.PreferMultiSampling = true;

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            //graphics.IsFullScreen = true;
            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            Components.Add(new MessageDisplayComponent(this));
            Components.Add(new GamerServicesComponent(this));

            // Activate the first screen.
            screenManager.AddScreen(new MainMenuScreen(), PlayerIndex.One);
            
            // Listen for invite notification events.
            NetworkSession.InviteAccepted += (sender, e)
                => NetworkSessionComponent.InviteAccepted(screenManager, e);

            // To test the trial mode behavior while developing,
            // uncomment this line:

            //Guide.SimulateTrialMode = true;

            // initialize the SoundManager
            SoundManager.Initialize(this); 
        }


        #endregion

        #region Draw


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }


        #endregion
    }


}

