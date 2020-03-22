#region File Description
//-----------------------------------------------------------------------------
// StateMachine.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
#endregion

namespace Nexxt.Engine.AI
{
    public class StateMachine
    {
        Dictionary<string, StateInfo> states = new Dictionary<string, StateInfo>();
        bool ticking = false;
        StateInfo currentStateInfo = null;
        string currentStateName = null;

        public void AddState(string stateName, StateDelegate begin, StateDelegate tick, StateDelegate end)
        {
            StateInfo info = new StateInfo();
            info.OnBegin = begin;
            info.OnTick = tick;
            info.OnEnd = end;

            states.Add(stateName, info);

            // If no states have been set yet, this state becomes the initial state.  
            if (currentStateName == null)
                State = stateName;
        }

        public string State
        {
            get { return currentStateName; }
            set
            {
                // End the previous state.  
                if (currentStateName != null)
                    currentStateInfo.End();

                // Set the new state  
                currentStateName = value;
                currentStateInfo = states[currentStateName];

                // Initialize it.  
                currentStateInfo.Begin();

                // If we're in the middle of the Tick function, go ahead and tick the new  
                // state now, as well.  
                if (ticking)
                    currentStateInfo.Tick();
            }
        }

        public void Tick()
        {
            // Set ticking so that we know that we're in the middle of ticking this machine.  
            // That way, if the state changes in the middle of this, we know to run the next  
            // state's Tick as well.  
            ticking = true;
            currentStateInfo.Tick();
            ticking = false;
        }
    }
}
