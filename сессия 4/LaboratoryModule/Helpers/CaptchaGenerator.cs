using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace LaboratoryModule.Helpers
{
    public static class CaptchaGenerator
    {
        private static readonly Random _rnd = new Random();
        private static readonly string _chars = "ABCDEFGHJKLMNPQRSTUVWXYZ0123456789";

        public static string GenerateCode(int length = 5)
        {
            char[] code = new char[length];
            for (int i = 0; i < length; i++)
            {
                code[i] = _chars[_rnd.Next(_chars.Length)];
            }
            return new string(code);
        }

        public static byte[] GenerateImage(string code, int width = 200, int height = 60)
        {
            using (var bmp = new Bitmap(width, height))
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.WhiteSmoke);

                for (int i = 0; i < 100; i++)
                    bmp.SetPixel(_rnd.Next(width), _rnd.Next(height), Color.LightGray);

                using (var font = new Font("Arial", 22, FontStyle.Bold))
                {
                    for (int i = 0; i < code.Length; i++)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(_rnd.Next(50, 150), _rnd.Next(50, 150), _rnd.Next(50, 150))))
                        {
                            float x = 15 + (i * 35);
                            float y = 12;
                            g.DrawString(code[i].ToString(), font, brush, x, y);
                        }
                    }
                }

                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }
    }
}