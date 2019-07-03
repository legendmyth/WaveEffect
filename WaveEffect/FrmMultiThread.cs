using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WaveEffect
{
    public partial class FrmMultiThread : Form
    {
        private List<WaveSource> waves = new List<WaveSource>();

        /// <summary>
        /// 渲染线程
        /// </summary>
        private Thread renderThread;

        /// <summary>
        /// 向量转换线程
        /// </summary>
        private Thread calcThread;

        private Graphics formGraphics;
        private Bitmap bitmap;
        private Rectangle bitmapArea;

        private byte[] arrDst;
        private byte[] sourceArray;
        private Vector[,] bitmapVector;
        System.Random random = null;

        private int bitmapHeight;
        private int bitmapWidth;
        public FrmMultiThread()
        {
            InitializeComponent();
        }

        private void FrmMultiThread_Shown(object sender, EventArgs e)
        {
            random = new Random(DateTime.Now.Millisecond);
            formGraphics = this.CreateGraphics();
            Bitmap sourceMap = global::WaveEffect.Properties.Resources.img13;
            

            bitmap = new Bitmap(sourceMap.Width, sourceMap.Height);

            Rectangle rect = new Rectangle(0, 0, sourceMap.Width, sourceMap.Height);
            System.Drawing.Imaging.BitmapData bmpData = sourceMap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            sourceArray = new byte[bmpData.Stride * bmpData.Height];
            Marshal.Copy(ptr, sourceArray, 0, bmpData.Stride * bmpData.Height);
            sourceMap.UnlockBits(bmpData);

            arrDst = new byte[sourceArray.Length];
            Array.Copy(sourceArray, arrDst, sourceArray.Length);

            bitmapArea = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            this.renderThread = new Thread(new ThreadStart(Render));
            this.renderThread.IsBackground = true;
            this.renderThread.Start();

            this.calcThread = new Thread(new ThreadStart(Calc));
            this.calcThread.IsBackground = true;
            this.calcThread.Start();

            bitmapVector = new Vector[bmpData.Height, bmpData.Width];
            for (int i = 0; i < bmpData.Height; i++)
            {
                for (int j = 0; j < bmpData.Width; j++)
                {
                    bitmapVector[i, j] = new Vector(0, 0);
                }
            }
            this.bitmapHeight = bmpData.Height;
            this.bitmapWidth = bmpData.Width;
        }

        /// <summary>
        /// 渲染线程
        /// </summary>
        private void Render()
        {
            while (true)
            {
                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);//curBitmap.PixelFormat
                IntPtr ptr = bmpData.Scan0;
                Marshal.Copy(arrDst, 0, ptr, arrDst.Length);
                bitmap.UnlockBits(bmpData);
                formGraphics.DrawImage(bitmap, 0, 0);

                Thread.Sleep(30);
            }
        }


        /// <summary>
        /// 向量转换线程，将叠加后的向量数据叠加到原有的图像上，计算出转换后的像素数据
        /// </summary>
        private void Calc()
        {
            while (true)
            {
                DateTime t1 = DateTime.Now;
                for (int j = 0; j < bitmapHeight; j++)
                {
                    for (int i = 0; i < bitmapWidth; i++)
                    {
                        int tranx = i + this.bitmapVector[j, i].x;
                        int trany = j + this.bitmapVector[j, i].y;
                        if (tranx >= 0 && tranx < bitmapWidth && trany >= 0 && trany < bitmapHeight)
                        {
                            arrDst[i * 3 + j * bitmapWidth * 3] = this.sourceArray[tranx * 3 + trany * bitmapWidth * 3];
                            arrDst[i * 3 + j * bitmapWidth * 3 + 1] = this.sourceArray[tranx * 3 + trany * bitmapWidth * 3 + 1];
                            arrDst[i * 3 + j * bitmapWidth * 3 + 2] = this.sourceArray[tranx * 3 + trany * bitmapWidth * 3 + 2];
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 单个波纹线程
        /// </summary>
        /// <param name="wave"></param>
        public void WaveThreadMethod(Object waveObj)
        {
            WaveSource wave = waveObj as WaveSource;
            double p1 = Math.Sqrt((bitmapWidth - wave.x) * (bitmapWidth - wave.x) + (bitmapHeight - wave.y) * (bitmapHeight - wave.y));
            double p2 = Math.Sqrt((bitmapWidth - wave.x) * (bitmapWidth - wave.x) + wave.y * wave.y);
            double p3 = Math.Sqrt(wave.x * wave.x + (bitmapHeight - wave.y) * (bitmapHeight - wave.y));
            double p4 = Math.Sqrt(wave.x * wave.x + wave.y * wave.y);
            while (wave.p < p1 || wave.p < p2 || wave.p < p3 || wave.p < p4)
            {
                for (int j = 0; j < bitmapHeight; j++)
                {
                    for (int i = 0; i < bitmapWidth; i++)
                    {
                        double p = (i - wave.x) * (i - wave.x) + (j - wave.y) * (j - wave.y);
                        double min = (wave.p - wave.waveLength)* (wave.p - wave.waveLength);
                        double max = (wave.p + wave.waveLength)* (wave.p + wave.waveLength);
                        if (p >= min && p <= max)
                        {
                            lock (this.bitmapVector[j, i])
                            {
                                double p0 = Math.Sqrt(p);
                                this.bitmapVector[j, i].x = this.bitmapVector[j, i].x + (int)(wave.amplitude * Math.Sin(p0 / wave.waveLength) * (i - wave.x) / p0);
                                this.bitmapVector[j, i].y = this.bitmapVector[j, i].y + (int)(wave.amplitude * Math.Sin(p0 / wave.waveLength) * (j - wave.y) / p0);
                            }
                        }
                    }
                }
                Thread.Sleep(30);
                for (int j = 0; j < bitmapHeight; j++)
                {
                    for (int i = 0; i < bitmapWidth; i++)
                    {
                        double p = (i - wave.x) * (i - wave.x) + (j - wave.y) * (j - wave.y);
                        double min = (wave.p - wave.waveLength) * (wave.p - wave.waveLength);
                        double max = (wave.p + wave.waveLength) * (wave.p + wave.waveLength);
                        if (p >= min && p <= max)
                        {
                            lock (this.bitmapVector[j, i])
                            {
                                double p0 = Math.Sqrt(p);
                                this.bitmapVector[j, i].x = this.bitmapVector[j, i].x - (int)(wave.amplitude * Math.Sin(p0 / wave.waveLength) * (i - wave.x) / p0);
                                this.bitmapVector[j, i].y = this.bitmapVector[j, i].y - (int)(wave.amplitude * Math.Sin(p0 / wave.waveLength) * (j - wave.y) / p0);
                            }
                        }
                    }
                }
                wave.p = (int)(wave.p + wave.waveLength * 0.5);
                
            }
        }

        private void FrmMultiThread_MouseMove(object sender, MouseEventArgs e)
        {
            int tmp = random.Next(1, 10);
            if (tmp == 1 && e.X > bitmapArea.X && e.Y > bitmapArea.Y && e.X < bitmapArea.X + bitmapArea.Width && e.Y < bitmapArea.Y + bitmapArea.Height)
            {
                WaveSource wave = new WaveSource(e.X, e.Y, 10, 5, 0);
                Thread waveThread = new Thread(new ParameterizedThreadStart(WaveThreadMethod));
                waveThread.IsBackground = true;
                waveThread.Start(wave);
            }
        }

        private void FrmMultiThread_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X > bitmapArea.X && e.Y > bitmapArea.Y && e.X < bitmapArea.X + bitmapArea.Width && e.Y < bitmapArea.Y + bitmapArea.Height)
            {
                WaveSource wave = new WaveSource(e.X, e.Y, 10, 5, 0);
                Thread waveThread = new Thread(new ParameterizedThreadStart(WaveThreadMethod));
                waveThread.IsBackground = true;
                waveThread.Start(wave);
            }
        }
    }

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
}
