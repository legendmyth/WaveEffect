using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WaveEffect
{
    public partial class FrmC : Form
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
        private IntPtr bitmapVector;
        System.Random random = null;

        private int bitmapHeight;
        private int bitmapWidth;
        public FrmC()
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
            this.bitmapHeight = bmpData.Height;
            this.bitmapWidth = bmpData.Width;
            bitmapVector = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Vector)) * bitmapHeight * bitmapWidth);

            this.calcThread = new Thread(new ThreadStart(Calc));
            this.calcThread.IsBackground = true;
            this.calcThread.Start();
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
                CalcMapTransform(this.bitmapHeight, this.bitmapWidth, this.bitmapVector, arrDst, sourceArray);
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 单个波纹线程
        /// </summary>
        /// <param name="wave"></param>
        public void WaveThreadMethod(Object waveObj)
        {
            WaveSource wave = (WaveSource)waveObj;
            SingleWaveCalc(wave, bitmapWidth, bitmapHeight, bitmapVector,30);
        }

        private void FrmMultiThread_MouseMove(object sender, MouseEventArgs e)
        {
            int tmp = random.Next(1, 10);
            if (tmp == 1 && e.X > bitmapArea.X && e.Y > bitmapArea.Y && e.X < bitmapArea.X + bitmapArea.Width && e.Y < bitmapArea.Y + bitmapArea.Height)
            {
                WaveSource wave = new WaveSource(e.X, e.Y, 10.0, 5.0, 0);
                Thread waveThread = new Thread(new ParameterizedThreadStart(WaveThreadMethod));
                waveThread.IsBackground = true;
                waveThread.Start(wave);
            }
        }

        private void FrmMultiThread_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X > bitmapArea.X && e.Y > bitmapArea.Y && e.X < bitmapArea.X + bitmapArea.Width && e.Y < bitmapArea.Y + bitmapArea.Height)
            {
                WaveSource wave = new WaveSource(e.X, e.Y, 10.0, 5.0, 0);                
                Thread waveThread = new Thread(new ParameterizedThreadStart(WaveThreadMethod));
                waveThread.IsBackground = true;
                waveThread.Start(wave);
            }
        }

        [DllImport("wave.dll")]
        extern static void CalcMapTransform(int height, int width, IntPtr vectorPtr, byte[] arrDst, byte[] arrSource);

        [DllImport("wave.dll")]
        extern static void SingleWaveCalc(WaveSource wave, int width, int height, IntPtr vectorPtr,int delay);
    }
}