using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nexxt.Common.Enums
{
    public enum BehaviorType
    {
        None = 0x00000,
        Seek = 0x00002,
        Flee = 0x00004,
        Arrive = 0x00008,
        Wander = 0x00010,
        Cohesion = 0x00020,
        Separation = 0x00040,
        Allignment = 0x00080,
        ObstacleAvoidance = 0x00100,
        WallAvoidance = 0x00200,
        FollowPath = 0x00400,
        Pursuit = 0x00800,
        Evade = 0x01000,
        Interpose = 0x02000,
        Hide = 0x04000,
        Flock = 0x08000,
        OffsetPursuit = 0x10000,
    };
}