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
        AtomicInt waveCnt = new AtomicInt(0);
        /// <summary>
        /// 渲染线程
        /// </summary>
        private Thread renderThread;

        private Thread renderVectorThread;

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
            //Bitmap sourceMap = global::WaveEffect.Properties.Resources.img13;


            Bitmap sourceMap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
            using (Graphics graphics = Graphics.FromImage(sourceMap))
            {
                graphics.CopyFromScreen(this.Location.X, this.Location.Y, 0, 0, new Size(this.Width, this.Height));
                sourceMap.Save("1.png", ImageFormat.Png);
            }

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

            this.renderVectorThread = new Thread(new ThreadStart(DrawVector));
            this.renderVectorThread.IsBackground = true;
            this.renderVectorThread.Start();

            
        }

        /// <summary>
        /// 渲染线程
        /// </summary>
        private void Render()
        {
            while (true)
            {
                lock (locker)
                {
                    if (render)
                    {
                        Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                        System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);//curBitmap.PixelFormat
                        IntPtr ptr = bmpData.Scan0;
                        Marshal.Copy(arrDst, 0, ptr, arrDst.Length);
                        bitmap.UnlockBits(bmpData);
                        formGraphics.DrawImage(bitmap, 0, 0);
                    }

                }
                if (waveCnt.Get() < 1)
                {

                    for (int i = 0; i < bitmapHeight; i++)
                    {
                        for (int j = 0; j < bitmapWidth; j++)
                        {
                            Marshal.WriteInt32(this.bitmapVector, (i * bitmapWidth + j) * Marshal.SizeOf(typeof(Vector)), 0);
                            Marshal.WriteInt32(this.bitmapVector, (i * bitmapWidth + j) * Marshal.SizeOf(typeof(Vector)) + 4, 0);
                        }
                    }
                }
                Thread.Sleep(5);
            }
        }
        private object locker = new object();

        private void DrawVector()
        {
            while (true)
            {
                lock (locker)
                {
                    if (!render)
                    {
                        Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                        System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                        IntPtr ptr = bmpData.Scan0;
                        for (int i = 0; i < bitmapHeight; i++)
                        {
                            for (int j = 0; j < bitmapWidth; j++)
                            {
                                int x = Marshal.ReadInt32(bitmapVector, (i * bitmapWidth + j) * Marshal.SizeOf(typeof(Vector)));
                                int y = Marshal.ReadInt32(bitmapVector, (i * bitmapWidth + j) * Marshal.SizeOf(typeof(Vector)) + 4);
                                byte r = (byte)(x + 128);
                                byte g = (byte)(y + 128);
                                byte b = 128;
                                Marshal.WriteByte(ptr, (i * bitmapWidth + j) * 3, r);
                                Marshal.WriteByte(ptr, (i * bitmapWidth + j) * 3 + 1, g);
                                Marshal.WriteByte(ptr, (i * bitmapWidth + j) * 3 + 2, b);
                                //Marshal.WriteByte(ptr, (i * bitmapWidth + j) * 3 + 3, alpha);
                            }
                        }
                        bitmap.UnlockBits(bmpData);
                        formGraphics.DrawImage(bitmap, 0, 0);
                    }
                    
                }
                Thread.Sleep(5);
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
        public void WaveThreadMethod(Object waveObj)
        {
            WaveSource wave = (WaveSource)waveObj;
            waveCnt.GetAndAdd(1);
            SingleWaveCalc(wave, bitmapWidth, bitmapHeight, bitmapVector, 20, 3);
            waveCnt.GetAndAdd(-1);
        }

        private void FrmMultiThread_MouseMove(object sender, MouseEventArgs e)
        {
            int tmp = random.Next(1, 10);
            if (tmp == 1 && e.X > bitmapArea.X && e.Y > bitmapArea.Y && e.X < bitmapArea.X + bitmapArea.Width && e.Y < bitmapArea.Y + bitmapArea.Height)
            {
                WaveSource wave = new WaveSource(e.X, e.Y, 50.0f, 20.0f, 0);
                Thread waveThread = new Thread(new ParameterizedThreadStart(WaveThreadMethod));
                waveThread.IsBackground = true;
                waveThread.Start(wave);
            }
        }

        private void FrmMultiThread_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X > bitmapArea.X && e.Y > bitmapArea.Y && e.X < bitmapArea.X + bitmapArea.Width && e.Y < bitmapArea.Y + bitmapArea.Height)
            {
                WaveSource wave = new WaveSource(e.X, e.Y, 50.0f, 20f, 0);                
                Thread waveThread = new Thread(new ParameterizedThreadStart(WaveThreadMethod));
                waveThread.IsBackground = true;
                waveThread.Start(wave);
            }
        }

        [DllImport("wave.dll")]
        extern static void CalcMapTransform(int height, int width, IntPtr vectorPtr, byte[] arrDst, byte[] arrSource);

        [DllImport("wave.dll")]
        extern static void SingleWaveCalc(WaveSource wave, int width, int height, IntPtr vectorPtr,int delay,int speed);

        private bool render = true;

        private void FrmC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                render = !render;
            }
        }
    }
}