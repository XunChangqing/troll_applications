using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace troll_ui_app
{
    public class PornClassifier
    {
        [DllImport("trollwiz-masatek.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public extern static void InitLib();
        [DllImport("trollwiz-masatek.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        extern static IntPtr CreatePornClassifier(string model_file);
        [DllImport("trollwiz-masatek.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        extern static int ClassifyImage(IntPtr classifier, IntPtr img, int width, int height, int stride, int channels,
            int types, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 7)] float[] ratio);
        [DllImport("trollwiz-masatek.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        extern static void ReleaseClassifier(IntPtr classifier);

        public enum ImageType{Normal, Disguise, Porn};
        //private enum InternalImageType{Other, Man, Sexy, Porn};
        //改成8类的nin模型
        private enum InternalImageType{Other, Man, Sexy, Porn, NormalGrayCartoon, NormalColorCartoon,
        PornGrayCartoon, PornColorCartoon};
        private static object syncRoot = new Object();
        private static PornClassifier instance;

        private IntPtr classifier_handle_;
        public static void Init()
        {
            InitLib();
            instance = new PornClassifier(Properties.Settings.Default.modelFile);
        }
        public static PornClassifier Instance
        {
            get { return instance; }
        }
        private PornClassifier(String model_file)
        {
            classifier_handle_ = CreatePornClassifier(model_file);
        }

        public ImageType Classify(String fname)
        {
            try
            {
                //Bitmap bmp = new Bitmap(fname);
                //return Classify(bmp);
                //dispose方法不会被自动调用，必须显示调用，或是使用using
                //否则图像文件会被锁住，这是由于bitmap可能会再次使用图像文件本身
                using (Bitmap bmp = new Bitmap(fname))
                {
                    return Classify(bmp);
                }
            }
            catch(Exception e)
            {
                return ImageType.Normal;
            }
        }
        public ImageType Classify(Bitmap bmp)
        {
            if (bmp.Width < Properties.Settings.Default.minWidth ||
                bmp.Height < Properties.Settings.Default.minHeight)
                return ImageType.Normal;

            lock (syncRoot)
            {
                // Lock the bitmap's bits.
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                //PixelF 
                //bmp.PixelFormat);

                // Get the address of the first line.
                IntPtr ptr = bmpData.Scan0;
                float[] ratio = new float[8];
                int ret = ClassifyImage(classifier_handle_, ptr, bmp.Width, bmp.Height, bmpData.Stride, 3,
                    ratio.Length, ratio);
                // Unlock the bits.
                bmp.UnlockBits(bmpData);
                int[] indices = new int[ratio.Length];
                for (int i = 0; i < indices.Length; i++) indices[i] = i;
                Array.Sort(ratio, indices);
                InternalImageType imt = (InternalImageType)indices[indices.Length - 1];
                float maxRatio = ratio[indices.Length-1];
                if (imt == InternalImageType.Porn && maxRatio > 0.5f)
                    return ImageType.Porn;
                else if ((imt == InternalImageType.PornGrayCartoon || imt == InternalImageType.PornColorCartoon) &&
                    maxRatio > 0.9f)
                    return ImageType.Porn;
                else if (imt == InternalImageType.Sexy)
                    return ImageType.Disguise;
                else
                    return ImageType.Normal;
            }
        }
    }
}
