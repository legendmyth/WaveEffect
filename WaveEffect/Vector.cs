using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WaveEffect
{
    [StructLayout(LayoutKind.Explicit)]
    public class Vector
    {
        [FieldOffset(0)]
        public int x;

        [FieldOffset(4)]
        public int y;

        public Vector(int _x, int _y)
        {
            this.x = _x;
            this.y = _y;
        }
    }
}
