using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WaveEffect
{
    [StructLayout(LayoutKind.Explicit)]
    public class WaveSource
    {
        [FieldOffset(0)]
        public int x;

        [FieldOffset(4)]
        public int y;

        [FieldOffset(8)]
        public int p;

        [FieldOffset(12)]//为了兼容C语言内存对齐
        public float waveLength;

        [FieldOffset(16)]
        public float amplitude;

        public WaveSource(int _x, int _y, float _waveLength, float _amplitude, int _p)
        {
            this.x = _x;
            this.y = _y;
            this.p = _p;
            this.waveLength = _waveLength;
            this.amplitude = _amplitude;
        }
    }
}
