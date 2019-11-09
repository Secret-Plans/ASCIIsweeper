using System;
using System.Collections.Generic;
using Pastel;

namespace SweepMiner
{
    class Program
    {
        const string version = "1.1.3";

        public static Map FloodFill(Map m, int startX, int startY)
        {
            Queue<Coord> q = new Queue<Coord>();
            q.Enqueue(new Coord(startX, startY));

            while(q.Count > 0)
            {
                Coord c = q.Dequeue();
                if(m.tiles[c.x, c.y].surroundingMines != 0)
                {
                    continue;
                }
                
                for(int relativeX = -1; relativeX != 2; relativeX++)
                {
                    for(int relativeY = -1; relativeY != 2; relativeY++)
                    {
                        int nearX = c.x + relativeX;
                        int nearY = c.y + relativeY;

                        if (nearX < 0 || nearX > m.width - 1 || nearY < 0 || nearY > m.height - 1)
                        {
                            continue;
                        }
                        
                        if(!m.tiles[nearX, nearY].uncovered && !m.tiles[nearX, nearY].flagged)
                        {
                            q.Enqueue(new Coord(nearX, nearY));
                        }

                        m.tiles[nearX, nearY].uncovered = true;
                    }
                }
            }

            return m;
        }

        public static void SetupUI(Map m)
        {
            Console.SetCursorPosition(0, 1);
            Console.Write('┌' + new string('─', m.width * 2 - 1) + '┐');
            for(int y = 0; y != m.height; y++)
            {
                Console.SetCursorPosition(0, y + 2);
                Console.Write('│' + new string(' ', m.width * 2 - 1) + '│');
            }
            Console.SetCursorPosition(0, m.height + 2);
            Console.Write('└' + new string('─', m.width * 2 - 1) + '┘');
        }

        public static void RenderStep(Dictionary<int, string> colours, Map m, int cursorX, int cursorY, int flags, bool lost, bool won)
        {
            Console.SetCursorPosition(0, m.height + 3);
            if(won || lost) { Console.Write("[R]estart"); }
            else { Console.Write("Flags: {0} ", flags.ToString().Pastel("E74856")); }

            Console.SetCursorPosition(m.width * 2 -1, m.height + 3);
            Console.Write(won ? ":)".Pastel("F9F1A5") : lost ? ":(".Pastel("F9F1A5") : ":|".Pastel("F9F1A5"));

            for (int y = 0; y != m.height; y++)
            {
                string line = "";
                for(int x = 0; x != m.width; x++)
                {
                    if (m.tiles[x, y].uncovered || lost)
                    {
                        if (m.tiles[x, y].mine)
                        {
                            line += '*';
                        }
                        else if(m.tiles[x,y].surroundingMines == 0)
                        {
                            line += '.';
                        }
                        else
                        {
                            line += m.tiles[x, y].surroundingMines.ToString().Pastel(colours[m.tiles[x,y].surroundingMines]);
                        }
                    }

                    else if(m.tiles[x, y].flagged)
                    {
                        line += "■".Pastel("E74856");
                    }

                    else
                    {
                        line += '■';
                    }
                    if(x < m.width - 1)
                    {
                        line += ' ';
                    }
                }
                Console.SetCursorPosition(1, y + 2);
                Console.Write(line);
            }

            Console.SetCursorPosition(cursorX * 2 + 1, cursorY + 2);
        }

        static void Game()
        {
            Console.Clear();
            Console.WriteLine("ASCIIsweeper - v{0}\n", version);
            Console.WriteLine("[E]asy, [M]ed, [H]ard");
            Console.Write("Select Difficulty: ");
            bool valid = false;
            Map m = new Map(1, 1, 1);
            do
            {
                char input = Console.ReadKey(true).KeyChar;
                input = char.ToLower(input);
                switch (input)
                {
                    case 'e':
                        m = new Map(9, 9, 10);
                        valid = true;
                        break;
                    case 'm':
                        m = new Map(16, 16, 40);
                        valid = true;
                        break;
                    case 'h':
                        m = new Map(30, 16, 99);
                        valid = true;
                        break;
                        //case 'c':
                        //    Console.WriteLine("Map Width: ");
                        //    Console.ReadLine()
                        //    break;
                }
            }
            while (!valid);

            Console.Clear();
            Console.Write("ASCIIsweeper - v{0}", version);

            int x = 0;
            int y = 0;
            int flags = m.mines;
            bool lost = false;
            bool won = false;
            bool firstClick = true;

            Dictionary<int, string> colours = new Dictionary<int, string>();
            colours.Add(1, "0037DA");
            colours.Add(2, "16C60C");
            colours.Add(3, "E74856");
            colours.Add(4, "B4009E");
            colours.Add(5, "C50F1F");
            colours.Add(6, "61D6D6");
            colours.Add(7, "F9F1A5");
            colours.Add(8, "F2F2F2");

            SetupUI(m);

            while (true)
            {

                won = m.AllMinesCleared;
                RenderStep(colours, m, x, y, flags, lost, won);
                if (lost || won)
                {
                    break;
                }
                char key = Console.ReadKey(true).KeyChar;
                key = char.ToLower(key);
                switch (key)
                {
                    case 'w':
                        if (y > 0) { y--; }
                        break;
                    case 'a':
                        if (x > 0) { x--; }
                        break;
                    case 's':
                        if (y < m.height - 1) { y++; }
                        break;
                    case 'd':
                        if (x < m.width - 1) { x++; }
                        break;
                    case 'e':
                        if (!m.tiles[x, y].uncovered && !m.tiles[x, y].flagged)
                        {
                            if (firstClick)
                            {
                                m.Generate(x, y);
                                m.SetNumbers();
                                firstClick = false;
                            }

                            if (m.tiles[x, y].surroundingMines == 0)
                            {
                                m = FloodFill(m, x, y);
                            }
                            else if (m.tiles[x, y].mine)
                            {
                                lost = true;
                            }
                            else
                            {
                                m.tiles[x, y].uncovered = true;
                            }
                        }
                        break;
                    case 'q':
                        if (flags > 0 && !m.tiles[x, y].flagged && !m.tiles[x, y].uncovered)
                        {
                            m.tiles[x, y].flagged = true;
                            flags -= 1;
                        }
                        else if (m.tiles[x, y].flagged)
                        {
                            m.tiles[x, y].flagged = false;
                            flags += 1;
                        }
                        break;
                }
            }
        }

        static void Main(string[] args)
        {
            char input = '?';
            do
            {
                Game();

                input = Console.ReadKey(true).KeyChar;
                input = char.ToLower(input);
            }
            while (input == 'r');
        }
    }
}