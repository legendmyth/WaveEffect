using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WaveEffect
{
    public class Vector
    {
        public int x { get; set; }
        public int y { get; set; }

        public Vector(int _x, int _y)
        {
            this.x = _x;
            this.y = _y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VectorC
    {
        public int x;
        public int y;
    }
}
