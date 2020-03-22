#region File Description
//-----------------------------------------------------------------------------
// BaseAIEntity.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Nexxt.Framework;
using Nexxt.Engine.AI;
#endregion

namespace Nexxt.Engine.Entities
{
    public class BaseEntity : GameObject
    {
        public StateMachine entityStateMachine;

        public bool IsTagged = false;

        public virtual void Tick()
        {
            entityStateMachine.Tick();
        }
    }
}
