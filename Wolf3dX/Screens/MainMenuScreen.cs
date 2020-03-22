#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Nexxt.Engine.Sound;
using Wolf3d.Helpers;
using Nexxt.UI.ScreenManager;
#endregion

namespace Wolf3d.StateManagement
{
    /// <summary>
    /// The main menu screen
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Fields

        ContentManager content;
        Texture2D mainMenuTexture;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("")
        {
            // Create our menu entries.
            MenuEntry singlePlayerMenuEntry = new MenuEntry("Single Player");
            MenuEntry liveMenuEntry = new MenuEntry("Online");
            MenuEntry systemLinkMenuEntry = new MenuEntry("Local");
            MenuEntry loadGameMenuEntry = new MenuEntry("Load Game");
            MenuEntry readThisMenuEntry = new MenuEntry("Credits");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");
            
 
            // Hook up menu event handlers.
            singlePlayerMenuEntry.Selected += SinglePlayerMenuEntrySelected;
            liveMenuEntry.Selected += LiveMenuEntrySelected;
            systemLinkMenuEntry.Selected += SystemLinkMenuEntrySelected;
            loadGameMenuEntry.Selected += loadGameMenuEntry_Selected;
            readThisMenuEntry.Selected += readThisMenuEntry_Selected;  
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(singlePlayerMenuEntry);
            MenuEntries.Add(liveMenuEntry);
            MenuEntries.Add(systemLinkMenuEntry);
            MenuEntries.Add(loadGameMenuEntry);
            MenuEntries.Add(readThisMenuEntry);  
            MenuEntries.Add(exitMenuEntry);
        }

       

        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Single Player menu entry is selected.
        /// </summary>
        void SinglePlayerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(null));
        }

        /// <summary>
        /// Event handler for when the Live menu entry is selected.
        /// </summary>
        void LiveMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            CreateOrFindSession(NetworkSessionType.PlayerMatch, e.PlayerIndex);
        }


        /// <summary>
        /// Event handler for when the System Link menu entry is selected.
        /// </summary>
        void SystemLinkMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            CreateOrFindSession(NetworkSessionType.SystemLink, e.PlayerIndex);
        }


        /// <summary>
        /// Helper method shared by the Live and System Link menu event handlers.
        /// </summary>
        void CreateOrFindSession(NetworkSessionType sessionType,
                                 PlayerIndex playerIndex)
        {
            // First, we need to make sure a suitable gamer profile is signed in.
            ProfileSignInScreen profileSignIn = new ProfileSignInScreen(sessionType);

            // Hook up an event so once the ProfileSignInScreen is happy,
            // it will activate the CreateOrFindSessionScreen.
            profileSignIn.ProfileSignedIn += delegate
            {
                GameScreen createOrFind = new CreateOrFindSessionScreen(sessionType);

                ScreenManager.AddScreen(createOrFind, playerIndex);
            };

            // Activate the ProfileSignInScreen.
            ScreenManager.AddScreen(profileSignIn, playerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the game.
        /// </summary>
        void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            MessageBoxScreen confirmExitMessageBox =
                                    new MessageBoxScreen(Helper.QuitMessage(),false);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, e.PlayerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        void readThisMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            MessageBoxScreen notImplementedMessageBox =
                                    new MessageBoxScreen("Not Implemented", false);
            ScreenManager.AddScreen(notImplementedMessageBox, e.PlayerIndex );
        }

        void loadGameMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            MessageBoxScreen notImplementedMessageBox =
                                                new MessageBoxScreen("Not Implemented", false);
            ScreenManager.AddScreen(notImplementedMessageBox, e.PlayerIndex );
        }

        #endregion

        #region Load Content
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            mainMenuTexture = content.Load<Texture2D>("Screens/Options");
             
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

        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);


            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            byte fade = TransitionAlpha;
            spriteBatch.Begin();

            spriteBatch.Draw(mainMenuTexture, fullscreen,
                             new Color(fade, fade, fade));

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (MediaPlayer.State != MediaState.Playing)
            {
                SoundManager.PlayMusic("Sound/Music/WONDERIN");
            }
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
        #endregion

    }
}
