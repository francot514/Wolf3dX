#region File Description
//-----------------------------------------------------------------------------
// Weapon.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nexxt.Engine.Graphics.Resources;
using Nexxt.Engine.Sound;
using Nexxt.Engine.AI;
using Nexxt.Engine.Entities;
using Wolf3d.StateManagement;
using Nexxt.Common.Enums;
using Nexxt.Engine.GameObjects;
using Nexxt.Engine.Entities.Actors;
using Nexxt.Engine.Animations;
#endregion

namespace Wolf3d.Entities
{
    /// <summary>
    /// Initial implementation of weapon entity (still a lot to do)
    /// </summary>
    public class Player : Actor
    {
        #region Constants
        // Sprite indexes stored inside the spritesheet
        const string KNIFE_START_INDEX = "1";
        const string PISTOL_START_INDEX = "6";
        const string MACHINEGUN_START_INDEX = "11";
        const string GATLINGGUN_START_INDEX = "16";

        // Asociated Sound Effects
        const string KNIFE_SOUND = "Sound/Sfx/023";
        const string PISTOL_SOUND = "Sound/Sfx/012";
        const string MACHINEGUN_SOUND = "Sound/Sfx/011";
        const string GATLINGGUN_SOUND = "Sound/Sfx/013";

        // Possible States
        const string IDLE = "IDLE";
        const string SHOOTING = "SHOOTING";
        const string CHANGE_WEAPON = "CHANGE_WEAPON";

        //Frames per second for weapon animations
        const int ANIMATION_FRAMES_PER_SECOND = 8;
        //Total number of Frames for each weapon
        const int ANIMATION_FRAMES = 5;
        #endregion

        #region Fields

        public SpriteSheet weaponSpriteSheet;
        public Weapons currentWeapon;
        public GameTime gameTime;

        int animationFrameCount = 1;
        int weaponLastFrameIndex;
        float time;
        int weaponFrameIndex;
        bool timeToShoot = false;
        bool changeWeapon = false;
        bool usingKeyboard = false;
        GamePadState currentGamepadState;
        KeyboardState currentKeyBoardState;
        // shooting timer used to sync the sound effect
        float wtime;

        double timeToCreateBullet = 0;

        // Textures used to render the current weapon in the HUD 
        Texture2D knifeTexture;
        Texture2D pistolTexture;
        Texture2D machineGunTexture;
        Texture2D gatlingGunTexture;

        public Texture2D currentWeaponTexture;

        public Map map;

        public Camera camera;

        #endregion

        #region Constructor
        public Player(Map _map, Camera _camera) : base(_map, new Vector2(0,0), new CharacterDefinition(), 0)
        {
            map = _map;
            camera = _camera;

            entityStateMachine = new StateMachine();
            // Idle doesn't need an end  
            entityStateMachine.AddState(IDLE, idleBegin, idleTick, null);
            // Shooting doesn't need Begin or End.  
            entityStateMachine.AddState(SHOOTING, null, shootingTick, null);

            entityStateMachine.AddState(CHANGE_WEAPON, changeWeaponBegin, changeWeaponTick, null);

            // current weapon is hardcoded for now
            // TODO: this should me managed by an inventory system.
            currentWeapon = Weapons.Pistol;
        }
        #endregion

        #region Weapon State Machine Implementation

        void idleBegin()
        {
            // sets the framecount to 1, just to draw the IDLE state weapon (not shooting)
            animationFrameCount = 1;

        }
        void idleTick()
        {
            if (timeToShoot) // is time to shoot
            {
                if (currentWeapon == Weapons.Pistol | currentWeapon == Weapons.Knife)
                {
                    // sets the framecount to 5 to draw the shooting animation
                    animationFrameCount = ANIMATION_FRAMES;
                }
                else
                {
                    animationFrameCount = 3;
                }

                //change state
                entityStateMachine.State = SHOOTING;



            }
            if (changeWeapon)
            {
                entityStateMachine.State = CHANGE_WEAPON;
            }
        }

