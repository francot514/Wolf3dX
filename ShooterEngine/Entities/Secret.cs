#region File Description
//-----------------------------------------------------------------------------
// Door.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Nexxt.Common.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nexxt.Engine.Sound;
using Nexxt.Engine.GameObjects;
using Nexxt.Engine.AI;
using Nexxt.Framework;
#endregion

namespace Nexxt.Engine.Entities
{
    public class Secret : BaseEntity
    {
        #region Constants
        public const string OPEN = "OPEN";
        public const string OPENING = "OPENING";
        public const string CLOSED = "CLOSED";
        #endregion

        #region Members
        public GameObject SecretSprite;
        bool timeToOpen = false;
        Vector2 oldPosition;
        Vector2 newPosition;
        public Vector2 originalPosition;
        public float finalPosition;
        public float startPosition;
        public GameTime gameTime;
        public DoorType doorType;
        #endregion

        #region Constructor
        public Secret()
        {
            SecretSprite = new GameObject();
            SecretSprite.IsActive = true;
            entityStateMachine = new StateMachine();
            entityStateMachine.AddState(CLOSED, null, ClosedTick, null);
            entityStateMachine.AddState(OPENING, null, OpeningTick, null);
            entityStateMachine.AddState(OPEN, null, OpenTick, null);
            //set the initial state
            entityStateMachine.State = CLOSED;
        }
        #endregion

        #region Secret State Machine Implementation
        void ClosedTick()
        {
            if (timeToOpen)
            {
                SoundManager.PlaySound(Constants.SECRET_DOOR_SOUND, true);
                entityStateMachine.State = OPENING;
            }
        }

        void OpeningTick()
        {
            if (this.SecretSprite.Orientation == Orientation.Horizontal)
            {

                if ((int)oldPosition.Y == (int)finalPosition + 1)
                {
                    timeToOpen = false;
                    entityStateMachine.State = OPEN;
                }
                else
                {
                    oldPosition = SecretSprite.Position;
                    newPosition = new Vector2(oldPosition.X, oldPosition.Y - 0.01f);
                    SecretSprite.Position = newPosition;
                }
            }
            else // Vertical
            {
                if ((int)oldPosition.X == (int)finalPosition + 1)
                {
                    timeToOpen = false;
                    entityStateMachine.State = OPEN;
                }
                else
                {
                    oldPosition = SecretSprite.Position;
                    newPosition = new Vector2(oldPosition.X - 0.01f, oldPosition.Y);
                    SecretSprite.Position = newPosition;
                }
            }
        }
        void OpenTick()
        {
            //null;
        }


        #endregion

        #region Handle Input
        public void HandleInput(GamePadState gamepadState)
        {
            if (entityStateMachine.State != OPENING)
            {
                if (gamepadState.IsButtonDown(Buttons.Y))
                {
                    
                    timeToOpen = true;
                }
            }
        }

        public void HandleInput(KeyboardState keyboardState)
        {

            if (entityStateMachine.State != OPENING)
            {
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                   
                    timeToOpen = true;
                }
            }
        }

        #endregion
    }

    public class SecretCollection<T> : List<T> where T : Secret
    {
        #region Load Content

        public void LoadContent(ContentManager content)
        {
            // loads related sound effects
            SoundManager.LoadSound(Constants.SECRET_DOOR_SOUND); // secret door sound effect
        }
        #endregion

        #region Helper Methods
        public void UpdateStates(ref Map map)
        {
            for (int secretIndex = 0; secretIndex < this.Count; secretIndex++)
            {
                if (this[secretIndex].entityStateMachine.State == Secret.OPENING)
                {
                    //map.ObjectDatabase.Add(this[secretIndex].SecretSprite);
 
                    if (this[secretIndex].SecretSprite.Orientation == Orientation.Horizontal)
                    {
                        if (this[secretIndex].SecretSprite.Position.Y <= this[secretIndex].finalPosition + 3)
                        {
                            map.WorldMap[(int)this[secretIndex].Position.X, (int)this[secretIndex].Position.Y] = 0;
                        }
                        else
                        {
                            map.WorldMap[(int)this[secretIndex].Position.X, (int)this[secretIndex].Position.Y] = Constants.COLLISION_BLOCK;
                        }
                    }
                    else
                    {
                        if (this[secretIndex].SecretSprite.Position.X <= this[secretIndex].finalPosition + 3)
                        {
                            map.WorldMap[(int)this[secretIndex].Position.X, (int)this[secretIndex].Position.Y] = 0;
                        }
                        else
                        {
                            map.WorldMap[(int)this[secretIndex].Position.X, (int)this[secretIndex].Position.Y] = Constants.COLLISION_BLOCK;
                        }
                    }
                }
                else if (this[secretIndex].entityStateMachine.State == Secret.OPEN)
                {
                    map.WorldMap[(int)this[secretIndex].Position.X, (int)this[secretIndex].Position.Y] = 0;

                    
                    if (this[secretIndex].SecretSprite.Orientation == Orientation.Horizontal)
                    {
                        if (this[secretIndex].SecretSprite.Position.Y <= this[secretIndex].finalPosition + 2)
                        {
                            map.ObjectDatabase.Remove(this[secretIndex].SecretSprite);  
                            map.WorldMap[(int)this[secretIndex].Position.X, (int)this[secretIndex].Position.Y - 1] = 0;
                        }
                    }
                    else
                    {
                        if (this[secretIndex].SecretSprite.Position.X <= this[secretIndex].finalPosition + 2)
                        {
                            map.ObjectDatabase.Remove(this[secretIndex].SecretSprite); 
                            map.WorldMap[(int)this[secretIndex].Position.X - 1, (int)this[secretIndex].Position.Y] = 0;
                        }
                    }
                }
                else
                {
                    //map.WorldMap[(int)this[secretIndex].Position.X, (int)this[secretIndex].Position.Y] = (int)DoorType.Secret;
                }
                this[secretIndex].Tick();
            }
        }

        public Secret getCurrentSecret(Map map, int mapX, int mapY)
        {
            Secret currentSecret = null;
            // check which door was hit 
            // is horizontal Door ?
            try
            {
                if (map.WorldMap[mapX, mapY] == (int)DoorType.Secret)
                {
                    // get a reference of the secret door
                    currentSecret = map.Secrets.Find(delegate(Secret s)
                    {
                        return (s.Position.X == mapX & s.Position.Y == mapY);
                    });
                }
            }
            // handle overflows and underflows
            catch (IndexOutOfRangeException)  // exception oriented programming, ha - FIXME
            {
                return null;
            }
            return currentSecret;
        }

        #endregion
    }


}