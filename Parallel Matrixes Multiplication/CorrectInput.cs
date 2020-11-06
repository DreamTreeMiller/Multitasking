using System;

namespace Multitasking
{
	class CorrectInput
    {

        int value;     // вводимое значение

        public CorrectInput()
        {
            this.value = 0;
        }

        /// <summary>
        /// Просит ввести число в заданном диапазоне, включая крайние значения.
        /// Реализована "защита от дурака"
        /// Возвращает введённое число
        /// </summary>
        /// <param name="introMessage">Сообщение, что надо ввести</param>
        /// <param name="outOfrangeMsg">Сообщение о выходе за пределы диапазона</param>
        /// <param name="minValue">Нижнее значение диапазона</param>
        /// <param name="maxValue">Вержнее значение диапазона</param>
        /// <returns></returns>
        public int Parse(string introMessage, string outOfrangeMsg,
                         int minValue, int maxValue, int x, int y)
        {
            bool isNumber;      // флаг, введено ли число, а не просто набор символов
            bool isInRange;     // флаг, введённое число находятся в заданном диапазоне 
            
            string msgToPrint;     // Выводимое сообщение, сформированное на основе переданных значений
            string inputMsg = "";  // Вводимая строка
            string errorMsg;       // Сообщение об ошибке
            // нам нужны эти две переменные, чтобы знать длины этих строк, чтобы очищать экран 
            // в нужных местах при следующей итерации после неправильного ввода
            int currX, currY;   // сохраняем текущие значения X and Y

            //currX = Console.CursorLeft;
            //currY = Console.CursorTop;

            #region Вводим уровень
            // Осуществляем "защиту от дурака" при вводе числа
            msgToPrint = $"{introMessage} от {minValue} до {maxValue:#,###,###,###}: ";
            errorMsg = "";
            do
            {
                Console.SetCursorPosition(x, y);
                Console.Write(msgToPrint + "".PadRight(inputMsg.Length, ' '));
                Console.SetCursorPosition(x + msgToPrint.Length, y);
                inputMsg = Console.ReadLine();
                isNumber = int.TryParse(inputMsg, out value);  //  вводим кол-во игроков
                isInRange = true;  // предполагаем, что оно в диапазоне от 2 до 10

                if (isNumber)     // введено числов ?
                {
                    // введено число, но надо проверить,
                    // оно в рамках диапазона minValue - maxValue
                    if ((value < minValue) || (maxValue < value))  
                    {
                        // Если на предыдущей итерации были введены неверные данные, то отались сообщения об ошибке,
                        // которые надо почистить
                        if (errorMsg != "")
                        {
                            Console.SetCursorPosition(x, y + 1);
                            Console.Write("".PadRight(errorMsg.Length, ' '));
                        }
                        Console.SetCursorPosition(x, y + 1);
                        errorMsg = $"Ошибка! {outOfrangeMsg} от {minValue} до {maxValue:#,###,###,###}!";
                        Console.WriteLine(errorMsg);
                        isInRange = false;
                    }
                }
                else
                {
                    // Если на предыдущей итерации были введены неверные данные, то отались сообщения об ошибке,
                    // которые надо почистить
                    if (errorMsg != "")
                    {
                        Console.SetCursorPosition(x, y + 1);
                        Console.Write("".PadRight(errorMsg.Length, ' '));
                    }
                    Console.SetCursorPosition(x, y + 1);
                    errorMsg = "Ошибка! Вы должны ввести число!";
                    Console.WriteLine(errorMsg);
                }

            } while (!isNumber || !isInRange);    // Если введено не число, или число вне диапазона
                                                  // Вводим число заново

            // Если были введены неверные данные, то отались сообщения об ошибке,
            // которые надо почистить
            if (errorMsg != "")
            {
                Console.SetCursorPosition(x, y + 1);
                Console.Write("".PadRight(errorMsg.Length, ' '));
            }

            #endregion
            // на выходе переменная value содержит введённое значение
            return value;
        }
    }
}
