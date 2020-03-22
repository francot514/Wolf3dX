

using Microsoft.Xna.Framework.Graphics;

namespace Nexxt.Engine.GameObjects
{
    public struct DoorSlice
    {
        public double Depth { get; internal set; }
        public int Height { get; internal set; }
        public int TextureX { get; internal set; }
        public Texture2D Texture { get; internal set; }
    }
}
