using System;

namespace app_1
{ 
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
               // Arguments Args = new Arguments(new string[] { "gradient", "even", "1", "avion.bmp", "avion1.jpg" });
                Arguments Args = new Arguments(args);
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
