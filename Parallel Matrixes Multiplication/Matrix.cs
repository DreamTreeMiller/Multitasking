using System;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;

namespace Multitasking
{
    public struct MenuItem
    {
        public ConsoleKey itemKey;     // Клавиша, которой соответствует пункт меню
        public string itemName;        // Текст пункта меню
    }


    class Matrix
    {

        #region объявления переменных

        int winHeight = 20;             // Высота экрана (для меню) 40 строк
        int winWidth = 80;              // Ширина экрана (для меню) 80 строк

        static MenuItem[] menuItems =          // Пункты меню для вывода на экран
                   {new MenuItem {itemKey = ConsoleKey.D1,    itemName = "1.   УМНОЖЕНИЕ МАТРИЦ" },
                    new MenuItem {itemKey = ConsoleKey.D2,    itemName = "2.   СУММА ЦИФР КРАТНА ПОСЛЕДНЕЙ" },
                    new MenuItem {itemKey = ConsoleKey.D3,    itemName = "3.   ТЕЛЕГРАМ БОТ" },
                    new MenuItem {itemKey = ConsoleKey.Escape,itemName = "ESC  ВЫХОД" } };

        // Окно, в котором будем выводить меню матрицы
        CorrectInput correctInput;
        WindowOutput windowM;

        Random r = new Random();

        #endregion объявления переменных

        public Matrix()
        {
            correctInput = new CorrectInput();
            windowM = new WindowOutput(winWidth, winHeight);
        }

        public void matrixMenu()
        {
            ConsoleKey action;       // Переменная, в которую будет считываться нажатие клавиши
            int currItem = 1;        // Текущий пункт меню


            do                  // Считываем нажатия, пока не будет ESC
            {
                windowM.newWindow(winWidth, winHeight);
                Console.CursorVisible = false;  // Делаем курсор невидимым
                windowM.HeaderCenter("МНОГОПОТОЧНОСТЬ", winWidth, 2, ConsoleColor.Yellow);
                action = windowM.MenuSelect(menuItems, currItem, winWidth, 4);

                switch (action)
                {
                    case ConsoleKey.D1:
                        multiplyMatrixes();
                        currItem = 1;
                        break;

                    case ConsoleKey.D2:
                        DigitsSum ds = new DigitsSum();
                        ds.DigitsSumMenu();
                        currItem = 2;
                        break;

                    case ConsoleKey.D3:
                        currItem = 3;
                        break;

                    case ConsoleKey.Escape:
                        Console.WriteLine("ДО СВИДАНИЯ!");
                        break;

                    default:
                        break;   // нажата любая другая клавиша - ничего не происходит
                }

            } while (action != ConsoleKey.Escape);

        }   // public void matrixMenu()

        /// <summary>
        /// Печатает матрицу в заданной области буфера экрана с заданной шириной ячейки. 
        /// Расстояние между ячейками - пробел. Выравнивание в ячейке по левому краю
        /// </summary>
        /// <param name="matrixToPrint">Матрица для вывода.</param>
        /// <param name="cellSize">Количество символов для вывода каждой ячейки матрицы</param>
        /// <param name="topLeftX">Столбец, с которого выводить матрицу</param>
        /// <param name="topLeftY">Строка, с которой выводить матрицу</param>
        public void printMatrix(int[,] matrixToPrint, int cellSize, int topLeftX, int topLeftY)
        {
            int i, j;

            for (i = 0; i < matrixToPrint.GetLength(0); i++)
            {
                Console.SetCursorPosition(topLeftX, topLeftY + i);
                Console.Write("| ");
                for (j = 0; j < matrixToPrint.GetLength(1); j++)
                {
                    Console.Write($"{matrixToPrint[i, j]}".PadLeft(cellSize));
                }
                Console.Write(" |");

            }
        }

