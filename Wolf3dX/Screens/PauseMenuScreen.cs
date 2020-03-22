#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nexxt.Engine.Sound;
using Wolf3d.Helpers;
using Nexxt.UI.ScreenManager;
using System;
#endregion

namespace Wolf3d.StateManagement
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume, load game, save game, quit, etc...
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        #region Fields

        NetworkSession networkSession;
        ContentManager content;
        Texture2D pauseMenuTexture;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen(NetworkSession networkSession)
            : base("")
        {
            this.networkSession = networkSession;

            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;

            // Add the Resume Game menu entry.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
            resumeGameMenuEntry.Selected += ResumeGameMenuEntry_Selected;
            MenuEntries.Add(resumeGameMenuEntry);

            if (networkSession == null)
            {
                // If this is a single player game, add the Quit menu entry.
                MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");
                quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;
                MenuEntries.Add(quitGameMenuEntry);
            }
            else
            {
                // If we are hosting a network game, add the Return to Lobby menu entry.
                if (networkSession.IsHost)
                {
                    MenuEntry lobbyMenuEntry = new MenuEntry("Return to Loby");
                    lobbyMenuEntry.Selected += ReturnToLobbyMenuEntrySelected;
                    MenuEntries.Add(lobbyMenuEntry);
                }

                // Add the End/Leave Session menu entry.
                string leaveEntryText = networkSession.IsHost ? "End Session" :
                                                                "Leave Session";

                MenuEntry leaveSessionMenuEntry = new MenuEntry(leaveEntryText);
                leaveSessionMenuEntry.Selected += LeaveSessionMenuEntrySelected;
                MenuEntries.Add(leaveSessionMenuEntry);
            }
        }

        private void ResumeGameMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            SoundManager.StopMusic();
            OnCancel(e.PlayerIndex);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            MessageBoxScreen confirmQuitMessageBox =
                                    new MessageBoxScreen(Helper.QuitMessage(),false);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null,new MainMenuScreen());
        }


        /// <summary>
        /// Event handler for when the Return to Lobby menu entry is selected.
        /// </summary>
        void ReturnToLobbyMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (networkSession.SessionState == NetworkSessionState.Playing)
            {
                networkSession.EndGame();
            }
        }


        /// <summary>
        /// Event handler for when the End/Leave Session menu entry is selected.
        /// </summary>
        void LeaveSessionMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            NetworkSessionComponent.LeaveSession(ScreenManager, e.PlayerIndex);
        }


        #endregion

        #region Load Content
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            pauseMenuTexture = content.Load<Texture2D>("Screens/Paused");
            SoundManager.PlayMusic("Sound/Music/WONDERIN"); 
            base.LoadContent();
        }
        public override void UnloadContent()
        {
            content.Unload(); 
            base.UnloadContent();
        }
        #endregion

        #region Update and Draw


        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            //SoundManager.PlayMusic(Helper.MENU_SONG);   
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            byte fade = TransitionAlpha;
            spriteBatch.Begin();  
            spriteBatch.Draw(pauseMenuTexture, fullscreen,
                                new Color(fade, fade, fade));
            spriteBatch.End(); 
            base.Draw(gameTime);
        }


        #endregion
    }
}
