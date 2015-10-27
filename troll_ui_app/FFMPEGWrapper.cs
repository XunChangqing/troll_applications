using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Web;

namespace troll_ui_app
{
    class FFMPEGWrapper : IDisposable
    {
        public struct FFMPEGFrameExtractorInfo
        {
            public IntPtr pdata;
            public int width;
            public int height;
            public int linesize;
            public Int64 duration;
            public Int64 nb_frames;
        };
        [DllImport("ffmpegwrapper.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        extern static void FFMPEGInitExt();
        [DllImport("ffmpegwrapper.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        extern static IntPtr CreateFFMPEGFrameExtractorExt(string fname);
        [DllImport("ffmpegwrapper.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        extern static FFMPEGFrameExtractorInfo FFMPEGGetInfoExt(IntPtr extractor);
        [DllImport("ffmpegwrapper.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        extern static bool FFMPEGFrameExtractorReadFrameExt(IntPtr extractor, Int64 ts);
        [DllImport("ffmpegwrapper.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        extern static void FFMPEGFrameExtractorReleaseExt(IntPtr extractor);
        public static void Init()
        {
            FFMPEGInitExt();
        }
        IntPtr _extractor = IntPtr.Zero;
        public FFMPEGFrameExtractorInfo FileInfo { get; set; }
        string _filename;
        public bool Open(string file)
        {
            _filename = file;
            _extractor = CreateFFMPEGFrameExtractorExt(file);
            if (_extractor != IntPtr.Zero)
            {
                FileInfo = FFMPEGGetInfoExt(_extractor);
                return true;
            }
            else
                return false;
        }
        public bool ReadFrame(Int64 ts)
        {
            if (_extractor != IntPtr.Zero)
            {
                if (FFMPEGFrameExtractorReadFrameExt(_extractor, ts))
                {
#if DEBUG
                    Bitmap nimg = new Bitmap(FileInfo.width, FileInfo.height, FileInfo.linesize, System.Drawing.Imaging.PixelFormat.Format24bppRgb, FileInfo.pdata);
                    nimg.Save(string.Format("videoframes/{0}.jpg", ts));
#endif
                    return true;
                }
            }
            return false;
        }
        public void Dispose()
        {
            if (_extractor != IntPtr.Zero)
                FFMPEGFrameExtractorReleaseExt(_extractor);
        }
    }
}
