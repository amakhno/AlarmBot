using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmBot
{
    public class Screen
    {
        public static Bitmap getScreen(int x, int y, int width, int height)
        {
            Image output = Screen.getScreenHelper(0, 0, 1024, 768);
            Bitmap source = new Bitmap(output);
            System.Drawing.Imaging.PixelFormat format =
                    source.PixelFormat;
            Rectangle cloneRect = new Rectangle(x, y, width, height);
            Bitmap bmpScreenCapture = source.Clone(cloneRect, format);
            source.Dispose();
            output.Dispose();
            return bmpScreenCapture;
        }

        private static Bitmap getScreenHelper(int x, int y, int width, int height)
        {
            Bitmap bmpScreenCapture = new Bitmap(width, height);
            Graphics p = Graphics.FromImage(bmpScreenCapture);
            p.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            p.Dispose();
            return bmpScreenCapture;
        }
    }
}