        public void multiplyMatrixes()
        {
            int m, n, k;         // размерности матрицы A: m - число строк (1 х 10), n - число столбцов (1 х 10)
                                 // размерности матрицы B: n - число строк (1 х 10), k - число столбцов (1 х 10)

            bool calculatByCells, printOnScreen;

            EnterMatrixDimensionsForMultiplication(out m, out n, out k, out calculatByCells, out printOnScreen);
            
            int[,] leftMatrix  = CreateAndFillRandomMatrix(m, n, 0, 1);
            int[,] rightMatrix = CreateAndFillRandomMatrix(n, k, 0, 1);
            int[,] productMatrixSeq = new int[m, k];

			Console.WriteLine();
			Console.WriteLine("Вычисление произведения матриц последовательно для сравнения ...");
            Stopwatch s = new Stopwatch();
 
            #region Calculate matrix sequentially
            s.Reset(); s.Start();

            for (int i = 0; i < m; i++)
                for (int j = 0; j < k; j++)
                    for (int e = 0; e < n; e++)
                        productMatrixSeq[i, j] += leftMatrix[i, e] * rightMatrix[e, j];

            s.Stop(); TimeSpan matrixParallelCalculationTime = s.Elapsed;
            #endregion

            Console.WriteLine();
            Console.WriteLine("Вычисление произведения матриц потоками ...");

            #region Calculate with threads
            s.Reset(); s.Start(); 
            
            int[,] productMatrix = MatrixProductThreads(leftMatrix, rightMatrix, calculatByCells);

            s.Stop(); TimeSpan matrixSequentialCalculationTime = s.Elapsed;
            #endregion

            if(!AreMatrixesEqual(productMatrix, productMatrixSeq))
			{
				Console.WriteLine("Matrixes are NOT equal!!!"); 
			}
            else
			{
				Console.WriteLine("Matrixes are EQUAL!!!");
			}

			#region Save matrixes to JSON
			s.Reset(); s.Start();

            if (printOnScreen)
			    PrintMatrixProduct(leftMatrix, rightMatrix, productMatrix);

            Console.WriteLine();
            Console.WriteLine("сохранение матриц в файл ...");

            SaveMatrixes(leftMatrix, rightMatrix, productMatrixSeq, productMatrix);

			s.Stop(); TimeSpan matrixPrintTime = s.Elapsed;
			#endregion
			Console.WriteLine();
            Console.WriteLine($"Время поточного вычисления произведения         {matrixParallelCalculationTime}");
            Console.WriteLine($"Время последовательного вычисления произведения {matrixSequentialCalculationTime}");
            Console.WriteLine($"Время печати матриц {matrixPrintTime}");

            windowM.HeaderCenter("НАЖМИТЕ ЛЮБУЮ КЛАВИШУ",
                                 winWidth,
                                 Console.CursorTop,
                                 ConsoleColor.DarkYellow);
            Console.ReadKey();

        }

        void EnterMatrixDimensionsForMultiplication(out int lines, out int col, out int common,
            out bool calculatByCells, out bool printOnScreen)
        {
            int winHeight = 30;        // Высота экрана (для меню) 40 строк
            int winWidth = 100;         // Ширина экрана (для меню) 80 строк
            bool cursorVisibility;

            windowM.newWindow(winWidth, winHeight);
            Console.SetBufferSize(200, 40);
            windowM.HeaderCenter("УМНОЖЕНИЕ МАТРИЦ", winWidth, 2, ConsoleColor.Yellow);
            Console.WriteLine();
            cursorVisibility = Console.CursorVisible;
            Console.CursorVisible = true;

			Console.WriteLine("Распараллеливать вычисление (1) для каждой ячейки или (2) для строки результирующей матрицы?");
			Console.Write("Введите 1 - для ячейки, любой другой символ для строки: ");
			char kkk = Console.ReadKey().KeyChar;
            calculatByCells = kkk == '1';

            int m = correctInput.Parse("Введите количество строк матрицы А", "Введите число", 1, 10_000, 0, 6);
            lines = m;

            int n = correctInput.Parse("Введите количество столбцов матрицы А", "Введите число", 1, 10_000, 0, 7);
            col = n;

            Console.SetCursorPosition(0, 9);
            Console.Write($"В матрице B {n} строк");
            int k = correctInput.Parse("Введите количество столбцов матрицы B", "Введите число", 1, 10_000, 0, 10);
            common = k;

            Console.SetCursorPosition(0, 11);
			Console.WriteLine("Выводить результат на экран? Это может занять много времени. 1 - да : ");
            kkk = Console.ReadKey().KeyChar;
            printOnScreen = kkk == '1';

            Console.CursorVisible = cursorVisibility;
        }

