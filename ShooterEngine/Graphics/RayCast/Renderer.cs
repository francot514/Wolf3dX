

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nexxt.Framework;
using Nexxt.Engine.GameObjects;

namespace Nexxt.Engine.Graphics.RayCast
{
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

		//the slices of the walls needing to be rendered
		public WallSlice[] WallSlices;

		//the screen's width and height
		public int ScreenWidth { get; private set; }
		public int ScreenHeight { get; private set; }

		public Renderer(int screenWidth, int screenHeight)
		{
			ScreenWidth = screenWidth;
			ScreenHeight = screenHeight;

			WallSlices = new WallSlice[screenWidth];
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

				bool wallHit = false;
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

                    //Check if ray has hit a wall
                    if (map.WorldMap[mapX, mapY] > 0)
                        wallHit = true;
                }

				//Calculate distance projected on camera direction (oblique distance will give fisheye effect!)
				if (side == 0)
					perpWallDist = Math.Abs((mapX - rayPosX + (1 - stepX) / 2.0) / rayDirX);
				else
					perpWallDist = Math.Abs((mapY - rayPosY + (1 - stepY) / 2.0) / rayDirY);

				//Calculate height of line to draw on screen
				int lineHeight = (int)Math.Abs(ScreenHeight / perpWallDist);

				//calculate lowest and highest pixel to fill in current stripe
				int drawStart = -lineHeight / 2 + ScreenHeight / 2;
				int drawEnd = lineHeight / 2 + ScreenHeight / 2;

				//texturing calculations
				int texNum = map.WorldMap[mapX, mapY] - 1;

				Texture2D tex = map.Textures[texNum];
				int texWidth = tex.Width;
				int texHeight = tex.Height;

				//calculate value of wallX
				//where exactly the wall was hit
				double wallX;
				if (side == 1)
					wallX = rayPosX + ((mapY - rayPosY + (1 - stepY) / 2.0) / rayDirY) * rayDirX;
				else
					wallX = rayPosY + ((mapX - rayPosX + (1 - stepX) / 2.0) / rayDirX) * rayDirY;
				wallX -= Math.Floor(wallX);

				//x coordinate on the texture
				int texX = (int)(wallX * (double)texWidth);
				if ((side == 0 && rayDirX > 0) ||
					(side == 1 && rayDirY < 0))
				{
					texX = texWidth - texX - 1;
				}

                if (map.WorldMap[mapX, mapY] == 13)
                {
                    WallSlices[x].Depth = perpWallDist;
                    WallSlices[x].Height = lineHeight- 50;
                    WallSlices[x].TextureX = texX;
                    WallSlices[x].Texture = tex;

                }
                else
                {
                    WallSlices[x].Depth = perpWallDist;
                    WallSlices[x].Height = lineHeight;
                    WallSlices[x].TextureX = texX;
                    WallSlices[x].Texture = tex;

                }

				
			}
            //sort the sprites
            map.ObjectDatabase.Sort(spriteSort);
		}

        public void Draw(SpriteBatch spriteBatch, Map map)
        {
            int drawStart = 0;
            int drawEnd = 0;
            spriteBatch.Begin();

            spriteBatch.Draw(map.Background, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);

            for (int x = 0; x < ScreenWidth; x++)
            {
                if (WallSlices[x].Texture != null)
                {

                    //calculate lowest and highest pixel to fill in current stripe
                    drawStart = -WallSlices[x].Height / 2 + ScreenHeight / 2;
                    drawEnd = WallSlices[x].Height / 2 + ScreenHeight / 2;
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
            spriteBatch.End();
        }

        public void DrawSprites(SpriteBatch spriteBatch, Map map, Camera camera)
        {
            int count = map.ObjectDatabase.Count;
            for (int i = 0; i < count; i++)
            {
                if(map.ObjectDatabase[i].IsActive)
                    DrawSprite(spriteBatch, camera, map.ObjectDatabase[i]);
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
