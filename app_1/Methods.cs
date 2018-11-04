using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace app_1
{
    public static class Methods
    {
        public static Bitmap mirror(Bitmap input, Axis axis)
        {
            int width = input.Width;
            int height = input.Height;
            Bitmap output = new Bitmap(width, height);
        
            if (axis == Axis.x)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        Color color = input.GetPixel(i, j);
                        output.SetPixel(width - 1 - i, j, color);
                    }
                }
            } else
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        Color color = input.GetPixel(i, j);
                        output.SetPixel(i, height - 1 - j, color);
                    }
                }
            }
            return output;
        }

        public static Bitmap rotate(Bitmap input, Direction direction, Angle angle)
        {
            int width = input.Width;
            int height = input.Height;
            Bitmap output;

            if (angle == Angle.oneQuarter || angle == Angle.threeQuarter)
            {
                output = new Bitmap(height, width);
                if ((angle == Angle.oneQuarter) != (direction == Direction.ccw))
                { // 90 deg cw
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            Color color = input.GetPixel(i, j);
                            output.SetPixel(height - 1 - j, i, color);
                        }
                    }
                } else
                { // 90 deg ccw
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            Color color = input.GetPixel(i, j);
                            output.SetPixel(j, width - 1 - i, color);
                        }
                    }
                }

            }
            else
            { // 180 deg
                output = new Bitmap(width, height);
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        Color color = input.GetPixel(i, j);
                        output.SetPixel(width - 1 - i, height - 1 - j, color);
                    }
                }
            }
            return output;
        }
        
        public static Bitmap median(Bitmap input, int rad)
        {
            int width = input.Width;
            int height = input.Height;
            Bitmap output = new Bitmap(width, height);

            Func<int, int, List<int>[]> getPixelsFromArea = (x, y) =>
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

                        if (sx < 0 || sx >= width || sy < 0 || sy >= height)
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
            };

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    List<int>[] pixels = getPixelsFromArea(i, j);
                    int[] r = pixels[0].ToArray();
                    int[] g = pixels[1].ToArray();
                    int[] b = pixels[2].ToArray();
                    Array.Sort(r);
                    Array.Sort(g);
                    Array.Sort(b);
                    int center = r.Length / 2;
                    Color color = Color.FromArgb(255, r[center], g[center], b[center]);
                    output.SetPixel(i, j, color);
                }
            }
            return output;
        }

        static int restrict(int x)
        {
            x = Math.Max(x, 0);
            x = Math.Min(x, 255);
            return x;
        }


        public static Color repGetPixel(this Bitmap input, int x, int y)
        {
            int proj_x, proj_y;
            proj_x = Math.Max(0, x);
            proj_x = Math.Min(input.Width - 1, proj_x);
            proj_y = Math.Max(0, y);
            proj_y = Math.Min(input.Height - 1, proj_y);
            Color projection = input.GetPixel(proj_x, proj_y);
            return projection;
        }

        public static Color evenGetPixel(this Bitmap input, int x, int y)
        { // 2k
            int w = input.Width - 1;
            int reflect_x = w - Math.Abs(Math.Abs(x) % (2 * w) - w);
            int h = input.Height - 1;
            int reflect_y = h - Math.Abs(Math.Abs(y) % (2 * h) - h);
            Color reflection = input.GetPixel(reflect_x, reflect_y);
            return reflection;
        }

        public static Color oddGetPixel(this Bitmap input, int x, int y)
        { // 2k + 1
            int proj_x, proj_y;
            proj_x = Math.Max(0, x);
            proj_x = Math.Min(input.Width - 1, proj_x);
            proj_y = Math.Max(0, y);
            proj_y = Math.Min(input.Height - 1, proj_y);
            Color projection = input.GetPixel(proj_x, proj_y);

            int w = input.Width - 1;
            int reflect_x = w - Math.Abs(Math.Abs(x) % (2 * w) - w);
            int h = input.Height - 1;
            int reflect_y = h - Math.Abs(Math.Abs(y) % (2 * h) - h);
            Color reflection = input.GetPixel(reflect_x, reflect_y);

            int r = 2 * projection.R - reflection.R;
            int g = 2 * projection.G - reflection.G;
            int b = 2 * projection.B - reflection.B;


            return Color.FromArgb(255, restrict(r), restrict(g), restrict(b));
        }
        
        public static Bitmap sobel(Bitmap input, Extra extra, Axis axis)
        {
            int width = input.Width;
            int height = input.Height;
            Bitmap output = new Bitmap(width, height);
            int[,] kernel;

            Func<int, int, Color> currentGetPixel = null; 

            switch (extra)
            {
                case Extra.odd:
                    currentGetPixel = input.oddGetPixel;
                    break;
                case Extra.even:
                    currentGetPixel = input.evenGetPixel;
                    break;
                case Extra.rep:
                    currentGetPixel = input.repGetPixel;
                    break;
            } 

            if (axis == Axis.x)
            {
                kernel = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            }
            else
            {
                kernel = new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
            }
            
            Func<int, int, Color> convolution = (x, y) =>
            {
                int r = 128;
                int g = 128;
                int b = 128;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Color color = currentGetPixel(x - 1 + i, y - 1 + j);
                        int kernel_el = kernel[i, j];

                        r += color.R * kernel_el;
                        g += color.G * kernel_el;
                        b += color.B * kernel_el;
                    }
                }
                return Color.FromArgb(255, restrict(r), restrict(g), restrict(b));
            };
            
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color color = convolution(i, j);
                    output.SetPixel(i, j, color);
                }
            }
            return output;
        }

        static double gaussian(double x, float sigma)
        {
            return Math.Exp(-Math.Pow(x, 2) / (2 * sigma * sigma)) / (sigma * Math.Sqrt(2 * Math.PI));
        }

        static double[,] getGaussKernel(float sigma)
        {
            int rad = (int) Math.Ceiling(2 * sigma);
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
                    double val = gaussian(Math.Sqrt(i*i + j*j), sigma);
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

        static Func<int, int, Color> getSafeGetPixel(this Bitmap input, Extra extra)
        {
            switch (extra)
            {
                case Extra.odd:
                    return  input.oddGetPixel;
                case Extra.even:
                    return  input.evenGetPixel;
                case Extra.rep:
                    return input.repGetPixel;
                default:
                    return null;
            }
        }

        public static Bitmap gauss(Bitmap input, Extra extra, float sigma)
        {
            double[,] kernel = getGaussKernel(sigma);
            
            Func<int, int, Color> currentGetPixel = input.getSafeGetPixel(extra);

            Func<int, int, Color> convolution = (x, y) =>
            {
                int rad = kernel.GetLength(0);
                double r = 0;
                double g = 0;
                double b = 0;

                for (int i = 1 - rad; i <= rad - 1; i++)
                {
                    for (int j = 1 - rad; j <= rad - 1; j++)
                    {
                        Color color = currentGetPixel(x + i, y + j);
                        double kernel_el = kernel[Math.Abs(i), Math.Abs(j)];
                        r += color.R * kernel_el;
                        g += color.G * kernel_el;
                        b += color.B * kernel_el;
                    }
                }
                return Color.FromArgb(255, (int)Math.Round(r), (int)Math.Round(g), (int)Math.Round(b));
            };

            int width = input.Width;
            int height = input.Height;
            Bitmap output = new Bitmap(width, height);
            
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color color = convolution(i, j);
                    output.SetPixel(i, j, color);
                }
            }

            return output;
        }

        static double[,] derivFilter(double[,] kernel, Axis axis)
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
                
            } else
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

        public static Bitmap gradient(Bitmap input, Extra extra, float sigma)
        {
            double[,] kernel = getGaussKernel(sigma);
            double[,] kernelX = derivFilter(kernel, Axis.x);
            double[,] kernelY = derivFilter(kernel, Axis.y);
            
            Func<int, int, Color> currentGetPixel = input.getSafeGetPixel(extra);

            int width = input.Width;
            int height = input.Height;
            Bitmap output = new Bitmap(width, height);
            double[,][] raw = new double[width, height][];

            Func<int, int, double[]> convolution = (x, y) =>
            {
                int rad = kernel.GetLength(0);
                double rx = 0;
                double gx = 0;
                double bx = 0;
                double ry = 0;
                double gy = 0;
                double by = 0;

                for (int i = 1 - rad; i <= rad - 1; i++)
                {
                    for (int j = 1 - rad; j <= rad - 1; j++)
                    {
                        Color color = currentGetPixel(x + i, y + j);

                        double kernel_X = kernelX[Math.Abs(i), Math.Abs(j)] * Math.Sign(j);
                        double kernel_Y = kernelY[Math.Abs(i), Math.Abs(j)] * Math.Sign(i);

                        rx += color.R * kernel_X;
                        gx += color.G * kernel_X;
                        bx += color.B * kernel_X;

                        ry += color.R * kernel_Y;
                        gy += color.G * kernel_Y;
                        by += color.B * kernel_Y;
                    }
                }
                return new double[] { Math.Sqrt(rx * rx + ry * ry), Math.Sqrt(gx * gx + gy * gy), Math.Sqrt(bx * bx + by * by) };
            };

            double rmax = 0;
            double gmax = 0;
            double bmax = 0;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    double[] val = convolution(i, j);
                    raw[i, j] = val;
                    if (val[0] > rmax) rmax = val[0];
                    if (val[1] > gmax) gmax = val[1];
                    if (val[2] > bmax) bmax = val[2];
                }
            }

            Console.WriteLine(rmax + " " + gmax + " " + bmax);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    double[] val = raw[i, j];
                    output.SetPixel(i, j, Color.FromArgb(255, (int)Math.Round(val[0] / rmax * 255), (int)Math.Round(val[1] / gmax * 255), (int)Math.Round(val[2] / bmax * 255)));
                }
            }


            return output;
        }

    }
}
