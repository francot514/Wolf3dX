#region File Description
//-----------------------------------------------------------------------------
// Hud.cs
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
using Nexxt.Engine.Graphics.Resources;
using Nexxt.Engine.Entities; 
#endregion

namespace Wolf3d.Entities
{
    /// <summary>
    /// This class contains the implementation of the in-game HUD, it shows
    /// Blazkowicz animated head, current level, score....
    /// 
    /// FOR NOW LETS KEEP IT SIMPLE
    /// </summary>
    public class Hud : BaseEntity 
    {
        #region Fields
        static Texture2D hudTexture;
        static SpriteSheet headSpriteSheet;
        #endregion

        #region Load Content
        public void LoadContent(ContentManager content )
        {


            // load the Hud
            hudTexture = content.Load<Texture2D>("Hud/hud");

            // load Blazkowicz head spritesheet
            headSpriteSheet = content.Load<SpriteSheet>("Sprites/Head/head");

        } 
        #endregion

        #region Draw
        public void Draw(SpriteBatch spriteBatch, Viewport viewport, GameTime gameTime, Texture2D currentWeaponTexture)
        {
            
            Rectangle HudContainer = new Rectangle(0, 0, viewport.Width, viewport.Height);
            
            //draw the game Hud on the screen
            spriteBatch.Draw(hudTexture, HudContainer, Color.White);

            // Draw Blazkowicz animated Head
            float time = (float)gameTime.TotalGameTime.TotalSeconds;
            const int animationFramesPerSecond = 1;
            const int animationFrameCount = 3;

            // Look up the index of the first head sprite.
            int headFrameIndex = headSpriteSheet.GetIndex("FACE1APIC");

            // Modify the index to select the current frame of the animation.
            headFrameIndex += (int)(time * animationFramesPerSecond) % animationFrameCount;

            // Draw the current head sprite.
            spriteBatch.Draw(headSpriteSheet.Texture, new Rectangle(334, 490, 78, 94),
                             headSpriteSheet.SourceRectangle(headFrameIndex), Color.White);

            spriteBatch.Draw(currentWeaponTexture, new Vector2(650, 515), Color.White);      
           
        } 
        #endregion
    }
}
