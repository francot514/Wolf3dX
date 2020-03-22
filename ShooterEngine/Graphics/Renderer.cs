#region File Description
//-----------------------------------------------------------------------------
// Renderer.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nexxt.Framework;
using Nexxt.Engine.GameObjects;
using System.Collections.Generic;
using Nexxt.Common.Enums;
using Nexxt.Engine.Entities;
using Nexxt.Engine.Entities.Actors;
#endregion

namespace Nexxt.Engine.Graphics.RayCast
{
	/// <summary>
	/// Raycast engine
	/// </summary>
    public class Renderer
	{
        #region Sort Method

        //we need a static reference to the camera for the comparison method
        public static Camera TempCamera { get; set; }

        //this sorts all the sprites in back-to-front order
        static int spriteSort(GameObject a, GameObject b)
        {
            Vector2 ad = a.Position - TempCamera.Position;
            Vector2 bd = b.Position - TempCamera.Position;

            return bd.Length().CompareTo(ad.Length());
        }
        #endregion

        
        public int mapX;
        public int mapY;
        
        //flag that indicates if a wall was hit
        public bool wallHit;

		//the slices of the walls needing to be rendered
		public WallSlice[] WallSlices, WallSlicesY;

		//the screen's width and height
		public int ScreenWidth { get; private set; }
		public int ScreenHeight { get; private set; }

        public bool openingSecretDoor = false;

		public Renderer(int screenWidth, int screenHeight)
		{
			ScreenWidth = screenWidth;
			ScreenHeight = screenHeight;
			WallSlices = new WallSlice[screenWidth];
            WallSlicesY = new WallSlice[screenWidth];

        }
        
