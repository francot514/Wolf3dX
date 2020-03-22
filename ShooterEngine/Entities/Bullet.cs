using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Nexxt.Common.Enums;
using Nexxt.Engine.AI;
using Nexxt.Engine.GameObjects;
using Nexxt.Engine.Entities.Actors;

namespace Nexxt.Engine.Entities
{
    public class Bullet : BaseEntity
    {

        #region Fields
        public Vector2 Direction;
        public Weapons WeaponType;
        public int HitPoints = 0;
        public float MaxDistanceToTravel = 0;
        public Vector2 OriginalPosition;
        public float Speed = 0;
        public GameTime GameTime;
        public Camera Camera;

        private Map map;
        private bool collisionWithMap = false;
        private bool collisionWithActor = false;
        private bool collisionWithMainPlayer = false;
        private Actor actorCollision;
        private Actor owner;

        private double elapsedTime = 0;

        #endregion

        #region Constants
        const string TRAVELING = "TRAVELING";
        const string FINISHED = "FINISHED";

        //max distance to notify neighbors that a bullet was shot
        const int AUDITIVE_PERCEPTION_MAX_DISTANCE = 14;
        #endregion

        #region Constructor
        public Bullet(Map _map, Weapons bulletType, Vector2 position, Vector2 direction, Actor _owner)
        {
            this.map = _map;
            this.OriginalPosition = position;
            this.Position = position;
            this.Direction = direction;
            this.WeaponType = bulletType;
            this.owner = _owner;

            CreateBulletProperties();

            entityStateMachine = new StateMachine();

            entityStateMachine.AddState(TRAVELING, null, TravelingTick, null);
            entityStateMachine.AddState(FINISHED, null, FinishedTick, null);
            elapsedTime = 0;
            
            //notify the neighbors that a bullet was shot nearly
            NotifyNeighborsBulletShot();
        }

        /// <summary>
        /// Notify the neighbors that a bullet was shot nearly
        /// </summary>
        private void NotifyNeighborsBulletShot()
        {
            int bulletStartX = (int)Position.X - AUDITIVE_PERCEPTION_MAX_DISTANCE/2;
            int bulletStartY = (int)Position.Y - AUDITIVE_PERCEPTION_MAX_DISTANCE/2;

            if (WeaponType != Weapons.Knife)
            {
                for (int i = 0; i < map.Actors.Actors.Length; i++)
                {
                    Actor actor = map.Actors.Actors[i];
                    
                    //check if the actor is active and is alive

                    if (actor.IsActive && (!actor.IsKilled))
                    {
                        if (actor != owner)
                        {
                            //check if the actor is close enough to receive the notification                          
                            int actorX = (int)actor.Position.X;
                            int actorY = (int)actor.Position.Y;
                            
                            //check intersection using rectangles
                            Rectangle rectangleBullet = new Rectangle(bulletStartX, bulletStartY, AUDITIVE_PERCEPTION_MAX_DISTANCE, AUDITIVE_PERCEPTION_MAX_DISTANCE);
                            Rectangle rectangleActor = new Rectangle(actorX, actorY, 1, 1);

                            if (rectangleBullet.Intersects(rectangleActor))
                            {
                                //notify the actor that a bullet was shot
                                actor.NotifyBulletShotByEnemy();
                            }
                        }
                    }
                }
            }
        }

        private void CreateBulletProperties()
        {
            Speed = 0.6f;
            switch (WeaponType)
            {
                case Weapons.Knife:
                    HitPoints = 5;
                    BoundingRadius = 1.2f;
                    MaxDistanceToTravel = 1f;
                    break;
                case Weapons.Pistol:
                    HitPoints = 20;
                    BoundingRadius = 0.2f;
                    MaxDistanceToTravel = 8f;
                    break;
                case Weapons.MachineGun:
                    HitPoints = 40;
                    BoundingRadius = 0.5f;
                    MaxDistanceToTravel = 8f;
                    break;
                case Weapons.GatlingGun:
                    HitPoints = 100;
                    BoundingRadius = 1f;
                    MaxDistanceToTravel = 12f;
                    break;

            }
        }
        #endregion

        #region Bullet state machine implementation

