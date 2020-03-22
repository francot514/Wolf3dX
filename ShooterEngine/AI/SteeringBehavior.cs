using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Nexxt.Common.Enums;
using Nexxt.Engine.Entities.Actors;
using Nexxt.Engine.Entities;
using Nexxt.Engine.GameObjects;
using Nexxt.Engine.AI.Algorithms;
using Nexxt.Engine.MathUtilities;

namespace Nexxt.Engine.AI
{
    public class SteeringBehavior
    {
        #region Constants
        //--------------------------- Constants ----------------------------------

        //the radius of the constraining circle for the wander behavior
        const double WanderRad = 0.06;
        //distance the wander circle is projected in front of the agent
        const double WanderDist = 15f;
        //the maximum amount of displacement along the circle each frame
        const double WanderJitterPerSec = 40.0;

        //used in path following
        const double WaypointSeekDist = 1.2;


        #endregion

        #region Fields

        Actor actor;

        //these can be used to keep track of friends, pursuers, or prey
        Actor m_pTargetAgent1;
        Actor m_pTargetAgent2;

        //the steering force created by the combined effect of all
        //the selected behaviors
        Vector2 m_vSteeringForce;

        //the current target
        Vector2 m_vTarget;

        //length of the 'detection box' utilized in obstacle avoidance
        double m_dDBoxLength;

        //a vertex buffer to contain the feelers rqd for wall avoidance  
        Vector2[] m_Feelers;

        //the length of the 'feeler/s' used in wall detection
        double m_dWallDetectionFeelerLength;



        //the current position on the wander circle the agent is
        //attempting to steer towards
        Vector2 m_vWanderTarget;

        //explained above
        double m_dWanderJitter;
        double m_dWanderRadius;
        double m_dWanderDistance;


        //multipliers. These can be adjusted to effect strength of the  
        //appropriate behavior. Useful to get flocking the way you require
        //for example.
        double m_dWeightSeparation;
        double m_dWeightCohesion;
        double m_dWeightAlignment;
        double m_dWeightWander;
        double m_dWeightObstacleAvoidance;
        double m_dWeightWallAvoidance;
        double m_dWeightSeek;
        double m_dWeightFlee;
        double m_dWeightArrive;
        double m_dWeightPursuit;
        double m_dWeightOffsetPursuit;
        double m_dWeightInterpose;
        double m_dWeightHide;
        double m_dWeightEvade;
        double m_dWeightFollowPath;
        //how far the agent can 'see'
        double m_dViewDistance;

        //pointer to any current path
        List<PathFinderNode> m_pPath;

        //current node in the path
        int CurrentNodeInPath = 0;

        //the distance (squared) a vehicle has to be from a path waypoint before
        //it starts seeking to the next waypoint
        double m_dWaypointSeekDistSq;

        //any offset used for formations or offset pursuit
        Vector2 m_vOffset;

        //binary flags to indicate whether or not a behavior should be active
        int m_iFlags;

        //default
        DecelerationType m_Deceleration;

        //is cell space partitioning to be used or not?
        bool m_bCellSpaceOn;

        //what type of method is used to sum any active behavior
        SummingMethod m_SummingMethod;


        Map map;
        #endregion

        #region Properties
        /// <summary>
        /// This function tests if a specific bit of m_iFlags is set
        /// </summary>
        /// <param name="bt"></param>
        /// <returns></returns>
        bool On(BehaviorType bt)
        {
            return ((int)(m_iFlags) & (int)bt) == (int)bt;
        }

        public void SetTarget(Vector2 t)
        {
            m_vTarget = t;
        }

        public void SetTargetAgent1(Actor Agent) { m_pTargetAgent1 = Agent; }
        public void SetTargetAgent2(Actor Agent) { m_pTargetAgent2 = Agent; }

        public void SetOffset(Vector2 offset) { m_vOffset = offset; }
        public Vector2 GetOffset()
        {
            return m_vOffset;
        }
        public void SetPath(List<PathFinderNode> new_path)
        {
            m_pPath = new_path;
            CurrentNodeInPath = 0;
        }

        void CreateRandomPath(int num_waypoints, int mx, int my, int cx, int cy)
        {
            //m_pPath->CreateRandomPath(num_waypoints, mx, my, cx, cy);
        }

        public Vector2 Force()
        {
            return m_vSteeringForce;
        }

        public void ToggleSpacePartitioningOnOff()
        {
            m_bCellSpaceOn = !m_bCellSpaceOn;
        }

        public bool isSpacePartitioningOn()
        {
            return m_bCellSpaceOn;
        }

        public void SetSummingMethod(SummingMethod sm)
        {
            m_SummingMethod = sm;
        }


        public void FleeOn() { m_iFlags |= (int)BehaviorType.Flee; }
        public void SeekOn() { m_iFlags |= (int)BehaviorType.Seek; }
        public void ArriveOn() { m_iFlags |= (int)BehaviorType.Arrive; }
        public void WanderOn() { m_iFlags |= (int)BehaviorType.Wander; }
        public void PursuitOn(Actor v) { m_iFlags |= (int)BehaviorType.Pursuit; m_pTargetAgent1 = v; }
        public void EvadeOn(Actor v) { m_iFlags |= (int)BehaviorType.Evade; m_pTargetAgent1 = v; }
        public void CohesionOn() { m_iFlags |= (int)BehaviorType.Cohesion; }
        public void SeparationOn() { m_iFlags |= (int)BehaviorType.Separation; }
        public void AlignmentOn() { m_iFlags |= (int)BehaviorType.Allignment; }
        public void ObstacleAvoidanceOn() { m_iFlags |= (int)BehaviorType.ObstacleAvoidance; }
        public void WallAvoidanceOn() { m_iFlags |= (int)BehaviorType.WallAvoidance; }
        public void FollowPathOn() { m_iFlags |= (int)BehaviorType.FollowPath; }
        public void InterposeOn(Actor v1, Actor v2) { m_iFlags |= (int)BehaviorType.Interpose; m_pTargetAgent1 = v1; m_pTargetAgent2 = v2; }
        public void HideOn(Actor v) { m_iFlags |= (int)BehaviorType.Hide; m_pTargetAgent1 = v; }
        public void OffsetPursuitOn(Actor v1, Vector2 offset) { m_iFlags |= (int)BehaviorType.OffsetPursuit; m_vOffset = offset; m_pTargetAgent1 = v1; }
        public void FlockingOn() { CohesionOn(); AlignmentOn(); SeparationOn(); WanderOn(); }
        public void FleeOff() { if (On(BehaviorType.Flee))   m_iFlags ^= (int)BehaviorType.Flee; }
        public void SeekOff() { if (On(BehaviorType.Seek))   m_iFlags ^= (int)BehaviorType.Seek; }
        public void ArriveOff() { if (On(BehaviorType.Arrive)) m_iFlags ^= (int)BehaviorType.Arrive; }
        public void WanderOff() { if (On(BehaviorType.Wander)) m_iFlags ^= (int)BehaviorType.Wander; }
        public void PursuitOff() { if (On(BehaviorType.Pursuit)) m_iFlags ^= (int)BehaviorType.Pursuit; }
        public void EvadeOff() { if (On(BehaviorType.Evade)) m_iFlags ^= (int)BehaviorType.Evade; }
        public void CohesionOff() { if (On(BehaviorType.Cohesion)) m_iFlags ^= (int)BehaviorType.Cohesion; }
        public void SeparationOff() { if (On(BehaviorType.Separation)) m_iFlags ^= (int)BehaviorType.Separation; }
        public void AlignmentOff() { if (On(BehaviorType.Allignment)) m_iFlags ^= (int)BehaviorType.Allignment; }
        public void ObstacleAvoidanceOff() { if (On(BehaviorType.ObstacleAvoidance)) m_iFlags ^= (int)BehaviorType.ObstacleAvoidance; }
        public void WallAvoidanceOff() { if (On(BehaviorType.WallAvoidance)) m_iFlags ^= (int)BehaviorType.WallAvoidance; }
        public void FollowPathOff() { if (On(BehaviorType.FollowPath)) m_iFlags ^= (int)BehaviorType.FollowPath; }
        public void InterposeOff() { if (On(BehaviorType.Interpose)) m_iFlags ^= (int)BehaviorType.Interpose; }
        public void HideOff() { if (On(BehaviorType.Hide)) m_iFlags ^= (int)BehaviorType.Hide; }
        public void OffsetPursuitOff() { if (On(BehaviorType.OffsetPursuit)) m_iFlags ^= (int)BehaviorType.OffsetPursuit; }
        public void FlockingOff() { CohesionOff(); AlignmentOff(); SeparationOff(); WanderOff(); }

