using System;

namespace app_1
{ 
    class Program
    {
        static void Main(string[] args)
        {
            try
			{
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
                Args.execute();
                Console.WriteLine("Success!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
    }
}
