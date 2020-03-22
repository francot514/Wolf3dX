#region File Description
//-----------------------------------------------------------------------------
// IMessageDisplay.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace Wolf3d.StateManagement
{
    /// <summary>
    /// Interface used to display notification messages when interesting events occur,
    /// for instance when gamers join or leave the network session. This interface
    /// is registered as a service, so any piece of code wanting to display a message
    /// can look it up from Game.Services, without needing to worry about how the
    /// message display is implemented. In this case, the MessageDisplayComponent
    /// class implement this IMessageDisplay service.
    /// </summary>
    interface IMessageDisplay : IDrawable, IUpdateable
    {
        void ShowMessage(string message, params object[] parameters);
    }
}