        public bool IsFleeOn() { return On(BehaviorType.Flee); }
        public bool IsSeekOn() { return On(BehaviorType.Seek); }
        public bool IsArriveOn() { return On(BehaviorType.Arrive); }
        public bool IsWanderOn() { return On(BehaviorType.Wander); }
        public bool IsPursuitOn() { return On(BehaviorType.Pursuit); }
        public bool IsEvadeOn() { return On(BehaviorType.Evade); }
        public bool IsCohesionOn() { return On(BehaviorType.Cohesion); }
        public bool IsSeparationOn() { return On(BehaviorType.Separation); }
        public bool IsAlignmentOn() { return On(BehaviorType.Allignment); }
        public bool IsObstacleAvoidanceOn() { return On(BehaviorType.ObstacleAvoidance); }
        public bool IsWallAvoidanceOn() { return On(BehaviorType.WallAvoidance); }
        public bool IsFollowPathOn() { return On(BehaviorType.FollowPath); }
        public bool IsInterposeOn() { return On(BehaviorType.Interpose); }
        public bool IsHideOn() { return On(BehaviorType.Hide); }
        public bool IsOffsetPursuitOn() { return On(BehaviorType.OffsetPursuit); }

        public double DBoxLength() { return m_dDBoxLength; }
        public Vector2[] GetFeelers() { return m_Feelers; }

        public double WanderJitter() { return m_dWanderJitter; }
        public double WanderDistance() { return m_dWanderDistance; }
        public double WanderRadius() { return m_dWanderRadius; }

        public double SeparationWeight() { return m_dWeightSeparation; }
        public double AlignmentWeight() { return m_dWeightAlignment; }
        public double CohesionWeight() { return m_dWeightCohesion; }
        #endregion

        #region Parameters
        float prWallAvoidance = 0.5f;
        float prObstacleAvoidance = 0.5f;
        float prSeparation = 0.2f;
        float prAlignment = 0.3f;
        float prCohesion = 0.6f;
        float prWander = 0.8f;
        float prSeek = 0.8f;
        float prFlee = 0.6f;
        float prEvade = 1.0f;
        float prHide = 0.8f;
        float prArrive = 0.5f;
        float MinDetectionBoxLength = 40.0f;
        #endregion

        #region Methods

        public SteeringBehavior(Actor _actor, Map _map)
        {
            #region default parameters
            actor = _actor;
            m_iFlags = 0;
            m_dDBoxLength = 40;
            m_dWeightCohesion = 2.0;
            m_dWeightAlignment = 1.0;
            m_dWeightSeparation = 1.0;
            m_dWeightObstacleAvoidance = 10.0;
            m_dWeightWander = 1.0;
            m_dWeightWallAvoidance = 10.0;
            m_dViewDistance = 50.0;
            m_dWallDetectionFeelerLength = 1;
            m_Feelers = new Vector2[3];
            m_Deceleration = DecelerationType.Normal;
            m_pTargetAgent1 = null;
            m_pTargetAgent2 = null;
            m_dWanderDistance = WanderDist;
            m_dWanderJitter = WanderJitterPerSec;
            m_dWanderRadius = WanderRad;
            m_dWaypointSeekDistSq = WaypointSeekDist * WaypointSeekDist;
            m_dWeightSeek = 1.0;
            m_dWeightFlee = 1.0;
            m_dWeightArrive = 1.0;
            m_dWeightPursuit = 1.0;
            m_dWeightOffsetPursuit = 1.0;
            m_dWeightInterpose = 1.0;
            m_dWeightHide = 1.0;
            m_dWeightEvade = 0.01;
            m_dWeightFollowPath = 10.0f;
            m_bCellSpaceOn = false;
            m_SummingMethod = SummingMethod.Prioritized;
            #endregion

            //stuff for the wander behavior
            double theta = MathUtils.RandFloat() * Constants.TWO_PI;

            //create a vector to a target position on the wander circle
            m_vWanderTarget = new Vector2((float)(m_dWanderRadius * Math.Cos(theta)),
                                          (float)(m_dWanderRadius * Math.Sin(theta)));

            map = _map;
            //create a Path
            //m_pPath = new Path();
            //m_pPath->LoopOn();
        }

        /// <summary>
        /// Calculates the accumulated steering force according to the method set
        /// in m_SummingMethod
        /// </summary>
        /// <returns></returns>
        public Vector2 Calculate()
        {
            //reset the steering force
            m_vSteeringForce = Vector2.Zero;

            //use space partitioning to calculate the neighbours of this vehicle
            //if switched on. If not, use the standard tagging system
            if (!isSpacePartitioningOn())
            {
                //tag neighbors if any of the following 3 group behaviors are switched on
                if (On(BehaviorType.Separation) || On(BehaviorType.Allignment) || On(BehaviorType.Cohesion))
                {
                    //m_pVehicle->World()->TagVehiclesWithinViewRange(m_pVehicle, m_dViewDistance);
                }
            }
            else
            {
                //calculate neighbours in cell-space if any of the following 3 group
                //behaviors are switched on
                if (On(BehaviorType.Separation) || On(BehaviorType.Allignment) || On(BehaviorType.Cohesion))
                {
                    //m_pVehicle->World()->CellSpace()->CalculateNeighbors(m_pVehicle->Pos(), m_dViewDistance);
                }
            }

            switch (m_SummingMethod)
            {
                case SummingMethod.WeightedAverage:

                    m_vSteeringForce = CalculateWeightedSum(); break;

                case SummingMethod.Prioritized:

                    m_vSteeringForce = CalculatePrioritized(); break;

                case SummingMethod.Dithered:

                    m_vSteeringForce = CalculateDithered(); break;

                default: m_vSteeringForce = new Vector2(0, 0); break;

            }//end switch

            return m_vSteeringForce;
        }

