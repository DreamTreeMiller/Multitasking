using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace parallel
{
    class Program
	{
		static int totalNumbers = 0;
		static readonly object o = new object();
		static void DigitSumCheck(int num) 
		{
			int lastDigit = num % 10;
			if (lastDigit == 0) return;

			int digitsSum = lastDigit;
			num /= 10;
			while (num != 0)
			{
				digitsSum += num % 10;
				num /= 10;
			}
			if (digitsSum % lastDigit == 0) 
				lock (o)
				{
					totalNumbers++;
				}
		}
		static void Main(string[] args)
		{
			Stopwatch s = new Stopwatch();
			s.Restart(); s.Start();
			for (int i = 1_000_000_000; i <= 2_000_000_000; i++)
			{
				int num = i;
				int lastDigit = num % 10;
				if (lastDigit == 0) continue;
				int digitsSum = lastDigit;
				while (num != 0)
				{
					digitsSum += num % 10;
					num /= 10;
				}
				if (digitsSum % lastDigit == 0)
						totalNumbers++;
			}
			s.Stop();
			Console.WriteLine($"totalNumbers = {totalNumbers}");
			Console.WriteLine($"Sync time elapsed = {s.ElapsedMilliseconds} ms");
			Console.WriteLine();

			totalNumbers = 0;
			s.Restart(); s.Start();
			Parallel.For(1_000_000_000, 2_000_000_000, DigitSumCheck);
			s.Stop();
			Console.WriteLine($"totalNumbers = {totalNumbers}");
			Console.WriteLine($"Parallel.For time elapsed = {s.ElapsedMilliseconds} ms");
			Console.WriteLine();

			Console.ReadKey();
		}
	}
}
