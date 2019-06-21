using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WaveEffect
{
    public partial class FrmClick : Form
    {
        private List<WaveSource> waves = new List<WaveSource>();

        private Thread renderThread;
        private Graphics formGraphics;

        private Bitmap bitmap;

        private Bitmap sourceMap;

        private byte[] arrDst;// = new byte[sourceArray.Length];

        private byte[] sourceArray;
        public FrmClick()
        {
            InitializeComponent();
            this.Shown += FrmMain_Shown;
        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            formGraphics = this.CreateGraphics();
            sourceMap = global::WaveEffect.Properties.Resources.img7;
            bitmap = new Bitmap(sourceMap.Width, sourceMap.Height);
            Rectangle rect = new Rectangle(0, 0, sourceMap.Width, sourceMap.Height);
            System.Drawing.Imaging.BitmapData bmpData = sourceMap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            int bytesCount = bmpData.Stride * bmpData.Height;
            sourceArray = new byte[bytesCount];
            Marshal.Copy(ptr, sourceArray, 0, bytesCount);
            sourceMap.UnlockBits(bmpData);

            arrDst = new byte[sourceArray.Length];
            Array.Copy(sourceArray, arrDst, sourceArray.Length);

            this.renderThread = new Thread(new ThreadStart(DrawBitmap));
            this.renderThread.IsBackground = true;
            this.renderThread.Start();

            //System.Timers.Timer timer = new System.Timers.Timer(30);
            //timer.AutoReset = true;
            //timer.Elapsed += Timer_Elapsed;
            //timer.Start();
        }

        private void DrawBitmap()
        {
            while (true)
            {
                lock (waves)
                {
                    for ( int i=0;i<waves.Count;i++)
                    {
                        WaveSource wave = waves[i];
                        Render(wave.x, wave.y, wave.p);
                        wave.p = wave.p + 10;
                        if (wave.p > Math.Sqrt(Math.Pow(bitmap.Width, 2) + Math.Pow(bitmap.Height, 2)))
                        {
                            waves.Remove(wave);
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }

        private void Render(int point_x, int point_y, int p_len)
        {
            double rate = 5;//bochang
            double ratio = 8;//zhenfu
            int width = bitmap.Width;
            int height = bitmap.Height;
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);//curBitmap.PixelFormat
            IntPtr ptr = bmpData.Scan0;
            Array.Copy(sourceArray, arrDst, sourceArray.Length);

            //int min = (int)(p_len - rate);
            //int max = (int)(p_len + rate);
            //for (double o = 0; o < 6.2831; o = o + 0.003)
            //{
            //    for (double p = min; p <= max; p = p + 0.1)
            //    {
            //        int x = (int)(p * Math.Cos(o)) + point_x;
            //        int y = (int)(p * Math.Sin(o)) + point_y;

            //        int pixx = (int)((p + ratio * Math.Sin(p / rate)) * Math.Cos(o)) + point_x;
            //        int pixy = (int)((p + ratio * Math.Sin(p / rate)) * Math.Sin(o)) + point_y;

            //        if (pixx > 0 && pixx < width && pixy > 0 && pixy < height && x > 0 && x < width && y > 0 && y < height)
            //        {
            //            arrDst[x * 3 + y * width * 3] = sourceArray[pixx * 3 + pixy * width * 3];
            //            arrDst[x * 3 + y * width * 3 + 1] = sourceArray[pixx * 3 + pixy * width * 3 + 1];
            //            arrDst[x * 3 + y * width * 3 + 2] = sourceArray[pixx * 3 + pixy * width * 3 + 2];

            //            //arrDst[x * 3 + y * width * 3] = 0;
            //            //arrDst[x * 3 + y * width * 3 + 1] = 0;
            //            //arrDst[x * 3 + y * width * 3 + 2] = 255;
            //        }
            //    }
            //}
            //
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    double p = Math.Sqrt(Math.Pow(i - point_x, 2) + Math.Pow(j - point_y, 2));
                    if (p >= p_len - 4 * rate && p <= p_len + 4 * rate)
                    {
                        int y = j - point_y;
                        int x = i - point_x;
                        if (x > 0 && x < width && y > 0 && y < height)
                        {
                            int tranx = (int)(x + x * ratio * Math.Sin(p / rate) / p);
                            int trany = (int)(y + y * ratio * Math.Sin(p / rate) / p);
                            int pixx = tranx + point_x;
                            int pixy = trany + point_y;

                            if (pixx > 0 && pixx < width && pixy > 0 && pixy < height)
                            {
                                arrDst[i * 3 + j * width * 3] = sourceArray[pixx * 3 + pixy * width * 3];
                                arrDst[i * 3 + j * width * 3 + 1] = sourceArray[pixx * 3 + pixy * width * 3 + 1];
                                arrDst[i * 3 + j * width * 3 + 2] = sourceArray[pixx * 3 + pixy * width * 3 + 2];
                            }
                        }
                    }
                }
            }
            Marshal.Copy(arrDst, 0, ptr, arrDst.Length);
            bitmap.UnlockBits(bmpData);
            formGraphics.DrawImage(bitmap, 0, 0);
        }

        private void FrmClick_Load(object sender, EventArgs e)
        {

        }

        private void FrmClick_MouseDown(object sender, MouseEventArgs e)
        {
            lock (waves)
            {
                waves.Add(new WaveSource(e.X, e.Y, 0));
            }
        }
    }

    public class WaveSource
    {
        public int x { get; set; }
        public int y { get; set; }
        public int p { get; set; }

        public WaveSource(int _x,int _y,int _p)
        {
            this.x = _x;
            this.y = _y;
            this.p = _p;
        }
    }
}