		public void Update(Map map, Camera camera)
		{
            for (int x = 0; x < ScreenWidth; x++)
            {
                //x-coordinate in camera space
                double cameraX = 2 * x / (double)ScreenWidth - 1;

                double rayPosX = camera.Position.X;
                double rayPosY = camera.Position.Y;
                double rayDirX = camera.Direction.X + camera.Plane.X * cameraX;
                double rayDirY = camera.Direction.Y + camera.Plane.Y * cameraX;

                //which box of the map we're in  
                mapX = (int)rayPosX;
                mapY = (int)rayPosY;

                //length of ray from current position to next x or y-side
                double sideDistX = 0;
                double sideDistY = 0;

                //length of ray from one x or y-side to next x or y-side
                double deltaDistX = Math.Sqrt(1 + (rayDirY * rayDirY) / (rayDirX * rayDirX));
                double deltaDistY = Math.Sqrt(1 + (rayDirX * rayDirX) / (rayDirY * rayDirY));
                double perpWallDist = 0;

                //what direction to step in x or y-direction (either +1 or -1)
                int stepX = 0;
                int stepY = 0;

                wallHit = false;
                int side = 0; //was a NS or a EW wall hit?

                //calculate step and initial sideDist
                if (rayDirX < 0)
                {
                    stepX = -1;
                    sideDistX = (rayPosX - mapX) * deltaDistX;
                }
                else
                {
                    stepX = 1;
                    sideDistX = (mapX + 1.0 - rayPosX) * deltaDistX;
                }
                if (rayDirY < 0)
                {
                    stepY = -1;
                    sideDistY = (rayPosY - mapY) * deltaDistY;
                }
                else
                {
                    stepY = 1;
                    sideDistY = (mapY + 1.0 - rayPosY) * deltaDistY;
                }

                //perform DDA
                while (!wallHit)
                {
                   
                    //jump to next map square, OR in x-direction, OR in y-direction
                    if (sideDistX < sideDistY)
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        side = 0;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        side = 1;
                    }

                    //Check if ray has hit a wall or an collidable object
                    if (
                            (
                                map.WorldMap[mapX, mapY] > 0 
                                & map.WorldMap[mapX,mapY] != Constants.COLLISION_BLOCK   // used for static sprite collitions (plants, tables,etc)
                                & map.WorldMap[mapX, mapY] != (int)DoorType.Normal
                                & map.WorldMap[mapX, mapY] != (int)DoorType.Exit
                                & map.WorldMap[mapX, mapY] != (int)DoorType.Gold_Key
                                & map.WorldMap[mapX, mapY] != (int)DoorType.Silver_Key  
                            )
                            
                        )
                        
                            
                    {
                        wallHit = true;
                    }
                }
           

                //Calculate distance projected on camera direction (oblique distance will give fisheye effect!)
                if (side == 0)
                    perpWallDist = Math.Abs((mapX - rayPosX + (1 - stepX) / 2.0) / rayDirX);
                else
                    perpWallDist = Math.Abs((mapY - rayPosY + (1 - stepY) / 2.0) / rayDirY);

                //Calculate height of line to draw on screen
                int lineHeight = (int)Math.Abs(ScreenHeight / perpWallDist);

   
                //texturing calculations
                int texNum = map.WorldMap[mapX, mapY] - 1;
                Texture2D tex = null;
                bool texturingSecretWall = false;
                // Texture map secret walls
                if (map.WorldMap[mapX, mapY] == (int)DoorType.Secret)
                {
                    // get a reference of the secret
                    Secret currentSecret = map.Secrets.Find(delegate(Secret s)
                    {
                        return (
                                    s.Position.X == mapX & s.Position.Y == mapY
                                );
                    });
                    if (currentSecret != null)
                    {                        
                        tex = currentSecret.SecretSprite.Texture;
                    }
                    texturingSecretWall = true;
                }
                //its not a secret wall
                else
                {
                    tex = map.Textures[texNum];
                }

                int texWidth = tex.Width;
                int texHeight = tex.Height;

                //calculate value of wallX
                //where exactly the wall was hit
                double wallX;
                
                if (side == 1)
                {
                    wallX = rayPosX + ((mapY - rayPosY + (1 - stepY) / 2.0) / rayDirY) * rayDirX;
                    WallSlices[x].Texture = tex;
                }
                else
                {
                    wallX = rayPosY + ((mapX - rayPosX + (1 - stepX) / 2.0) / rayDirX) * rayDirY;
                    if (!texturingSecretWall)
                    {
                        WallSlices[x].Texture = map.Textures[texNum + 1];
                    }
                    else
                    {
                        WallSlices[x].Texture = tex;
                    }
                }

                wallX -= Math.Floor(wallX);

                //x coordinate on the texture
                int texX = (int)(wallX * (double)texWidth);
                if ((side == 0 && rayDirX > 0) ||
                    (side == 1 && rayDirY < 0))
                {
                    texX = texWidth - texX - 1;
                }
                WallSlices[x].Depth = perpWallDist;
                WallSlices[x].Height = lineHeight;
                WallSlices[x].TextureX = texX;


                // get the map doors and put their respective sides, in wolf3d all doors but secrets has the form 
                // an 'H' shape (2 sides + door itself)

                for (int doorIndex = 0; doorIndex < map.Doors.Count; doorIndex++)
                {

                    if ((map.Doors[doorIndex].LeftSideX == mapX | map.Doors[doorIndex].RightSideX == mapX)
                         &
                        (map.Doors[doorIndex].LeftSideY == mapY | map.Doors[doorIndex].RightSideY == mapY))
                    {
                        if (map.Doors[doorIndex].orientation == Orientation.Horizontal)
                        {
                            if (side == 0)
                            {
                                WallSlices[x].Texture = map.Textures[Constants.DOOR_SIDES];
                            }
                            else
                            {
                                WallSlices[x].Texture = tex;
                            }
                        }
                        if (map.Doors[doorIndex].orientation == Orientation.Vertical)
                        {
                            if (side == 1)
                            {
                                WallSlices[x].Texture = map.Textures[Constants.DOOR_SIDES];
                            }
                            else
                            {
                                // match wall Shadows 
                                WallSlices[x].Texture = map.Textures[texNum + 1];
                            }
                        }
                    }
                }


                WallSlicesY = WallSlices;
                


                // correct elevator sides problem (wall shading is no applied to this one)
                if (map.WorldMap[mapX, mapY] == Constants.ELEVATOR_SIDES)
                {
                    WallSlices[x].Texture = map.Textures[texNum];
                }
            }
            //sort the sprites
            map.ObjectDatabase.Sort(spriteSort);
		}

        public void Draw(SpriteBatch spriteBatch, Map map)
        {
            int drawStart = 0;
            int drawEnd = 0;

            spriteBatch.Draw(map.Background, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);

            for (int x = 0; x < ScreenWidth; x++)
            {
                
                if (WallSlices[x].Texture != null && WallSlicesY[x].Texture != null)
                {
         
                        //calculate lowest and highest pixel to fill in current stripe
                        drawStart = -WallSlices[x].Height / 2 + ScreenHeight / 2;
                        drawEnd = WallSlices[x].Height / 2 + ScreenHeight / 2;

                    int drawStart2 = -WallSlices[x].Height + (-WallSlices[x].Height / 2 ) + ScreenHeight / 2;
                    int drawEnd2 = WallSlices[x].Height + (-WallSlices[x].Height / 2) + ScreenHeight / 2;

                    spriteBatch.Draw(
                          WallSlicesY[x].Texture,
                          new Rectangle(x, drawStart2, 1, drawEnd2 - drawStart),
                          new Rectangle(WallSlicesY[x].TextureX, 0, 1, WallSlicesY[x].Texture.Height),
                          Color.White);

                    spriteBatch.Draw(
                            WallSlices[x].Texture,
                            new Rectangle(x, drawStart, 1, drawEnd - drawStart),
                            new Rectangle(WallSlices[x].TextureX, 0, 1, WallSlices[x].Texture.Height),
                            Color.White);

                    

                }
                else
                {
                    break;
                }
            }
            
        }

