using System;
using System.Collections.Generic;
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
        
        public static Bitmap median(Bitmap input, int rad, bool progress = false)
        {
            int width = input.Width;
            int height = input.Height;
            Bitmap output = new Bitmap(width, height);
			int size = width * height;
			Progress prog = new Progress("Median", size);

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

					if (progress) prog.inc();
                }
			}
			if (progress) prog.finish();
			return output;
        }

        public static Bitmap sobel(Bitmap input, Extra extra, Axis axis, bool progress = false)
        {
            int width = input.Width;
            int height = input.Height;
            Bitmap output = new Bitmap(width, height);
			int size = width * height;
			Progress prog = progress ? new Progress("Sobel", size) : null;

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
            
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color color = Utils.sobelConvolution(i, j, kernel, currentGetPixel);
                    output.SetPixel(i, j, color);

					if (progress) prog.inc();
                }
			}
			if (progress) prog.finish();
			return output;
        }
        

        public static Bitmap gauss(Bitmap input, Extra extra, float sigma, bool progress = false)
        {
            double[,] kernel = Utils.getGaussKernel(sigma);
            
            Func<int, int, Color> currentGetPixel = input.getSafeGetPixel(extra);
            
            int width = input.Width;
            int height = input.Height;
			int size = height * width;

			Progress prog = progress ? new Progress("Gauss", size) : null;

            Bitmap output = new Bitmap(width, height);
            
            for (int i = 0; i < width; i++)
            {
				for (int j = 0; j < height; j++)
				{
					Color color = Utils.gaussConvolution(i, j, kernel, currentGetPixel);
					output.SetPixel(i, j, color);

					if (progress) prog.inc();
				}
            }

			if (progress) prog.finish();
			return output;
        }
        
        public static Bitmap gradient(Bitmap input, Extra extra, float sigma, bool progress = false)
        {
            double[,] kernel = Utils.getGaussKernel(sigma);
            double[,] kernelX = Utils.derivFilter(kernel);
		
			Func<int, int, Color> currentGetPixel = input.getSafeGetPixel(extra);

            int width = input.Width;
            int height = input.Height;
            Bitmap output = new Bitmap(width, height);
            double[,][] raw = new double[width, height][];
			int size = width * height;
			Progress prog = progress ? new Progress("Gradient", size) : null;
			
			double max = 0;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    double[] val = Utils.gradientConvolution(i, j,  currentGetPixel, kernelX);
					raw[i, j] = val;
                    if (val[0] > max) max = val[0];
                    if (val[1] > max) max = val[1];
                    if (val[2] > max) max = val[2];

					if (progress) prog.inc();
                }
            }

			if (max < Utils.eps) max = 255;

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					double[] val = raw[i, j];

					int r = Utils.scale(val[0], max);
					int g = Utils.scale(val[1], max);
					int b = Utils.scale(val[2], max);

					output.SetPixel(i, j, Color.FromArgb(255, r, g, b));
				}
				
			}
			if (progress) prog.finish();
			return output;
        }

		public static double mse(Bitmap img1, Bitmap img2)
		{
			int width = img1.Width;
			int height = img1.Height;

			if (img2.Width != width || img2.Height != height)
			{
				throw new Exception("Size of images are not equal");
			}

			double sumError = 0;

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					Color color1 = img1.GetPixel(i, j);
					Color color2 = img2.GetPixel(i, j);

					int diffR = color1.R - color2.R;
					int diffG = color1.G - color2.G;
					int diffB = color1.B - color2.B;

					int error = (diffR * diffR + diffG * diffG + diffB * diffB) / 3;

					sumError += error;
				}
			}

			return sumError / (width * height);
		}

		public static double psnr(Bitmap img1, Bitmap img2)
		{
			int L = 255;
			double MSE = mse(img1, img2);
			return 10 * Math.Log10(L * L / MSE);
		}

		public static double ssim(Bitmap img1, Bitmap img2, int n = -1, int m = -1)
		{
			int width = img1.Width;
			int height = img1.Height;

			if (n != -1 && m != -1 & (img2.Width != width || img2.Height != height))
			{
				throw new Exception("Size of images are not equal");
			}

			int L = 255;
			double k1 = 0.01;
			double k2 = 0.03;

			double avg1 = Utils.avg(img1, n, m);
			double avg2 = Utils.avg(img2, n, m);

			double cov = Utils.cov(img1, img2, avg1, avg2, n, m);

			double var1 = Utils.var(img1, avg1, n, m);
			double var2 = Utils.var(img2, avg2, n, m);

			double c1 = (L * k1) * (L * k1);
			double c2 = (L * k2) * (L * k2);

			return ((2 * avg1 * avg2 + c1) * (2 * cov + c2)) / 
				((avg1 * avg1 + avg2 * avg2 + c1) * (var1 + var2 + c2));
		}

		public static double mssim(Bitmap img1, Bitmap img2)
		{
			int width = img1.Width;
			int height = img1.Height;

			if (img2.Width != width || img2.Height != height)
			{
				throw new Exception("Size of images are not equal");
			}

			if (width % 8 != 0 || height % 8 != 0)
			{
				throw new Exception("Size of images not devides by 8");
			}

			int W = width / 8;
			int H = height / 8;

			double sum = 0;

			for (int n = 0; n < W; n++)
			{
				for (int m = 0; m < H; m++)
				{
					sum += ssim(img1, img2, n, m);
				}
			}

			return sum / (W * H);
		}

		public static Bitmap dir(Bitmap img, float sigma, Extra extra = Extra.rep, bool progress = false)
		{
			int width = img.Width;
			int height = img.Height;
			int size = width * height;
			Progress prog = progress ? new Progress("Dir", size) : null;

			double[,] kernel = Utils.getGaussKernel(sigma);
			double[,] kernelX = Utils.derivFilter(kernel);

			Func<int, int, Color> currentGetPixel = img.getSafeGetPixel(extra);

			Bitmap output = new Bitmap(width, height);

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					int val = Utils.gradientDir(i, j, kernel, currentGetPixel, kernelX);
					Color color = Color.FromArgb(255, val, val, val);
					output.SetPixel(i, j, color);

					if (progress) prog.inc();
				}
			}

			if (progress) prog.finish();
			return output;
		}

		public static Bitmap nonmax(Bitmap img, float sigma, Extra extra = Extra.rep, bool progress = false)
		{
			Bitmap dirGrad = dir(img, sigma, extra, progress: progress);
			Bitmap modGrad = gradient(img, extra, sigma, progress: progress);
			int width = dirGrad.Width;
			int height = dirGrad.Height;
			int size = width * height;
			Progress prog = progress ? new Progress("Nonmax", size) : null;

			Bitmap output = new Bitmap(width, height);
			int[,] raw = new int[width, height];

			int max = 0;

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					Color dirColor = dirGrad.GetPixel(i, j);
					Color modColor = modGrad.GetPixel(i, j);

					double dir = dirColor.R;
					int mod = (modColor.R + modColor.G + modColor.B) / 3;

					if (dir == 64 && (i == 0 || dirGrad.GetPixel(i - 1, j).R == 0 || dirGrad.GetPixel(i - 1, j).R == 64 && mod >= Utils.getGrayShade(modGrad.GetPixel(i - 1, j))) && (i == width - 1 || dirGrad.GetPixel(i + 1, j).R == 0 || dirGrad.GetPixel(i + 1, j).R == 64 && mod >= Utils.getGrayShade(modGrad.GetPixel(i + 1, j))) ||
						dir == 128 && (j == 0 || dirGrad.GetPixel(i, j - 1).R == 0 || dirGrad.GetPixel(i, j - 1).R == 128 && mod >= Utils.getGrayShade(modGrad.GetPixel(i, j - 1))) && (j == height - 1 || dirGrad.GetPixel(i, j + 1).R == 0 || dirGrad.GetPixel(i, j + 1).R == 128 && mod >= Utils.getGrayShade(modGrad.GetPixel(i, j + 1))) ||
						dir == 192 && (j == 0 || i == 0 || dirGrad.GetPixel(i - 1, j - 1).R == 0 || dirGrad.GetPixel(i - 1, j - 1).R == 192 && mod >= Utils.getGrayShade(modGrad.GetPixel(i - 1, j - 1))) && (i == width - 1 || j == height - 1 || dirGrad.GetPixel(i + 1, j + 1).R == 0 || dirGrad.GetPixel(i + 1, j + 1).R == 192 && mod >= Utils.getGrayShade(modGrad.GetPixel(i + 1, j + 1))) ||
						dir == 255 && (j == height - 1 || i == 0 || dirGrad.GetPixel(i - 1, j + 1).R == 0 || dirGrad.GetPixel(i - 1, j + 1).R == 255 && mod >= Utils.getGrayShade(modGrad.GetPixel(i - 1, j + 1))) && (i == width - 1 || j == 0 || dirGrad.GetPixel(i + 1, j - 1).R == 0 || dirGrad.GetPixel(i + 1, j - 1).R == 255 && mod >= Utils.getGrayShade(modGrad.GetPixel(i + 1, j - 1)))
					)
					{
						raw[i, j] = mod;
						if (mod > max) max = mod;
					} else
					{
						output.SetPixel(i, j, Color.FromArgb(255, 0, 0, 0));
					}

					if (progress) prog.inc();
				}
			}
			
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					int val = Utils.scale(raw[i, j], max);
					Color color = Color.FromArgb(255, val, val, val);
					output.SetPixel(i, j, color);
				}
			}

			if (progress) prog.finish();
			return output;
		}

		public static Bitmap canny(Bitmap img, float sigma, int thr_high, int thr_low, 
			Extra extra = Extra.rep, bool progress = false)
		{
			Bitmap nomaxed = nonmax(img, sigma, extra, progress: progress);

			int width = nomaxed.Width;
			int height = nomaxed.Height;
			int size = width * height;
			Progress prog = progress ? new Progress("Canny", size) : null;

			Bitmap tresholded1 = new Bitmap(width, height);
			Bitmap tresholded2 = new Bitmap(width, height);

			List<Tuple<int, int>> q = new List<Tuple<int, int>>();

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					tresholded2.SetPixel(i, j, Color.Black);
					   Color color = nomaxed.GetPixel(i, j);
					int val = color.R;

					if ((double)val / 255 < (double)thr_low / 1000)
					{
						val = 0;
					}

					if ((double)val / 255 >= (double)thr_high / 1000)
					{
						q.Add(new Tuple<int, int>(i, j));
					}
					color = Color.FromArgb(255, val, val, val);
					tresholded1.SetPixel(i, j, color);
					prog.inc();
				}
			}
			

			int index = 0;

			while (index < q.Count)
			{
				int x = q[index].Item1;
				int y = q[index].Item2;

				tresholded2.SetPixel(x, y, Color.White);

				int up = y == 0 ? 0 : tresholded1.GetPixel(x, y - 1).R;
				int down = y == height - 1 ? 0 : tresholded1.GetPixel(x, y + 1).R;
				int left = x == 0 ? 0 : tresholded1.GetPixel(x - 1, y).R;
				int right = x == width - 1 ? 0 : tresholded1.GetPixel(x + 1, y).R;

				if (up > 0 && (double)up / 255 < (double)thr_high / 1000)
				{
					tresholded1.SetPixel(x, y - 1, Color.White);
					q.Add(new Tuple<int, int>(x, y - 1));
				}
				if (down > 0 && (double)down / 255 < (double)thr_high / 1000)
				{
					tresholded1.SetPixel(x, y + 1, Color.White);
					q.Add(new Tuple<int, int>(x, y + 1));
				}
				if (left > 0 && (double)left / 255 < (double)thr_high / 1000)
				{
					tresholded1.SetPixel(x - 1, y, Color.White);
					q.Add(new Tuple<int, int>(x - 1, y));
				}
				if (right > 0 && (double)right / 255 < (double)thr_high / 1000)
				{
					tresholded1.SetPixel(x + 1, y, Color.White);
					q.Add(new Tuple<int, int>(x + 1, y));
				}
				index++;
			}
		
			for (int i = 1; i < width - 1; i++)
			{
				for (int j = 1; j < height - 1; j++)
				{
					Color color = tresholded2.GetPixel(i, j);
					if (color.R == 255) continue;
					bool up = tresholded2.GetPixel(i, j - 1).R == 255;
					bool down = tresholded2.GetPixel(i, j + 1).R == 255;
					bool left = tresholded2.GetPixel(i - 1, j).R == 255;
					bool right = tresholded2.GetPixel(i + 1, j).R == 255;
					bool leftUp = tresholded2.GetPixel(i - 1, j - 1).R == 255;
					bool rightDown = tresholded2.GetPixel(i + 1, j + 1).R == 255;
					bool leftDown = tresholded2.GetPixel(i - 1, j + 1).R == 255;
					bool rightUp = tresholded2.GetPixel(i + 1, j - 1).R == 255;

					if (up && down || left && right || leftDown && rightUp || leftUp && rightDown) tresholded2.SetPixel(i, j, Color.White); 
				}
			}

			if (progress) prog.finish();
			return tresholded2;
		}

	}
}
