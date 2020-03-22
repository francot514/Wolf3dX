#region File Description
//-----------------------------------------------------------------------------
// Fps.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Text; 
#endregion

namespace Wolf3d.Entities
{
    /// <summary>
    /// This class implements the FPS counter indicator
    /// </summary>
    public class Fps : DrawableGameComponent
    {
        #region Fields
        
        ContentManager content;
        SpriteFont gameFont;
        SpriteBatch spriteBatch;
        int frameCount = 0;
        int totalTime = 0;
        int fps;
        Vector2 messagePosition;
        #endregion

        #region Initialization
        public Fps(Game game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            messagePosition = new Vector2(30, 40);
            base.Initialize();
        } 
        #endregion

        #region Load Content
        protected override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(Game.Services, "Content");

            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the Game font
            gameFont = content.Load<SpriteFont>("Fonts/gamefont");
            
            base.LoadContent();
        } 
        #endregion

        #region Draw
       
        public override void Draw(GameTime gameTime)
        {
            calculateFPS(gameTime);
            spriteBatch.Begin();
            spriteBatch.DrawString
                (
                    gameFont,
                    string.Format("fps: {0}", fps.ToString()),
                    messagePosition,
                    Color.White
                );
            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion

        #region Helper Methods
        private void calculateFPS(GameTime time)
        {
            frameCount++;
            totalTime += time.ElapsedGameTime.Milliseconds;
            if (totalTime > 1000)
            {
                fps = frameCount;
                frameCount = 0;
                totalTime = 0;
            }
        }
        #endregion

    }
}
