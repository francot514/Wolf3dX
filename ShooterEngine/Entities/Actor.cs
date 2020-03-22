using System;
using System.Collections.Generic;
using System.Text;
using Nexxt.Engine.Entities;
using Nexxt.Engine.AI;
using Microsoft.Xna.Framework.Graphics;
using Nexxt.Engine.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Nexxt.Engine.GameObjects;
using Nexxt.Common;
using Nexxt.Engine.Entities.Actors.Scripts;
using Nexxt.Common.Enums;
using Nexxt.Engine.AI.Algorithms;
using Nexxt.Engine.MathUtilities;

namespace Nexxt.Engine.Entities.Actors
{
    public class Actor : BaseEntity
    {
        #region Constants
        // Possible States
        const string IDLE = "IDLE";
        const string PATROLLING = "PATROLLING";
        const string CHASING_PLAYER = "CHASING_PLAYER";
        const string ATTACKING = "ATTACKING";
        const string SUFFERING_PAIN = "SUFFERINGPAIN";
        const string DYING = "DYING";
        const string KILLED = "KILLED";




        //constants to calculate the appropiated texture depending on the angle, by default ( 90-fov ) = 24 degrees
        const double RANGE_SIDE_TEXTURES_IN_RADIANS = 0.418879;

        const double DEGREES_90_IN_RADIANS = 1.5708;
        const double DEGREES_360_IN_RADIANS = 6.283185307;
        const double DEGREES_270_IN_RADIANS = 4.71238898;

        //offset to the center of the actor
        public const float CENTER_OFFSET_X = 0.5f;
        public const float CENTER_OFFSET_Y = 0.5f;

        const float IDLE_DISTANCE = 16f;
        const float ATTACK_MAX_DISTANCE = 3.5f;
        const float ATTACK_MIN_DISTANCE = 1.8f;
        const float ATTACK_BODY_TO_BODY_MIN_DISTANCE = 1.5f;
        const float MAX_DISTANCE_ACTOR_CAN_SEE = 7f;

        #endregion

        #region Fields

        //Fields related to animation
        private CharacterDefinition characterDefinition;
        int ActorID = 0;
        private Script script;
        public String AnimationName;
        public int AnimationFrame;
        public int Anim;
        float time = 0;


        //attributes

        //number of hit points that the actor can receive before he dies
        public int HitPoints = 0;

        //number of hits that were received by the actor
        public int CurrentHitPoints = 0;

        // actor's max speed
        public float MaxSpeed = 0.5f;

        // actor's speed
        public int Speed = 0;

        // if the actor begin patrolling the zone
        public bool StartPatrolling = false;

        // max force
        public double MaxForce = 0.03;

        // if the actor suffers pain when he receives hits
        public bool HasPain = false;

        // it is the score granted when de actor dies
        public int Score = 0;

        // heading of the actor
        public Vector2 Direction;

        // vector perpendicular to the heading
        public Vector2 Side;

        // velocity
        public Vector2 Velocity;

        public String Team = "NAZIS";

        private double timeToCreateBullet;

        public double TimeElapsed = 0;

        // the target 
        public Vector2 Target;

        private SteeringBehavior Behavior;

        public Map Map;

        private double PainTime = 0;

        private double DyingTime = 0;

        public bool IsKilled = false;

        public bool IsDying = false;

        public Weapons WeaponType = Weapons.Pistol;

        private GameTime gameTime;

        public bool MustGetCloseEnoughToAttack = false;

        #endregion

        #region Properties
        public CharacterDefinition GetCharDef()
        {
            return characterDefinition;
        }

        public GameTime GameTime
        {
            set
            {
                gameTime = value;
                TimeElapsed = (double)value.ElapsedGameTime.Ticks;
            }
            get { return gameTime; }
        }

        #endregion

