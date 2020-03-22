#region File Description
//-----------------------------------------------------------------------------
// Camera.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nexxt.Common.Enums;   
#endregion

namespace Nexxt.Engine.GameObjects
{
	public class Camera
	{
		public Vector2 Position;
		public Vector2 Direction;
		public Vector2 Plane;
		public float Radius = .3f;
		public float MoveSpeed = 3f;
		public float RotationSpeed = 3f;
        public bool DoorHit;

		public void Update(float dt, GamePadState gps, Map map)
		{
			HandleTranslation(gps, map, dt);
			HandleRotation(gps, dt);
		}

        public void Update(float dt, KeyboardState kbs, Map map)
        {
            HandleTranslation(kbs, map, dt);
            HandleRotation(kbs , dt);
        }


       

        private void HandleTranslation(KeyboardState kbs, Map map, float dt)
        {
            //allow touchpad movement...
            Vector2 move = Vector2.Zero; //  = Direction; //*(dt * MoveSpeed);

            //or dpad movement
            if (kbs.IsKeyDown(Keys.Up))
                move = Direction * (dt * MoveSpeed);
            if (kbs.IsKeyDown (Keys.Down))
                move = -Direction * (dt * MoveSpeed);

            if (move.X != 0)
            {
                //take into account the camera's radius when moving
                float rX = (move.X > 0) ? Radius : -Radius;

                //make sure there is no wall where we're trying to move
                if (map.WorldMap[(int)(Position.X + move.X + rX), (int)Position.Y] == 0)
                {
                    Position.X += move.X;
                }

                if (
                    map.WorldMap[(int)(Position.X + move.X + rX), (int)Position.Y] == (int)DoorType.Normal
                    |
                    map.WorldMap[(int)(Position.X + move.X + rX), (int)Position.Y] == (int)DoorType.Exit
                    |
                    map.WorldMap[(int)(Position.X + move.X + rX), (int)Position.Y] == (int)DoorType.Gold_Key 
                    |
                    map.WorldMap[(int)(Position.X + move.X + rX), (int)Position.Y] == (int)DoorType.Silver_Key 
                    |
                    map.WorldMap[(int)(Position.X + move.X + rX), (int)Position.Y] == (int)DoorType.Secret 
                    )
                {
                    DoorHit = true;
                }
                else
                {
                    DoorHit = false;
                }
            }

            if (move.Y != 0)
            {
                //take into account the camera's radius when moving
                float rY = (move.Y > 0) ? Radius : -Radius;

                //make sure there is no wall where we're trying to move
                if (map.WorldMap[(int)Position.X, (int)(Position.Y + move.Y + rY)] == 0)
                {
                    Position.Y += move.Y;
                }
                if (
                    map.WorldMap[(int)Position.X, (int)(Position.Y + move.Y + rY)] == (int)DoorType.Normal
                    |
                    map.WorldMap[(int)Position.X, (int)(Position.Y + move.Y + rY)] == (int)DoorType.Exit 
                    |
                    map.WorldMap[(int)Position.X, (int)(Position.Y + move.Y + rY)] == (int)DoorType.Gold_Key 
                    |
                    map.WorldMap[(int)Position.X, (int)(Position.Y + move.Y + rY)] == (int)DoorType.Silver_Key 
                    |
                    map.WorldMap[(int)Position.X, (int)(Position.Y + move.Y + rY)] == (int)DoorType.Secret 
                    )
                {
                    DoorHit = true;
                }

            }
        }

