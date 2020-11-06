using System;
using System.Text;

namespace Multitasking
{
	class WindowOutput
    {
        /// <summary>
        /// Конструктор класса. Устанавливает одинаковые размеры буфера экрана и окна консоли. 
        /// Очищает буфер экрана
        /// </summary>
        /// <param name="width">Задаёт ширину окна консоли в символах</param>
        /// <param name="height">Задаёт высоту окна консоли в символах</param>
        public WindowOutput(int width, int height)
        {
			Console.InputEncoding = Encoding.Unicode; Console.OutputEncoding = Encoding.Unicode;
            newWindow(width, height);
        }

        /// <summary>
        /// Инициализирует буфер экрана и окно консоли. 
        /// Очищает буфер экрана, устанавливает его размер равным окну консоли
        /// </summary>
        /// <param name="width">Ширина буфера экрана и окна консоли</param>
        /// <param name="height">Высота буфера экрана и окна консоли</param>
        public void newWindow(int width, int height)
        { 
            // Это делаем в начале, и всякий раз, когда заканчивается работа пункта меню
            // Для этого надо очистить буфер экрана
            // Сделать размер буфера экрана равным размеру окна консоли
            // Как я обнаружил экспериментально, буфер экрана не может быть меньше окна консоли.
            // Если вдруг размер буфера экрана меньше размера окна консоли, то срабатывает исключение.
            // Поэтому сначала делаем окно консоли равным 1х1, потом уст. размер буфера экрана,
            // и в конце делаем размер окна консоли равным размеру буфера экрана.
            // Лучше их сделать одинаковыми, чтобы не путаться.
            // Потому что позиция курсора определяется относительно буфера экрана, а не окна консоли
            // а буфер экрана может быть длиннющим и широченным полотном 5000 на 400 символов, например

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.Clear();                    // Очищаем БУФЕР экрана
            Console.SetWindowSize(1, 1);        // Делаем минимальным размер окна консоли
            Console.SetBufferSize(width, height);      // Размер буфера экрана
            width  = width  < Console.LargestWindowWidth  ? width  : Console.LargestWindowWidth-20;
            height = height < Console.LargestWindowHeight ? height : Console.LargestWindowHeight-10;
            Console.SetWindowSize(width, height);      // Размер окна консоли такой же, как и буфер экрана
            
        }

        /// <summary>
        /// Выводит строку в центре окна консоли в указанной строке
        /// </summary>
        /// <param name="msg">Выводимая строка</param>
        /// <param name="winWid">ширина окна консоли</param>
        /// <param name="curVert">строка, в которой надо вывести заголовок</param>
        /// <param name="color">цвет символов строки</param>
        public void HeaderCenter (string msg, int winWid, int curVert, ConsoleColor color)
        {
            int curL = (winWid - msg.Length) / 2;
            ConsoleColor BC, FC;

            BC = Console.BackgroundColor;
            FC = Console.ForegroundColor;
            Console.SetCursorPosition(curL, curVert);
            Console.ForegroundColor = color;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(msg);
            Console.BackgroundColor = BC;
            Console.ForegroundColor = FC;

        }

        /// <summary>
        /// Заполняет пробелами заданную строку
        /// </summary>
        /// <param name="winWid">ширина окна консоли</param>
        /// <param name="curVert">строка, в которой надо вывести заголовок</param>
        /// <param name="bckgColor">цвет фона строки</param>

        public void CleanLine(int winWid, int curVert, ConsoleColor bckgColor)
        {
            ConsoleColor BC;

            BC = Console.BackgroundColor;

            Console.BackgroundColor = bckgColor;
            Console.SetCursorPosition(0, curVert);
            Console.Write(("").PadRight(winWid));

            Console.BackgroundColor = BC;

        }


