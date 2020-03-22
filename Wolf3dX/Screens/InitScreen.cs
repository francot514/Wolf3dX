#region File Description
//-----------------------------------------------------------------------------
// InitScreen.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
#endregion

namespace Wolf3d.StateManagement
{
    /// <summary>
    /// This class implements the init screen, which presents some 
    /// information related to the platform in which the game is running
    /// 
    /// For the moment its just a bitmap, but in the future will tell u for example
    /// if a gamepad is connected (if u r running the game on windows) and if
    /// XBLA is accesible.
    /// </summary>
    class InitScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;
        Texture2D initScreenTexture;

        #endregion

    
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public InitScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("Fonts/gamefont");
            initScreenTexture = content.Load<Texture2D>("Screens/InitScreen");

            // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
           // ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
               
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (ControllingPlayer.HasValue)
            {
                // In single player games, handle input for the controlling player.
                HandlePlayerInput(input, ControllingPlayer.Value);
            }
        }


        /// <summary>
        /// Handles input for the specified player. In local game modes, this is called
        /// just once for the controlling player. In network modes, it can be called
        /// more than once if there are multiple profiles playing on the local machine.
        /// Returns true if we should continue to handle input for subsequent players,
        /// or false if this player has paused the game.
        /// </summary>
        bool HandlePlayerInput(InputState input, PlayerIndex playerIndex)
        {
            // Look up inputs for the specified player profile.
            KeyboardState keyboardState = input.CurrentKeyboardStates[(int)playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[(int)playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[(int)playerIndex];

            if (gamePadDisconnected)
            {
                MessageBoxScreen gamePadDisconectedMessageBox =
                        new MessageBoxScreen("Gamepad Disconnected", false);
                ScreenManager.AddScreen(gamePadDisconectedMessageBox, playerIndex);
                return false;

            }
            else
            {
                if (gamePadState.IsConnected)
                {
                    // load PGScreen
                    if (gamePadState.IsButtonDown(Buttons.A))
                    {
                        LoadPGScreen(playerIndex);
                    }
                }
                else 
                {
                    if (
                        keyboardState.IsKeyDown(Keys.Enter)
                        |
                        keyboardState.IsKeyDown(Keys.Escape)
                        |
                        keyboardState.IsKeyDown(Keys.Space)
                       )
                    {
                        LoadPGScreen(playerIndex);
                    }
                }
            }   
            return true;
        }

        /// <summary>
        /// loads the PG screen
        /// </summary>
        /// <param name="playerIndex">current player Index</param>
        private void LoadPGScreen(PlayerIndex playerIndex)
        {
            ScreenManager.RemoveScreen(this);
            LoadingScreen.Load(ScreenManager, false, playerIndex,
               new PGScreen());
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
           
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);
           
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            byte fade = TransitionAlpha;
            spriteBatch.Begin();

            spriteBatch.Draw(initScreenTexture, fullscreen,
                             new Color(fade, fade, fade));

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }


        #endregion
    }
}
