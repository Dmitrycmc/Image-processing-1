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
            
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    List<int>[] pixels = input.getPixelsFromArea(i, j, rad);
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

        public static Bitmap sobel(Bitmap input, Extra extra, Axis axis)
        {
            int width = input.Width;
            int height = input.Height;
            Bitmap output = new Bitmap(width, height);
           
            Func<int, int, Color> currentGetPixel = input.getSafeGetPixel(extra);

            int[,] kernel;
            if (axis == Axis.x)
            {
                kernel = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            }
            else
            {
                kernel = new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
            }
            
            //convo
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
                return Color.FromArgb(255, Utils.restrict(r), Utils.restrict(g), Utils.restrict(b));
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
        

        public static Bitmap gauss(Bitmap input, Extra extra, float sigma)
        {
            double[,] kernel = Utils.getGaussKernel(sigma);
            
            Func<int, int, Color> currentGetPixel = input.getSafeGetPixel(extra);

            // convo
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
        
        public static Bitmap gradient(Bitmap input, Extra extra, float sigma)
        {
            double[,] kernel = Utils.getGaussKernel(sigma);
            double[,] kernelX = Utils.derivFilter(kernel, Axis.x);
            double[,] kernelY = Utils.derivFilter(kernel, Axis.y);
            
            Func<int, int, Color> currentGetPixel = input.getSafeGetPixel(extra);

            int width = input.Width;
            int height = input.Height;
            Bitmap output = new Bitmap(width, height);
            double[,][] raw = new double[width, height][];

            // convo
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

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    double[] val = raw[i, j];

                    int r = Utils.scale(val[0], rmax);
                    int g = Utils.scale(val[1], gmax);
                    int b = Utils.scale(val[2], gmax);

                    output.SetPixel(i, j, Color.FromArgb(255, r, g, b));
                }
            }
            
            return output;
        }
        // todo put convolutions into a separate file
    }
}