        /// <summary>
        /// Создает матрицу размром M строк, N столбцов, и наполняет ее случайными числами
        /// от min до max включительно
        /// </summary>
        /// <returns></returns>
        int[,] CreateAndFillRandomMatrix(int m, int n, int min, int max)
        {
            int[,] matrix = new int[m, n];

            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = r.Next(min, max + 1);
                }
            return matrix;
        }

        int[,] MatrixProductThreads(int[,] leftMatrix, int[,] rightMatrix, bool calculateByCells)
        {
            int m = leftMatrix.GetLength(0);
            int n = leftMatrix.GetLength(1);
            int k = rightMatrix.GetLength(1);

            int[,] productMatrix = new int[m, k];

            if (calculateByCells)
			    for (int i = 0; i < m; i++)
				    for (int j = 0; j < k; j++)
				    {
					    MatrixWrapper matrixWrapper =
						    new MatrixWrapper(leftMatrix, rightMatrix, productMatrix, i, j);

					    ThreadPool.QueueUserWorkItem(new WaitCallback(CalculateOneCell), matrixWrapper);
				    }
            else
			    for (int i = 0; i < m; i++)
			    {
                    MatrixWrapper mw = new MatrixWrapper(leftMatrix, rightMatrix, productMatrix, i, -1);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(CalculateOneRow), mw);
			    }

