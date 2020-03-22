#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
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
using Microsoft.Xna.Framework.Media;
using Nexxt.Engine.Graphics.RayCast;
using Nexxt.Engine.GameObjects;
using Nexxt.Engine.Sound;
using Nexxt.Engine.Entities;
using Nexxt.Framework.TypeReaders;
using Nexxt.Common.Enums;
using Wolf3d.Helpers;
using Wolf3d.Entities;
using Nexxt.Engine.Entities.Actors;
#endregion

namespace Wolf3d.StateManagement
{
    /// <summary>
    /// This screen implements the actual Wolf3dX game logic.
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        NetworkSession networkSession;

        ContentManager content;
        SpriteFont gameFont;

        //our map
        Map map;

        //the camera
        Camera camera;

        //the renderer
        Renderer renderer;

        //determine the desired FOV
        float fov = .66f;

        //current PlayerIndex
        PlayerIndex currentPlayerIndex;

        // SpriteBatch Shader (used to improve the sprite drawing process by using the GPU)
        // NOTE: not supported on ZUNE
        Effect spriteBatchShader;

        Viewport viewport;

        Vector2 viewportSize;

        // inputs for the specified player profile.
        KeyboardState keyboardState;
        GamePadState gamePadState;

        Player player;

        Hud hud;

        //GameTime currentGameTime;

        Door currentDoor;
        Secret currentSecret;

        //the map
        RealTimeMap realtimeMap;

        #endregion

        #region Properties


        /// <summary>
        /// The logic for deciding whether the game is paused depends on whether
        /// this is a networked or single player game. If we are in a network session,
        /// we should go on updating the game even when the user tabs away from us or
        /// brings up the pause menu, because even though the local player is not
        /// responding to input, other remote players may not be paused. In single
        /// player modes, however, we want everything to pause if the game loses focus.
        /// </summary>
        new bool IsActive
        {
            get
            {
                if (networkSession == null)
                {
                    // Pause behavior for single player games.
                    return base.IsActive;
                }
                else
                {
                    // Pause behavior for networked games.
                    return !IsExiting;
                }
            }
        }


        #endregion

        #region Initialization



        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(NetworkSession networkSession)
        {
            this.networkSession = networkSession;
            camera = new Camera();
            player = new Player(map, camera);
            hud = new Hud();
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            viewport = ScreenManager.GraphicsDevice.Viewport;

            viewportSize = new Vector2(viewport.Width, viewport.Height);

            // add the FPS drawable game component 
            this.ScreenManager.Game.Components.Add(new Fps(ScreenManager.Game));

            // Load the spriteBatch Shader used to draw the sprites using the GPU
            spriteBatchShader = content.Load<Effect>("Shaders/SpriteBatch");

            // Load the Game font
            gameFont = content.Load<SpriteFont>("Fonts/gamefont");

            //TODO: Somehow should be dynamic - need to implement an episode Manager
            map = content.Load<Map>("Maps/e1m1");


            // add the Map drawable game component 
            //realtimeMap = new RealTimeMap(ScreenManager.Game, camera, map);
            //this.ScreenManager.Game.Components.Add(realtimeMap);

            //map.Actors.LoadContent(content);


            //Once the map object is loaded get the level's soundtrack stored of course in the map file
            SoundManager.PlayMusic("Sound/Music/" + map.AmbientAudio);

            hud.LoadContent(content);

            player.LoadContent(content);

            map.Doors.LoadContent(content);

            //create a new renderer at whaterever the preferred buffer size is
            renderer = new Renderer(
                ScreenManager.GraphicsDevice.Viewport.Width,
                ScreenManager.GraphicsDevice.Viewport.Height);


            //create some values for the Position and Direction.
            //Direction should be a normalized vector.
            camera.Position = map.SpawnPoint;
            camera.Direction = new Vector2(0f, 1f);

            //get the perpendicular vector to the Direction multiplying by the FOV
            camera.Plane = new Vector2(camera.Direction.Y, -camera.Direction.X) * fov;

            //store the camera in the static reference for sprite sorting
            Renderer.TempCamera = camera;

            player.map = map;

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
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
                //play background music
                if (MediaPlayer.State != MediaState.Playing)
                    SoundManager.PlayMusic("Sound/Music/" + map.AmbientAudio);

                // Look up inputs for the specified player profile.
                keyboardState = Keyboard.GetState(currentPlayerIndex);
                gamePadState = GamePad.GetState(currentPlayerIndex);

                if (gamePadState.IsConnected)
                {
                    // update the camera using the gamepad as input
                    camera.Update(
                        (float)gameTime.ElapsedGameTime.TotalSeconds,
                        gamePadState,
                        map);
                }
                else
                {
                    // update the camera using the keyboard as input
                    camera.Update(
                        (float)gameTime.ElapsedGameTime.TotalSeconds,
                        keyboardState,
                        map);
                }

                map.Actors.AnimateActors(camera, gameTime);

                //update the renderer
                renderer.Update(map, camera);

                // check if a door was hit
                if (camera.DoorHit)
                {
                    // TODO: optimize this crap!!!
                    currentSecret = map.Secrets.getCurrentSecret(map, renderer.mapX, renderer.mapY);
                    if (currentSecret != null)
                    {
                        currentSecret.gameTime = gameTime;
                    }

                    currentDoor = map.Doors.getCurrentDoor(map, renderer.mapX, renderer.mapY);
                    if (currentDoor != null)
                    {
                        currentDoor.gameTime = gameTime;
                    }
                }
                else
                {
                    currentDoor = null;
                }

            }



