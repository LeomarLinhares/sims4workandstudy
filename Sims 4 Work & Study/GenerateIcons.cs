using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace Sims_4_Work___Study
{
    public class GenerateIcons
    {
        public static void CreateTaskbarIcons()
        {
            string assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets");
            Directory.CreateDirectory(assetsPath);

            // Ícone de Previous (◄)
            CreateIconWithText(Path.Combine(assetsPath, "previous.png"), "◄", 16);
            
            // Ícone de Pause (❚❚)
            CreateIconWithText(Path.Combine(assetsPath, "pause.png"), "❚❚", 16);
            
            // Ícone de Play (▶)
            CreateIconWithText(Path.Combine(assetsPath, "play.png"), "▶", 16);
            
            // Ícone de Next (►)
            CreateIconWithText(Path.Combine(assetsPath, "next.png"), "►", 16);
        }

        private static void CreateIconWithText(string filePath, string text, int size)
        {
            using (Bitmap bitmap = new Bitmap(size, size))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.Clear(Color.Transparent);

                using (Font font = new Font("Segoe UI Symbol", size * 0.6f, FontStyle.Regular))
                using (SolidBrush brush = new SolidBrush(Color.White))
                {
                    StringFormat format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    graphics.DrawString(text, font, brush, new RectangleF(0, 0, size, size), format);
                }

                bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
    }
}
