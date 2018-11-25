using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_1
{
	class Progress
	{
		private int cnt;
		private int full;
		private int batch;

		private double percents(int cnt, int full)
		{
			return (double)cnt * 100 / full;
		}

		public Progress (string methodName, int full)
		{
			cnt = 0;
			this.full = full;
			batch = full / 1000;
			Console.WriteLine();
			Console.WriteLine(methodName + ":");
			Console.Write(string.Format("{0:0.0}", percents(cnt, full)) + " %");
		}
		public void inc()
		{
			cnt++;
			if (cnt % batch == 0)
			{
				Console.Write("\b\b\b\b\b\b" + string.Format("{0:0.0}", percents(cnt, full)) + " %");
			}
		}
		public void finish()
		{
			Console.WriteLine("\b\b\b\b\b\b\b100.0 %");
		}
	}
}
