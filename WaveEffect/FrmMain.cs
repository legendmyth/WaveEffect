using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WaveEffect
{
    public partial class FrmMain : Form
    {
        private Graphics formGraphics;

        private Bitmap bitmap;

        private Bitmap sourceMap;

        private byte[] sourceArray;
        public FrmMain()
        {
            InitializeComponent();
            this.Shown += FrmMain_Shown;
        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            formGraphics = this.CreateGraphics();
            sourceMap = global::WaveEffect.Properties.Resources.img6;
            bitmap = new Bitmap(sourceMap.Width, sourceMap.Height);
            Rectangle rect = new Rectangle(0, 0, sourceMap.Width, sourceMap.Height);
            System.Drawing.Imaging.BitmapData bmpData = sourceMap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            int bytesCount = bmpData.Stride * bmpData.Height;
            sourceArray = new byte[bytesCount];
            Marshal.Copy(ptr, sourceArray, 0, bytesCount);
            sourceMap.UnlockBits(bmpData);
        }

        private void tbWavelength_Scroll(object sender, EventArgs e)
        {
            this.Render();
        }
        private void tbAmplitude_Scroll(object sender, EventArgs e)
        {
            this.Render();
        }

        private void Render()
        {
            double rate = this.tbWavelength.Value;
            double ratio = this.tbAmplitude.Value * 0.01;

            int width = bitmap.Width;
            int height = bitmap.Height;
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);//curBitmap.PixelFormat
            IntPtr ptr = bmpData.Scan0;
            int bytesCount = bmpData.Stride * height;
            byte[] arrDst = new byte[sourceArray.Length];
            Array.Copy(sourceArray, arrDst, sourceArray.Length);
            for (int i = 0; i < bytesCount; i += 3)
            {
                int y = (int)(i / bmpData.Stride) - height / 2;
                int x = (i % bmpData.Stride) / 3 - width / 2;
                double p = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                int tranx = (int)(x + x * ratio * Math.Sin(p / rate));
                int trany = (int)(y + y * ratio * Math.Sin(p / rate));
                int pixx = tranx + width / 2;
                int pixy = trany + height / 2;

                if (pixx > 0 && pixx < width && pixy > 0 && pixy < height)
                {
                    arrDst[pixx * 3 + pixy * bmpData.Stride] = sourceArray[i];
                    arrDst[pixx * 3 + pixy * bmpData.Stride + 1] = sourceArray[i + 1];
                    arrDst[pixx * 3 + pixy * bmpData.Stride + 2] = sourceArray[i + 2];
                }
            }
            Marshal.Copy(arrDst, 0, ptr, arrDst.Length);
            bitmap.UnlockBits(bmpData);
            formGraphics.DrawImage(bitmap, 100, 100);
        }


    }
}
