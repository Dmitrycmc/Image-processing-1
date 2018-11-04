using System;

namespace app_1
{ 
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Arguments Args = new Arguments(new string[] { "gradient", "even", "2", "avion.bmp", "5555.bmp" });
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
