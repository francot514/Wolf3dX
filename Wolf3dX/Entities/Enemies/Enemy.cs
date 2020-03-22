using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nexxt.Engine.Entities;
using Nexxt.Engine.AI;
using Microsoft.Xna.Framework.Graphics;
using Nexxt.Engine.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Nexxt.Engine.GameObjects;
using Wolf3d.StateManagement;
using Nexxt.Common;

namespace Wolf3d.Entities.Enemies
{
    public class Enemy : BaseEntity
    {
        #region Constants
        // Possible States
        const string IDLE = "IDLE";
        const string WALKING = "WALKING";
        const string SEARCHING_PLAYER = "SEARCHING_PLAYER";
        const string ATTACKING = "ATTACKING";
        const string KILLED = "KILLED";

        //start and end texture numbers associated to all the enemies
        const int START_TEXTURE = 50;
        const int END_TEXTURE = 407;
        

        //constants to calculate the appropiated texture depending on the angle, by default ( 90-fov ) = 24 degrees
        const double RANGE_SIDE_TEXTURES_IN_RADIANS = 0.418879;
        
        const double DEGREES_90_IN_RADIANS = 1.5708;
        const double DEGREES_360_IN_RADIANS = 6.283185307;
        const double DEGREES_270_IN_RADIANS = 4.71238898;
        #endregion

        #region Fields

        public static Texture2D[] Textures = new Texture2D[END_TEXTURE-START_TEXTURE+1];
        private CharacterDefinition charDef;
        int ID = 0;     
        private Script.Script script;
        public String AnimName;
        public int AnimFrame;
        public int Anim;
        float time = 0;
        public Vector2 Direction;

        #endregion

        #region Properties
        public CharacterDefinition GetCharDef()
        {
            return charDef;
        }

        
        #endregion

        #region Constructor
        public Enemy(Vector2 _position, CharacterDefinition newCharDef, int newID)
        {
            entityStateMachine = new StateMachine();

            // Idle doesn't need an end  
            entityStateMachine.AddState(IDLE, IdleBegin, IdleTick, null);

            // Shooting doesn't need Begin or End.  
            entityStateMachine.AddState(WALKING, null, WalkingTick, null);

            entityStateMachine.AddState(SEARCHING_PLAYER, null, SearchingPlayerTick, null);
            entityStateMachine.AddState(ATTACKING, null, AttackingTick, null);
            entityStateMachine.AddState(KILLED, null, KilledTick, null);

            charDef = newCharDef;
            ID = newID;
            Position = _position;
            script = new Script.Script(this);
            
            //executes the init script if exists
            InitScript();
            AnimName = "";
            
            //starts with the idle animation
            SetAnim("idle");
            time = 0;

        } 
        #endregion

        #region Enemy state machine implementation
        void IdleBegin()
        {

        }

        void IdleTick()
        {

        }

        void WalkingTick()
        {

        }

        void SearchingPlayerTick()
        {

        }

        void AttackingTick()
        {

        }

        void KilledTick()
        {

        } 
        #endregion

        #region Animation implementation
        private void InitScript()
        {
            SetAnim("init");
            if (AnimName == "init")
            {
                for (int i = 0; i < charDef.GetAnimation(Anim).getKeyFrameArray().Length; i++)
                {
                    if (charDef.GetAnimation(Anim).GetKeyFrame(i).Frame > -1)
                        script.DoScript(Anim, i);
                }
            }
        }

        public void SetAnim(String newAnim)
        {

            if (AnimName != newAnim)
            {
                for (int i = 0; i < charDef.GetAnimationArray().Length; i++)
                {
                    if (charDef.GetAnimation(i).Name == newAnim)
                    {
                        Anim = i;
                        AnimFrame = 0;
                        AnimName = newAnim;
                        time = 0;
                        Texture = Textures[charDef.GetAnimation(i).GetKeyFrame(0).Frame];
                    }

                }
            }
        }

        public void SetFrame(int newFrame)
        {
            AnimFrame = newFrame;
            time = 0;
            Texture = Textures[newFrame];
        }

        /// <summary>
        /// Load the object's textures
        /// </summary>
        /// <param name="Content"></param>
        internal static void LoadTextures(ContentManager Content)
        {
            for (int i = START_TEXTURE; i < 100; i++)
                Textures[i - START_TEXTURE] = Content.Load<Texture2D>(@"Sprites/Enemies/0" + i.ToString());

            for (int i = 100; i < END_TEXTURE+1; i++)
                Textures[i - START_TEXTURE] = Content.Load<Texture2D>(@"Sprites/Enemies/" + i.ToString());
        }

