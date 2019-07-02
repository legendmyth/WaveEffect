using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WaveEffect
{
    public partial class FrmInterference : Form
    {
        private List<WaveSource> waves = new List<WaveSource>();

        private Thread renderThread;
        private Graphics formGraphics;
        private Bitmap bitmap;
        private Bitmap sourceMap;
        private byte[] arrDst;
        private byte[] arrTmp;
        private byte[] sourceArray;
        private Rectangle bitmapArea;
        System.Random random = null;
        public FrmInterference()
        {
            InitializeComponent();
        }

        private void FrmInterference_Shown(object sender, EventArgs e)
        {
            random = new Random(DateTime.Now.Millisecond);
            formGraphics = this.CreateGraphics();
            sourceMap = global::WaveEffect.Properties.Resources.img13;
            bitmap = new Bitmap(sourceMap.Width, sourceMap.Height);
            Rectangle rect = new Rectangle(0, 0, sourceMap.Width, sourceMap.Height);
            System.Drawing.Imaging.BitmapData bmpData = sourceMap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            int bytesCount = bmpData.Stride * bmpData.Height;
            sourceArray = new byte[bytesCount];
            Marshal.Copy(ptr, sourceArray, 0, bytesCount);
            sourceMap.UnlockBits(bmpData);
            arrDst = new byte[sourceArray.Length];
            arrTmp = new byte[sourceArray.Length];
            Array.Copy(sourceArray, arrDst, sourceArray.Length);
            Array.Copy(sourceArray, arrTmp, sourceArray.Length);
            System.Drawing.Imaging.BitmapData bmpData1 = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);//curBitmap.PixelFormat
            IntPtr ptr1 = bmpData1.Scan0;
            Marshal.Copy(sourceArray, 0, ptr1, arrDst.Length);
            bitmap.UnlockBits(bmpData1);
            bitmapArea = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            this.renderThread = new Thread(new ThreadStart(DrawBitmap));
            this.renderThread.IsBackground = true;
            this.renderThread.Start();
        }
        private void DrawBitmap()
        {
            while (true)
            {
                DateTime dt = DateTime.Now;
                Render(waves);
                DateTime time = DateTime.Now;
                TimeSpan timeSpan = time.Subtract(dt);
                //Console.WriteLine("Render耗时：" + timeSpan.TotalMilliseconds);

                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);//curBitmap.PixelFormat
                IntPtr ptr = bmpData.Scan0;
                Marshal.Copy(arrDst, 0, ptr, arrDst.Length);
                bitmap.UnlockBits(bmpData);
                formGraphics.DrawImage(bitmap, 0, 0);

                //Console.WriteLine("绘图耗时：" + DateTime.Now.Subtract(time).TotalMilliseconds);

                for (int i = 0; i < waves.Count; i++)
                {
                    WaveSource wave = waves[i];
                    wave.p = (int)(wave.p + wave.waveLength * 0.5);
                    double p1 = Math.Sqrt((bitmap.Width - wave.x)* (bitmap.Width - wave.x) + (bitmap.Height - wave.y)* (bitmap.Height - wave.y));
                    double p2 = Math.Sqrt((bitmap.Width - wave.x)* (bitmap.Width - wave.x) + wave.y* wave.y);
                    double p3 = Math.Sqrt(wave.x* wave.x + (bitmap.Height - wave.y)*(bitmap.Height - wave.y));
                    double p4 = Math.Sqrt(wave.x * wave.x + wave.y * wave.y);
                    if (wave.p > p1 && wave.p > p2 && wave.p > p3 && wave.p > p4)
                    {
                        waves.Remove(wave);
                    }
                }

                int times = (int)DateTime.Now.Subtract(dt).TotalMilliseconds > 30 ? 1 : 30 - (int)DateTime.Now.Subtract(dt).TotalMilliseconds;
                Thread.Sleep(times);
            }
        }


        private void Render(List<WaveSource> waves)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            DateTime t1 = DateTime.Now;
            Array.Copy(sourceArray, arrDst, sourceArray.Length);
            DateTime t2 = DateTime.Now;
            for (int k = 0; k < waves.Count; k++)
            {
                WaveSource wave = waves[k];
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        double p = Math.Sqrt((i - wave.x) * (i - wave.x) + (j - wave.y) * (j - wave.y));
                        double min = wave.p - wave.waveLength * 0.5;
                        double max = wave.p + wave.waveLength * 0.5;
                        if (p >= min && p <= max)
                        {
                            int pixx = (int)((1 + wave.amplitude * (1 + Math.Sin(p / wave.waveLength)) / p) * (i - wave.x) + wave.x);
                            int pixy = (int)((1 + wave.amplitude * (1 + Math.Sin(p / wave.waveLength)) / p) * (j - wave.y) + wave.y);
                            //int pixx = (int)(p * (i - wave.x) / (p + wave.amplitude * Math.Sin(p / wave.waveLength)) + wave.x);
                            //int pixy = (int)(p * (j - wave.y) / (p + wave.amplitude * Math.Sin(p / wave.waveLength)) + wave.y);
                            if (pixx > 0 && pixx < width && pixy > 0 && pixy < height)
                            {
                                arrDst[i * 3 + j * width * 3] = arrTmp[pixx * 3 + pixy * width * 3];
                                arrDst[i * 3 + j * width * 3 + 1] = arrTmp[pixx * 3 + pixy * width * 3 + 1];
                                arrDst[i * 3 + j * width * 3 + 2] = arrTmp[pixx * 3 + pixy * width * 3 + 2];
                            }

                        }
                    }
                }
                Array.Copy(arrDst, arrTmp, arrDst.Length);
            }
            DateTime t3 = DateTime.Now;
            //Console.WriteLine("t2-t1:" + t2.Subtract(t1).TotalMilliseconds + "||t3-t2:" + t3.Subtract(t2).TotalMilliseconds);

        }

        private void FrmInterference_MouseMove(object sender, MouseEventArgs e)
        {
            int tmp = random.Next(1, 10);
            if (tmp == 1 && e.X > bitmapArea.X && e.Y > bitmapArea.Y && e.X < bitmapArea.X + bitmapArea.Width && e.Y < bitmapArea.Y + bitmapArea.Height)
            {
                lock (waves)
                {
                    waves.Add(new WaveSource(e.X, e.Y, 10, 5, 0));
                }
            }
        }

        private void FrmInterference_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X > bitmapArea.X && e.Y > bitmapArea.Y && e.X < bitmapArea.X + bitmapArea.Width && e.Y < bitmapArea.Y + bitmapArea.Height)
            {

                lock (waves)
                {
                    waves.Add(new WaveSource(e.X, e.Y, 10, 5, 0));
                }
            }
        }
    }
}
