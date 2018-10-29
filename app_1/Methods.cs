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



        /*


                public static Bitmap sobel(Bitmap input, Extra extra, Axis axis)
                {
                    int width = input.Width;
                    int height = input.Height;
                    Bitmap output = new Bitmap(width, height);
                    int[,] kernel;

                    Func<int, int, Color> uniGetPixel = (x, y) =>
                    {
                        x = Math.Max(0, x);
                        x = Math.Min(width - 1, x);
                        y = Math.Max(0, y);
                        y = Math.Min(height - 1, y);
                        return input.GetPixel(x, y);
                    };

                    if (axis == Axis.x)
                    {
                        kernel = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
                    }
                    else
                    {
                        kernel = new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
                    }

                    Func<int, int, int> convolution = (x, y) =>
                    {
                        int sum = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                int l = uniGetPixel(x - 1 + i, y - 1 + j).R;
                                int kernel_el = kernel[i, j];

                                sum += l * kernel_el;

                            }
                        }
                        return sum;
                    };
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            Color color = input.GetPixel(i, j);
                            int[] rgb = new int[] { color.R, color.G, color.B };
                            int l = (rgb.Max() - rgb.Min()) / 2; 
                            input.SetPixel(i, j, Color.FromArgb(255, l, l, l));
                        }
                    }
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            int conv = convolution(i, j);
                            int l = conv / 8 + 128;
                            Color color = Color.FromArgb(255, l, l, l);
                            output.SetPixel(i, j, color);
                        }
                    }
                    return output;
                }
                 */
    }
}
