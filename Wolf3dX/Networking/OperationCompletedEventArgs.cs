#region File Description
//-----------------------------------------------------------------------------
// OperationCompletedEventArgs.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
#endregion

namespace Wolf3d.StateManagement
{
    /// <summary>
    /// Custom EventArgs class used by the NetworkBusyScreen.OperationCompleted event.
    /// </summary>
    class OperationCompletedEventArgs : EventArgs
    {
        #region Properties


        /// <summary>
        /// Gets or sets the IAsyncResult associated with
        /// the network operation that has just completed.
        /// </summary>
        public IAsyncResult AsyncResult
        {
            get { return asyncResult; }
            set { asyncResult = value; }
        }

        IAsyncResult asyncResult;


        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new event arguments class.
        /// </summary>
        public OperationCompletedEventArgs(IAsyncResult asyncResult)
        {
            this.asyncResult = asyncResult;
        }


        #endregion
    }
}