        #region Constructor
        public Actor(Map _map, Vector2 _position, CharacterDefinition newCharacterDefinition, int newActorID)
        {
            entityStateMachine = new StateMachine();

            // Idle doesn't need an end  
            entityStateMachine.AddState(IDLE, IdleBegin, IdleTick, null);

            // Shooting doesn't need Begin or End.  
            entityStateMachine.AddState(PATROLLING, null, PatrollingTick, null);

            entityStateMachine.AddState(CHASING_PLAYER, null, ChasingPlayerTick, null);
            entityStateMachine.AddState(ATTACKING, null, AttackingTick, null);
            entityStateMachine.AddState(SUFFERING_PAIN, null, SufferingPainTick, null);
            entityStateMachine.AddState(DYING, null, DyingTick, null);
            entityStateMachine.AddState(KILLED, null, KilledTick, null);

            characterDefinition = newCharacterDefinition;
            ActorID = newActorID;
            
            script = new Script(this);

            //executes the init script if exists
            InitScript();
            AnimationName = "";

            //starts with the idle animation
            SetAnim("idle");
            time = 0;


            //copy the actor's attributes from the character definition

            MaxSpeed = characterDefinition.MaxSpeed;
            MaxForce = characterDefinition.MaxForce;
            HasPain = characterDefinition.HasPain;
            Score = characterDefinition.Score;
            WeaponType = characterDefinition.WeaponType;
            BoundingRadius = 0.7f;
            MustGetCloseEnoughToAttack = characterDefinition.MustGetCloseEnoughToAttack;

            //TODO read the difficulty from the game variables
            HitPoints = characterDefinition.HitPoints[(int)Difficulty.None];
            CurrentHitPoints = 0;
            Map = _map;
            
            MoveTo(_position);

            Behavior = new SteeringBehavior(this, _map);
        }
        #endregion

        #region Enemy state machine implementation
        void IdleBegin()
        {

        }

        void IdleTick()
        {
            float distanceToTarget = CalculateDistanceToTarget(Target);

            if (StartPatrolling)
            {
                if (distanceToTarget < IDLE_DISTANCE)
                {
                    SetPatrolSpeed();
                    // if the target is close enough, start patrolling the zone
                    entityStateMachine.State = PATROLLING;
                }
            }
            else
            {
                // starts patrolling only if the target is in the fov
                if (IsTargetInFOV(Target))
                {
                    SetPatrolSpeed();
                    entityStateMachine.State = PATROLLING;
                }
            }
        }

        void SetPatrolSpeed()
        {
            MaxForce = MaxForce * 0.250f;
            MaxSpeed = MaxSpeed * 0.250f;
        }

        void SetChaseSpeed()
        {
            MaxForce = MaxForce * 4f;
            MaxSpeed = MaxSpeed * 4f;
        }

        void PatrollingTick()
        {
            SetAnim("walk");

            // Wander and wall avoidance behavior
            Behavior.WanderOn();
            Behavior.WallAvoidanceOn();
            Behavior.SeparationOn();
            Vector2 velocity = Behavior.Calculate();
            Behavior.WanderOff();
            Behavior.WallAvoidanceOff();
            Behavior.SeparationOff();

            Vector2 newPosition = Position + velocity;
            // makes the actor look to the new position
            LookAt(newPosition);
            MoveTo(newPosition);

            float distanceToTarget = CalculateDistanceToTarget(Target);

            if (IsTargetInFOV(Target))
            {
                SetChaseSpeed();
                // the target is in the actor´s fov, so it is time to chase him
                entityStateMachine.State = CHASING_PLAYER;
            }
            else if (distanceToTarget >= IDLE_DISTANCE)
            {
                // if the target is so far, do nothing
                entityStateMachine.State = IDLE;
            }
        }