            while (ThreadPool.PendingWorkItemCount > 0) ;
			return productMatrix;
        }

        void CalculateOneCell(object args)
		{

            var leftMatrix    = (args as MatrixWrapper).leftMatrix; 
            var rightMatrix   = (args as MatrixWrapper).rightMatrix;
            var productMatrix = (args as MatrixWrapper).productMatrix;
            int n = leftMatrix.GetLength(1);
            int i = (args as MatrixWrapper).line;
            int j = (args as MatrixWrapper).col;
            int cell = 0;

			for (int e = 0; e < n; e++)
				cell += leftMatrix[i, e] * rightMatrix[e, j];

            productMatrix[i, j] = cell;
		}

        void CalculateOneRow(object args)
		{
            var leftMatrix      = (args as MatrixWrapper).leftMatrix;
            var rightMatrix     = (args as MatrixWrapper).rightMatrix;
            var productMatrix   = (args as MatrixWrapper).productMatrix;
            int n = leftMatrix.GetLength(1);
            int k = rightMatrix.GetLength(1);
            int i = (args as MatrixWrapper).line;

            for (int j = 0; j < k; j++)
                for (int e = 0; e < n; e++)
                    productMatrix[i,j] += leftMatrix[i, e] * rightMatrix[e, j];

        }

        bool AreMatrixesEqual(int[,] mA, int[,] mB)
		{
            int m = mA.GetLength(0);
            int k = mA.GetLength(1);
            if (m != mB.GetLength(0)) return false;
            if (k != mB.GetLength(1)) return false;

            for (int i = 0; i < m; i++)
                for (int j = 0; j < k; j++)
                    if (mA[i, j] != mB[i, j]) return false;
            return true;
        }
 
        void PrintMatrixProduct(int[,] leftMatrix, int[,] rightMatrix, int[,] productMatrix)
		{
            int m = leftMatrix.GetLength(0);
            int n = leftMatrix.GetLength(1);
            int k = rightMatrix.GetLength(1);

            int offsetA = (m >= n) ? 0 : (n - m) / 2;
            int offsetB = (m >= n) ? (m - n) / 2 : 0;
            int verticalSize = ((m > n) ? m : n) + 20;

            int cellSize = 4;
            windowM.newWindow(n * cellSize + 2 * k * cellSize + 100, verticalSize);
            // Выводим матрицу A
            // 3 - ширина поля для вывода каждого элемента матрицы
            // 5 и 8  - координаты Х и У левого верхнего угла матрицы
            printMatrix(leftMatrix, cellSize, 5, 10 + offsetA);

            // Выводим знак умножить "X"  между матрицами
            Console.SetCursorPosition(5 + 1 + n * cellSize + 2 + 3,
                                      10 + ((offsetA == 0) ? offsetB + n / 2 : offsetA + m / 2));
            Console.Write("x");

            // Выводим матрицу B
            // 5 - ширина поля для вывода каждого элемента матрицы
            //  5 + 1 + n * cellSize + 2 + 5 и 8  - координаты Х и У левого верхнего угла матрицы
            printMatrix(rightMatrix, cellSize, 5 + 1 + n * cellSize + 2 + 6, 10 + offsetB);

            // Выводим знак равно "="  между матрицами
            Console.SetCursorPosition(5 + (1 + n * cellSize + 2 + 6) + (1 + k * cellSize + 2) + 3,
                                      10 + ((offsetA == 0) ? offsetB + n / 2 : offsetA + m / 2));
            Console.Write($"=");

            // Выводим получившуюся матрицу
            // 5 - ширина поля для вывода каждого элемента матрицы
            // 5+1+n*cellSize+2+20 и 8  - координаты Х и У левого верхнего угла матрицы
            printMatrix(productMatrix, cellSize, 5 + (1 + n * cellSize + 2 + 6) + (1 + k * cellSize + 2) + 6, 10 + offsetA);

            windowM.HeaderCenter("НАЖМИТЕ ЛЮБУЮ КЛАВИШУ",
                                 winWidth,
                                 10 + ((m > n) ? m : n) + 2,
                                 ConsoleColor.DarkYellow);
            Console.ReadKey();
        }

        void SaveMatrixes(int[,] leftMatrix, int[,] rightMatrix, int[,] productMatrixSeq, int[,] productMatrix)
		{
            string leftM        = JsonConvert.SerializeObject(leftMatrix);
            string rightM       = JsonConvert.SerializeObject(rightMatrix);
            string productMSeq  = JsonConvert.SerializeObject(productMatrixSeq);
            string productM     = JsonConvert.SerializeObject(productMatrix);

            System.IO.File.WriteAllText("leftmatrix.txt",    leftM);
			Console.WriteLine("Левая матрица сохранена в файл Debug\\leftmatrix.txt");

            System.IO.File.WriteAllText("rightmatrix.txt",   rightM);
            Console.WriteLine("Правая матрица сохранена в файл Debug\\rightmatrix.txt");

            System.IO.File.WriteAllText("productmatrixSeq.txt", productMSeq);
            Console.WriteLine("Последовательно вычисленная матрица сохранена в файл Debug\\productmatrixSeq.txt");

            System.IO.File.WriteAllText("productmatrix.txt", productM);
            Console.WriteLine("Поточно вычесленная матрица сохранена в файл Debug\\productmatrix.txt");
        }

    }   // class Matrix

    class MatrixWrapper
	{
        public int[,] leftMatrix;
        public int[,] rightMatrix;
        public int[,] productMatrix;
        public int line;
        public int col;

        public MatrixWrapper(int[,] lm, int[,] rm, int[,] pm, int i, int j)
		{
            leftMatrix = lm;
            rightMatrix = rm;
            productMatrix = pm;
            line = i;
            col = j;
		}

	}
}   // namespace Homework_Theme_04
