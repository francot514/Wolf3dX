#region File Description
//-----------------------------------------------------------------------------
// NetworkErrorScreen.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
#endregion

namespace Wolf3d.StateManagement
{
    /// <summary>
    /// Specialized message box subclass, used to display network error messages.
    /// </summary>
    class NetworkErrorScreen : MessageBoxScreen
    {
        #region Initialization


        /// <summary>
        /// Constructs an error message box from the specified exception.
        /// </summary>
        public NetworkErrorScreen(Exception exception)
            : base(GetErrorMessage(exception), false)
        { }


        /// <summary>
        /// Converts a network exception into a user friendly error message.
        /// </summary>
        static string GetErrorMessage(Exception exception)
        {
            Trace.WriteLine("Network operation threw " + exception);

            // Is this a GamerPrivilegeException?
            if (exception is GamerPrivilegeException)
            {
                if (Guide.IsTrialMode)
                    return "Trial Mode";
                else
                    return "Full Mode";
            }

            // Is it a NetworkSessionJoinException?
            NetworkSessionJoinException joinException = exception as
                                                            NetworkSessionJoinException;

            if (joinException != null)
            {
                switch (joinException.JoinError)
                {
                    case NetworkSessionJoinError.SessionFull:
                        return "Session Full";

                    case NetworkSessionJoinError.SessionNotFound:
                        return "Session Not Found";

                    case NetworkSessionJoinError.SessionNotJoinable:
                        return "Session Cant Join";
                }
            }

            // Is this a NetworkNotAvailableException?
            if (exception is NetworkNotAvailableException)
            {
                return "Network Not Avaliable";
            }

            // Is this a NetworkException?
            if (exception is NetworkException)
            {
                return "Network Not Avaliable";
            }

            // Otherwise just a generic error message.
            return "Network Not Avaliable";
        }


        #endregion
    }
}