        void shootingTick()
        {
            time = (float)gameTime.TotalGameTime.TotalSeconds;
            // obtain the current weapon Index on the spriteSheet 
            SetWeaponStartFrameIndex();

            CreateBullets();

            // Modify the index to select the current frame of the animation           
            if (currentWeapon == Weapons.GatlingGun | currentWeapon == Weapons.MachineGun)
            {
                // lets speed up the sequence for this two weapons
                weaponFrameIndex++;
                weaponFrameIndex += (int)(time * (ANIMATION_FRAMES_PER_SECOND + 7)) % animationFrameCount; // lets speed up the things 
                if (!usingKeyboard)
                {
                    if (!currentGamepadState.IsButtonDown(Buttons.A))
                    {
                        timeToShoot = false;
                        SetWeaponStartFrameIndex();
                        entityStateMachine.State = IDLE;
                    }
                }
                else
                {
                    if (!currentKeyBoardState.IsKeyDown(Keys.RightControl) &
                        !currentKeyBoardState.IsKeyDown(Keys.LeftControl))
                    {
                        timeToShoot = false;
                        SetWeaponStartFrameIndex();
                        entityStateMachine.State = IDLE;
                    }
                }
            }
            else // Knife and Pistol have a normal animation sequence
            {
                weaponFrameIndex += (int)(time * ANIMATION_FRAMES_PER_SECOND) % animationFrameCount;

                // State ending conditions Conditions 
                if ((weaponFrameIndex == weaponLastFrameIndex)
                & animationFrameCount == ANIMATION_FRAMES)
                {
                    timeToShoot = false;
                    SetWeaponStartFrameIndex();
                    entityStateMachine.State = IDLE;
                }
            }
        }


        void changeWeaponBegin()
        {
            if (!usingKeyboard) //we are using the gamePad
            {
                currentWeapon++;
                if (currentWeapon > Weapons.GatlingGun)
                {
                    currentWeapon = Weapons.Knife;
                }
                // sets current weapon start index on the spriteSheet 
                // then tick... 
                SetWeaponStartFrameIndex();
            }
            else
            {
                SetWeaponStartFrameIndex();
                entityStateMachine.State = IDLE;
            }

        }

        void changeWeaponTick()
        {
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //litte tick when changing weapon (less than a second)
            if (time > 0.4)
            {
                time = 0;
                entityStateMachine.State = IDLE;
            }
        }

        void CreateBullets()
        {
            timeToCreateBullet += gameTime.ElapsedGameTime.TotalSeconds;
            //reload time to every 0.5 seconds
            if (timeToCreateBullet > 0.5)
            {

                Bullet bullet = new Bullet(map, currentWeapon, camera.Position, camera.Direction, null);
                bullet.IsActive = true;
                map.Bullets.Add(bullet);

                timeToCreateBullet = 0;
            }
        }
        #endregion

        #region Load Content
        public void LoadContent(ContentManager content)
        {
            // load weapon spritesheet
            weaponSpriteSheet = content.Load<SpriteSheet>("Sprites/Gun/gun");

            knifeTexture = content.Load<Texture2D>("Hud/KNIFEPIC");
            pistolTexture = content.Load<Texture2D>("Hud/GUNPIC");
            machineGunTexture = content.Load<Texture2D>("Hud/MACHINEGUNPIC");
            gatlingGunTexture = content.Load<Texture2D>("hud/GATLINGGUNPIC");

            // loads related sound effects
            SoundManager.LoadSound(KNIFE_SOUND); // Knife Sound    
            SoundManager.LoadSound(MACHINEGUN_SOUND); // Machinegun Sound
            SoundManager.LoadSound(PISTOL_SOUND); // Pistol Sound
            SoundManager.LoadSound(GATLINGGUN_SOUND); // GatlingGun Sound
            SetWeaponStartFrameIndex(); // sets current weapon start index on the spriteSheet

        }
        #endregion

