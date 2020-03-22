#region File Description
//-----------------------------------------------------------------------------
// CreateOrFindSessionScreen.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nexxt.UI.ScreenManager;
#endregion

namespace Wolf3d.StateManagement
{
    /// <summary>
    /// This menu screen lets the user choose whether to create a new
    /// network session, or search for an existing session to join.
    /// </summary>
    class CreateOrFindSessionScreen : MenuScreen
    {
        #region Fields

        NetworkSessionType sessionType;
        ContentManager content;
        Texture2D multiplayerMenuTexture;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public CreateOrFindSessionScreen(NetworkSessionType sessionType)
            : base("")
        {
            this.sessionType = sessionType;

            // Create our menu entries.
            MenuEntry createSessionMenuEntry = new MenuEntry("Create Session");
            MenuEntry findSessionsMenuEntry = new MenuEntry("Find Session");
            MenuEntry backMenuEntry = new MenuEntry("Back");

            // Hook up menu event handlers.
           // createSessionMenuEntry.Selected += CreateSessionMenuEntrySelected;
            //findSessionsMenuEntry.Selected += FindSessionsMenuEntrySelected;
            backMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(createSessionMenuEntry);
            MenuEntries.Add(findSessionsMenuEntry);
            MenuEntries.Add(backMenuEntry);
        }


        /// <summary>
        /// Helper chooses an appropriate menu title for the specified session type.
        /// </summary>
        static string GetMenuTitle(NetworkSessionType sessionType)
        {
            switch (sessionType)
            {
                case NetworkSessionType.PlayerMatch:
                    return "Online Match";

                case NetworkSessionType.SystemLink:
                    return "Local Match";

                default:
                    throw new NotSupportedException();
            }
        }


        #endregion

        #region Event Handlers


        /// <summary>
        /// Event handler for when the Create Session menu entry is selected.
        /// </summary>
        void CreateSessionMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            try
            {
                // Which local profiles should we include in this session?
                IEnumerable<SignedInGamer> localGamers =
                        NetworkSessionComponent.ChooseGamers(sessionType,
                                                             ControllingPlayer.Value);

                // Begin an asynchronous create network session operation.
                IAsyncResult asyncResult = NetworkSession.BeginCreate(
                                                    sessionType, localGamers,
                                                    NetworkSessionComponent.MaxGamers,
                                                    0, null, null, null);

                // Activate the network busy screen, which will display
                // an animation until this operation has completed.
                NetworkBusyScreen busyScreen = new NetworkBusyScreen(asyncResult);

                busyScreen.OperationCompleted += CreateSessionOperationCompleted;

                ScreenManager.AddScreen(busyScreen, ControllingPlayer);
            }
            catch (Exception exception)
            {
                NetworkErrorScreen errorScreen = new NetworkErrorScreen(exception);

                ScreenManager.AddScreen(errorScreen, ControllingPlayer);
            }
        }


        /// <summary>
        /// Event handler for when the asynchronous create network session
        /// operation has completed.
        /// </summary>
        void CreateSessionOperationCompleted(object sender,
                                             OperationCompletedEventArgs e)
        {
            try
            {
                // End the asynchronous create network session operation.
                NetworkSession networkSession = NetworkSession.EndCreate(e.AsyncResult);

                // Create a component that will manage the session we just created.
                NetworkSessionComponent.Create(ScreenManager, networkSession);

                // Go to the lobby screen. We pass null as the controlling player,
                // because the lobby screen accepts input from all local players
                // who are in the session, not just a single controlling player.
                ScreenManager.AddScreen(new LobbyScreen(networkSession), null);
            }
            catch (Exception exception)
            {
                NetworkErrorScreen errorScreen = new NetworkErrorScreen(exception);

                ScreenManager.AddScreen(errorScreen, ControllingPlayer);
            }
        }


        /// <summary>
        /// Event handler for when the Find Sessions menu entry is selected.
        /// </summary>
        void FindSessionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            try
            {
                // Which local profiles should we include in this session?
                IEnumerable<SignedInGamer> localGamers =
                        NetworkSessionComponent.ChooseGamers(sessionType,
                                                             ControllingPlayer.Value);

                // Begin an asynchronous find network sessions operation.
                IAsyncResult asyncResult = NetworkSession.BeginFind(sessionType,
                                                        localGamers, null, null, null);

                // Activate the network busy screen, which will display
                // an animation until this operation has completed.
                NetworkBusyScreen busyScreen = new NetworkBusyScreen(asyncResult);

                busyScreen.OperationCompleted += FindSessionsOperationCompleted;

                ScreenManager.AddScreen(busyScreen, ControllingPlayer);
            }
            catch (Exception exception)
            {
                NetworkErrorScreen errorScreen = new NetworkErrorScreen(exception);

                ScreenManager.AddScreen(errorScreen, ControllingPlayer);
            }
        }


        /// <summary>
        /// Event handler for when the asynchronous find network sessions
        /// operation has completed.
        /// </summary>
        void FindSessionsOperationCompleted(object sender,
                                            OperationCompletedEventArgs e)
        {
            GameScreen nextScreen;

            try
            {
                // End the asynchronous find network sessions operation.
                AvailableNetworkSessionCollection availableSessions =
                                                NetworkSession.EndFind(e.AsyncResult);

                if (availableSessions.Count == 0)
                {
                    // If we didn't find any sessions, display an error.
                    availableSessions.Dispose();

                    nextScreen = new MessageBoxScreen("No Session Found", false);
                }
                else
                {
                    // If we did find some sessions, proceed to the JoinSessionScreen.
                    nextScreen = new JoinSessionScreen(availableSessions);
                }
            }
            catch (Exception exception)
            {
                nextScreen = new NetworkErrorScreen(exception);
            }

            ScreenManager.AddScreen(nextScreen, ControllingPlayer);
        }


        #endregion

        #region Load Content
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            multiplayerMenuTexture = content.Load<Texture2D>("Screens/Multiplayer");


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

            spriteBatch.Draw(multiplayerMenuTexture, fullscreen,
                             new Color(fade, fade, fade));

            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion
    }
}
