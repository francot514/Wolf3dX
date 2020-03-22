#region File Description
//-----------------------------------------------------------------------------
// GameObjectContent.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.IO;
using System.Collections.Specialized;
using Nexxt.Framework.HelperObjects;
using Nexxt.Framework;
using Nexxt.Common.Enums; 
#endregion

namespace Nexxt.Pipeline.GameObjectContentProcessor
{
    public class GameObjectContent
    {
        /// <summary>
        /// Provides access to the building context to get Textures and other assets built
        /// this is just in compile time
        /// </summary>
        public ContentProcessorContext Context { get; set; }

        #region Serializable Fields

        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsMovable { get; set; }
        public bool IsCollectable { get; set; }
        public string Orientation { get; set; }
        public Vector2 Position { get; set; }
        public ExternalReference<TextureContent> Texture { protected set; get; }
        public XnaStringDictionary StateBag { get; protected set; }
        public string PickupSound { get; set; }
        public Difficulty Difficulties { get; set; }

        #endregion

        #region Helper Fields

        public string PositionCoords 
        {
            protected get { return Position.ToString(); }
            set
            {
                string[] values = value.Split(',');
                if(values.Length == 2)
                {
                    //Valid coordinates passed
                    float x, y;
                    if (float.TryParse(values[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out x))
                    {
                        if (float.TryParse(values[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out y))
                        {
                            Position = new Vector2(x, y);
                        }
                        else
                            Position = Vector2.Zero;
                    }
                    else
                        Position = Vector2.Zero;
                }
                else
                {
                    //disable the component
                    IsActive=false;
                    Position = Vector2.Zero;
                }
            }
        }

        /// <summary>
        /// Modify to support SpriteSheets
        /// </summary>
        public string TextureName
        {
            get
            {
                return Texture.Name;
            }
            set
            {
                Texture = Context.BuildAsset<TextureContent, TextureContent>(
                        new ExternalReference<TextureContent>(
                            Path.Combine(Directory.GetCurrentDirectory(), "Objects\\" + value)),
                        "TextureProcessor");

            }
        }

        public string AvailableDifficulties
        {
            get
            {
                return Difficulties.ToString();
            }
            set
            {
                string[] difficulties = value.Split(',');
                if (difficulties.Length > 0)
                {
                    for (int i = 0; i < difficulties.Length; i++)
                    {
                        if (Enum.IsDefined(typeof(Difficulty), difficulties[i]))
                        {
                            Difficulties = Difficulties | (Difficulty)Enum.Parse(typeof(Difficulty), difficulties[i]);
                        }
                    }
                }
                else
                    Difficulties = Difficulty.None;
            }
        }

        #endregion

        public GameObjectContent()
        {
            StateBag = new XnaStringDictionary();
            PickupSound = "";
            Difficulties = Difficulty.None;
        }
    }
}
