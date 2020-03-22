using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Nexxt.Engine.AI.Algorithms
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PathFinderNodeFast
    {
        #region Variables Declaration
        public int F; // f = gone + heuristic
        public int G;
        public ushort PX; // Parent
        public ushort PY;
        public byte Status;
        #endregion
    }

    public struct PathFinderNode
    {
        #region Variables Declaration
        public int F;
        public int G;
        public int H;  // f = gone + heuristic
        public int X;
        public int Y;
        public int PX; // Parent
        public int PY;
        #endregion
    }
}
