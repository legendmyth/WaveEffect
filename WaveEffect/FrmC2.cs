using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WaveEffect
{
    public partial class FrmC2 : Form
    {
        private IntPtr waves;

        private int maxWaveSize = 100;

        /// <summary>
        /// 渲染线程
        /// </summary>
        private Thread renderThread;

        /// <summary>
        /// 向量转换线程
        /// </summary>
        private Thread calcThread;

        /// <summary>
        /// 水波计算线程
        /// </summary>
        private Thread waveThread;

        private Graphics formGraphics;
        private Bitmap bitmap;
        private Rectangle bitmapArea;

        private byte[] arrDst;
        private byte[] sourceArray;
        private IntPtr bitmapVector;
        System.Random random = null;

        private int bitmapHeight;
        private int bitmapWidth;
        public FrmC2()
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

            waves = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WaveSource)) * maxWaveSize);
            InitWaves(waves, maxWaveSize);
            this.waveThread = new Thread(new ThreadStart(MultiWaveThreadMethod));
            this.waveThread.IsBackground = true;
            this.waveThread.Start();
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
                Thread.Sleep(16);
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
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 单个波纹线程
        /// </summary>
        /// <param name="wave"></param>
        public void MultiWaveThreadMethod()
        {
            MultiWaveCalc(waves, bitmapWidth, bitmapHeight, bitmapVector, 20, maxWaveSize, 10);
        }

        private void FrmMultiThread_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X > bitmapArea.X && e.Y > bitmapArea.Y && e.X < bitmapArea.X + bitmapArea.Width && e.Y < bitmapArea.Y + bitmapArea.Height)
            {
                WaveSource waveSource = new WaveSource(e.X, e.Y, 10.0f, 5.0f, 0);
                AddWave(this.waves, maxWaveSize, waveSource);
            }
        }
        private void FrmC2_MouseMove(object sender, MouseEventArgs e)
        {
            FrmMultiThread_MouseClick(sender,e);
        }

        [DllImport("wave.dll")]
        extern static void CalcMapTransform(int height, int width, IntPtr vectorPtr, byte[] arrDst, byte[] arrSource);

        [DllImport("wave.dll")]
        extern static void SingleWaveCalc(WaveSource wave, int width, int height, IntPtr vectorPtr,int delay);

        [DllImport("wave.dll")]
        extern static void MultiWaveCalc(IntPtr waves, int width, int height, IntPtr vectorPtr,int waveSpeed,int waveCnt, int delay);

        [DllImport("wave.dll")]
        extern static void AddWave(IntPtr waves,int waveCnt, WaveSource waveSource);

        [DllImport("wave.dll")]
        extern static void InitWaves(IntPtr waves, int waveCnt);

        [DllImport("wave.dll")]
        extern static void HelloWorld();

        private void button1_Click(object sender, EventArgs e)
        {
            HelloWorld();
        }


    }
}