        /// <summary>
        /// Выводит меню из пунктов, указанных в массиве структур,
        /// с указаной строки, по центру окна консоли
        /// Организует перемещение по пунктам меню с помощью стрелок вверх-вниз
        /// Обеспечивает выбор пункта меню либо нажатием клавиши Ввод,
        /// либо нажатием соответствующей клавиши
        /// Возвращает нажатую клавишу
        /// 
        /// В идеале надо бы реализовать сразу и вызов функии, соответствующей нажатой клавиши.
        /// Для этого надо ещё бы передать массив ссылок на эти функции, но я не умею ещё этого делать.
        /// </summary>
        /// <param name="menuItems">Массив структуры {клавиша, строка меню}</param>
        /// <param name="winWid">ширина экрана</param>
        /// <param name="curVert">Строка, с которой выводить меню</param>
        /// <returns></returns>
        public ConsoleKey MenuSelect(MenuItem[] menuItems, int currentItem, int winWid, int curVert)
        {
            ConsoleKey keyPress;    // нажатая клавиша
            bool selecting = true;  // флаг, процесс выбора пункта меню ещё идёт
            int longest = 0;        // длина самой длинной строки в массиве строчек меню
            int menuOffset = 0;     // крайняя левая позиция, откуда выводим меню
                                    // длины строк разыне, поэтому сначала находим самую длинную
                                    // относительно неё выравниваем
            
            // Находим длину самой длинной строки
            foreach (var e in menuItems)
            {
                if (e.itemName.Length > longest) longest = e.itemName.Length;
            }

            // Вычисляем крайнюю левую позицию
            menuOffset = (winWid - longest) / 2;
            Console.CursorVisible = false;

            // Выводим первый пункт меню в инверсном виде - белый фон, чёрные буквы
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            
            for (int i = 1; i <= menuItems.Length; i++)
            {
                if (i == currentItem)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(menuOffset, curVert + i - 1);
                    Console.Write(menuItems[i - 1].itemName);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.SetCursorPosition(menuOffset, curVert + i - 1);
                    Console.Write(menuItems[i - 1].itemName);
                }
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            string msgNavKeys1 = "Стрелки вверх/вниз - перемещение. Выбор пункта меню - Enter";
            Console.SetCursorPosition((winWid - msgNavKeys1.Length) / 2, curVert + menuItems.Length + 2);
            Console.Write(msgNavKeys1);
            string msgNavKeys2 = "Или нажмите цифру пункта меню. Выход - Esc";
            Console.SetCursorPosition((winWid - msgNavKeys2.Length) / 2, curVert + menuItems.Length + 3);
            Console.Write(msgNavKeys2);

            do          //  цикл - выбираем пункт меню
            {
                keyPress = Console.ReadKey(true).Key;
                switch (keyPress)
                {
                    case ConsoleKey.Enter:              // Нажата клавиша Enter
                        keyPress = menuItems[currentItem-1].itemKey;
                        selecting = false;              // Значит выбран пункт меню. Выходим
                        break;
                    
                    case ConsoleKey.Escape:             // Нажата клавиша Esc
                        keyPress = ConsoleKey.Escape;
                        selecting = false;              // Выходим из цикла
                        break;
                    
                    case ConsoleKey.DownArrow:          // нажата стрелка вниз

                        // проверяем, не уперлись ли в дно
                        if (currentItem < (menuItems.Length))
                        {
                            // Если не упёрлись, то 
                            // выводим текущий пункт меню белыми буквами на чёрном фоне
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.SetCursorPosition(menuOffset, curVert + currentItem - 1);
                            Console.Write(menuItems[currentItem - 1].itemName);

                            // переходим к следующему вниз пункту меню
                            currentItem++;

                            // выводим его чёрные буквы на белом фоне
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.SetCursorPosition(menuOffset, curVert + currentItem - 1);
                            Console.Write(menuItems[currentItem - 1].itemName);
                        }
                        // если в конце списка - ничего не делаем
                        selecting = true;       // продолжаем
                        break;
                    
                    case ConsoleKey.UpArrow:          // нажата стрелка вверх

                        // проверяем, не уперлись ли в потолок
                        if (currentItem > 1)
                        {
                            // Если не упёрлись, то 
                            // выводим текущий пункт меню белыми буквами на чёрном фоне
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.SetCursorPosition(menuOffset, curVert + currentItem - 1);
                            Console.Write(menuItems[currentItem - 1].itemName);

                            // переходим к следующему вверх пункту меню
                            currentItem--;

                            // выводим его чёрные буквы на белом фоне
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.SetCursorPosition(menuOffset, curVert + currentItem - 1);
                            Console.Write(menuItems[currentItem - 1].itemName);
                        }
                        // если в начале списка - ничего не делаем
                        selecting = true;       // продолжаем
                        break;
                    
                    default:
                        
                        // Если нажатая клавиша находится в списке клавиш меню
                        // То возвращаем её
                        foreach (var e in menuItems)
                        {
                            if (keyPress == e.itemKey) return keyPress;
                        }
                        // Если нет, то ничего не делаем
                        break;
                }   // end of switch (keyPress)

            } while (selecting); 
            return keyPress;
        }   // конец public ConsoleKey MenuSelect(MenuItem[] menuItems, int winWid, int curVert)
    }
}