        /// <summary>
        /// returns the forward component of the steering force
        /// </summary>
        /// <returns></returns>
        public double ForwardComponent()
        {
            return Vector2.Dot(actor.Direction, m_vSteeringForce);
        }

        /// <summary>
        /// returns the side component of the steering force
        /// </summary>
        /// <returns></returns>
        public double SideComponent()
        {
            //return Vector2.Dot(m_pActor.Side, m_vSteeringForce);
            return 0;
        }

        /// <summary>
        /// This function calculates how much of its max steering force the 
        /// vehicle has left to apply and then applies that amount of the
        /// force to add.
        /// </summary>
        /// <param name="RunningTot"></param>
        /// <param name="ForceToAdd"></param>
        /// <returns></returns>
        public bool AccumulateForce(ref Vector2 RunningTot, Vector2 ForceToAdd)
        {

            //calculate how much steering force the vehicle has used so far
            double MagnitudeSoFar = RunningTot.Length();

            //calculate how much steering force remains to be used by this vehicle
            double MagnitudeRemaining = actor.MaxForce - MagnitudeSoFar;

            //return false if there is no more force left to use
            if (MagnitudeRemaining <= 0.0) return false;

            //calculate the magnitude of the force we want to add
            double MagnitudeToAdd = ForceToAdd.Length();

            //if the magnitude of the sum of ForceToAdd and the running total
            //does not exceed the maximum force available to this vehicle, just
            //add together. Otherwise add as much of the ForceToAdd vector is
            //possible without going over the max.
            if (MagnitudeToAdd < MagnitudeRemaining)
            {
                RunningTot += ForceToAdd;
            }

            else
            {
                //add it to the steering force
                RunningTot += Vector2.Multiply(Vector2.Normalize(ForceToAdd), (float)MagnitudeRemaining);
            }

            return true;
        }

