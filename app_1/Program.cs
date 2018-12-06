using System;

namespace app_1
{ 
    class Program
    {
        static void Main(string[] args)
        {
            try
			{
				if (args.Length == 0)
				{
					Arguments.help();
					return;
				}
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
