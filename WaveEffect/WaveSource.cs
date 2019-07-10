using System;
using System.Collections.Generic;
using System.Text;

namespace WaveEffect
{
    public class WaveSource
    {
        public int x { get; set; }
        public int y { get; set; }
        public int p { get; set; }

        public double waveLength { get; set; }

        public double amplitude { get; set; }

        public WaveSource(int _x, int _y, double _waveLength, double _amplitude, int _p)
        {
            this.x = _x;
            this.y = _y;
            this.p = _p;
            this.waveLength = _waveLength;
            this.amplitude = _amplitude;
        }
    }

    public struct WaveSourceC
    {
        public int x;
        public int y;
        public int p;
        public double waveLength;
        public double amplitude;
    }
}