		private void HandleTranslation(GamePadState gps, Map map, float dt)
		{
			//allow touchpad movement...
			Vector2 move = Direction * gps.ThumbSticks.Left.Y * (dt * MoveSpeed);

			//or dpad movement
			if (gps.DPad.Up == ButtonState.Pressed)
				move = Direction * (dt * MoveSpeed);
			if (gps.DPad.Down == ButtonState.Pressed)
				move = -Direction * (dt * MoveSpeed);

			if (move.X != 0)
			{
				//take into account the camera's radius when moving
				float rX = (move.X > 0) ? Radius : -Radius;

				//make sure there is no wall where we're trying to move
				if (map.WorldMap[(int)(Position.X + move.X + rX), (int)Position.Y] == 0)
					Position.X += move.X;
                
                if (
                    map.WorldMap[(int)(Position.X + move.X + rX), (int)Position.Y] == (int)DoorType.Normal
                    |
                    map.WorldMap[(int)(Position.X + move.X + rX), (int)Position.Y] == (int)DoorType.Exit 
                    |
                    map.WorldMap[(int)(Position.X + move.X + rX), (int)Position.Y] == (int)DoorType.Gold_Key 
                    |
                    map.WorldMap[(int)(Position.X + move.X + rX), (int)Position.Y] == (int)DoorType.Silver_Key
                    |
                    map.WorldMap[(int)(Position.X + move.X + rX), (int)Position.Y] == (int)DoorType.Secret 
                    )
                {
                    DoorHit = true;
                }
                else
                {
                    DoorHit = false;
                }
			}

			if (move.Y != 0)
			{
				//take into account the camera's radius when moving
				float rY = (move.Y > 0) ? Radius : -Radius;

				//make sure there is no wall where we're trying to move
				if (map.WorldMap[(int)Position.X, (int)(Position.Y + move.Y + rY)] == 0)
					Position.Y += move.Y;

                if (
                    map.WorldMap[(int)Position.X, (int)(Position.Y + move.Y + rY)] == (int)DoorType.Normal
                    |
                    map.WorldMap[(int)Position.X, (int)(Position.Y + move.Y + rY)] == (int)DoorType.Exit
                    |
                    map.WorldMap[(int)Position.X, (int)(Position.Y + move.Y + rY)] == (int)DoorType.Gold_Key
                    |
                    map.WorldMap[(int)Position.X, (int)(Position.Y + move.Y + rY)] == (int)DoorType.Silver_Key
                    |
                    map.WorldMap[(int)Position.X, (int)(Position.Y + move.Y + rY)] == (int)DoorType.Secret
                    )
                {
                    DoorHit = true;
                }
                
			}
		}

		private void HandleRotation(GamePadState gps, float dt)
		{
			//allow touchpad rotation...
			float rot = -gps.ThumbSticks.Left.X * (dt * RotationSpeed);

			//or DPad rotation
			if (gps.DPad.Right == ButtonState.Pressed)
				rot = -(dt * RotationSpeed);
			if (gps.DPad.Left == ButtonState.Pressed)
				rot = (dt * RotationSpeed);

			//calculate the sin and cos values
			float sRot = (float)Math.Sin(rot);
			float cRot = (float)Math.Cos(rot);

			//rotate the Direction vector
			float odX = Direction.X;
			Direction.X = Direction.X * cRot - Direction.Y * sRot;
			Direction.Y = odX * sRot + Direction.Y * cRot;

			//rotate the Plane vector
			float opX = Plane.X;
			Plane.X = Plane.X * cRot - Plane.Y * sRot;
			Plane.Y = opX * sRot + Plane.Y * cRot;
		}

        private void HandleRotation(KeyboardState kbs, float dt)
        {
            //allow touchpad rotation...
            float rot=0; //= (dt); //* RotationSpeed);

            //or DPad rotation
            if (kbs.IsKeyDown(Keys.Right))
                rot = -(dt * RotationSpeed);
            if (kbs.IsKeyDown(Keys.Left))   
                rot = (dt * RotationSpeed);

            //calculate the sin and cos values
            float sRot = (float)Math.Sin(rot);
            float cRot = (float)Math.Cos(rot);

            //rotate the Direction vector
            float odX = Direction.X;
            Direction.X = Direction.X * cRot - Direction.Y * sRot;
            Direction.Y = odX * sRot + Direction.Y * cRot;

            //rotate the Plane vector
            float opX = Plane.X;
            Plane.X = Plane.X * cRot - Plane.Y * sRot;
            Plane.Y = opX * sRot + Plane.Y * cRot;
        }

	}
}
