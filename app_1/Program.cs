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
            try
            {
                Arguments Args = new Arguments(new string[] { "gradient", "even", "5", "barbara.bmp", "5555.bmp" });
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
