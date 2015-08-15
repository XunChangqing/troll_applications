using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace troll_ui_app
{
    class PornClassifier
    {
        [DllImport("caffe.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public extern static void InitLib();
        [DllImport("caffe.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        extern static IntPtr CreatePornClassifier(string model_file, string trained_file, string mean_file);
        [DllImport("caffe.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        extern static int ClassifyImage(IntPtr classifier, IntPtr img, int width, int height, int stride, int channels,
            int types, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 7)] float[] ratio);
        [DllImport("caffe.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        extern static void ReleaseClassifier(IntPtr classifier);

        public enum ImageType{Normal, Disguise, Porn};
        private enum InternalImageType{Other, Man, Sexy, Porn};
        private static object syncRoot = new Object();
        private static PornClassifier instance;

        private IntPtr classifier_handle_;
        public static void Init()
        {
            InitLib();
            instance = new PornClassifier(Properties.Settings.Default.modelFile,
                Properties.Settings.Default.trainedFile,
                Properties.Settings.Default.meanFile);
        }
        public static PornClassifier Instance
        {
            get { return instance; }
        }
        private PornClassifier(String model_file, String trained_file, String mean_file)
        {
            classifier_handle_ = CreatePornClassifier(model_file,
                trained_file,
                mean_file);
        }

        public ImageType Classify(Bitmap bmp)
        {
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
                float[] ratio = new float[4];
                int ret = ClassifyImage(classifier_handle_, ptr, bmp.Width, bmp.Height, bmpData.Stride, 3,
                    4, ratio);
                // Unlock the bits.
                bmp.UnlockBits(bmpData);
                int[] indices = new int[ratio.Length];
                for (int i = 0; i < indices.Length; i++) indices[i] = i;
                Array.Sort(ratio, indices);
                if (indices[3] == 3)
                    return ImageType.Porn;
                else if (indices[3] == 2)
                    return ImageType.Disguise;
                else
                    return ImageType.Normal;
            }
        }
    }
}