        private void DrawDirectionalSprite(SpriteBatch spriteBatch, Camera camera, Map map, GameObject sprite)
        {          
            //translate sprite position to relative to camera
            double spriteX = sprite.Position.X - camera.Position.X;
            double spriteY = sprite.Position.Y - camera.Position.Y;
            int spriteMapX = (int)sprite.Position.X;
            int spriteMapY = (int)sprite.Position.Y;

            Texture2D tex = sprite.Texture;
            int texWidth = tex.Width;
            int texHeight = tex.Height;

            double objectOffset = 0;

            if (sprite.Orientation == Orientation.None)
            {
                string errorMessage = "";
                errorMessage = String.Format("The orientation of the Sprite in the location [{0}],[{1}] has not been defined!", sprite.Position.X.ToString(),sprite.Position.Y.ToString());
                throw new Exception(errorMessage);
            }
            Orientation spriteOrientation = sprite.Orientation;


            //Find the sprite offset, that is how deep is the sprite inside the block depending on the orientation X or Y
            if (spriteOrientation == Orientation.Horizontal)
            {
                if (camera.Plane.X > 0)
                    objectOffset = (sprite.Position.Y - (int)sprite.Position.Y);
                else
                    objectOffset = 1 - (sprite.Position.Y - (int)sprite.Position.Y);
            }
            else
            {
                if(camera.Plane.Y < 0 )
                    objectOffset = (sprite.Position.X - (int)sprite.Position.X);
                else
                    objectOffset = 1 -(sprite.Position.X - (int)sprite.Position.X);
            }

            
            double transformY = 0;
            double transformStartY = 0;
            double transformEndY = 0;
            int drawStartX = 0;
            int drawEndX = 0;

            int drawCenterX = ComputeScreenX(camera, spriteX, spriteY, out transformY);
            
            // add or substract 0.5 because the position of the sprite is the middle
            if (spriteOrientation == Orientation.Horizontal)
            {
                drawStartX = ComputeScreenX(camera, spriteX - 0.5, spriteY, out transformStartY);
                drawEndX = ComputeScreenX(camera, spriteX + 0.5, spriteY, out transformEndY);
            }
            else
            {
                drawStartX = ComputeScreenX(camera, spriteX, spriteY-0.5, out transformStartY);
                drawEndX = ComputeScreenX(camera, spriteX, spriteY+0.5, out transformEndY);
            }

            //validate sprite's start and end
            if (drawStartX < 0) drawStartX = 0;
            if (drawStartX > ScreenWidth) drawStartX = ScreenWidth;

            if (drawEndX < 0) drawEndX = 0;
            if (drawEndX > ScreenWidth) drawEndX = ScreenWidth;

            //switch variables depending on the orientation of the camera
            if (drawStartX > drawEndX)
            {
                int tempX = drawStartX;
                drawStartX = drawEndX;
                drawEndX = tempX;
            }

            //temporal slices for the sprite
            WallSlice[] spriteSlices = new WallSlice[drawEndX - drawStartX];
            
            //loop through every vertical stripe of the sprite on screen
            for (int x = drawStartX; x < drawEndX; x++)
            {
                //x-coordinate in camera space
                double cameraX = 2 * x / (double)ScreenWidth - 1;

                double rayPosX = camera.Position.X;
                double rayPosY = camera.Position.Y;
                double rayDirX = camera.Direction.X + camera.Plane.X * cameraX;
                double rayDirY = camera.Direction.Y + camera.Plane.Y * cameraX;

                //which box of the map we're in  
                int mapX = (int)rayPosX;
                int mapY = (int)rayPosY;

                //length of ray from current position to next x or y-side
                double sideDistX = 0;
                double sideDistY = 0;

                //length of ray from one x or y-side to next x or y-side
                double deltaDistX = Math.Sqrt(1 + (rayDirY * rayDirY) / (rayDirX * rayDirX));
                double deltaDistY = Math.Sqrt(1 + (rayDirX * rayDirX) / (rayDirY * rayDirY));
                double perpWallDist = 0;

                //what direction to step in x or y-direction (either +1 or -1)
                int stepX = 0;
                int stepY = 0;

                int side = 0; //was a NS or a EW wall hit?

                if (spriteOrientation == Orientation.Horizontal)
                    side = 1;
                else
                    side = 0;

                //calculate step and initial sideDist
                if (rayDirX < 0)
                {
                    stepX = -1;
                    sideDistX = (rayPosX - mapX) * deltaDistX;
                }
                else
                {
                    stepX = 1;
                    sideDistX = (mapX + 1.0 - rayPosX) * deltaDistX;
                }
                if (rayDirY < 0)
                {
                    stepY = -1;
                    sideDistY = (rayPosY - mapY) * deltaDistY;
                }
                else
                {
                    stepY = 1;
                    sideDistY = (mapY + 1.0 - rayPosY) * deltaDistY;
                }

                mapX = spriteMapX;
                mapY = spriteMapY;

                //Calculate distance projected on camera direction (oblique distance will give fisheye effect!)
                if (side == 0)
                    perpWallDist = Math.Abs((mapX - rayPosX + (1 - stepX) / 2.0) / rayDirX) + Math.Abs(objectOffset / rayDirX);
                else
                    perpWallDist = Math.Abs((mapY - rayPosY + (1 - stepY) / 2.0) / rayDirY) + Math.Abs(objectOffset / rayDirY);

                //Calculate height of line to draw on screen
                int lineHeight = (int)Math.Abs(ScreenHeight / perpWallDist);

                //calculate lowest and highest pixel to fill in current stripe
                int drawStart = -lineHeight / 2 + ScreenHeight / 2;
                int drawEnd = lineHeight / 2 + ScreenHeight / 2;

                //calculate value of wallX
                //where exactly the wall was hit
                double wallX;
                if (side == 0)
                {
                    //vertical
                    wallX = rayPosY + ((mapX - rayPosX + (1 - stepX) / 2.0) / rayDirX + Math.Abs(objectOffset / rayDirX)) * rayDirY;
                }
                else
                {
                    //horizontal
                    wallX = rayPosX + (((mapY - rayPosY + (1 - stepY) / 2.0) / rayDirY) + Math.Abs(objectOffset / rayDirY)) * rayDirX;
                }

                wallX -= Math.Floor(wallX);

                double spriteCenter = 0;

                //Find the sprite center relative to the block containg it
                if(spriteOrientation == Orientation.Horizontal)
                   spriteCenter = (sprite.Position.X - (int)sprite.Position.X);
                else
                   spriteCenter = (sprite.Position.Y - (int)sprite.Position.Y);
                
                //Map correctly the texture depending on center of the sprite. 
                //By default the center is an offset of 0.5 in X or Y axis
                if( spriteCenter > 0.5 )
                    wallX = wallX - (spriteCenter - 0.5);
                else if (spriteCenter < 0.5)
                    wallX = wallX - (spriteCenter + 0.5);

                if (wallX < 0) wallX = 1 + wallX;

                //x coordinate on the texture
                int texX = (int)(wallX * (double)texWidth);

                spriteSlices[x - drawStartX].Depth = perpWallDist;
                spriteSlices[x - drawStartX].Height = lineHeight;
                spriteSlices[x - drawStartX].TextureX = texX;
                spriteSlices[x - drawStartX].Texture = tex;

            }

            //Draw the stripes
            for (int stripe = drawStartX; stripe < drawEndX; stripe++)
            {
                int drawStart = -spriteSlices[stripe - drawStartX].Height / 2 + ScreenHeight / 2;
                int drawEnd = spriteSlices[stripe - drawStartX].Height / 2 + ScreenHeight / 2;
                double spriteDepth = spriteSlices[stripe - drawStartX].Depth;
                //the conditions in the if are:
                //1) it's in front of camera plane so you don't see things behind you
                //2) it's on the screen (left)
                //3) it's on the screen (right)
                //4) ZBuffer, with perpendicular distance
                if (spriteDepth > objectOffset && stripe > 0 && stripe < ScreenWidth && spriteDepth < WallSlices[stripe].Depth && transformY > objectOffset)
                    spriteBatch.Draw(
                        tex,
                        new Rectangle(stripe, drawStart, 1, drawEnd - drawStart),
                        new Rectangle(spriteSlices[stripe-drawStartX].TextureX, 0, 1, spriteSlices[stripe-drawStartX].Texture.Height),
                        Color.White);
            }
        }

