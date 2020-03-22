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

    
    /// <summary>
    /// This class represents a Door object
    /// </summary>
    /*     
       
     Each Door is shaped like an 'H' Two sides + Door itself and its implemented using the following data structure:
     
                                         MAP             
                                          |
                    -------------   Door Collection -------------
                  /                       |          ....         \
                Door(1)                Door(2)                  Door(n) 
            /          \            /          \             /           \
            Left Side  Right Side   Left Side  Right Side   Left Side    Right Side 
            
     
      
    */
    public class Door : BaseEntity   
    {

        #region Constants
       
        // Possible States
        public const string OPEN = "OPEN";
        public const string OPENING = "OPENING";
        public const string CLOSED = "CLOSED";
        public const string CLOSING = "CLOSING";
        #endregion

        #region Members
        public Orientation orientation;
        public GameObject DoorSprite;
        public DoorType doorType;

        public int LeftSideX;
        public int LeftSideY;

        public int RightSideX;
        public int RightSideY;

        bool timeToOpen=false;

        Vector2 oldPosition;
        Vector2 newPosition;
        public Vector2 originalPosition;
        public float finalPosition;
        public float startPosition;
        float time;
        public GameTime gameTime;

        #endregion

        #region Constructor
        public Door()
        {
            DoorSprite = new GameObject();
            DoorSprite.IsActive = true;
            entityStateMachine = new StateMachine();
            entityStateMachine.AddState(CLOSED, null, ClosedTick, null);
            entityStateMachine.AddState(OPENING, null, OpeningTick, null);
            entityStateMachine.AddState(OPEN, null, OpenTick, null);
            entityStateMachine.AddState(CLOSING, null, ClosingTick, null);
            //set the initial state
            entityStateMachine.State = CLOSED;
            

 
        }
        #endregion

        #region Door State Machine Implementation
        void ClosedTick()
        {
            if (timeToOpen)
            {

                SoundManager.PlaySound(Constants.OPENING_DOOR_SOUND, true);
                entityStateMachine.State = OPENING;
            }
        }

 
        void OpeningTick()
        {
            if (this.orientation == Orientation.Horizontal)
            {

                if (oldPosition.X >= finalPosition)
                {
                    timeToOpen = false;
                    entityStateMachine.State = OPEN;
                }
                else
                {
                    oldPosition = DoorSprite.Position;
                    newPosition = new Vector2(oldPosition.X + 0.02f, oldPosition.Y);
                    DoorSprite.Position = newPosition;
                }
            }
            else // Vertical
            {
                if (oldPosition.Y >= finalPosition)
                {
                    timeToOpen = false;
                    entityStateMachine.State = OPEN;
                }
                else
                {
                    oldPosition = DoorSprite.Position;
                    newPosition = new Vector2(oldPosition.X, oldPosition.Y + 0.02f);
                    DoorSprite.Position = newPosition;
                }
            }
        }


        
        void OpenTick()
        {
            if( gameTime != null)
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //litte tick before closing the door
            if (time > 3)
            {
                time = 0;
                SoundManager.PlaySound(Constants.CLOSING_DOOR_SOUND, true); 
                entityStateMachine.State = CLOSING;
            }
        }
      
        void ClosingTick()
        {
            if (this.orientation == Orientation.Horizontal)
            {
                if (oldPosition.X <= startPosition)
                {
                    entityStateMachine.State = CLOSED;
                    DoorSprite.Position = originalPosition;
                }
                else
                {
                    oldPosition = DoorSprite.Position;
                    newPosition = new Vector2(oldPosition.X - 0.02f, oldPosition.Y);
                    DoorSprite.Position = newPosition;
                }
            }
            else
            {
                if (oldPosition.Y <= startPosition)
                {
                    entityStateMachine.State = CLOSED;
                    DoorSprite.Position = originalPosition;
                }
                else
                {
                    oldPosition = DoorSprite.Position;
                    newPosition = new Vector2(oldPosition.X, oldPosition.Y - 0.02f);
                    DoorSprite.Position = newPosition;
                }
            }
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

        public void Open()
        {
            timeToOpen = true;
        }
        #endregion

        #region Helper Methods

        #region Texture Map Door Sides
        public void putDoorXSides(int mapX, int mapY)
        {
            // left side
            LeftSideX = mapX - 1;
            LeftSideY = mapY;
            //right side
            RightSideX = mapX + 1;
            RightSideY = mapY; 
            
        }
        public void putDoorYSides(int mapX, int mapY)
        {
            //left side
            LeftSideX = mapX;
            LeftSideY = mapY - 1;
            
            // right side
            RightSideX = mapX;
            RightSideY = mapY + 1;

        } 
        #endregion

        
        #endregion

    }

    public class DoorCollection<T> : List<T> where T : Door
    {
        #region Load Content
        // load content goes here cuz there have to happen just once not for every door in the level
        public void LoadContent(ContentManager content)
        {
            // loads related sound effects
            SoundManager.LoadSound(Constants.OPENING_DOOR_SOUND); // opening door sound effect    
            SoundManager.LoadSound(Constants.CLOSING_DOOR_SOUND); // closing door sound effect
            SoundManager.LoadSound(Constants.SECRET_DOOR_SOUND);  // secret door sound effect
        }
        #endregion

 

        #region Helper Methods
      
        public void UpdateStates(ref Map map) 
        {
            for (int doorIndex = 0; doorIndex < this.Count; doorIndex++)
            {
                if (this[doorIndex].entityStateMachine.State == Door.OPEN)
                {
                    map.WorldMap[(int)this[doorIndex].Position.X, (int)this[doorIndex].Position.Y] = 0;
                }
                else
                {
                    map.WorldMap[(int)this[doorIndex].Position.X, (int)this[doorIndex].Position.Y] = (int)this[doorIndex].doorType;
                }
                this[doorIndex].Tick();
            }
        }

        public Door getCurrentDoor(Map map, int mapX, int mapY)
        {
            Door currentDoor= null;
            // check which door was hit 
            // is horizontal Door ?
            try
            {
                if (
                    // front side handling - Horizontal
                        (
                            map.WorldMap[mapX - 1, mapY] == (int)DoorType.Normal
                            |
                            map.WorldMap[mapX - 1, mapY] == (int)DoorType.Exit
                            |
                            map.WorldMap[mapX - 1, mapY] == (int)DoorType.Gold_Key
                            |
                            map.WorldMap[mapX - 1, mapY] == (int)DoorType.Silver_Key
                            |
                            map.WorldMap[mapX - 1, mapY] == (int)DoorType.Secret
                        )
                        |
                    // back side handling - Horizontal
                        (
                            map.WorldMap[mapX + 1, mapY] == (int)DoorType.Normal
                            |
                            map.WorldMap[mapX + 1, mapY] == (int)DoorType.Exit
                            |
                            map.WorldMap[mapX + 1, mapY] == (int)DoorType.Gold_Key
                            |
                            map.WorldMap[mapX + 1, mapY] == (int)DoorType.Silver_Key
                            |
                            map.WorldMap[mapX + 1, mapY] == (int)DoorType.Secret
                         )
                        )
                {
                    // get a reference of the door
                    currentDoor = map.Doors.Find(delegate(Door d)
                       {
                           return (
                                       d.Position.X == mapX - 1 & d.Position.Y == mapY
                                       |
                                       d.Position.X == mapX + 1 & d.Position.Y == mapY
                                   );
                       });
                }
                else //vertical door Handling
                {
                    if (
                        // front side handling  - Vertical
                            (
                                map.WorldMap[mapX, mapY - 1] == (int)DoorType.Normal
                                |
                                map.WorldMap[mapX, mapY - 1] == (int)DoorType.Exit
                                |
                                map.WorldMap[mapX, mapY - 1] == (int)DoorType.Gold_Key
                                |
                                map.WorldMap[mapX, mapY - 1] == (int)DoorType.Silver_Key
                                |
                                map.WorldMap[mapX, mapY - 1] == (int)DoorType.Secret
                            )
                            |
                        // back side handling - Vertical
                            (
                                map.WorldMap[mapX, mapY + 1] == (int)DoorType.Normal
                                |
                                map.WorldMap[mapX, mapY + 1] == (int)DoorType.Exit
                                |
                                map.WorldMap[mapX, mapY + 1] == (int)DoorType.Gold_Key
                                |
                                map.WorldMap[mapX, mapY + 1] == (int)DoorType.Silver_Key
                                |
                                map.WorldMap[mapX, mapY + 1] == (int)DoorType.Secret
                            )
                        )
                    {
                        // get a reference of the door
                        currentDoor = map.Doors.Find(delegate(Door d)
                        {
                            return (
                                        d.Position.X == mapX & d.Position.Y == mapY - 1
                                        |
                                        d.Position.X == mapX & d.Position.Y == mapY + 1
                                    );
                        });

                    }

                }
            }
            // handle overflows and underflows
            catch (IndexOutOfRangeException)  // exception oriented programming, ha - FIXME
            {
                return null;  
            }
            return currentDoor; 
        }

        #endregion
    }
}