        void ChasingPlayerTick()
        {

            float distanceToTarget = CalculateDistanceToTarget(Target);
            SetAnim("walk");

            Map.PathFinder.CheckActors = true;
            Map.PathFinder.CheckDoors = false;
            List<PathFinderNode> path = Map.PathFinder.FindPath(Map.Actors.Actors, Position, Target);
            Map.PathFinder.CheckActors = false;
            Map.PathFinder.CheckDoors = false;

            // check if a door is in the path. Open the door if it is necessary.
            bool waitingForDoor = CheckIfDoorIsInPath(path);

            if (!waitingForDoor && (path != null))
            {
                // set the path to the follow path steering behavior
                Behavior.SetPath(path);
                Behavior.FollowPathOn();
                Behavior.SeparationOn();
                Vector2 velocity = Behavior.Calculate();
                Behavior.FollowPathOff();
                Behavior.SeparationOff();

                Vector2 newPosition = Position + velocity;

                // adjust the actor's direction to look at the target
                LookAt(newPosition);
                
                // set the new position
                MoveTo(newPosition);
            }

            //calculate a random distance to attack between min and max attack distance
            float rnd = new Random().Next((int)ATTACK_MIN_DISTANCE * 1000, (int)(ATTACK_MAX_DISTANCE * 1000));
            float distanceToAttack = rnd / 1000f;


            if (path == null)
                distanceToAttack = ATTACK_MAX_DISTANCE;

            //if it is a dog, the actor must get close enough to attack
            if (MustGetCloseEnoughToAttack)
                distanceToAttack = ATTACK_BODY_TO_BODY_MIN_DISTANCE; 

            if (distanceToTarget <= distanceToAttack)
            {
                // if the target is close enough, start attacking him
                entityStateMachine.State = ATTACKING;
            }

        }

        /// <summary>
        /// Check if a door is in the path. If the door exists, and it is close enough to the actor, open the door.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool CheckIfDoorIsInPath(List<PathFinderNode> path)
        {
            bool waitingForDoor = false;
            if (path != null)
            {
                for (int x = 0; x < path.Count; x++)
                {
                    // path's position
                    int pX = path[x].X;
                    int pY = path[x].Y;
                    for (int y = 0; y < Map.Doors.Count; y++)
                    {
                        Door door = Map.Doors[y];
                        if (door.entityStateMachine.State == Door.CLOSED)
                        {
                            // door's position
                            int dX = (int)door.originalPosition.X;
                            int dY = (int)door.originalPosition.Y;
                            if (pX == dX && pY == dY)
                            {
                                // actor´s position
                                int actorX = (int)Position.X;
                                int actorY = (int)Position.Y;
                                float distanceToDoor = (door.originalPosition - Position).Length();
                                if (distanceToDoor < 1.00f)
                                {
                                    //open the door
                                    door.Open();
                                    // do nothing while the door is opening
                                    waitingForDoor = true;
                                }
                            }
                        }
                    }
                }
            }
            return waitingForDoor;
        }

        void AttackingTick()
        {
            SetAnim("attack");

            CreateBullets();

            float distanceToTarget = CalculateDistanceToTarget(Target);

            float maxDistance = ATTACK_MAX_DISTANCE;

            // check the distance in the case of actor which must be close enough to the target to attack
            if (MustGetCloseEnoughToAttack)
                maxDistance = ATTACK_BODY_TO_BODY_MIN_DISTANCE;

            if (distanceToTarget > maxDistance)
            {
                // if the target is too far, start chasing him
                entityStateMachine.State = CHASING_PLAYER;
            }
        }

        void SufferingPainTick()
        {
            SetAnim("pain");
            PainTime += (double)GameTime.ElapsedGameTime.TotalMilliseconds;
            if (PainTime > 175)
            {
                entityStateMachine.State = CHASING_PLAYER;
                PainTime = 0;
            }
        }

        void DyingTick()
        {
            if (!IsDying)
            {
                SetAnim("falling");
                IsDying = true;
            }
        }

        void KilledTick()
        {
            SetAnim("killed");
            IsKilled = true;
            IsDying = false;
        }

        /// <summary>
        /// This method is called from the script when the 'falling' animation ends
        /// </summary>
        public void Kill()
        {
            CleanPositionInMap(this.Position);
            IsKilled = true;
            entityStateMachine.State = KILLED;
        }


        #endregion

        #region Animation implementation
        private void InitScript()
        {
            SetAnim("init");
            if (AnimationName == "init")
            {
                for (int i = 0; i < characterDefinition.GetAnimation(Anim).getKeyFrameArray().Length; i++)
                {
                    if (characterDefinition.GetAnimation(Anim).GetKeyFrame(i).Frame > -1)
                        script.DoScript(Anim, i);
                }
            }
        }

