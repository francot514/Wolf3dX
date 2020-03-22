using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nexxt.Engine.GameObjects;
using Nexxt.Common.Enums;
using Nexxt.Engine.Entities;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Wolf3d.Entities
{
    public class RealTimeMap : DrawableGameComponent
    {

        #region Fields
        ContentManager content;
        Texture2D background;
        Texture2D arrow;
        SpriteBatch spriteBatch;
        SpriteFont gameFont;
        Camera camera;
        Map map;

        bool[,] visitedWorldMap;

        #endregion

        #region Constants
        const int FULL_SCREEN_SIZE = 384;
        const int DEFAULT_SCREEN_SIZE = 128;
        const int DEFAULT_POSITION_X = 17;
        const int DEFAULT_POSITION_Y = 353;
        const int DEFAULT_RANGE = 16;
        const int VISIT_RANGE = 8;

        #endregion

        #region Variables
        
        // size of the map
        private int Height = DEFAULT_SCREEN_SIZE;
        private int Width = DEFAULT_SCREEN_SIZE;

        //Range is the number of blocks that the map display on the X and Y axis
        private int Range = DEFAULT_RANGE;
        
        //Position on the screen
        private int X = DEFAULT_POSITION_X;
        private int Y = DEFAULT_POSITION_Y;
        

        public bool IsVisible = true;
        private bool isFullScreen = false;

        public bool IsFullScreen { get { return isFullScreen; } }

        #endregion

        #region Properties
        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void SetRange(int range)
        {
            Range = range;
        }

        public void SetSize(int width, int height)
        {
            Height = height;
            Width = width;
        } 
        #endregion

        #region Initialize
        public RealTimeMap(Game game, Camera _camera, Map _map)
            : base(game)
        {
            camera = _camera;
            map = _map;
            visitedWorldMap = new bool[map.WorldMap.GetUpperBound(0) + 1, map.WorldMap.GetUpperBound(1) + 1];
        }

        public override void Initialize()
        {
            base.Initialize();
        }
        
        #endregion

        #region Load Content
        protected override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(Game.Services, "Content");

            spriteBatch = new SpriteBatch(GraphicsDevice);
            gameFont = content.Load<SpriteFont>("Fonts/gamefont");
            background = CreateBackground(Width, Height);
            arrow = content.Load<Texture2D>("Hud/arrow");

            base.LoadContent();
        } 
        #endregion

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            if (IsVisible)
            {
                spriteBatch.Begin();
                //render the map
                DrawMap();
                spriteBatch.End();
            }
            base.Draw(gameTime);
        } 
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            //mark the surrounding map blocks as visited
            MarkVisitedPositions();
            base.Update(gameTime);
        } 
        #endregion

        #region Helper Methods
        
        /// <summary>
        /// This method marks the player´s surrounding map blocks as visited
        /// </summary>
        private void MarkVisitedPositions()
        {

            //calculate range to mark
            int playerX = (int)camera.Position.Y;
            int playerY = (int)camera.Position.X;
            int startX = playerX - VISIT_RANGE / 2;
            int endX = playerX + VISIT_RANGE / 2;

            int startY = playerY - VISIT_RANGE / 2;
            int endY = playerY + VISIT_RANGE / 2;

            //boundaries check
            if (startX < 0)
                startX = 0;
            if (startY < 0)
                startY = 0;
            if (endX > map.WorldMap.GetUpperBound(1))
                endX = map.WorldMap.GetUpperBound(1);
            if (endY > map.WorldMap.GetUpperBound(0))
                endY = map.WorldMap.GetUpperBound(0);

            //mark the surrounding blocks as visited
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    visitedWorldMap[y, x] = true;
                }
            }

        }

        /// <summary>
        /// Set the fullscreen or default mode to draw the map
        /// </summary>
        /// <param name="fullScreen"></param>
        public void SetFullScreenMode(bool fullScreen)
        {
            if (fullScreen)
            {
                isFullScreen = true;
                SetSize(FULL_SCREEN_SIZE, FULL_SCREEN_SIZE);
                int centerX = Game.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - FULL_SCREEN_SIZE / 2;
                int centerY = Game.GraphicsDevice.PresentationParameters.BackBufferHeight / 2 - FULL_SCREEN_SIZE / 2 - 50;
                SetPosition(centerX, centerY);
                SetRange(map.WorldMap.GetUpperBound(0) + 1);
            }
            else
            {
                isFullScreen = false;
                SetSize(DEFAULT_SCREEN_SIZE, DEFAULT_SCREEN_SIZE);
                SetPosition(DEFAULT_POSITION_X, DEFAULT_POSITION_Y);
                SetRange(DEFAULT_RANGE);
                
            }
            background = CreateBackground(Width, Height);
        }

        /// <summary>
        /// Draw the map depending on the player's position
        /// </summary>
        private void DrawMap()
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////
            //   In this method we have to invert the axis, because the map is stored as [columns,rows]  //
            ///////////////////////////////////////////////////////////////////////////////////////////////

            Vector2 startPosition = new Vector2(X, Y);
            //draws the background
            spriteBatch.Draw(background, startPosition, Color.White);

            //draws the elements of the map

            //find start and final position in both axis
            float startX = (int)camera.Position.Y - Range / 2;
            float endX = (int)camera.Position.Y + (Range / 2) -1;

            float startY = (int)camera.Position.X - Range / 2;
            float endY = (int)camera.Position.X + (Range / 2) - 1;

            
            //check boundaries
            if (startX < 0)
            {
                startX = 0;
                endX = Range - 1;
            }
            if (startY < 0)
            {
                startY = 0;
                endY = Range - 1;
            }
            if (endX > map.WorldMap.GetUpperBound(1))
            {
                endX = map.WorldMap.GetUpperBound(1);
                startX = endX - Range + 1;
            }
            if (endY > map.WorldMap.GetUpperBound(0))
            {
                endY = map.WorldMap.GetUpperBound(0);
                startY = endY - Range + 1;
            }

            int destX = 0;
            int destY = 0;
            
            //calculate individual block size

            int blockWidth = Width / Range;
            int blockHeight = Height / Range;

            //loop every block in the map
            for (int x = (int)startX; x <= endX; x++)
            {
                for (int y = (int)startY; y <= endY; y++)
                {
                    int value = map.WorldMap[y, x]; //inverted axis
                    if (value != 0)
                    {
                        Texture2D texture = null;
                        if (value < map.Textures.Count)
                        {
                            //draw walls
                            texture = map.Textures[value];
                        }
                        else if ((value >= (int)DoorType.Normal) && (value <= (int)DoorType.Exit))
                        {
                            //draw doors
                            Door d = map.Doors.getCurrentDoor(map, y-1,x); //inverted axis
                            if (d != null)
                            {
                                texture = d.DoorSprite.Texture;
                            }
                        }
                        else if (value == (int)DoorType.Secret)
                        {
                           //secret doors
                            Secret s = map.Secrets.getCurrentSecret(map, y, x);
                            if (s != null)
                                texture = s.SecretSprite.Texture;
                        }

                        if (visitedWorldMap[y, x])
                        {
                            if (texture != null)
                            {
                                //render the texture
                                spriteBatch.Draw(
                                    texture,
                                    new Rectangle(destX + X, destY + Y, blockWidth, blockHeight),
                                    new Rectangle(0, 0, texture.Width, texture.Height),
                                    Color.White);
                            }
                        }
                    }
                    destY = destY + blockHeight;
                }
                destX = destX + blockWidth;
                destY = 0;
            }

            float playerX = 0;
            float playerY = 0;
            
            //compute the player's position
            playerX = camera.Position.Y - startX; //( inverted axis )
            playerY = camera.Position.X - startY; //( inverted axis )
            playerX = X + playerX * blockWidth;
            playerY = Y + playerY * blockHeight;

            //the rotation angle
            float playerAngle = (float)Math.Atan2(camera.Direction.X, camera.Direction.Y);
            playerAngle = playerAngle + (float)Math.PI / 2;

            Vector2 textureCenter = new Vector2(arrow.Width / 2, arrow.Height / 2);
            
            //draw the arrow
            spriteBatch.Draw(
                arrow,
                new Rectangle((int)playerX, (int)playerY, blockWidth, blockHeight),
                new Rectangle(0, 0, arrow.Width, arrow.Height),
                Color.White,
                playerAngle,
                textureCenter, 
                SpriteEffects.None,
                0);

            //draw title
            if (isFullScreen)
            {
                spriteBatch.DrawString(gameFont, "Map", new Vector2(X+5, Y-4), Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            }
        }


        /// <summary>
        /// Create the map background
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Texture2D CreateBackground(int width, int height)
        {
            Texture2D rectangleTexture = new Texture2D(GraphicsDevice, width, height);// create the rectangle texture, ,but it will have no color! lets fix that
            Color[] colors = new Color[width * height];//set the color to the amount of pixels in the textures

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int i = x * width + y;
                    Color color = new Color(255, 255, 255, 96);
                    Color blackColor = new Color(0, 0, 0, 255);

                    //create a black frame
                    if (y == 0 || y == height - 1 || y == 1 || y == height - 2)
                        color = blackColor;
                    if (x == 0 || x == width - 1 || x == 1 || x == width - 2)
                        color = blackColor;

                    colors[i] = color;
                }
            }

            rectangleTexture.SetData(colors);//set the color data on the texture
            return rectangleTexture;//return the texture
        }
        #endregion

        #region Handle Input
        public void HandleInput(KeyboardState keyBoardState)
        {
            bool fullScreenMap = false;
            if (keyBoardState.IsKeyDown(Keys.M))
            {
                fullScreenMap = true;
            }
            if (isFullScreen != fullScreenMap)
                SetFullScreenMode(fullScreenMap);
        } 
        #endregion

    }
}