        /// <summary>
        /// This method calls each active steering behavior in order of priority
        /// and acumulates their forces until the max steering force magnitude
        /// is reached, at which time the function returns the steering force 
        /// accumulated to that point
        /// </summary>
        /// <returns></returns>
        public Vector2 CalculatePrioritized()
        {
            Vector2 force = Vector2.Zero;

            if (On(BehaviorType.WallAvoidance))
            {
                //TODO: Fix this
                force = Vector2.Multiply(WallAvoidance(actor.Map.WorldMap), (float)m_dWeightWallAvoidance);

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            if (On(BehaviorType.ObstacleAvoidance))
            {
                //TODO: Fix this
                //force = ObstacleAvoidance(m_pVehicle->World()->Obstacles()) * m_dWeightObstacleAvoidance;

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            if (On(BehaviorType.Evade))
            {
                //assert(m_pTargetAgent1 && "Evade target not assigned");

                force = Vector2.Multiply(Evade(m_pTargetAgent1), (float)m_dWeightEvade);

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }


            if (On(BehaviorType.Flee))
            {
                force = Vector2.Multiply(Flee(actor.Target), (float)m_dWeightFlee);

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }



            //these next three can be combined for flocking behavior (wander is
            //also a good behavior to add into this mix)
            if (!isSpacePartitioningOn())
            {
                if (On(BehaviorType.Separation))
                {
                    force = Vector2.Multiply(Separation(map.Actors.Actors), (float)m_dWeightSeparation);

                    if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
                }

                if (On(BehaviorType.Allignment))
                {
                    force = Vector2.Multiply(Alignment(map.Actors.Actors), (float)m_dWeightAlignment);

                    if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
                }

                if (On(BehaviorType.Cohesion))
                {
                    force = Vector2.Multiply(Cohesion(map.Actors.Actors), (float)m_dWeightCohesion);

                    if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
                }
            }

            else
            {

                /*if (On(BehaviorType.separation))
                {
                    force = SeparationPlus(map.Actors.Actors) * m_dWeightSeparation;

                    if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
                }

                if (On(BehaviorType.allignment))
                {
                    force = AlignmentPlus(map.Actors.Actors) * m_dWeightAlignment;

                    if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
                }

                if (On(BehaviorType.cohesion))
                {
                    force = CohesionPlus(map.Actors.Actors) * m_dWeightCohesion;

                    if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
                }*/
            }

            if (On(BehaviorType.Seek))
            {
                force = Vector2.Multiply(Seek(actor.Target), (float)m_dWeightSeek);

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }


            if (On(BehaviorType.Arrive))
            {
                force = Vector2.Multiply(Arrive(actor.Target, m_Deceleration), (float)m_dWeightArrive);

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            if (On(BehaviorType.Wander))
            {
                force = Vector2.Multiply(Wander(), (float)m_dWeightWander);

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            if (On(BehaviorType.Pursuit))
            {
                //assert(m_pTargetAgent1 && "pursuit target not assigned");

                force = Vector2.Multiply(Pursuit(m_pTargetAgent1), (float)m_dWeightPursuit);

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            if (On(BehaviorType.OffsetPursuit))
            {
                //assert(m_pTargetAgent1 && "pursuit target not assigned");
                //assert(!m_vOffset.isZero() && "No offset assigned");

                force = OffsetPursuit(m_pTargetAgent1, m_vOffset);

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            if (On(BehaviorType.Interpose))
            {
                //assert(m_pTargetAgent1 && m_pTargetAgent2 && "Interpose agents not assigned");

                force = Vector2.Multiply(Interpose(m_pTargetAgent1, m_pTargetAgent2), (float)m_dWeightInterpose);

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            if (On(BehaviorType.Hide))
            {
                //assert(m_pTargetAgent1 && "Hide target not assigned");

                //TODO: Fix this
                //force = Hide(m_pTargetAgent1, m_pVehicle->World()->Obstacles()) * m_dWeightHide;

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }


            if (On(BehaviorType.FollowPath))
            {
                force = Vector2.Multiply(FollowPath(), (float)m_dWeightFollowPath);

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            return m_vSteeringForce;
        }

        /// <summary>
        /// This simply sums up all the active behaviors X their weights and 
        /// truncates the result to the max available steering force before 
        /// returning
        /// </summary>
        /// <returns></returns>
        public Vector2 CalculateWeightedSum()
        {
            if (On(BehaviorType.WallAvoidance))
            {
                //TODO: Fix this
                m_vSteeringForce += Vector2.Multiply(WallAvoidance(actor.Map.WorldMap), (float)m_dWeightWallAvoidance);
            }

            if (On(BehaviorType.ObstacleAvoidance))
            {
                //TODO: Fix this
                //m_vSteeringForce += ObstacleAvoidance(m_pVehicle->World()->Obstacles()) * m_dWeightObstacleAvoidance;
            }

            if (On(BehaviorType.Evade))
            {
                //assert(m_pTargetAgent1 && "Evade target not assigned");

                m_vSteeringForce += Vector2.Multiply(Evade(m_pTargetAgent1), (float)m_dWeightEvade);
            }


            //these next three can be combined for flocking behavior (wander is
            //also a good behavior to add into this mix)
            if (!isSpacePartitioningOn())
            {
                if (On(BehaviorType.Separation))
                {
                    m_vSteeringForce += Vector2.Multiply(Separation(map.Actors.Actors), (float)m_dWeightSeparation);
                }

                if (On(BehaviorType.Allignment))
                {
                    m_vSteeringForce += Vector2.Multiply(Alignment(map.Actors.Actors), (float)m_dWeightAlignment);
                }

                if (On(BehaviorType.Cohesion))
                {
                    m_vSteeringForce += Vector2.Multiply(Cohesion(map.Actors.Actors), (float)m_dWeightCohesion);
                }
            }
            else
            {
                /* if (On(BehaviorType.separation))
                 {
                     m_vSteeringForce += SeparationPlus(map.Actors.Actors) * m_dWeightSeparation;
                 }

                 if (On(BehaviorType.allignment))
                 {
                     m_vSteeringForce += AlignmentPlus(map.Actors.Actors) * m_dWeightAlignment;
                 }

                 if (On(BehaviorType.cohesion))
                 {
                     m_vSteeringForce += CohesionPlus(map.Actors.Actors) * m_dWeightCohesion;
                 }*/
            }


            if (On(BehaviorType.Wander))
            {
                m_vSteeringForce += Vector2.Multiply(Wander(), (float)m_dWeightWander);
            }

            if (On(BehaviorType.Seek))
            {
                m_vSteeringForce += Vector2.Multiply(Seek(actor.Target), (float)m_dWeightSeek);
            }

            if (On(BehaviorType.Flee))
            {
                m_vSteeringForce += Vector2.Multiply(Flee(actor.Target), (float)m_dWeightFlee);
            }

            if (On(BehaviorType.Arrive))
            {
                m_vSteeringForce += Vector2.Multiply(Arrive(actor.Target, m_Deceleration), (float)m_dWeightArrive);
            }

            if (On(BehaviorType.Pursuit))
            {
                //assert(m_pTargetAgent1 && "pursuit target not assigned");

                m_vSteeringForce += Vector2.Multiply(Pursuit(m_pTargetAgent1), (float)m_dWeightPursuit);
            }

            if (On(BehaviorType.OffsetPursuit))
            {
                //assert(m_pTargetAgent1 && "pursuit target not assigned");
                //assert(!m_vOffset.isZero() && "No offset assigned");

                m_vSteeringForce += Vector2.Multiply(OffsetPursuit(m_pTargetAgent1, m_vOffset), (float)m_dWeightOffsetPursuit);
            }

            if (On(BehaviorType.Interpose))
            {
                //assert(m_pTargetAgent1 && m_pTargetAgent2 && "Interpose agents not assigned");

                m_vSteeringForce += Vector2.Multiply(Interpose(m_pTargetAgent1, m_pTargetAgent2), (float)m_dWeightInterpose);
            }

            if (On(BehaviorType.Hide))
            {
                //assert(m_pTargetAgent1 && "Hide target not assigned");
                //TODO: Fix this
                //m_vSteeringForce += Hide(m_pTargetAgent1, m_pVehicle->World()->Obstacles()) * m_dWeightHide;
            }

            if (On(BehaviorType.FollowPath))
            {
                m_vSteeringForce += Vector2.Multiply(FollowPath(), (float)m_dWeightFollowPath);
            }

            m_vSteeringForce = TruncateVector2(m_vSteeringForce, actor.MaxForce);
            return m_vSteeringForce;
        }

        public Vector2 TruncateVector2(Vector2 vector, double max)
        {
            Vector2 result;
            result = vector;
            if (vector.Length() > max)
            {
                result.Normalize();
                result = Vector2.Multiply(result, (float)max);
            }
            return result;
        }

        bool IsVector2Zero(Vector2 vector) { return (vector.X * vector.X + vector.Y * vector.Y) < Double.MinValue; }

        /// <summary>
        /// Given 2 lines in 2D space AB, CD this returns true if an 
        /// intersection occurs and sets dist to the distance the intersection
        /// occurs along AB. Also sets the 2d vector point to the point of
        /// intersection
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <param name="dist"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool LineIntersection2D(Vector2 A,
                               Vector2 B,
                               Vector2 C,
                               Vector2 D,
                               ref double dist,
                               ref Vector2 point)
        {

            double rTop = (A.Y - C.Y) * (D.X - C.X) - (A.X - C.X) * (D.Y - C.Y);
            double rBot = (B.X - A.X) * (D.Y - C.Y) - (B.Y - A.Y) * (D.X - C.X);

            double sTop = (A.Y - C.Y) * (B.X - A.X) - (A.X - C.X) * (B.Y - A.Y);
            double sBot = (B.X - A.X) * (D.Y - C.Y) - (B.Y - A.Y) * (D.X - C.X);

            if ((rBot == 0) || (sBot == 0))
            {
                //lines are parallel
                return false;
            }

            double r = rTop / rBot;
            double s = sTop / sBot;

            if ((r > 0) && (r < 1) && (s > 0) && (s < 1))
            {
                dist = Vector2.Distance(A, B) * r;

                point = A + Vector2.Multiply((B - A), (float)r);

                return true;
            }

            else
            {
                dist = 0;

                return false;
            }
        }

        /// <summary>
        ///  this method sums up the active behaviors by assigning a probabilty
        ///  of being calculated to each behavior. It then tests the first priority
        ///  to see if it should be calcukated this simulation-step. If so, it
        ///  calculates the steering force resulting from this behavior. If it is
        ///  more than zero it returns the force. If zero, or if the behavior is
        ///  skipped it continues onto the next priority, and so on.
        ///  NOTE: Not all of the behaviors have been implemented in this method,
        ///        just a few, so you get the general idea
        /// </summary>
        /// <returns></returns>
        public Vector2 CalculateDithered()
        {
            //reset the steering force
            m_vSteeringForce = Vector2.Zero;

            if (On(BehaviorType.WallAvoidance) && MathUtils.RandFloat() < prWallAvoidance)
            {
                //TODO: Fix this
                //m_vSteeringForce = WallAvoidance(m_pVehicle->World()->Walls()) * m_dWeightWallAvoidance / prWallAvoidance;

                if (!IsVector2Zero(m_vSteeringForce))
                {
                    m_vSteeringForce = TruncateVector2(m_vSteeringForce, actor.MaxForce);
                    return m_vSteeringForce;
                }
            }

            if (On(BehaviorType.ObstacleAvoidance) && MathUtils.RandFloat() < prObstacleAvoidance)
            {
                //TODO: Fix this
                //m_vSteeringForce += ObstacleAvoidance(m_pVehicle->World()->Obstacles()) * m_dWeightObstacleAvoidance / prObstacleAvoidance;

                if (!IsVector2Zero(m_vSteeringForce))
                {
                    m_vSteeringForce = TruncateVector2(m_vSteeringForce, actor.MaxForce);
                    return m_vSteeringForce;
                }
            }

            if (!isSpacePartitioningOn())
            {
                if (On(BehaviorType.Separation) && MathUtils.RandFloat() < prSeparation)
                {
                    m_vSteeringForce += Vector2.Multiply(Separation(map.Actors.Actors), (float)
                                        m_dWeightSeparation / prSeparation);

                    if (!IsVector2Zero(m_vSteeringForce))
                    {
                        m_vSteeringForce = TruncateVector2(m_vSteeringForce, actor.MaxForce);
                        return m_vSteeringForce;
                    }
                }
            }

            else
            {
                if (On(BehaviorType.Separation) && MathUtils.RandFloat() < prSeparation)
                {
                    //m_vSteeringForce += SeparationPlus(map.Actors.Actors) *
                    //                    m_dWeightSeparation / prSeparation;

                    //if (!IsVector2Zero(m_vSteeringForce))
                    //{
                    //    m_vSteeringForce = TruncateVector2(m_vSteeringForce, m_pActor.MaxForce);
                    //    return m_vSteeringForce;
                    //}
                }
            }


            if (On(BehaviorType.Flee) && MathUtils.RandFloat() < prFlee)
            {
                m_vSteeringForce += Vector2.Multiply(Flee(actor.Target), (float)m_dWeightFlee / prFlee);

                if (!IsVector2Zero(m_vSteeringForce))
                {
                    m_vSteeringForce = TruncateVector2(m_vSteeringForce, actor.MaxForce);
                    return m_vSteeringForce;
                }
            }

            if (On(BehaviorType.Evade) && MathUtils.RandFloat() < prEvade)
            {
                //assert(m_pTargetAgent1 && "Evade target not assigned");

                m_vSteeringForce += Vector2.Multiply(Evade(m_pTargetAgent1), (float)(m_dWeightEvade / prEvade));

                if (!IsVector2Zero(m_vSteeringForce))
                {
                    m_vSteeringForce = TruncateVector2(m_vSteeringForce, actor.MaxForce);
                    return m_vSteeringForce;
                }
            }


            if (!isSpacePartitioningOn())
            {
                if (On(BehaviorType.Allignment) && MathUtils.RandFloat() < prAlignment)
                {
                    m_vSteeringForce += Vector2.Multiply(Alignment(map.Actors.Actors), (float)
                                        m_dWeightAlignment / prAlignment);

                    if (!IsVector2Zero(m_vSteeringForce))
                    {
                        m_vSteeringForce = TruncateVector2(m_vSteeringForce, actor.MaxForce);
                        return m_vSteeringForce;
                    }
                }

                if (On(BehaviorType.Cohesion) && MathUtils.RandFloat() < prCohesion)
                {
                    m_vSteeringForce += Vector2.Multiply(Cohesion(map.Actors.Actors), (float)
                                        m_dWeightCohesion / prCohesion);

                    if (!IsVector2Zero(m_vSteeringForce))
                    {
                        m_vSteeringForce = TruncateVector2(m_vSteeringForce, actor.MaxForce);
                        return m_vSteeringForce;
                    }
                }
            }
            else
            {
                /*if (On(BehaviorType.allignment) && RandFloat() < prAlignment)
                {
                    m_vSteeringForce += AlignmentPlus(map.Actors.Actors) *
                                        m_dWeightAlignment / prAlignment;

                    if (!IsVector2Zero(m_vSteeringForce))
                    {
                        m_vSteeringForce = TruncateVector2(m_vSteeringForce, m_pActor.MaxForce);
                        return m_vSteeringForce;
                    }
                }

                if (On(BehaviorType.cohesion) && RandFloat() < prCohesion)
                {
                    m_vSteeringForce += CohesionPlus(map.Actors.Actors) *
                                        m_dWeightCohesion / prCohesion;

                    if (!IsVector2Zero(m_vSteeringForce))
                    {
                        m_vSteeringForce = TruncateVector2(m_vSteeringForce, m_pActor.MaxForce);
                        return m_vSteeringForce;
                    }
                }*/
            }

            if (On(BehaviorType.Wander) && MathUtils.RandFloat() < prWander)
            {
                m_vSteeringForce += Vector2.Multiply(Wander(), (float)(m_dWeightWander / prWander));

                if (!IsVector2Zero(m_vSteeringForce))
                {
                    m_vSteeringForce = TruncateVector2(m_vSteeringForce, actor.MaxForce);
                    return m_vSteeringForce;
                }
            }

            if (On(BehaviorType.Seek) && MathUtils.RandFloat() < prSeek)
            {
                m_vSteeringForce += Vector2.Multiply(Seek(actor.Target), (float)m_dWeightSeek / prSeek);

                if (!IsVector2Zero(m_vSteeringForce))
                {
                    m_vSteeringForce = TruncateVector2(m_vSteeringForce, actor.MaxForce);
                    return m_vSteeringForce;
                }
            }

            if (On(BehaviorType.Arrive) && MathUtils.RandFloat() < prArrive)
            {
                m_vSteeringForce += Vector2.Multiply(Arrive(actor.Target, m_Deceleration), (float)
                                    m_dWeightArrive / prArrive);

                if (!IsVector2Zero(m_vSteeringForce))
                {
                    m_vSteeringForce = TruncateVector2(m_vSteeringForce, actor.MaxForce);
                    return m_vSteeringForce;
                }
            }

            return m_vSteeringForce;
        }
        #endregion

        #region Behaviors

        /// <summary>
        ///  Given a target, this behavior returns a steering force which will
        ///  direct the agent towards the target
        /// </summary>
        /// <param name="TargetPos"></param>
        /// <returns></returns>
        Vector2 Seek(Vector2 targetPosition)
        {
            Vector2 DesiredVelocity = Vector2.Multiply(
                                            Vector2.Normalize(targetPosition - actor.Position),
                                            actor.MaxSpeed);

            return (DesiredVelocity - actor.Velocity);
        }

        /// <summary>
        /// Does the opposite of Seek
        /// </summary>
        /// <param name="TargetPos"></param>
        /// <returns></returns>
        Vector2 Flee(Vector2 TargetPos)
        {
            //only flee if the target is within 'panic distance'. Work in distance
            //squared space.
            /* const double PanicDistanceSq = 100.0f * 100.0;
             if (Vec2DDistanceSq(m_pVehicle->Pos(), target) > PanicDistanceSq)
             {
               return Vector2D(0,0);
             }
             */

            Vector2 DesiredVelocity = Vector2.Multiply(
                                        Vector2.Normalize(actor.Position - TargetPos),
                                        actor.MaxSpeed);

            return (DesiredVelocity - actor.Velocity);
        }

        /// <summary>
        /// This behavior is similar to seek but it attempts to arrive at the
        /// target with a zero velocity
        /// </summary>
        /// <param name="TargetPos"></param>
        /// <param name="deceleration"></param>
        /// <returns></returns>
        Vector2 Arrive(Vector2 TargetPos, DecelerationType deceleration)
        {
            Vector2 ToTarget = TargetPos - actor.Position;

            //calculate the distance to the target
            double dist = ToTarget.Length();

            if (dist > 0)
            {
                //because Deceleration is enumerated as an int, this value is required
                //to provide fine tweaking of the deceleration..
                const double DecelerationTweaker = 0.3;

                //calculate the speed required to reach the target given the desired
                //deceleration
                double speed = dist / ((double)deceleration * DecelerationTweaker);

                //make sure the velocity does not exceed the max
                speed = Math.Min(speed, actor.MaxSpeed);

                //from here proceed just like Seek except we don't need to normalize 
                //the ToTarget vector because we have already gone to the trouble
                //of calculating its length: dist. 
                Vector2 DesiredVelocity = Vector2.Multiply(ToTarget, (float)(speed / dist));

                return (DesiredVelocity - actor.Velocity);
            }

            return Vector2.Zero;
        }

        /// <summary>
        /// This behavior creates a force that steers the agent towards the evader
        /// </summary>
        /// <param name="evader"></param>
        /// <returns></returns>
        Vector2 Pursuit(Actor evader)
        {
            //if the evader is ahead and facing the agent then we can just seek
            //for the evader's current position.
            Vector2 ToEvader = evader.Position - actor.Position;

            double RelativeHeading = Vector2.Dot(actor.Direction, evader.Direction);

            if ((Vector2.Dot(ToEvader, actor.Direction) > 0) &&
                 (RelativeHeading < -0.95))  //acos(0.95)=18 degs
            {
                return Seek(evader.Position);
            }

            //Not considered ahead so we predict where the evader will be.

            //the lookahead time is propotional to the distance between the evader
            //and the pursuer; and is inversely proportional to the sum of the
            //agent's velocities
            double LookAheadTime = ToEvader.Length() /
                                  (actor.MaxSpeed + evader.Speed);

            //now seek to the predicted future position of the evader
            return Seek(evader.Position + Vector2.Multiply(evader.Velocity, (float)LookAheadTime));
        }

        /// <summary>
        /// similar to pursuit except the agent Flees from the estimated future
        /// position of the pursuer
        /// </summary>
        /// <param name="pursuer"></param>
        /// <returns></returns>
        Vector2 Evade(Actor pursuer)
        {
            /* Not necessary to include the check for facing direction this time */

            Vector2 ToPursuer = pursuer.Position - actor.Position;

            //uncomment the following two lines to have Evade only consider pursuers 
            //within a 'threat range'
            const double ThreatRange = 100.0;
            if (ToPursuer.LengthSquared() > ThreatRange * ThreatRange) return Vector2.Zero;

            //the lookahead time is propotional to the distance between the pursuer
            //and the pursuer; and is inversely proportional to the sum of the
            //agents' velocities
            double LookAheadTime = ToPursuer.Length() /
                                   (actor.MaxSpeed + pursuer.Speed);

            //now flee away from predicted future position of the pursuer
            return Flee(pursuer.Position + Vector2.Multiply(pursuer.Velocity, (float)LookAheadTime));
        }

        /// <summary>
        /// This behavior makes the agent wander about randomly
        /// </summary>
        /// <returns></returns>
        public Vector2 Wander()
        {
            //this behavior is dependent on the update rate, so this line must
            //be included when using time independent framerate.
            double JitterThisTimeSlice = m_dWanderJitter * actor.TimeElapsed;

            //first, add a small random vector to the target's position
            m_vWanderTarget += new Vector2((float)(MathUtils.RandomClamped() * JitterThisTimeSlice),
                                        (float)(MathUtils.RandomClamped() * JitterThisTimeSlice));

            //reproject this new vector back on to a unit circle
            m_vWanderTarget.Normalize();

            //increase the length of the vector to the same as the radius
            //of the wander circle
            m_vWanderTarget = Vector2.Multiply(m_vWanderTarget, (float)m_dWanderRadius);

            //move the target into a position WanderDist in front of the agent
            Vector2 target = m_vWanderTarget + new Vector2((float)m_dWanderDistance, 0);

            //project the target into world space
            Vector2 Target = MathUtils.PointToWorldSpace(target,
                                                 actor.Direction,
                                                 actor.Side,
                                                 actor.Position);

            //and steer towards it
            return Target - actor.Position;
        }

        /// <summary>
        /// Given a vector of Obstacles, this method returns a steering force
        /// that will prevent the agent colliding with the closest obstacle
        /// </summary>
        /// <param name="obstacles"></param>
        /// <returns></returns>
        Vector2 ObstacleAvoidance(BaseEntity[] obstacles)
        {
            //the detection box length is proportional to the agent's velocity
            m_dDBoxLength = MinDetectionBoxLength +
                            (actor.Speed / actor.MaxSpeed) * MinDetectionBoxLength;

            //TODO: FIX this
            //tag all obstacles within range of the box for processing
            //m_pActor->World()->TagObstaclesWithinViewRange(m_pActor, m_dDBoxLength);

            //this will keep track of the closest intersecting obstacle (CIB)
            BaseEntity ClosestIntersectingObstacle = null;

            //this will be used to track the distance to the CIB
            double DistToClosestIP = Double.MaxValue;

            //this will record the transformed local coordinates of the CIB
            Vector2 LocalPosOfClosestObstacle = Vector2.Zero;

            foreach (BaseEntity curOb in obstacles)
            {
                //if the obstacle has been tagged within range proceed
                if (curOb.IsTagged)
                {
                    //calculate this obstacle's position in local space
                    Vector2 LocalPos = MathUtils.PointToLocalSpace(curOb.Position,
                                                           actor.Direction,
                                                           actor.Side,
                                                           actor.Position);

                    //if the local position has a negative x value then it must lay
                    //behind the agent. (in which case it can be ignored)
                    if (LocalPos.X >= 0)
                    {
                        //if the distance from the x axis to the object's position is less
                        //than its radius + half the width of the detection box then there
                        //is a potential intersection.
                        double ExpandedRadius = curOb.BoundingRadius + actor.BoundingRadius;

                        if (Math.Abs(LocalPos.Y) < ExpandedRadius)
                        {
                            //now to do a line/circle intersection test. The center of the 
                            //circle is represented by (cX, cY). The intersection points are 
                            //given by the formula x = cX +/-sqrt(r^2-cY^2) for y=0. 
                            //We only need to look at the smallest positive value of x because
                            //that will be the closest point of intersection.
                            double cX = LocalPos.X;
                            double cY = LocalPos.Y;

                            //we only need to calculate the sqrt part of the above equation once
                            double SqrtPart = Math.Sqrt(ExpandedRadius * ExpandedRadius - cY * cY);

                            double ip = cX - SqrtPart;

                            if (ip <= 0.0)
                            {
                                ip = cX + SqrtPart;
                            }

                            //test to see if this is the closest so far. If it is keep a
                            //record of the obstacle and its local coordinates
                            if (ip < DistToClosestIP)
                            {
                                DistToClosestIP = ip;

                                ClosestIntersectingObstacle = curOb;

                                LocalPosOfClosestObstacle = LocalPos;
                            }
                        }
                    }
                }
            }

            //if we have found an intersecting obstacle, calculate a steering 
            //force away from it
            Vector2 SteeringForce = Vector2.Zero;

            if (ClosestIntersectingObstacle != null)
            {
                //the closer the agent is to an object, the stronger the 
                //steering force should be
                double multiplier = 1.0 + (m_dDBoxLength - LocalPosOfClosestObstacle.X) /
                                    m_dDBoxLength;

                //calculate the lateral force
                SteeringForce.Y = (float)((ClosestIntersectingObstacle.BoundingRadius -
                                            LocalPosOfClosestObstacle.Y) * multiplier);

                //apply a braking force proportional to the obstacles distance from
                //the vehicle. 
                const float BrakingWeight = 0.2f;

                SteeringForce.X = (float)(ClosestIntersectingObstacle.BoundingRadius -
                                   LocalPosOfClosestObstacle.X) * BrakingWeight;
            }

            //finally, convert the steering vector from local to world space
            return MathUtils.VectorToWorldSpace(SteeringForce,
                                      actor.Direction,
                                      actor.Side);

        }

        /// <summary>
        /// Creates the antenna utilized by WallAvoidance
        /// </summary>
        void CreateFeelers()
        {
            //feeler pointing straight in front
            m_Feelers[0] = actor.Position + Vector2.Multiply(actor.Direction, (float)m_dWallDetectionFeelerLength);

            //feeler to left
            Vector2 temp = actor.Direction;
            MathUtils.Vec2DRotateAroundOrigin(ref temp, Constants.HALF_PI * 3.5f);
            m_Feelers[1] = actor.Position + Vector2.Multiply(temp, (float)(m_dWallDetectionFeelerLength / 2.0f));

            //feeler to right
            temp = actor.Direction;
            MathUtils.Vec2DRotateAroundOrigin(ref temp, Constants.HALF_PI * 0.5f);
            m_Feelers[2] = actor.Position + Vector2.Multiply(temp, (float)(m_dWallDetectionFeelerLength / 2.0f));
        }

        /// <summary>
        /// This returns a steering force that will keep the agent away from any
        /// walls it may encounter
        /// </summary>
        /// <param name="walls"></param>
        /// <returns></returns>
        public Vector2 WallAvoidance(int[,] WorldMap)
        {

            List<Vector2> wallsFrom;
            List<Vector2> wallsTo;
            // the vectors containg the walls surrounding de actor
            CreateWallsFromMap(WorldMap, out wallsFrom, out wallsTo);

            //the feelers are contained in a std::vector, m_Feelers
            CreateFeelers();

            double DistToThisIP = 0.0;
            double DistToClosestIP = Double.MaxValue;

            //this will hold an index into the vector of walls
            int ClosestWall = -1;

            Vector2 SteeringForce = Vector2.Zero;
            Vector2 point = Vector2.Zero;         //used for storing temporary info
            Vector2 ClosestPoint = Vector2.Zero;  //holds the closest intersection point

            //examine each feeler in turn
            for (uint flr = 0; flr < 1; ++flr)
            {
                //run through each wall checking for any intersection points
                for (int w = 0; w < wallsFrom.Count; ++w)
                {
                    if (LineIntersection2D(actor.Position,
                                           m_Feelers[flr],
                                           wallsFrom[w],
                                           wallsTo[w],
                                           ref DistToThisIP,
                                           ref point))
                    {
                        //is this the closest found so far? If so keep a record
                        if (DistToThisIP < DistToClosestIP)
                        {
                            DistToClosestIP = DistToThisIP;
                            ClosestWall = (int)w;
                            ClosestPoint = point;
                        }
                    }
                }//next wall


                //if an intersection point has been detected, calculate a force  
                //that will direct the agent away
                if (ClosestWall >= 0)
                {
                    //calculate by what distance the projected position of the agent
                    //will overshoot the wall
                    Vector2 OverShoot = m_Feelers[flr] - ClosestPoint;

                    //create a force in the oposite direction of the actor
                    Vector2 v2 = -actor.Direction;
                    v2.Normalize();
                    v2 = new Vector2(-v2.Y, v2.X);

                    SteeringForce = v2 * OverShoot.Length();
                }

            }
            return SteeringForce;
        }

        /// <summary>
        /// Create a series of vectors from the map to use it in the wall avoidance algorithm
        /// </summary>
        /// <param name="WorldMap"></param>
        /// <param name="wallsFrom"></param>
        /// <param name="wallsTo"></param>
        private void CreateWallsFromMap(int[,] WorldMap, out List<Vector2> wallsFrom, out List<Vector2> wallsTo)
        {
            int startX = 0;
            int endX = 0;
            int startY = 0;
            int endY = 0;
            startX = (int)actor.Position.X - 2;
            endX = (int)actor.Position.X + 2;

            startY = (int)actor.Position.Y - 2;
            endY = (int)actor.Position.Y + 2;

            if (startX < 0) startX = 0;
            if (startY < 0) startY = 0;
            if (endX > WorldMap.GetUpperBound(0)) endX = WorldMap.GetUpperBound(0);
            if (endY > WorldMap.GetUpperBound(1)) endY = WorldMap.GetUpperBound(1);

            wallsFrom = new List<Vector2>();
            wallsTo = new List<Vector2>();

            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    if ((WorldMap[i, j] != 0) && (WorldMap[i, j] != Constants.COLLISION_BLOCK))
                    {
                        Vector2 v = new Vector2(i, j);
                        wallsFrom.Add(v);
                        Vector2 w = new Vector2(i + 1, j);
                        wallsTo.Add(w);

                        v = new Vector2(i, j);
                        wallsFrom.Add(v);
                        w = new Vector2(i, j + 1);
                        wallsTo.Add(w);

                        v = new Vector2(i + 1, j);
                        wallsFrom.Add(v);
                        w = new Vector2(i + 1, j + 1);
                        wallsTo.Add(w);

                        v = new Vector2(i, j + 1);
                        wallsFrom.Add(v);
                        w = new Vector2(i + 1, j + 1);
                        wallsTo.Add(w);
                    }
                }
            }
        }

        /// <summary>
        /// This calculates a force repelling from the other neighbors
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        Vector2 Separation(Actor[] neighbors)
        {
            Vector2 SteeringForce = Vector2.Zero;

            for (uint a = 0; a < neighbors.Length; ++a)
            {
                //make sure this agent isn't included in the calculations and that
                //the agent being examined is close enough. ***also make sure it doesn't
                //include the evade target ***
                //if ((neighbors[a] != actor) && neighbors[a].IsTagged &&
                //  (neighbors[a] != m_pTargetAgent1))
                if (neighbors[a] != actor & neighbors[a] != null)
                {
                    if ((neighbors[a].IsActive) && (!neighbors[a].IsKilled))
                    {
                        Vector2 ToAgent = actor.Position - neighbors[a].Position;

                        //scale the force inversely proportional to the agents distance  
                        //from its neighbor.
                        if (ToAgent.Length() <= 0.25f)
                            SteeringForce += Vector2.Normalize(ToAgent) / ToAgent.Length();
                    }
                }
            }

            return SteeringForce;
        }

        /// <summary>
        /// returns a force that attempts to align this agents heading with that
        /// of its neighbors
        /// </summary>
        /// <param name="neighbors"></param>
        /// <returns></returns>
        Vector2 Alignment(Actor[] neighbors)
        {
            //used to record the average heading of the neighbors
            Vector2 AverageHeading = Vector2.Zero;

            //used to count the number of vehicles in the neighborhood
            int NeighborCount = 0;

            //iterate through all the tagged vehicles and sum their heading vectors  
            for (uint a = 0; a < neighbors.Length; ++a)
            {
                //make sure *this* agent isn't included in the calculations and that
                //the agent being examined  is close enough ***also make sure it doesn't
                //include any evade target ***
                if ((neighbors[a] != actor) && neighbors[a].IsTagged &&
                  (neighbors[a] != m_pTargetAgent1))
                {
                    AverageHeading += neighbors[a].Direction;

                    ++NeighborCount;
                }
            }

            //if the neighborhood contained one or more vehicles, average their
            //heading vectors.
            if (NeighborCount > 0)
            {
                AverageHeading = Vector2.Divide(AverageHeading, (float)NeighborCount);
                AverageHeading -= actor.Direction;
            }

            return AverageHeading;
        }

        /// <summary>
        /// returns a steering force that attempts to move the agent towards the
        /// center of mass of the agents in its immediate area
        /// </summary>
        /// <param name="neighbors"></param>
        /// <returns></returns>
        Vector2 Cohesion(Actor[] neighbors)
        {
            //first find the center of mass of all the agents
            Vector2 CenterOfMass = Vector2.Zero;
            Vector2 SteeringForce = Vector2.Zero;

            int NeighborCount = 0;

            //iterate through the neighbors and sum up all the position vectors
            for (uint a = 0; a < neighbors.Length; ++a)
            {
                //make sure *this* agent isn't included in the calculations and that
                //the agent being examined is close enough ***also make sure it doesn't
                //include the evade target ***
                if ((neighbors[a] != actor) && neighbors[a].IsTagged &&
                  (neighbors[a] != m_pTargetAgent1))
                {
                    CenterOfMass += neighbors[a].Position;

                    ++NeighborCount;
                }
            }

            if (NeighborCount > 0)
            {
                //the center of mass is the average of the sum of positions
                CenterOfMass = Vector2.Divide(CenterOfMass, (float)NeighborCount);

                //now seek towards that position
                SteeringForce = Seek(CenterOfMass);
            }

            //the magnitude of cohesion is usually much larger than separation or
            //allignment so it usually helps to normalize it.
            return Vector2.Normalize(SteeringForce);
        }

        /// <summary>
        /// Given two agents, this method returns a force that attempts to 
        /// position the vehicle between them
        /// </summary>
        /// <param name="AgentA"></param>
        /// <param name="AgentB"></param>
        /// <returns></returns>
        Vector2 Interpose(Actor AgentA, Actor AgentB)
        {
            //first we need to figure out where the two agents are going to be at 
            //time T in the future. This is approximated by determining the time
            //taken to reach the mid way point at the current time at at max speed.
            Vector2 MidPoint = Vector2.Divide((AgentA.Position + AgentB.Position), 2.0f);

            double TimeToReachMidPoint = Vector2.Distance(actor.Position, MidPoint) /
                                         actor.MaxSpeed;

            //now we have T, we assume that agent A and agent B will continue on a
            //straight trajectory and extrapolate to get their future positions
            Vector2 APos = AgentA.Position + Vector2.Multiply(AgentA.Velocity, (float)TimeToReachMidPoint);
            Vector2 BPos = AgentB.Position + Vector2.Multiply(AgentB.Velocity, (float)TimeToReachMidPoint);

            //calculate the mid point of these predicted positions
            MidPoint = Vector2.Divide((APos + BPos), 2.0f);

            //then steer to Arrive at it
            return Arrive(MidPoint, DecelerationType.Fast);
        }

        /// <summary>
        /// Hide
        /// </summary>
        /// <param name="hunter"></param>
        /// <param name="obstacles"></param>
        /// <returns></returns>
        Vector2 Hide(Actor hunter, BaseEntity[] obstacles)
        {
            double DistToClosest = Double.MaxValue;
            Vector2 BestHidingSpot = Vector2.Zero;

            foreach (BaseEntity curOb in obstacles)
            {
                //calculate the position of the hiding spot for this obstacle
                Vector2 HidingSpot = GetHidingPosition(curOb.Position,
                                                         curOb.BoundingRadius,
                                                          hunter.Position);

                //work in distance-squared space to find the closest hiding
                //spot to the agent

                double dist = Vector2.DistanceSquared(HidingSpot, actor.Position);

                if (dist < DistToClosest)
                {
                    DistToClosest = dist;
                    BestHidingSpot = HidingSpot;
                }

            }//end while

            //if no suitable obstacles found then Evade the hunter
            if (DistToClosest == float.MaxValue)
            {
                return Evade(hunter);
            }

            //else use Arrive on the hiding spot
            return Arrive(BestHidingSpot, DecelerationType.Fast);
        }

        /// <summary>
        /// Given the position of a hunter, and the position and radius of
        /// an obstacle, this method calculates a position DistanceFromBoundary 
        /// away from its bounding radius and directly opposite the hunter
        /// </summary>
        /// <param name="posOb"></param>
        /// <param name="radiusOb"></param>
        /// <param name="posHunter"></param>
        /// <returns></returns>
        Vector2 GetHidingPosition(Vector2 posOb, double radiusOb, Vector2 posHunter)
        {
            //calculate how far away the agent is to be from the chosen obstacle's
            //bounding radius
            const double DistanceFromBoundary = 30.0;
            double DistAway = radiusOb + DistanceFromBoundary;

            //calculate the heading toward the object from the hunter
            Vector2 ToOb = Vector2.Normalize(posOb - posHunter);

            //scale it to size and add to the obstacles position to get
            //the hiding spot.
            return (Vector2.Multiply(ToOb, (float)DistAway)) + posOb;
        }

        /// <summary>
        /// Given a series of Vector2Ds, this method produces a force that will
        /// move the agent along the waypoints in order. The agent uses the
        /// 'Seek' behavior to move to the next waypoint - unless it is the last
        /// waypoint, in which case it 'Arrives'
        /// </summary>
        /// <returns></returns>
        public Vector2 FollowPath()
        {
            //move to next target if close enough to current target (working in
            //distance squared space)
            Vector2 currentPosition = new Vector2(m_pPath[CurrentNodeInPath].X + Actor.CENTER_OFFSET_X, m_pPath[CurrentNodeInPath].Y + Actor.CENTER_OFFSET_Y);

            if (Vector2.DistanceSquared(currentPosition, actor.Position) <
               m_dWaypointSeekDistSq)
            {
                CurrentNodeInPath++;
                if (CurrentNodeInPath > m_pPath.Count)
                    CurrentNodeInPath = m_pPath.Count;
            }

            if (!(CurrentNodeInPath >= m_pPath.Count))
            {
                currentPosition = new Vector2(m_pPath[CurrentNodeInPath].X + Actor.CENTER_OFFSET_X, m_pPath[CurrentNodeInPath].Y + Actor.CENTER_OFFSET_Y);
                return Seek(currentPosition);
            }

            else
            {
                return Arrive(currentPosition, DecelerationType.Normal);
            }
            return Vector2.Zero;
        }

        /// <summary>
        /// Produces a steering force that keeps a vehicle at a specified offset
        /// from a leader vehicle
        /// </summary>
        /// <param name="leader"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Vector2 OffsetPursuit(Actor leader, Vector2 offset)
        {
            //calculate the offset's position in world space
            Vector2 WorldOffsetPos = MathUtils.PointToWorldSpace(offset,
                                                            leader.Direction,
                                                            leader.Side,
                                                            leader.Position);

            Vector2 ToOffset = WorldOffsetPos - actor.Position;

            //the lookahead time is propotional to the distance between the leader
            //and the pursuer; and is inversely proportional to the sum of both
            //agent's velocities
            double LookAheadTime = ToOffset.Length() /
                                  (actor.MaxSpeed + leader.Speed);

            //now Arrive at the predicted future position of the offset
            return Arrive(WorldOffsetPos + Vector2.Multiply(leader.Velocity, (float)LookAheadTime), DecelerationType.Fast);
        }

        #endregion
    }

}
