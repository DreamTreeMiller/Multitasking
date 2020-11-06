using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Multitasking
{
    class DigitsSum
	{
		int totalNumbers = 0;
		readonly object o = new object();
		private void DigitSumCheck(int num) 
		{
			int lastDigit = num % 10;
			if (lastDigit == 0)
			{
				lock (o) { totalNumbers++; }
				return;
			}

			int digitsSum = lastDigit;
			num /= 10;
			while (num != 0)
			{
				digitsSum += num % 10;
				num /= 10;
			}
			if (digitsSum % lastDigit == 0) 
				lock (o) { totalNumbers++; }
		}

		public void DigitsSumMenu()
		{
			int winW = 100, winH = 40;
			WindowOutput win = new WindowOutput(winW, winH);
			CorrectInput ci = new CorrectInput();

			win.HeaderCenter("СУММА ЦИФР КРАТНА ПОСЛЕДНЕЙ ЦИФРЕ ЧИСЛА", winW, 1, ConsoleColor.Yellow);
			win.HeaderCenter("Программа вычисляет количество чисел в заданном диапазоне,", winW, 3, ConsoleColor.Gray);
			win.HeaderCenter("сумма цифр которых кратна последней цифре.", winW, 4, ConsoleColor.Gray);
			Console.CursorVisible = true;
			int min = ci.Parse("Введите нижнюю границу диапазона", "Введите число", 0, 2_147_483_647, 2, 6);
			int max = ci.Parse("Введите нижнюю границу диапазона", "Введите число", min, 2_147_483_647, 2, 8);
			Console.WriteLine();


			Stopwatch s = new Stopwatch();

			#region Последовательное вычисление
			Console.WriteLine("Последовательное вычисление. Это может занять и несколько минут ...");
			s.Restart(); s.Start();

			DigitSumSync(min, max);
			
			s.Stop();
			Console.WriteLine($"totalNumbers = {totalNumbers}");
			Console.WriteLine($"Sync time elapsed = {s.ElapsedMilliseconds} ms");
			Console.WriteLine();

			#endregion

			totalNumbers = 0;

			#region Потоковое вычисление
			Console.WriteLine("Вычисление через Parallel.For. Это может занять и несколько минут ...");
			s.Restart(); s.Start();

			Parallel.For(min, max, DigitSumCheck);

			s.Stop();
			Console.WriteLine($"totalNumbers = {totalNumbers}");
			Console.WriteLine($"Parallel.For time elapsed = {s.ElapsedMilliseconds} ms");
			Console.WriteLine();

			#endregion

			win.HeaderCenter("НАЖМИТЕ ЛЮБУЮ КЛАВИШУ",
								 winW,
								 Console.CursorTop,
								 ConsoleColor.DarkYellow);
			Console.ReadKey();
		}

		void DigitSumSync(int min, int max)
		{
			for (int i = min; i <= max; i++)
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
		}
	}
}