        #region Handle Input
        /// <summary>
        /// Handle input using the conected gamepad
        /// </summary>
        /// <param name="gamepadState">GamepadState</param>
        public void HandleInput(GamePadState gamepadState)
        {
            currentGamepadState = gamepadState;
            // not shooting and not changing weapon, which means IDLE
            if (entityStateMachine.State != SHOOTING)
            {
                if (currentGamepadState.IsButtonDown(Buttons.A))
                {
                    timeToShoot = true;
                }
            }
            if (entityStateMachine.State != CHANGE_WEAPON & !timeToShoot)
            {
                if (currentGamepadState.IsButtonDown(Buttons.RightTrigger))
                {
                    entityStateMachine.State = CHANGE_WEAPON;
                }
            }

        }
        /// <summary>
        /// Handle input using the keyBoard
        /// </summary>
        /// <param name="keyBoardState">KeyboardState</param>
        public void HandleInput(KeyboardState keyBoardState)
        {
            currentKeyBoardState = keyBoardState;
            // not shooting and not changing weapon, which means IDLE
            if (entityStateMachine.State != SHOOTING)
            {
                // use CTRL KEY to shoot
                if (currentKeyBoardState.IsKeyDown(Keys.RightControl)
                    | currentKeyBoardState.IsKeyDown(Keys.LeftControl))
                {
                    //verify if player is not currently shooting
                    if (entityStateMachine.State != SHOOTING)
                    {
                        timeToShoot = true;
                    }
                }
            }
            if (entityStateMachine.State != CHANGE_WEAPON & !timeToShoot)
            {
                if (keyBoardState.IsKeyDown(Keys.D1))
                {
                    currentWeapon = Weapons.Knife;
                }
                if (keyBoardState.IsKeyDown(Keys.D2))
                {
                    currentWeapon = Weapons.Pistol;
                }
                if (keyBoardState.IsKeyDown(Keys.D3))
                {
                    currentWeapon = Weapons.MachineGun;
                }
                if (keyBoardState.IsKeyDown(Keys.D4))
                {
                    currentWeapon = Weapons.GatlingGun;
                }
            }
            usingKeyboard = true;
            entityStateMachine.State = CHANGE_WEAPON;
        }
        /// <summary>
        /// Draws the current spritesheet Frame handled by the states (IDLE, SHOOTING)
        /// </summary>
        /// <param name="spriteBatch">spriteBatch</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // state machine tick (u can override this method if necessary)
            base.Tick();
            if (entityStateMachine.State == SHOOTING)
            {
                StartSound();
            }
            spriteBatch.Draw(weaponSpriteSheet.Texture, new Rectangle(220, 190, 300, 300),
                                     weaponSpriteSheet.SourceRectangle(weaponFrameIndex), Color.White);



        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// Sets the current weapon start frame index inside the spritesheet
        /// </summary>
        private void SetWeaponStartFrameIndex()
        {
            switch (currentWeapon)
            {
                case Weapons.Knife:
                    weaponFrameIndex = weaponSpriteSheet.GetIndex(KNIFE_START_INDEX);
                    currentWeaponTexture = knifeTexture;
                    break;
                case Weapons.Pistol:
                    weaponFrameIndex = weaponSpriteSheet.GetIndex(PISTOL_START_INDEX);
                    currentWeaponTexture = pistolTexture;
                    break;
                case Weapons.MachineGun:
                    weaponFrameIndex = weaponSpriteSheet.GetIndex(MACHINEGUN_START_INDEX);
                    currentWeaponTexture = machineGunTexture;
                    break;
                case Weapons.GatlingGun:
                    weaponFrameIndex = weaponSpriteSheet.GetIndex(GATLINGGUN_START_INDEX);
                    currentWeaponTexture = gatlingGunTexture;
                    break;
            }
            weaponLastFrameIndex = weaponFrameIndex + ANIMATION_FRAMES - 1;
        }
        /// <summary>
        /// Gets the current weapon start frame index
        /// </summary>
        /// <returns>integer value of the current weapon start frame index on the spritsheet</returns>
        private int GetWeaponStartFrameIndex()
        {
            switch (currentWeapon)
            {
                case Weapons.Knife:
                    return weaponSpriteSheet.GetIndex(KNIFE_START_INDEX);
                case Weapons.Pistol:
                    return weaponSpriteSheet.GetIndex(PISTOL_START_INDEX);
                case Weapons.MachineGun:
                    return weaponSpriteSheet.GetIndex(MACHINEGUN_START_INDEX);
                case Weapons.GatlingGun:
                    return weaponSpriteSheet.GetIndex(GATLINGGUN_START_INDEX);
                default: return 0;
            }
        }
        /// <summary>
        /// Starts the sound effects for the current weapon
        /// </summary>
        public void StartSound()
        {
            switch (currentWeapon)
            {
                case Weapons.Knife:
                    SoundManager.PlaySound(KNIFE_SOUND, false);
                    break;
                case Weapons.Pistol:
                    SoundManager.PlaySound(PISTOL_SOUND, false);
                    break;
                case Weapons.MachineGun:
                    wtime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //litte tick when when firing 
                    if (wtime > 0.2)
                    {
                        SoundManager.PlaySound(MACHINEGUN_SOUND, true);
                        wtime = 0;
                    }
                    break;
                case Weapons.GatlingGun:
                    wtime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //litte tick when firing
                    if (wtime > 0.06)
                    {
                        SoundManager.PlaySound(GATLINGGUN_SOUND, true);
                        wtime = 0;
                    }
                    break;
                default: break;
            }
        }
        #endregion
    }
}
