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

        private byte[] arrTmp;

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
            arrTmp = new byte[sourceArray.Length];

            Array.Copy(sourceArray, arrDst, sourceArray.Length);
            Array.Copy(sourceArray, arrTmp, sourceArray.Length);

            System.Drawing.Imaging.BitmapData bmpData1 = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);//curBitmap.PixelFormat
            IntPtr ptr1 = bmpData1.Scan0;
            Marshal.Copy(sourceArray, 0, ptr1, arrDst.Length);
            bitmap.UnlockBits(bmpData1);

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
                    for (int i = 0; i < waves.Count; i++)
                    {
                        WaveSource wave = waves[i];
                        Render(wave.x, wave.y, wave.waveLength, wave.amplitude, wave.p);
                        formGraphics.DrawImage(bitmap, 0, 0);
                        wave.p = (int)(wave.p + wave.waveLength);
                        double p1 = Math.Sqrt(Math.Pow(bitmap.Width - wave.x, 2) + Math.Pow(bitmap.Height - wave.y, 2));
                        double p2= Math.Sqrt(Math.Pow(bitmap.Width - wave.x, 2) + Math.Pow(0 - wave.y, 2));
                        double p3 = Math.Sqrt(Math.Pow(0 - wave.x, 2) + Math.Pow(bitmap.Height - wave.y, 2));
                        double p4 = Math.Sqrt(Math.Pow(0 - wave.x, 2) + Math.Pow(0 - wave.y, 2));
                        if (wave.p > p1 && wave.p > p2 && wave.p > p3 && wave.p > p4)
                        {
                            waves.Remove(wave);
                        }
                        //Thread.Sleep(200);
                    }
                    formGraphics.DrawImage(bitmap, 0, 0);

                }
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 渲染
        /// </summary>
        /// <param name="point_x">中心点坐标X</param>
        /// <param name="point_y">中心点坐标Y</param>
        /// <param name="p_len">距离中心点长度</param>
        /// <param name="waveLength">波长</param>
        /// <param name="amplitude">振幅</param>
        private void Render(int point_x, int point_y, double waveLength, double amplitude, int p_len)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            Array.Copy(sourceArray, arrDst, sourceArray.Length);

            #region jizuobiao 
            //Marshal.Copy(ptr, arrDst, 0, arrDst.Length);
            //Marshal.Copy(ptr, arrDst, 0, arrDst.Length);
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
            #endregion

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    double p = Math.Sqrt(Math.Pow(i - point_x, 2) + Math.Pow(j - point_y, 2));
                    if (p >= p_len - waveLength && p <= p_len + waveLength)
                    {
                        #region zhuanhuan
                        //int y = j - point_y;
                        //int x = i - point_x;
                        //int tranx = (int)(x + x * amplitude * Math.Sin(p / waveLength) / p);
                        //int trany = (int)(y + y * amplitude * Math.Sin(p / waveLength) / p);
                        //int pixx = tranx + point_x;
                        //int pixy = trany + point_y;

                        //int pixx = (int)((1 + amplitude * Math.Sin(p / waveLength) / p) * (i - point_x) + point_x);
                        //int pixy = (int)((1 + amplitude * Math.Sin(p / waveLength) / p) * (j - point_y) + point_y);
                        #endregion


                        int pixx = (int)(p * (i - point_x) / (p + amplitude * Math.Sin(p / waveLength)) + point_x);
                        int pixy = (int)(p * (j - point_y) / (p + amplitude * Math.Sin(p / waveLength)) + point_y);

                        if (pixx > 0 && pixx < width && pixy > 0 && pixy < height)
                        {
                            arrDst[i * 3 + j * width * 3] = arrTmp[pixx * 3 + pixy * width * 3];
                            arrDst[i * 3 + j * width * 3 + 1] = arrTmp[pixx * 3 + pixy * width * 3 + 1];
                            arrDst[i * 3 + j * width * 3 + 2] = arrTmp[pixx * 3 + pixy * width * 3 + 2];
                        }

                    }
                }
            }
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);//curBitmap.PixelFormat
            IntPtr ptr = bmpData.Scan0;
            //bmpData.
            Marshal.Copy(arrDst, 0, ptr, arrDst.Length);
            Marshal.Copy(ptr, arrTmp, 0, arrDst.Length);
            bitmap.UnlockBits(bmpData);
            //formGraphics.DrawImage(bitmap, 0, 0);

        }

        private void FrmClick_Load(object sender, EventArgs e)
        {

        }

        private void FrmClick_MouseDown(object sender, MouseEventArgs e)
        {
            lock (waves)
            {
                waves.Add(new WaveSource(e.X, e.Y, 5.0, 8.0, 0));
            }
        }
    }

    
}
