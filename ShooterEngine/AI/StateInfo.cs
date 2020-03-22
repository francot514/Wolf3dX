#region File Description
//-----------------------------------------------------------------------------
// StateInfo.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
#endregion

namespace Nexxt.Engine.AI
{
    public delegate void StateDelegate();

    public class StateInfo
    {

        public StateDelegate OnBegin { get; set; }
        public StateDelegate OnTick { get; set; }
        public StateDelegate OnEnd { get; set; }

        public void Begin()
        {
            if (OnBegin != null)
                OnBegin();
        }

        public void Tick()
        {
            if (OnTick != null)
                OnTick();
        }

        public void End()
        {
            if (OnEnd != null)
                OnEnd();
        }

        public StateInfo()
        {
            OnBegin = null;
            OnTick = null;
            OnEnd = null;
        }  
    }
}