        public void SetAnim(String newAnim)
        {

            if (AnimationName != newAnim)
            {
                for (int i = 0; i < characterDefinition.GetAnimationArray().Length; i++)
                {
                    if (characterDefinition.GetAnimation(i).Name == newAnim)
                    {
                        Anim = i;
                        AnimationFrame = 0;
                        AnimationName = newAnim;
                        time = 0;
                        if (ActorCollection<Actor>.TexturesLoaded) // only if the textures were loaded
                            Texture = ActorCollection<Actor>.Textures[characterDefinition.GetAnimation(i).GetKeyFrame(0).Frame];
                    }

                }
            }
        }

        public void SetFrame(int newFrame)
        {
            AnimationFrame = newFrame;
            time = 0;
            Texture = ActorCollection<Actor>.Textures[newFrame];
        }



        /// <summary>
        /// Animate the enemy
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="gameTime"></param>
        public void Animate(Camera camera, GameTime gameTime)
        {
            #region Update Animation

            Animation animation = characterDefinition.GetAnimation(Anim);
            KeyFrame keyframe = animation.GetKeyFrame(AnimationFrame);

            //calculate new object time
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (time > keyframe.Duration)
            {
                int pframe = AnimationFrame;

                //execute the associated script
                script.DoScript(Anim, AnimationFrame);

                time -= (float)keyframe.Duration;
                if (AnimationFrame == pframe)
                    AnimationFrame++;

                keyframe = animation.GetKeyFrame(AnimationFrame);

                if (AnimationFrame >=
                    animation.getKeyFrameArray().Length)
                {
                    AnimationFrame = 0;
                }
            }


            if (keyframe.Frame < 0)
            {
                AnimationFrame = 0;
            }

            //find the texture depending on the angle of vision
            if (keyframe.Frame >= 0)
            {
                if (animation.MustBeAnimatedFromDistinctAngles)
                    SetTextureDependingOnAngle(camera, keyframe.Frame);
                else
                    Texture = ActorCollection<Actor>.Textures[keyframe.Frame];
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
            Texture = ActorCollection<Actor>.Textures[frameBase + index];

        }

        #endregion

        #region AI
        /// <summary>
        /// Returns the distance to the target
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        float CalculateDistanceToTarget(Vector2 target)
        {
            float result = 0f;
            result = (Position - target).Length();
            return result;
        }

        /// <summary>
        /// Change the actor´s direction to point the specified vector
        /// </summary>
        /// <param name="newPosition"></param>
        public void LookAt(Vector2 newPosition)
        {
            if (newPosition != Position)
            {
                Vector2 v = newPosition - Position;
                this.Direction = Vector2.Normalize(v);
                this.Side = MathUtils.Perpendicular(this.Direction);
            }
        }

        /// <summary>
        /// Change the actor´s position to the specified vector
        /// </summary>
        /// <param name="newPosition"></param>
        public void MoveTo(Vector2 newPosition)
        {
            if (newPosition != Position)
            {
                //clean old position
                CleanPositionInMap(Position);
                
                //set new position
                SetPositionInMap(newPosition);
                
                this.Position = newPosition;
            }
        }

        /// <summary>
        /// Clean the position used by the actor in the map
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void CleanPositionInMap(Vector2 pos)
        {
            int x = (int)pos.X;
            int y = (int)pos.Y;
            if (Map != null)
            {
                if (Map.WorldMap[x, y] == Constants.COLLISION_BLOCK)
                {
                    Map.WorldMap[x, y] = 0;
                }
            }
        }

        /// <summary>
        /// Set the position used by the actor in the map
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SetPositionInMap(Vector2 pos)
        {
            int x = (int)pos.X;
            int y = (int)pos.Y;

            if (Map != null)
            {
                if (Map.WorldMap[x, y] == 0)
                {
                    Map.WorldMap[x, y] = Constants.COLLISION_BLOCK;
                }
            }
        }

        /// <summary>
        /// Calculates if the target is in the field of view of the actor
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsTargetInFOV(Vector2 target)
        {
            bool result = false;
            float distance = CalculateDistanceToTarget(target);
            if (distance <= MAX_DISTANCE_ACTOR_CAN_SEE)
            {
                Vector2[] feelers = CreateFeelers(MAX_DISTANCE_ACTOR_CAN_SEE);
                result = MathUtils.IsPointInTriangle(target, Position, feelers[1], feelers[2]);
            }
            return result;
        }

        /// <summary>
        /// Create vector pointing in the actor´s direction
        /// </summary>
        /// <param name="maxDistance"></param>
        /// <returns></returns>
        private Vector2[] CreateFeelers(float maxDistance)
        {
            Vector2[] feelers = new Vector2[3];
            //feeler pointing straight in front
            feelers[0] = Position + Vector2.Multiply(Direction, (float)maxDistance);

            //feeler to left
            Vector2 temp = Direction;
            MathUtils.Vec2DRotateAroundOrigin(ref temp, Constants.HALF_PI * 3.2f);
            feelers[1] = Position + Vector2.Multiply(temp, (float)(maxDistance));

            //feeler to right
            temp = Direction;
            MathUtils.Vec2DRotateAroundOrigin(ref temp, Constants.HALF_PI * 0.8f);
            feelers[2] = Position + Vector2.Multiply(temp, (float)(maxDistance));

            return feelers;
        }
        #endregion

        #region GameLogic

        /// <summary>
        /// Notify the actor that a bullet was shot nearly
        /// </summary>
        public void NotifyBulletShotByEnemy()
        {
            if (entityStateMachine.State == PATROLLING || entityStateMachine.State == IDLE)
                entityStateMachine.State = CHASING_PLAYER;
        }

        /// <summary>
        /// Do collision between the actor and the bullet
        /// </summary>
        /// <param name="bullet"></param>
        public void DoCollision(Bullet bullet)
        {
            //reduce hit points
            int points = CalculateHitPoints(bullet, bullet.HitPoints);
            HitPoints -= points;

            if (HitPoints <= 0)
            {
                // time to die
                entityStateMachine.State = DYING;
            }
            else
            {
                PainTime = 0;
                if (HasPain)
                {
                    //suffer pain state
                    entityStateMachine.State = SUFFERING_PAIN;
                }
                else
                {
                    //if is patrolling or idle start chasing the player
                    if (entityStateMachine.State == PATROLLING || entityStateMachine.State == IDLE)
                        entityStateMachine.State = CHASING_PLAYER;
                }
            }
        }

        /// <summary>
        /// Calculate the hit points to subtract depending on the actor´s state and distance traveled by the bullet
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="pointsOriginal"></param>
        /// <returns></returns>
        private int CalculateHitPoints(Bullet bullet, int pointsOriginal)
        {
            //compute the distance traveled by the bullet
            float distanceTraveled = (bullet.OriginalPosition - bullet.Position).Length();
            float distanceTraveledFactor = 0f;
            if (distanceTraveled <= 2)
            {
                distanceTraveledFactor = 1f;
            }
            else if (distanceTraveled > 2 && distanceTraveled <= 6)
            {
                distanceTraveledFactor = 0.8f;
            }
            else
            {
                distanceTraveledFactor = 0.7f;
            }

            ////compute the distance to the center of the actor
            //float distanceToCenter = (Position - bullet.Position).Length();
            //float distanceToCenterFactor = 0f;
            //if (distanceToCenter <= (BoundingRadius / 2))
            //{
            //    distanceToCenterFactor = 1;
            //}
            //else
            //{
            //    distanceToCenterFactor = 0.8f;
            //}

            //compute the factor depending on the actor's state
            float actorStateFactor = 0f;
            if (entityStateMachine.State == IDLE)
            {
                actorStateFactor = 1.25f;
            }
            else if (entityStateMachine.State == PATROLLING)
            {
                actorStateFactor = 1.10f;
            }
            else
            {
                actorStateFactor = 1f;
            }

            //compute points to rest to the actor's hitpoints
            pointsOriginal = (int)((float)pointsOriginal * actorStateFactor * distanceTraveledFactor);
            return pointsOriginal;
        }

        /// <summary>
        /// Create bullets while attacking
        /// </summary>
        void CreateBullets()
        {
            timeToCreateBullet += gameTime.ElapsedGameTime.TotalSeconds;
            //reload time to every 0.5 seconds
            if (timeToCreateBullet > 0.5)
            {
                Bullet bullet = new Bullet(Map, WeaponType, Position, Direction, this);
                bullet.IsActive = true;
                Map.Bullets.Add(bullet);

                timeToCreateBullet = 0;
            }
        }
        #endregion
    }
}
