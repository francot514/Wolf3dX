using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nexxt.Common.Enums
{
    /// <summary>
    /// Defines the Door Types
    /// </summary>
    [Flags]
    public enum DoorType:int
    {
        Normal = 250,      // Normal Door
        Gold_Key = 251,    // Door that requires a Gold Key 
        Silver_Key = 252,  // Door that requires a Silver Key
        Exit = 253,        // Exit level door
        Secret = 254       // Secret (Push - door)
    }
}
