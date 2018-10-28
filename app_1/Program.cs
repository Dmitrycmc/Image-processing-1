using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace app_1
{ 
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Arguments Args = new Arguments(new string[] { "rotate", "cw", "180", "3.bmp", "4.bmp" });
                Console.WriteLine(Args);

                Bitmap input = new Bitmap(Args.input);
                Bitmap output = null;

                switch (Args.command)
                {
                    case Command.mirror:
                        output = Methods.mirror(input, Args.axis);
                        break;
                    case Command.rotate:
                        output = Methods.rotate(input, Args.direction, Args.angle);
                        break;

                }
            
                output.Save(Args.output);
                Console.WriteLine("Success!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return;
            }
        }
    }
}
