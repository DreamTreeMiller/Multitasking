using System;
using System.Text;

namespace Multitasking
{

    class Program
    {
        static Random random = new Random();
        static Matrix m = new Matrix();

        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.Unicode; Console.OutputEncoding = Encoding.Unicode;
            m.matrixMenu();

        }
    }
}

