using System;
using System.Collections.Generic;
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

        public static double[,] derivFilter(double[,] kernel)
        {
            int rad = kernel.GetLength(0);
            double[,] res = new double[rad, rad];
			
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
            return res;
        }
        
        public static int scale(double val, double from, int to = 255)
        {
            return (int)Math.Round(val / from * to);
        }

        public static Color sobelConvolution(int x, int y, int[,] kernel, Func<int, int, Color> currentGetPixel)
        {
            int r = 128;
            int g = 128;
            int b = 128;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Color color = currentGetPixel(x + i, y + j);
                    int kernel_el = kernel[i + 1, j + 1];

                    r += color.R * kernel_el;
                    g += color.G * kernel_el;
                    b += color.B * kernel_el;
                }
            }
            return Color.FromArgb(255, Utils.restrict(r), Utils.restrict(g), Utils.restrict(b));
        }

        static public Color gaussConvolution(int x, int y, double[,] kernel, Func<int, int, Color> currentGetPixel)
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
        }

		public static double[] gradientConvolution(int x, int y,
			double[,] kernel, Func<int, int, Color> currentGetPixel, double[,] kernelX)
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

					double elX = kernelX[Math.Abs(i), Math.Abs(j)] * Math.Sign(j);
					double elY = kernelX[Math.Abs(j), Math.Abs(i)] * Math.Sign(i);

					rx += color.R * elX;
					gx += color.G * elX;
					bx += color.B * elX;

					ry += color.R * elY;
					gy += color.G * elY;
					by += color.B * elY;
				}
			}

			return new double[] {
				Math.Sqrt(rx * rx + ry * ry),
				Math.Sqrt(gx * gx + gy * gy),
				Math.Sqrt(bx * bx + by * by)
			};
		}

		public static int angle(double x, double y)
		{
			if (x == 0 && y == 0) return 0;
			double angle = Math.Atan2(y, x);
			angle /= Math.PI / 4;
			angle = Math.Round(angle);
			angle %= 4;
			if (angle < 0) angle += 4;
			switch (angle)
			{
				case 0:
					return 64;
				case 1:
					return 255;
				case 2:
					return 128;
				case 3:
					return 192;
				default:
					return -1;
			}
		}

		public static double getGrayShade(Color color)
		{
			return (color.R + color.G + color.B) / 3;
		}

		public static int gradientDir(int x, int y,
			double[,] kernel, Func<int, int, Color> currentGetPixel, double[,] kernelX)
		{
			int rad = kernel.GetLength(0);
			double gradX = 0;
			double gradY = 0;

			for (int i = 1 - rad; i <= rad - 1; i++)
			{
				for (int j = 1 - rad; j <= rad - 1; j++)
				{
					Color color = currentGetPixel(x + i, y + j);

					double elX = kernelX[Math.Abs(i), Math.Abs(j)] * Math.Sign(j);
					double elY = kernelX[Math.Abs(j), Math.Abs(i)] * Math.Sign(i);

					double intensive = (color.R + color.G + color.B) / 3;

					gradX += intensive * elX;
					gradY += intensive * elY;
				}
			}

			return angle(gradX, gradY);
		}

		public static double avg(Bitmap img, int n = -1, int m = -1)
		{
			int width = img.Width;
			int height = img.Height;

			double sum = 0;

			bool mMode = n != -1 && m != -1;

			int iStart = mMode ? 8 * n : 0;
			int iEnd = mMode ? 8 * n + 7: width;
			int jStart = mMode ? 8 * m : 0;
			int jEnd = mMode ? 8 * m + 7 : height;

			for (int i = iStart; i < iEnd; i++)
			{
				for (int j = jStart; j < jEnd; j++)
				{
					Color color = img.GetPixel(i, j);
					sum += (color.R + color.G + color.B) / 3;
				}
			}

			return sum / (width * height);
		}

		public static double cov(Bitmap img1, Bitmap img2, double avg1, double avg2, int n = -1, int m = -1)
		{
			int width = img1.Width;
			int height = img1.Height;
			
			double sum = 0;

			bool mMode = n != -1 && m != -1;
			
			int iStart = mMode ? 8 * n : 0;
			int iEnd = mMode ? 8 * n + 7 : width;
			int jStart = mMode ? 8 * m : 0;
			int jEnd = mMode ? 8 * m + 7 : height;

			for (int i = iStart; i < iEnd; i++)
			{
				for (int j = jStart; j < jEnd; j++)
				{
					Color color1 = img1.GetPixel(i, j);
					Color color2 = img2.GetPixel(i, j);
					
					double intensive1 = (color1.R + color1.G + color1.B) / 3;
					double intensive2 = (color2.R + color2.G + color2.B) / 3;

					sum += (intensive1 - avg1) * (intensive2 - avg2);
				}
			}

			return sum / (width * height);
		}

		public static double var(Bitmap img, double avg, int n = -1, int m = -1)
		{
			return cov(img, img, avg, avg, n, m);
		}
	}
}
