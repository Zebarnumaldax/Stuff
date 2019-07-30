using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace _3com
{
    class Program
    {
        // преобразование в чб
        public static Bitmap ToGrayScale(Bitmap Bmp)
        {
            int rgb;
            Color c;

            for (int y = 0; y < Bmp.Height; y++)
            {
                for (int x = 0; x < Bmp.Width; x++)
                {
                    c = Bmp.GetPixel(x, y);
                    rgb = (int)Math.Round(.299 * c.R + .587 * c.G + .114 * c.B);
                    Bmp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            }
            return Bmp;
        }

        static void Main(string[] args)
        {
            try
            {
                int i = 0;
                int x = SystemInformation.VirtualScreen.Width;
                int y = SystemInformation.VirtualScreen.Height;
                int screenLeft = SystemInformation.VirtualScreen.Left;
                int screenTop = SystemInformation.VirtualScreen.Top;
                var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                var encParams = new EncoderParameters() { Param = new[] { new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 70L) } };
                Bitmap memoryImage;
                memoryImage = new Bitmap(x, y);
                Size s = new Size(memoryImage.Width, memoryImage.Height);
                Graphics memoryGraphics = Graphics.FromImage(memoryImage);
                string dir = "\\\\files\\director$\\test\\" + System.Environment.MachineName + DateTime.Now.ToString("yyyy-MM-dd");
                while (i == 0)
                {
                    dir= dir = "\\\\files\\director$\\test\\" + System.Environment.MachineName + DateTime.Now.ToString("yyyy-MM-dd");
                    if (Directory.Exists(dir))
                    {
                        memoryGraphics.CopyFromScreen(screenLeft, screenTop, 0, 0, s);
                        string str = dir + "\\" + DateTime.Now.ToString("yyyy -MM-ddTHH-mm-ss") + ".png";
                        memoryImage.Save(str, encoder, encParams);
                        System.Threading.Thread.Sleep(60000);
                    }
                    else if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                        memoryGraphics.CopyFromScreen(screenLeft, screenTop, 0, 0, s);
                        string str = dir + "\\" + DateTime.Now.ToString("yyyy -MM-ddTHH-mm-ss") + ".png";
                        memoryImage.Save(str, encoder, encParams);
                        System.Threading.Thread.Sleep(60000);
                    }
                }
            }
            catch (Exception err)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Environment.CurrentDirectory + @"\3com.txt"))
                {
                    file.WriteLine(err.Message);
                }
            }


        }
    }
}