        private int ComputeScreenX(Camera camera, double spriteX, double spriteY, out double transformY)
        {
            //transform sprite with the inverse camera matrix
            // [ planeX   dirX ] -1                                       [ dirY      -dirX ]
            // [               ]       =  1/(planeX*dirY-dirX*planeY) *   [                 ]
            // [ planeY   dirY ]                                          [ -planeY  planeX ]
            int spriteScreenX = 0;

            double dirX = camera.Direction.X;
            double dirY = camera.Direction.Y;
            double planeX = camera.Plane.X;
            double planeY = camera.Plane.Y;

            //required for correct matrix multiplication
            double invDet = 1.0 / (planeX * dirY - dirX * planeY);

            double transformX = invDet * (dirY * spriteX - dirX * spriteY);

            //this is actually the depth inside the screen, that what Z is in 3D       
            transformY = invDet * (-planeY * spriteX + planeX * spriteY);

            spriteScreenX = (int)((ScreenWidth / 2.0) * (1 + transformX / transformY));
            
            return spriteScreenX;
        }



        public void DrawSprites(SpriteBatch spriteBatch, Map map, Camera camera)
        {
            int count = map.ObjectDatabase.Count;
            for (int i = 0; i < count; i++)
            {
                if (map.ObjectDatabase[i].IsActive)
                {
                    if (map.ObjectDatabase[i].Name == Constants.DOOR_SPRITE_IDENTIFIER
                        |
                        map.ObjectDatabase[i].Name == "Secret"
                        )
                    {
                        DrawDirectionalSprite(spriteBatch, camera, map, map.ObjectDatabase[i]);
                    }
                    else
                    {
                        DrawSprite(spriteBatch, camera, map.ObjectDatabase[i]);
                    }
                }
            }
        }

