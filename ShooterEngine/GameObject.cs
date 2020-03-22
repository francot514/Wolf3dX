#region File Description
//-----------------------------------------------------------------------------
// GameObject.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nexxt.Framework.HelperObjects;
using Nexxt.Common.Enums;
#endregion

namespace Nexxt.Framework
{
    public class GameObject
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsMovable { get; set; }
        public bool IsCollectable { get; set; }
        public Orientation Orientation { get; set; }

        public Vector2 Position { get; set; }

        public double BoundingRadius { get; set; }

        public Texture2D Texture { set; get; }

        public string PickupSound { get; set; }

        public XnaStringDictionary StateBag { get; set; }

        public Difficulty Difficulties { get; set; }

        public GameObject()
        {
            StateBag = new XnaStringDictionary();
        }
    }
}
