using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using log4net;

namespace troll_ui_app
{
    public class PornClassifier
    {
        static readonly ILog log = Log.Get();
        [DllImport("trollwiz-masatek.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        extern static void InitLib();
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

        public ImageType Classify(String fname, out Exception excep)
        {
            try
            {
                log.Info("Classify Image File: " + fname);
                //Bitmap bmp = new Bitmap(fname);
                //return Classify(bmp);
                //dispose方法不会被自动调用，必须显示调用，或是使用using
                //否则图像文件会被锁住，这是由于bitmap可能会再次使用图像文件本身
                using (Bitmap bmp = new Bitmap(fname))
                {
                    excep = null;
                    return Classify(bmp);
                }
            }
            catch(Exception e)
            {
                excep = e;
                return ImageType.Normal;
            }
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
                ImageType ret = Classify(ptr, bmp.Width, bmp.Height, bmpData.Stride);
                // Unlock the bits.
                bmp.UnlockBits(bmpData);
                return ret;
            }
        }
        public ImageType Classify(IntPtr pimg, int width, int height, int linesize)
        {
            if (width < Properties.Settings.Default.minWidth ||
                height < Properties.Settings.Default.minHeight)
                return ImageType.Normal;

            lock (syncRoot)
            {
                float[] ratio = new float[8];
                log.Info("Classify Image: " + pimg + " w: " + width + " h: " + height + " linesize: " + linesize);
                int ret = ClassifyImage(classifier_handle_, pimg, width, height, linesize, 3,
                    ratio.Length, ratio);
                log.Info("Classify Image Over!");
                // Unlock the bits.
                float pornRatio = ratio[(int)InternalImageType.Porn];
                float sexyRatio = ratio[(int)InternalImageType.Sexy];
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
                else if (imt == InternalImageType.Sexy &&
                    pornRatio+sexyRatio>0.8)
                    return ImageType.Disguise;
                else
                    return ImageType.Normal;
            }
        }

        //一个视频分成多少段提取帧
        static readonly int VideoSegments = 11;
        static readonly int PornNumThd = 3;
        //视频允许的最少长度，通常即为帧数
        static readonly int MinFrames = 4000;
        public bool ClassifyVideoFile(string filename, out Exception excep)
        {
            log.Info("Classify Video File: " + filename);
            FFMPEGWrapper ffmpeg = new FFMPEGWrapper();
            if (!ffmpeg.Open(filename))
            {
                log.Info("Video File Cannot Open: " + filename);
                excep = new Exception("Cannot Open File!");
                ffmpeg.Dispose();
                return false;
            }
            if(ffmpeg.FileInfo.nb_frames<MinFrames)
            {
                log.Info("Video File Short of Frames: " + filename + " frames: " + ffmpeg.FileInfo.nb_frames); ;   
                ffmpeg.Dispose();
                excep = new Exception("Video File Too Short");
                return false;
            }
            Int64 step = ffmpeg.FileInfo.duration/VideoSegments;
            PornClassifier.ImageType []types = new PornClassifier.ImageType[VideoSegments-1];
            int pornNum = 0;
            for(Int64 ts=step;ts<ffmpeg.FileInfo.duration-step/2;ts+=step)
            {
                if (ffmpeg.ReadFrame(ts))
                {
                    if (Instance.Classify(ffmpeg.FileInfo.pdata, ffmpeg.FileInfo.width, ffmpeg.FileInfo.height, ffmpeg.FileInfo.linesize)
                        == ImageType.Porn)
                        pornNum++;
                }
            }
            ffmpeg.Dispose();
            excep = null;
            if (pornNum >= PornNumThd)
            {
                log.Info("Porn Video File: " + filename);
                return true;
            }
            else
                return false;
        }
    }
}
