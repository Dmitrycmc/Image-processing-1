using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace app_1
{
    public enum Command { mirror, rotate, sobel, median, gauss, gradient };
    public enum Axis { x, y };
    public enum Direction { cw, ccw };
    public enum Angle { oneQuarter, twoQuarter, threeQuarter };
    public enum Extra { rep, odd, even };

    class Arguments
    {
        public Command command;
        public string input;
        public string output;
        public string[] props;

        public Axis axis;
        public Direction direction;
        public Angle angle;
        public int rad;
        public Extra extra;
        public float sigma;

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
            int len = args.Length;

            if (len < 3)
            {
                throw new Exception("Too few arguments!");
            }

            input = args[len - 2];
            output = args[len - 1];
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

                default: throw new Exception("Invalid command \"" + args[0] + "\""); 
            }

        }

        public void execute()
        {
            Bitmap input = new Bitmap(this.input);
            Bitmap output = null;

            switch (this.command)
            {
                case Command.mirror:
                    output = Methods.mirror(input, this.axis);
                    break;
                case Command.rotate:
                    output = Methods.rotate(input, this.direction, this.angle);
                    break;
                case Command.sobel:
                    output = Methods.sobel(input, this.extra, this.axis);
                    break;
                case Command.median:
                    output = Methods.median(input, this.rad);
                    break;
                case Command.gauss:
                    output = Methods.gauss(input, this.extra, this.sigma);
                    break;
                case Command.gradient:
                    output = Methods.gradient(input, this.extra, this.sigma);
                    break;
            }
            output.Save(this.output);
        }
        
        public override string ToString()
        {
            string res = "";
            res += "Command: " + command + '\n';
            res += "Props: " + string.Join(", ", props) + '\n';
            res += "Input: " + input + '\n';
            res += "Output: " + output;
            return res;
        }
    }
}
