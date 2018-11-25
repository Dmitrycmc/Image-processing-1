using System;
using System.Drawing;

namespace app_1
{ 
    class Program
    {
        static void Main(string[] args)
        {
            try
			{
				Bitmap mask = new Bitmap("beg49d.bmp");
				Bitmap canny = new Bitmap("beg4.jpg");
				Bitmap orig = new Bitmap("beg9.bmp");
				
				int width = mask.Width;
				int height = mask.Height;

				Bitmap output1 = new Bitmap(width, height);
				Bitmap output2 = new Bitmap(width, height);

				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						Color color = mask.GetPixel(i, j);
						Color color1 = Color.Wheat;
						color1 = (color.R != 0 ? canny : orig).GetPixel(i, j);
						output1.SetPixel(i, j, color1);
						color1 = (color.R == 0 ? canny : orig).GetPixel(i, j);
						output2.SetPixel(i, j, color1);
					}
				}
				output1.Save("rr.bmp");
				output2.Save("rr1.bmp");

				//Console.WriteLine(Utils.angle(5, 0));
				Console.WriteLine(
					"mirror {x|y}\n" +
					"rotate { cw | ccw} (angle)\n" +
					"sobel { rep | odd | even} { x | y}\n" +
					"median(rad)\n" +
					"gauss { rep | odd | even} (sigma)\n" +
					"gradient { rep | odd | even} (sigma)\n" +
					"mse\n" +
					"psnr\n" +
					"ssim\n" +
					"mssim\n" +
					"dir (sigma)\n" +
					"nonmax (sigma)\n" +
					"canny (sigma) (thr_high) (thr_low)\n"
				);

				Arguments Args = new Arguments(new string[] { "canny", "2", "100", "30", "beg3.bmp", "beg5.jpg"});
                //Arguments Args = new Arguments(args);
                Console.WriteLine(Args);
                //Args.execute();
                Console.WriteLine("Success!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
    }
}