        void TravelingTick()
        {
            if (IsActive)
            {
                elapsedTime += (double)GameTime.ElapsedGameTime.TotalMilliseconds;

                //30 miliseconds
                if (elapsedTime > 30)
                {
                    //project the bullet 
                    Position += Vector2.Multiply(Direction, Speed);

                    collisionWithMap = false;
                    collisionWithActor = false;
                    collisionWithMainPlayer = false;
                    //check for collisions
                    if (CheckCollisions())
                    {
                        if (collisionWithActor)
                        {
                            actorCollision.DoCollision(this);
                        }
                        else if (collisionWithMainPlayer)
                        {
                            //TODO: manage the collision with the main player
                        }
                        entityStateMachine.State = FINISHED;
                    }

                    //check if the bullet has achieved its maximum distance to travel
                    float distance = (OriginalPosition - Position).Length();
                    if (distance > this.MaxDistanceToTravel)
                        entityStateMachine.State = FINISHED;

                    elapsedTime = 0;
                }

            }
        }

        void FinishedTick()
        {
            this.IsActive = false;
            collisionWithActor = false;
            collisionWithMap = false;
        }


        /// <summary>
        /// Check for collisions with the map and actors
        /// </summary>
        /// <returns></returns>
        public bool CheckCollisions()
        {
            //Check collisions with actor
            for (int i = 0; i < map.Actors.Actors.Length; i++)
            {
                Actor actor = map.Actors.Actors[i];
                string ownerTeam = String.Empty;
                if (owner != null)
                    ownerTeam = owner.Team;

                //check x and y position, and verify that the actor shot is not the owner(same team)
                if (actor != owner && actor.Team != ownerTeam &&
                    !actor.IsKilled && actor.IsActive && !actor.IsDying
                    )
                {
                    //Creates a bounding box and check if an intesection occurs 
                    if (CheckCollisionWithBoundingRadius(actor.Position, actor.BoundingRadius))
                    {
                        collisionWithActor = true;
                        actorCollision = actor;
                        break;
                    }
                }
            }

            //check collisions with Main Player 
            if (this.owner != null)
            {
                if (CheckCollisionWithBoundingRadius(Camera.Position, Camera.Radius))
                {
                    collisionWithMainPlayer = true;
                }
            }

            //check collisions with map
            int x = (int)Position.X;
            int y = (int)Position.Y;
            int value = map.WorldMap[x, y];
            if ((value > 0) && (value != Constants.COLLISION_BLOCK))
            {
                collisionWithMap = true;
            }

            return collisionWithActor || collisionWithMap || collisionWithMainPlayer;
        }
        /// <summary>
        /// Creates a bounding box and check if an intesection occurs between the actor an the bullet
        /// </summary>
        /// <param name="actorPosition"></param>
        /// <param name="actorRadius"></param>
        /// <returns></returns>
        public bool CheckCollisionWithBoundingRadius(Vector2 actorPosition, double actorRadius)
        {
            bool result = false;
            float factor = 1000.00f;

            // start positions
            float startActorX = actorPosition.X - (float)(actorRadius / 2);
            float startActorY = actorPosition.Y - (float)(actorRadius / 2);
            float startBulletX = Position.X - (float)BoundingRadius;
            float startBulletY = Position.Y - (float)BoundingRadius;

            startActorX = startActorX * factor;
            startActorY = startActorY * factor;
            startBulletX = startBulletX * factor;
            startBulletY = startBulletY * factor;

            //the width of the box
            int bulletRadiusInt = (int)(BoundingRadius * factor);
            int actorRadiusInt = (int)(actorRadius * factor);

            Rectangle boxActor = new Rectangle((int)startActorX, (int)startActorY, actorRadiusInt, actorRadiusInt);
            Rectangle boxBullet = new Rectangle((int)startBulletX, (int)startBulletY, bulletRadiusInt, bulletRadiusInt);

            //check for intersection
            result = boxActor.Intersects(boxBullet);

            return result;
        }
        #endregion

    }

    public class BulletCollection<T> : List<T> where T : Bullet
    {
        public void UpdateStates(GameTime gameTime, Camera camera)
        {
            for (int bulletIndex = 0; bulletIndex < this.Count; bulletIndex++)
            {
                Bullet bullet = this[bulletIndex];
                if (bullet.IsActive)
                {
                    bullet.Camera = camera;
                    bullet.GameTime = gameTime;
                    bullet.Tick();
                }
            }

            //remove finished bullets
            for (int bulletIndex = 0; bulletIndex < this.Count; bulletIndex++)
            {
                Bullet bullet = this[bulletIndex];
                if (!bullet.IsActive)
                {
                    this.Remove((T)bullet);
                }
            }
        }
    }
}