        public void DrawSprite(SpriteBatch spriteBatch, Camera camera, GameObject sprite)
        {
            //translate sprite position to relative to camera
            double spriteX = sprite.Position.X - camera.Position.X;
            double spriteY = sprite.Position.Y - camera.Position.Y;

            Texture2D tex = sprite.Texture;
            int texWidth = tex.Width;
            int texHeight = tex.Height;


            //transform sprite with the inverse camera matrix
            // [ planeX   dirX ] -1                                       [ dirY      -dirX ]
            // [               ]       =  1/(planeX*dirY-dirX*planeY) *   [                 ]
            // [ planeY   dirY ]                                          [ -planeY  planeX ]

            double dirX = camera.Direction.X;
            double dirY = camera.Direction.Y;
            double planeX = camera.Plane.X;
            double planeY = camera.Plane.Y;

            //required for correct matrix multiplication
            double invDet = 1.0 / (planeX * dirY - dirX * planeY);

            double transformX = invDet * (dirY * spriteX - dirX * spriteY);
          
            //this is actually the depth inside the screen, that what Z is in 3D       
            double transformY = invDet * (-planeY * spriteX + planeX * spriteY);

            int spriteScreenX = (int)((ScreenWidth / 2.0) * (1 + transformX / transformY));

            //calculate height of the sprite on screen
            //using "transformY" instead of the real distance prevents fisheye
            int spriteHeight = (int)Math.Abs((int)(ScreenHeight / transformY));

            //calculate lowest and highest pixel to fill in current stripe
            int drawStartY = -spriteHeight / 2 + ScreenHeight / 2;
            int drawEndY = spriteHeight / 2 + ScreenHeight / 2;

            //calculate width of the sprite
            int spriteWidth = (int)Math.Abs((int)(ScreenHeight / transformY));
            int drawStartX = -spriteWidth / 2 + spriteScreenX;
            int drawEndX = spriteWidth / 2 + spriteScreenX;

            //loop through every vertical stripe of the sprite on screen
            for (int stripe = drawStartX; stripe < drawEndX; stripe++)
            {

              
                int texX = (int)(256 * (stripe - (-spriteWidth / 2 + spriteScreenX)) * texWidth / spriteWidth) / 256;
                //the conditions in the if are:
                //1) it's in front of camera plane so you don't see things behind you
                //2) it's on the screen (left)
                //3) it's on the screen (right)
                //4) ZBuffer, with perpendicular distance
                if (transformY > 0 && stripe > 0 && stripe < ScreenWidth && transformY < WallSlices[stripe].Depth)
                    spriteBatch.Draw(
                        tex,
                        new Rectangle(stripe, drawStartY, 1, drawEndY - drawStartY),
                        new Rectangle(texX, 0, 1, tex.Height),
                        Color.White);
            }
        }
	}
}
