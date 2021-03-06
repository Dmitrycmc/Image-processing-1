﻿using System;
using System.Drawing;

namespace app_1
{
	public enum Command {
		mirror, rotate, sobel, median, gauss, gradient,
		mse, psnr, ssim, mssim, dir, nonmax, canny, diff
	};

    public enum Axis { x, y };
    public enum Direction { cw, ccw };
    public enum Angle { oneQuarter, twoQuarter, threeQuarter };
    public enum Extra { rep, odd, even };

    class Arguments
    {
        public Command command;
        public string img1;
        public string img2;
        public string[] props;

        public Axis axis;
        public Direction direction;
        public Angle angle;
        public int rad;
        public Extra extra;
        public float sigma;
		public int thr_high, thr_low;
		public bool progress;

		static Axis parseAxis(string str)
        {
            switch (str)
            {
                case "x": return Axis.x;
                case "y": return Axis.y;
                default: throw new Exception("Invalid prop \"" + str + "\"");
            }
        }

        static Extra parseExtra(string str)
        {
            switch (str)
            {
                case "rep": return Extra.rep;
                case "odd": return  Extra.odd; 
                case "even": return Extra.even;
                default: throw new Exception("Invalid prop \"" + str + "\"");
            }
        }

        static Direction parseDirection(string str)
        {
            switch (str)
            {
                case "cw": return Direction.cw;
                case "ccw": return Direction.ccw;
                default: throw new Exception("Invalid prop \"" + str + "\"");
            }
        }

        static Angle parseAngle(string str)
        {
            switch (str)
            {
                case "90": return Angle.oneQuarter; 
                case "180": return Angle.twoQuarter; 
                case "270": return Angle.threeQuarter; 
                default: throw new Exception("Invalid prop \"" + str + "\"");
            }
        }

        public Arguments(string[] args)
        {
			progress = args[args.Length - 1] == "progress";
            int len = args.Length - (progress ? 1 : 0);

            if (len < 3)
            {
                throw new Exception("Too few arguments!");
            }

			img1 = args[len - 2];
			img2 = args[len - 1];
            props = new string[len - 3];
            Array.Copy(args, 1, props, 0, len - 3);

            switch (args[0])
            {
                case "mirror":
                    command = Command.mirror;
                    if (props.Length != 1)
                    {
                        throw new Exception("Invalid number of props!");
                    }
                    axis = parseAxis(props[0]);
                    break;

                case "rotate":
                    command = Command.rotate;
                    if (props.Length != 2)
                    {
                        throw new Exception("Invalid number of props!");
                    }
                    direction = parseDirection(props[0]);
                    angle = parseAngle(props[1]);
                    break;

                case "sobel":
                    command = Command.sobel;
                    if (props.Length != 2)
                    {
                        throw new Exception("Invalid number of props!");
                    }
                    extra = parseExtra(props[0]);
                    axis = parseAxis(props[1]);
                    break;

                case "median":
                    command = Command.median;
                    if (props.Length != 1)
                    {
                        throw new Exception("Invalid number of props!");
                    }
                    rad = int.Parse(props[0]);
                    break;

                case "gauss":
                    command = Command.gauss;
                    if (props.Length != 2)
                    {
                        throw new Exception("Invalid number of props!");
                    }
                    extra = parseExtra(props[0]);
                    sigma = float.Parse(props[1]);
                    break;

				case "gradient":
					command = Command.gradient;
					if (props.Length != 2)
					{
						throw new Exception("Invalid number of props!");
					}
					extra = parseExtra(props[0]);
					sigma = float.Parse(props[1]);
					break;

				case "mse":
					command = Command.mse;
					if (props.Length != 0)
					{
						throw new Exception("Invalid number of props!");
					}
					break;

				case "psnr":
					command = Command.psnr;
					if (props.Length != 0)
					{
						throw new Exception("Invalid number of props!");
					}
					break;

				case "ssim":
					command = Command.ssim;
					if (props.Length != 0)
					{
						throw new Exception("Invalid number of props!");
					}
					break;

				case "mssim":
					command = Command.mssim;
					if (props.Length != 0)
					{
						throw new Exception("Invalid number of props!");
					}
					break;

				case "dir":
					command = Command.dir;
					if (props.Length != 1)
					{
						throw new Exception("Invalid number of props!");
					}
					sigma = float.Parse(props[0]);
					break;

				case "nonmax":
					command = Command.nonmax;
					if (props.Length != 1)
					{
						throw new Exception("Invalid number of props!");
					}
					sigma = float.Parse(props[0]);
					break;

				case "canny":
					command = Command.canny;
					if (props.Length != 3)
					{
						throw new Exception("Invalid number of props!");
					}
					sigma = float.Parse(props[0]);
					thr_high = int.Parse(props[1]);
					thr_low = int.Parse(props[2]);
					break;

				case "diff":
					command = Command.diff;
					if (props.Length != 0)
					{
						throw new Exception("Invalid number of props!");
					}
					break;

				default: throw new Exception("Invalid command \"" + args[0] + "\""); 
            }

        }

        public void execute()
        {
            Bitmap img1 = new Bitmap(this.img1);
			Bitmap img2 = Array.FindIndex(new Command[] { Command.mse, Command.psnr, Command.ssim, Command.mssim, Command.diff }, x => x == this.command) != -1  ? new Bitmap(this.img2) : null;
			Bitmap res = null;

			switch (this.command)
            {
                case Command.mirror:
					res = Methods.mirror(img1, this.axis);
                    break;
                case Command.rotate:
					res = Methods.rotate(img1, this.direction, this.angle);
                    break;
                case Command.sobel:
					res = Methods.sobel(img1, this.extra, this.axis, progress: progress);
                    break;
                case Command.median:
					res = Methods.median(img1, this.rad, progress: progress);
                    break;
                case Command.gauss:
					res = Methods.gauss(img1, this.extra, this.sigma, progress: progress);
                    break;
				case Command.gradient:
					res = Methods.gradient(img1, this.sigma, this.extra, progress: progress)[0];
					break;
				case Command.mse:
					Console.WriteLine(Methods.mse(img1, img2));
					return;
				case Command.psnr:
					Console.WriteLine(Methods.psnr(img1, img2));
					return;
				case Command.ssim:
					Console.WriteLine(Methods.ssim(img1, img2));
					return;
				case Command.mssim:
					Console.WriteLine(Methods.mssim(img1, img2));
					return;
				case Command.dir:
					res = Methods.gradient(img1, this.sigma, progress: progress)[1];
					break;
				case Command.nonmax:
					res = Methods.nonmax(img1, this.sigma, progress: progress);
					break;
				case Command.canny:
					res = Methods.canny(img1, this.sigma, this.thr_high, this.thr_low, progress: progress);
					break;
				case Command.diff:
					res = Methods.diff(img1, img2, progress: progress);
					res.Save(this.img1 + " ^ " + this.img2);
					return;
			}
			res.Save(this.img2);
        }
        
        public override string ToString()
        {
            string res = "";
            res += "Command: " + command + '\n';
            res += "Props: " + string.Join(", ", props) + '\n';
            res += "img1: " + img1 + '\n';
            res += "img2: " + img2;
            return res;
        }

		public static void help()
		{
			Console.Write(
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
						"canny (sigma) (thr_high) (thr_low)\n" + 
						"diff\n"
					);
		}
	}
}