            // If we are in a network game, check if we should return to the lobby.
            if ((networkSession != null) && !IsExiting)
            {
                if (networkSession.SessionState == NetworkSessionState.Lobby)
                {
                    LoadingScreen.Load(ScreenManager, false, null,
                                       new LobbyScreen(networkSession));
                }
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
            else if (networkSession != null)
            {
                // In network game modes, handle input for all the
                // local players who are participating in the session.
                foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
                {
                    if (!HandlePlayerInput(input, gamer.SignedInGamer.PlayerIndex))
                        break;
                }
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
            keyboardState = input.CurrentKeyboardStates[(int)playerIndex];
            gamePadState = input.CurrentGamePadStates[(int)playerIndex];

            // set the local variable currentPlayerIndex

            currentPlayerIndex = playerIndex;

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[(int)playerIndex];

            if (input.IsPauseGame(playerIndex))
            {
                SoundManager.StopMusic();
                ScreenManager.AddScreen(new PauseMenuScreen(networkSession), playerIndex);
                return false;
            }
            else if (gamePadDisconnected)
            {
                MessageBoxScreen gamePadDisconectedMessageBox =
                       new MessageBoxScreen("Disconnected", false);
                ScreenManager.AddScreen(gamePadDisconectedMessageBox, playerIndex);
                return false;
            }
            else
            {
                // Gamepad found
                if (gamePadState.IsConnected)
                {
                    // handle weapon input a gamepad
                    player.HandleInput(gamePadState);

                    // handle map input using the keyboard
                    realtimeMap.HandleInput(keyboardState);

                    if (currentDoor != null)
                    {
                        currentDoor.HandleInput(gamePadState);
                    }
                    if (currentSecret != null)
                    {
                        currentSecret.HandleInput(gamePadState);
                    }
                }
                // use the keyboard
                else
                {
                    // handle weapon input using the keyboard
                    player.HandleInput(keyboardState);
                    
                    // handle map input using the keyboard
                    //realtimeMap.HandleInput(keyboardState);

                    if (currentDoor != null)
                    {
                        currentDoor.HandleInput(keyboardState);
                    }
                    if (currentSecret != null)
                    {
                        currentSecret.HandleInput(keyboardState);
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {


            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatchShader.Parameters["ViewportSize"].SetValue(viewportSize);

            spriteBatch.Begin();



            //spriteBatchShader.Begin();
            //spriteBatchShader.CurrentTechnique.Passes[0].Begin();


            //draw the map. this draws the background and all the walls.
            renderer.Draw(spriteBatch, map);

            ////Draw actors
            //renderer.DrawActors(spriteBatch, map, camera);


            //Now the sprites are inside the map so handle the map to the renderer to render all the sprites
            renderer.DrawSprites(spriteBatch, map, camera);



            // update all doors states
            map.Doors.UpdateStates(ref map);

            map.Secrets.UpdateStates(ref map);

            // update all actors' states
            map.Actors.UpdateStates(gameTime, ref map, camera);

            // update bullets
            map.Bullets.UpdateStates(gameTime, camera);


            // Draw the current gun (all the logic here should be managed by an inventory subsystem)
            player.gameTime = gameTime;
            player.Draw(spriteBatch);

            // Draw in-Game Hud
            hud.Draw(spriteBatch, viewport, gameTime, player.currentWeaponTexture);


            spriteBatch.DrawString(gameFont, player.CurrentHitPoints.ToString(), new Vector2(0, 600), Color.White);
#if DEBUG

            // if we are on a network session write the number of players (debug only)
            if (networkSession != null)
            {
                string message = "Players: " + networkSession.AllGamers.Count;
                Vector2 messagePosition = new Vector2(30, 80);
                spriteBatch.DrawString(gameFont, message, messagePosition, Color.White);
            }
#endif

            spriteBatch.End();
            //spriteBatchShader.CurrentTechnique.Passes[0].End();
           // spriteBatchShader.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }


        #endregion


    }
}
