using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace app_1
{
    public static class Utils
    {        
        public static List<int>[] getPixelsFromArea(this Bitmap input, int x, int y, int rad)
        {
            List<int> r = new List<int>();
            List<int> g = new List<int>();
            List<int> b = new List<int>();

            for (int i = -rad; i <= +rad; i++)
            {
                for (int j = -rad; j <= +rad; j++)
                {
                    int sx = x + i;
                    int sy = y + j;
                    if (sx < 0 || sx >= input.Width || sy < 0 || sy >= input.Height)
                    {
                        continue;
                    }
                    Color color = input.GetPixel(sx, sy);
                    r.Add(color.R);
                    g.Add(color.G);
                    b.Add(color.B);
                }
            }
            return new List<int>[] { r, g, b };
        }
        
        public static int restrict(int x, int to = 255, int from = 0)
        {
            x = Math.Max(x, from);
            x = Math.Min(x, to);
            return x;
        }

        public static Tuple<int, int> getProjectionPoint(this Bitmap input, int x, int y)
        {
            return new Tuple<int, int>(
                restrict(x, input.Width - 1),
                restrict(y, input.Height - 1)
            );
        }

        public static Tuple<int, int> getReflectionPoint(this Bitmap input, int x, int y)
        {
            int w = input.Width - 1;
            int reflect_x = w - Math.Abs(Math.Abs(x) % (2 * w) - w);
            int h = input.Height - 1;
            int reflect_y = h - Math.Abs(Math.Abs(y) % (2 * h) - h);

            return new Tuple<int, int>(
                reflect_x,
                reflect_y
            );
        }

        public static Color repGetPixel(this Bitmap input, int x, int y)
        {
            Tuple<int, int> proj = input.getProjectionPoint(x, y);
            return input.GetPixel(proj.Item1, proj.Item2);
        }

        public static Color evenGetPixel(this Bitmap input, int x, int y)
        { // 2k
            Tuple<int, int> refl = input.getReflectionPoint(x, y);
            return input.GetPixel(refl.Item1, refl.Item2);
        }

        public static Color oddGetPixel(this Bitmap input, int x, int y)
        { // 2k + 1
            Tuple<int, int> proj = input.getProjectionPoint(x, y);
            Color projection = input.GetPixel(proj.Item1, proj.Item2);

            Tuple<int, int> refl = input.getReflectionPoint(x, y);
            Color reflection = input.GetPixel(refl.Item1, refl.Item2);

            int r = 2 * projection.R - reflection.R;
            int g = 2 * projection.G - reflection.G;
            int b = 2 * projection.B - reflection.B;
            
            return Color.FromArgb(255, restrict(r), restrict(g), restrict(b));
        }
        
        public static Func<int, int, Color> getSafeGetPixel(this Bitmap input, Extra extra)
        {
            switch (extra)
            {
                case Extra.odd:
                    return input.oddGetPixel;
                case Extra.even:
                    return input.evenGetPixel;
                case Extra.rep:
                    return input.repGetPixel;
                default:
                    return null;
            }
        }
        
        public static double gaussian(double x, float sigma)
        {
            return Math.Exp(-Math.Pow(x, 2) / (2 * sigma * sigma)) / (sigma * Math.Sqrt(2 * Math.PI));
        }
        
        public static double[,] getGaussKernel(float sigma)
        {
            int rad = (int)Math.Ceiling(2 * sigma);
            double[,] kernel = new double[rad, rad];

            for (int i = 0; i < rad; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    if (Math.Sqrt(i * i + j * j) > 2 * sigma)
                    {
                        kernel[i, j] = kernel[j, i] = 0;
                        continue;
                    }
                    double val = gaussian(Math.Sqrt(i * i + j * j), sigma);
                    kernel[i, j] = kernel[j, i] = val;
                }
            }

            double total = 0;

            for (int i = 1 - rad; i <= rad - 1; i++)
            {
                for (int j = 1 - rad; j <= rad - 1; j++)
                {
                    total += kernel[Math.Abs(i), Math.Abs(j)];
                }
            }

            for (int i = 0; i < rad; i++)
            {
                for (int j = 0; j < rad; j++)
                {
                    kernel[i, j] /= total;
                }
            }

            return kernel;
        }

        public static double[,] derivFilter(double[,] kernel, Axis axis)
        {
            int rad = kernel.GetLength(0);
            double[,] res = new double[rad, rad];

            if (axis == Axis.x)
            {
                for (int i = 0; i < rad; i++)
                {
                    for (int j = 0; j < rad; j++)
                    {
                        double prev;
                        double next;
                        int m = 2;

                        prev = kernel[Math.Abs(i), Math.Abs(j - 1)];
                        if (j + 1 == rad || (next = kernel[Math.Abs(i), Math.Abs(j + 1)]) == 0)
                        {
                            next = kernel[Math.Abs(i), Math.Abs(j)];
                            m = 1;
                        }
                        res[i, j] = (next - prev) / m;
                    }
                }

            }
            else
            {
                for (int i = 0; i < rad; i++)
                {
                    for (int j = 0; j < rad; j++)
                    {
                        double prev;
                        double next;
                        int m = 2;

                        prev = kernel[Math.Abs(i - 1), Math.Abs(j)];
                        if (i + 1 == rad || (next = kernel[Math.Abs(i + 1), Math.Abs(j)]) == 0)
                        {
                            next = kernel[Math.Abs(i), Math.Abs(j)];
                            m = 1;
                        }

                        res[i, j] = (next - prev) / m;
                    }
                }

            }

            return res;
        }
        
        public static int scale(double val, double from, int to = 255)
        {
            return (int)Math.Round(val / from * to);
        }
    }
}
