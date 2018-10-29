using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

/*
mirror {x|y} 
Отражение по горизонтали или по вертикали, в зависомсти от указанного параметра

rotate {cw|ccw} (angle) 
Поворот по или против часовой стрелки на заданное количество градусов, например: rotate cw 90

sobel {rep|odd|even} {x|y} 
Фильтр Собеля, обнаруживающий горизонтальные или вертикальные контуры. 
Первый параметр отвечает за способ экстраполяции изображений

median (rad) 
Медианная фильтрация, параметр rad — целочисленный радиус фильтра, 
то есть размер фильтра — квадрат со стороной (2 * rad + 1)

gauss {rep|odd|even} (sigma) 
Фильтр Гаусса, параметр sigma — вещественный параметр фильтра

gradient {rep|odd|even} (sigma) 
Модуль градиента
*/

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
                    switch (props[0])
                    {
                        case "x": axis = Axis.x; break;
                        case "y": axis = Axis.y; break;
                        default: throw new Exception("Invalid prop \"" + props[0] + "\"");
                    }
                    break;
                case "rotate":
                    command = Command.rotate;
                    if (props.Length != 2)
                    {
                        throw new Exception("Invalid number of props!");
                    }
                    switch (props[0])
                    {
                        case "cw": direction = Direction.cw; break;
                        case "ccw": direction = Direction.ccw; break;
                        default: throw new Exception("Invalid prop \"" + props[0] + "\"");
                    }
                    switch (props[1])
                    {
                        case "90": angle = Angle.oneQuarter; break;
                        case "180": angle = Angle.twoQuarter; break;
                        case "270": angle = Angle.threeQuarter; break;
                        default: throw new Exception("Invalid prop \"" + props[0] + "\"");
                    }
                    break;
                case "sobel":
                    command = Command.sobel;
                    if (props.Length != 2)
                    {
                        throw new Exception("Invalid number of props!");
                    }
                    switch (props[0])
                    {
                        case "rep": extra = Extra.rep; break;
                        case "odd": extra = Extra.odd; break;
                        case "even": extra = Extra.even; break;
                        default: throw new Exception("Invalid prop \"" + props[0] + "\"");
                    }
                    switch (props[1])
                    {
                        case "x": axis = Axis.x; break;
                        case "y": axis = Axis.y; break;
                        default: throw new Exception("Invalid prop \"" + props[0] + "\"");
                    }
                    break;
                case "median":
                    command = Command.median;
                    if (props.Length != 1)
                    {
                        throw new Exception("Invalid number of props!");
                    }
                    rad = int.Parse(props[0]);
                    break;
                case "gauss": command = Command.gauss; break;
                case "gradient": command = Command.gradient; break;
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
