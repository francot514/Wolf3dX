#region File Description
//-----------------------------------------------------------------------------
// Difficulty.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace Nexxt.Common.Enums
{
    /// <summary>
    /// Defines the allowed difficulties for the player in the game
    /// </summary>
    [Flags]
    public enum Difficulty
    {
        None = 0,    // Can I Play Daddy?
        Easy = 1,    // Don't Hurt me
        Medium = 2,  // Bring 'em on!
        Hard = 3     // I'm a Death incarnate! 
    }
}