        /// <summary>
        /// Animate the enemy
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="gameTime"></param>
        public void Animate(Camera camera, GameTime gameTime)
        {
            #region Update Animation

            Animation animation = charDef.GetAnimation(Anim);
            KeyFrame keyframe = animation.GetKeyFrame(AnimFrame);

            //calculate new object time
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (time > keyframe.Duration)
            {
                int pframe = AnimFrame;

                //execute the associated script
                script.DoScript(Anim, AnimFrame);

                time -= (float)keyframe.Duration;
                if (AnimFrame == pframe)
                    AnimFrame++;

                keyframe = animation.GetKeyFrame(AnimFrame);

                if (AnimFrame >=
                    animation.getKeyFrameArray().Length)
                    AnimFrame = 0;

            }


            if (keyframe.Frame < 0)
                AnimFrame = 0;

            //find the texture depending on the angle of vision
            if (keyframe.Frame >= 0)
            {
                if (animation.MustBeAnimatedFromDistinctAngles)
                    SetTextureDependingOnAngle(camera, keyframe.Frame);
                else
                    Texture = Textures[keyframe.Frame];
            }
            #endregion
        }
        
        /// <summary>
        /// Fin the texture depending on the angle between the camera and the object.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="frameBase">the frame where the animation starts</param>
        private void SetTextureDependingOnAngle(Camera camera, int frameBase)
        {
            
            //compute the angle between de camera and the objetc
            Vector2 v = Position - camera.Position;
            double angleR = Math.Atan2(v.Y, v.X);
            
            //compute the angle of the object's direction
            double angleL = Math.Atan2(Direction.Y, Direction.X);
            
            //obtain the difference between de angle of objects and the angle of object's direction
            double angleO = angleL - angleR;
            
            // invert the result 270 degrees - angle
            double angleF = DEGREES_270_IN_RADIANS - angleO;

            int index = 0;

            //calculates de range of the interleaved textures (90 - range of the nort/east/west/south textures )
            double interleavedTextureRange = DEGREES_90_IN_RADIANS - RANGE_SIDE_TEXTURES_IN_RADIANS;

            // if angle is greater than 360 degrees
            if (angleF > DEGREES_360_IN_RADIANS)
                angleF = angleF - DEGREES_360_IN_RADIANS;

            
            //calculates the ranges for every texture based on the ranges constants
            double range1 = RANGE_SIDE_TEXTURES_IN_RADIANS / 2;
            double range2 = range1 + interleavedTextureRange;
            double range3 = range2 + RANGE_SIDE_TEXTURES_IN_RADIANS;
            double range4 = range3 + interleavedTextureRange;
            double range5 = range4 + RANGE_SIDE_TEXTURES_IN_RADIANS;
            double range6 = range5 + interleavedTextureRange;
            double range7 = range6 + RANGE_SIDE_TEXTURES_IN_RADIANS;
            double range8 = range7 + interleavedTextureRange;


            //compute the texture index depending on the ranges
            if (angleF >= 0 && angleF < range1)
                index = 6;
            else if (angleF >= range1 && angleF < range2)
                index = 7;
            else if (angleF >= range2 && angleF < range3)
                index = 0;
            else if (angleF >= range3 && angleF < range4)
                index = 1;
            else if (angleF >= range4 && angleF < range5)
                index = 2;
            else if (angleF >= range5 && angleF < range6)
                index = 3;
            else if (angleF >= range6 && angleF < range7)
                index = 4;
            else if (angleF >= range7 && angleF < range8)
                index = 5;
            else if (angleF >= range8 && angleF < DEGREES_360_IN_RADIANS)
                index = 6;
            
            //assign the correct texture
            Texture = Textures[frameBase+index];
            
        }

        #endregion

        #region Temporal

        public static Enemy[] Enemies = new Enemy[2];

        public static void CreateEnemiesTemporal(Map map)
        {
            CharacterDefinition definition = new CharacterDefinition("", EnemyType.Soldier, 50, 98);
            
            Enemy soldier = new Enemy(new Vector2(61.5f, 39.5f), definition, 1);
            soldier.IsActive = true;
            soldier.IsCollectable = false;
            soldier.IsMovable = false;
            soldier.Name = "ENEMY";
            soldier.Direction = new Vector2(1, 0);
            map.ObjectDatabase.Add(soldier);

            Enemy soldier2 = new Enemy(new Vector2(61.5f, 29.5f), definition, 1);
            soldier2.IsActive = true;
            soldier2.IsCollectable = false;
            soldier2.IsMovable = false;
            soldier2.Name = "ENEMY2";
            soldier2.Direction = new Vector2(-1, 0);
            map.ObjectDatabase.Add(soldier2);


          

            Enemies[0] = soldier;
            Enemies[1] = soldier2;
 
        }
        public static void AnimateEnemies(Camera camera, GameTime gameTime)
        {
            for( int i = 0; i < Enemies.Length; i++)
            {
                Enemies[i].Animate(camera, gameTime);
            }
        }
        #endregion

    }
}
