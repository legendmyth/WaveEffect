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
        public FrmInterference()
        {
            InitializeComponent();
        }

        private void FrmInterference_Shown(object sender, EventArgs e)
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
        }
        private void DrawBitmap()
        {
            while (true)
            {
                
                Render(waves);
                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);//curBitmap.PixelFormat
                IntPtr ptr = bmpData.Scan0;
                Marshal.Copy(arrDst, 0, ptr, arrDst.Length);
                bitmap.UnlockBits(bmpData);
                formGraphics.DrawImage(bitmap, 0, 0);

                for (int i = 0; i < waves.Count; i++)
                {
                    WaveSource wave = waves[i];
                    wave.p = (int)(wave.p + wave.waveLength * 0.5);
                    double p1 = Math.Sqrt(Math.Pow(bitmap.Width - wave.x, 2) + Math.Pow(bitmap.Height - wave.y, 2));
                    double p2 = Math.Sqrt(Math.Pow(bitmap.Width - wave.x, 2) + Math.Pow(0 - wave.y, 2));
                    double p3 = Math.Sqrt(Math.Pow(0 - wave.x, 2) + Math.Pow(bitmap.Height - wave.y, 2));
                    double p4 = Math.Sqrt(Math.Pow(0 - wave.x, 2) + Math.Pow(0 - wave.y, 2));
                    if (wave.p > p1 && wave.p > p2 && wave.p > p3 && wave.p > p4)
                    {
                        waves.Remove(wave);
                    }
                }
                
                Thread.Sleep(1);
            }
        }


        private void Render(List<WaveSource> waves)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            Array.Copy(sourceArray, arrDst, sourceArray.Length);

            for (int k = 0; k < waves.Count; k++)
            {
                WaveSource wave = waves[k];
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        double p = Math.Sqrt(Math.Pow(i - wave.x, 2) + Math.Pow(j - wave.y, 2));
                        if (p >= wave.p - wave.waveLength * 0.5 && p <= wave.p + wave.waveLength * 0.5)
                        {
                            int pixx = (int)((1 + wave.amplitude * Math.Sin(p / wave.waveLength) / p) * (i - wave.x) + wave.x);
                            int pixy = (int)((1 + wave.amplitude * Math.Sin(p / wave.waveLength) / p) * (j - wave.y) + wave.y);
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
            

        }

        private void FrmClick_MouseDown(object sender, MouseEventArgs e)
        {
            lock (waves)
            {
                waves.Add(new WaveSource(e.X, e.Y, 20, 5, 0));
            }
        }        
    }
}
