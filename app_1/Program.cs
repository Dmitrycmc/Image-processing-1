using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_1
{ 
    class Program
    {
        static void Main(string[] args)
        {
            Func<int, int, int> f = (x, y) =>
            {
                int w = 5 - 1;
                x = w - Math.Abs(Math.Abs(x) % (2 * w) - w);
                int h = 5 - 1;
                y = h - Math.Abs(Math.Abs(y) % (2 * h) - h);
                Console.WriteLine(x + " " + y);
                return 0;
            };

            f(5, 5);
            f(0, 0);
            f(-2, -5);

            try
            {
                Arguments Args = new Arguments(new string[] { "sobel", "rep", "x", "in/barbara.bmp", "rep.bmp" });
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
