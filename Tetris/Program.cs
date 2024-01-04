using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris {

    internal class Program {

        class Screen {
            public const byte height = 24;
            public const byte width = 52;
            public char[,] field = new char[height, width];

            public Screen() {
                clear();
            }

            public void display() {
                Console.SetCursorPosition(0, 0);
                for (int y = 0; y < height; y++) {
                    var str = new char[width];

                    for (int x = 0; x < width; x++) {
                        str[x] = field[y, x];
                    }

                    Console.WriteLine(str);
                }
            }

            public void set(string str, int y, int x) {
                int l = str.Length;
                for (int i = 0; i < l; i++) {
                    field[y, x+i] = str[i];
                }
            }

            public void clear() {
                for (int i = 0; i < height; i++) {
                    for (int j = 0; j < width; j++) {
                        this.field[i, j] = ' ';
                    }
                }
            }
        }

        class Стакан {
            public const byte height = 20;
            public const byte width = 10;
            public byte[,] m = new byte[height, width];

            public Стакан() {
                clear();
            }

            public void display(Screen s) {
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        if (m[y, x] == 0) {
                            s.field[y + 1, x*2 + 4] = ' ';
                            s.field[y + 1, x*2 + 5] = '.';
                        }
                        else if (m[y, x] == 1) {
                            s.field[y + 1, x * 2 + 4] = '[';
                            s.field[y + 1, x * 2 + 5] = ']';
                        }
                    }
                }
            }

            bool checkString(int i) {
                for (int j = 0; j < width; j++) {
                    if (m[i, j] == 0) return false;
                }
                return true;
            }

            void removeString(int i) {
                for (int j = 0; j < width; j++) {
                    for (int i1 = i; i1 > 0; i1--) {
                        m[i1, j] = m[i1 - 1, j];
                    }
                    m[0, j] = 0;
                }
            }

            public int update() {
                int i = height - 1;
                int c = 0;
                while (i >= 0) {
                    if (checkString(i)) {
                        removeString(i);
                        c++;
                    }
                    else {
                        i--;
                    }
                }

                return c;
            }

            public void displayBorder(Screen s) {
                for (int i = 0; i < Стакан.height + 1; i++) {
                    s.field[i + 1, 2] = '<';
                    s.field[i + 1, 3] = '!';
                    s.field[i + 1, 24] = '!';
                    s.field[i + 1, 25] = '>';
                }
                for (int i = 4; i < 24; i++) {
                    s.field[height + 1, i] = '=';
                    if (i % 2 == 0)
                        s.field[height + 2, i] = '\\';
                    else
                        s.field[height + 2, i] = '/';
                }

                /*for (int i = 5; i < 10; i++) {
                    for (int j = 5; j < 10; j++) {
                        s.field[i, j] = '*';
                    }
                }*/
            }

            public void clear() {
                for (int i = 0; i < height; i++) {
                    for (int j = 0; j < width; j++) {
                        m[i, j] = 0;
                    }
                }
            }
        }

        class Block {
            public static byte[][][,] tem;
            int x, y, r, type;
            
            static Block() {
                tem = new byte[7][][,];

                tem[0] = new byte[4][,];
                {
                    tem[0][0] = new byte[4, 4] {
                        {0,0,0,0},
                        {1,1,1,1},
                        {0,0,0,0},
                        {0,0,0,0}
                };
                    tem[0][1] = new byte[4, 4] {
                        {0,0,1,0},
                        {0,0,1,0},
                        {0,0,1,0},
                        {0,0,1,0}
                    };
                    tem[0][2] = new byte[4, 4]{
                        {0,0,0,0},
                        {0,0,0,0},
                        {1,1,1,1},
                        {0,0,0,0}
                    };
                    tem[0][3] = new byte[4, 4]{
                        {0,1,0,0},
                        {0,1,0,0},
                        {0,1,0,0},
                        {0,1,0,0}
                    };
                }

                tem[1] = new byte[4][,];
                {
                    tem[1][0] = new byte[3, 3] {
                        {0,0,1},
                        {1,1,1},
                        {0,0,0}
                    };
                    tem[1][1] = new byte[3, 3] {
                        {0,1,0},
                        {0,1,0},
                        {0,1,1}
                    };
                    tem[1][2] = new byte[3, 3] {
                        {0,0,0},
                        {1,1,1},
                        {1,0,0}
                    };
                    tem[1][3] = new byte[3, 3] {
                        {1,1,0},
                        {0,1,0},
                        {0,1,0}
                    };
                }

                tem[2] = new byte[4][,];
                {
                    tem[2][0] = new byte[3, 3] {
                        {1,0,0},
                        {1,1,1},
                        {0,0,0}
                    };
                    tem[2][1] = new byte[3, 3] {
                        {0,1,1},
                        {0,1,0},
                        {0,1,0}
                    };
                    tem[2][2] = new byte[3, 3] {
                        {0,0,0},
                        {1,1,1},
                        {0,0,1}
                    };
                    tem[2][3] = new byte[3, 3] {
                        {0,1,0},
                        {0,1,0},
                        {1,1,0}
                    };
                }

                tem[3] = new byte[4][,];
                {
                    tem[3][0] = new byte[3, 3] {
                        {0,1,0},
                        {1,1,1},
                        {0,0,0}
                    };
                    tem[3][1] = new byte[3, 3] {
                        {0,1,0},
                        {0,1,1},
                        {0,1,0}
                    };
                    tem[3][2] = new byte[3, 3] {
                        {0,0,0},
                        {1,1,1},
                        {0,1,0}
                    };
                    tem[3][3] = new byte[3, 3] {
                        {0,1,0},
                        {1,1,0},
                        {0,1,0}
                    };
                }

                tem[4] = new byte[4][,];
                {
                    tem[4][0] = new byte[3, 3] {
                        {1,1,0},
                        {0,1,1},
                        {0,0,0}
                    };
                    tem[4][1] = new byte[3, 3] {
                        {0,0,1},
                        {0,1,1},
                        {0,1,0}
                    };
                    tem[4][2] = new byte[3, 3] {
                        {0,0,0},
                        {1,1,0},
                        {0,1,1}
                    };
                    tem[4][3] = new byte[3, 3] {
                        {0,1,0},
                        {1,1,0},
                        {1,0,0}
                    };
                }

                tem[5] = new byte[4][,];
                {
                    tem[5][0] = new byte[3, 3] {
                        {0,1,1},
                        {1,1,0},
                        {0,0,0}
                    };
                    tem[5][1] = new byte[3, 3] {
                        {0,1,0},
                        {0,1,1},
                        {0,0,1}
                    };
                    tem[5][2] = new byte[3, 3] {
                        {0,0,0},
                        {0,1,1},
                        {1,1,0}
                    };
                    tem[5][3] = new byte[3, 3] {
                        {1,0,0},
                        {1,1,0},
                        {0,1,0}
                    };
                }

                tem[6] = new byte[4][,];
                {
                    tem[6][0] = new byte[2, 2] {
                        {1,1},
                        {1,1}
                    };
                    tem[6][1] = new byte[2, 2] {
                        {1,1},
                        {1,1}
                    };
                    tem[6][2] = new byte[2, 2] {
                        {1,1},
                        {1,1}
                    };
                    tem[6][3] = new byte[2, 2] {
                        {1,1},
                        {1,1}
                    };
                }
            }

            public Block(byte t) {
                type = t;
                if (type == 6) x = 4;
                else x = 3;
                y = 0;
                r = 0;
            }

            bool value(int iy, int ix, int dr = 0) {
                return tem[type][(r+dr+4) % 4][iy, ix] == 1;
            }

            byte size() {
                if (type == 0) {
                    return 4;
                }
                if (type == 6) {
                    return 2;
                }
                else {
                    return 3;
                }
            }

            void connect(Стакан st) {
                byte l = size();
                for (int i = 0; i < l; i++) {
                    for (int j = 0; j < l; j++) {
                        if (value(i, j)) {
                            st.m[i + y, j + x] = 1;
                        }
                    }
                }
            }

            public bool canPut(int dy, int dx, int dr, Стакан st) {
                byte l = size();
                for (int i = 0; i < l; i++) {
                    for (int j = 0; j < l; j++) {
                        if (value(i, j, dr)) {
                            if (y+dy+i >= Стакан.height || x+dx+j < 0 || x+dx+j >= Стакан.width || st.m[y+dy+i, x+dx+j] == 1) {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }

            public bool fall(Стакан st) {
                if (!canPut(1, 0, 0, st)) {
                    connect(st);
                    return false;
                }
                y++;
                return true;
            }

            public void move(int d, Стакан st) {
                if (canPut(0, d, 0, st)) x += d;
            }

            public void rotate(int d, Стакан st) {
                if (canPut(0, 0, d, st)) r = (r+d+4) % 4;
                else {
                    if (canPut(0, 1, d, st)) {
                        r = (r + d + 4) % 4;
                        x += 1;
                    }
                    else if (canPut(0, -1, d, st)) {
                        r = (r + d + 4) % 4;
                        x -= 1;
                    }
                    else if (canPut(-1, 0, d, st)) {
                        r = (r + d + 4) % 4;
                        y -= 1;
                    }
                    else if (canPut(0, 2, d, st)) {
                        r = (r + d + 4) % 4;
                        x += 2;
                    }
                    else if (canPut(0, -2, d, st)) {
                        r = (r + d + 4) % 4;
                        x -= 2;
                    }
                    else if (canPut(-2, 0, d, st)) {
                        r = (r + d + 4) % 4;
                        y -= 2;
                    }
                }
            }

            public void display(Screen s) {
                byte l = size();
                for (int i = 0; i < l; i++) {
                    for (int j = 0; j < l; j++) {
                        if (value(i, j)) {
                            int fy = i + y + 1, fx = (j + x) * 2 + 4;
                            s.field[fy, fx] = '[';
                            s.field[fy, fx+1] = ']';
                        }
                    }
                }
            }
        }

        class Leaderboard {

        }

        static void Main(string[] args) {
            Screen screen = new Screen();
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);
            Console.SetWindowSize(Screen.width, Screen.height + 1);

            int score = 0;
            int level = 1;
            int timerS = 0;
            int timerM = 0;

            int FPS = 100;
            int cFPS = 0;
            int speed = level+1;
            int start = 1;
            int cSpeed = 0;

            bool pause = false;

            Random rnd = new Random();

            Стакан стакан = new Стакан();
            стакан.displayBorder(screen);

            Block fblock = new Block((byte)rnd.Next(0, 7));
            byte nrnd = (byte)rnd.Next(0, 7);

            bool startScreen() {
                bool isStart = true;
                while (isStart) {
                    if (Console.KeyAvailable) {
                        ConsoleKey key = Console.ReadKey(true).Key;
                        if (key == ConsoleKey.Escape) {
                            Console.Clear();
                            return false;
                        }
                        if (key == ConsoleKey.D0 || key == ConsoleKey.NumPad0) {
                            newGame();
                            level = 0;
                            return true;
                        }
                        if (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1) {
                            newGame();
                            level = 1;
                            return true;
                        }
                        if (key == ConsoleKey.D2 || key == ConsoleKey.NumPad2) {
                            newGame();
                            level = 2;
                            return true;
                        }
                        if (key == ConsoleKey.D3 || key == ConsoleKey.NumPad3) {
                            newGame();
                            level = 3;
                            return true;
                        }
                        if (key == ConsoleKey.D4 || key == ConsoleKey.NumPad4) {
                            newGame();
                            level = 4;
                            return true;
                        }
                        if (key == ConsoleKey.D5 || key == ConsoleKey.NumPad5) {
                            newGame();
                            level = 5;
                            return true;
                        }
                        if (key == ConsoleKey.D6 || key == ConsoleKey.NumPad6) {
                            newGame();
                            level = 6;
                            return true;
                        }
                        if (key == ConsoleKey.D7 || key == ConsoleKey.NumPad7) {
                            newGame();
                            level = 7;
                            return true;
                        }
                        if (key == ConsoleKey.D8 || key == ConsoleKey.NumPad8) {
                            newGame();
                            level = 8;
                            return true;
                        }
                        if (key == ConsoleKey.D9 || key == ConsoleKey.NumPad9) {
                            newGame();
                            level = 9;
                            return true;
                        }
                    }

                    screen.clear();

                    screen.set("   []", 8, 20);
                    screen.set("   TETRIS", 9, 20);
                    screen.set("       []", 10, 20);
                    screen.set("LEVELS (0-9)", 12, 20);
                    screen.set("LEADERBOARD (T)", 13, 20);
                    screen.set("EXIT (ESC)", 14, 20);


                    screen.display();
                }
                screen = new Screen();

                return true;
            }

            void updateBlock() {
                int c = стакан.update();
                if (c == 1)
                    score += 100;
                else if (c == 2)
                    score += 300;
                else if (c == 3)
                    score += 700;
                else if (c == 4)
                    score += 1500;

                if (score >= start * 5000) {
                    speed = (start * level) + 1;
                    if (start < 11) {
                        start += 1;
                    }
                }

                fblock = new Block(nrnd);
                nrnd = (byte)rnd.Next(0, 7);
            }

            void newGame() {
                screen.clear();
                стакан.clear();
                стакан.displayBorder(screen);
                cSpeed = 0;
                cFPS = 0;
                score = 0;
                timerS = 0;
                timerM = 0;
                start = 1;
                nrnd = (byte)rnd.Next(0, 7);
                fblock = new Block(nrnd);
                nrnd = (byte)rnd.Next(0, 7);
                pause = false;
            }

            if (!startScreen())
                return;

            while (true) {
                if (Console.KeyAvailable) {
                    ConsoleKey key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Escape) {
                        if (!startScreen())
                            return;
                    }
                    else if (key == ConsoleKey.LeftArrow)
                        fblock.move(-1, стакан);
                    else if (key == ConsoleKey.RightArrow)
                        fblock.move(1, стакан);
                    else if (key == ConsoleKey.UpArrow)
                        fblock.rotate(1, стакан);
                    else if (key == ConsoleKey.DownArrow) {
                        if (!fblock.fall(стакан))
                            updateBlock();
                        else score++;
                    }
                    else if (key == ConsoleKey.Spacebar) {
                        while (fblock.fall(стакан)) ;
                        updateBlock();
                    }
                    else if (key == ConsoleKey.R) {
                        newGame();
                    }
                    else if (key == ConsoleKey.P) {
                        pause = !pause;
                    }
                }
                else if (!pause) {
                    int[] d = { 4, 3, 3, 3, 3, 3, 2 };
                    стакан.display(screen);
                    fblock.display(screen);
                    screen.set($"SCORE: {score}", 1, 34);
                    screen.set($"LEVEL: {level}", 3, 34);
                    screen.set($"TIME: {timerM}:{timerS} ", 5, 34);
                    screen.set($"NEXT:", 8, 34);
                    for (int i = 10; i < 14; i++) {
                        for (int j = 36; j < 44; j++) {
                            screen.field[i, j] = ' ';
                        }
                    }
                    for (int i = 0; i < d[nrnd]; i++) {
                        for (int j = 0; j < d[nrnd]; j++) {
                            if (Block.tem[nrnd][0][i, j] == 0) {
                                screen.field[10 + i, 36 + j * 2] = ' ';
                                screen.field[10 + i, 37 + j * 2] = ' ';
                            }
                            else {
                                screen.field[10 + i, 36 + j * 2] = '[';
                                screen.field[10 + i, 37 + j * 2] = ']';
                            }
                        }
                    }

                    screen.display();

                    if (cSpeed >= 1000 / speed) {
                        cSpeed = 0;
                        if (!fblock.fall(стакан)) {
                            updateBlock();
                        }
                    }
                    if (cFPS == 100) {
                        timerS += 1;
                        if (timerS >= 60) {
                            timerM += 1;
                            timerS = 0;
                            if (timerM % 100 >= 60) timerM += 40;
                        }
                        cFPS = 0;
                    }

                    cFPS += 1;
                    cSpeed += 1000 / FPS;
                    Thread.Sleep(1000 / FPS);
                }

                if (pause) {
                    Console.SetCursorPosition(21, 10);
                    Console.Write("╔═══════╗");
                    Console.SetCursorPosition(21, 11);
                    Console.Write("║ PAUSE ║");
                    Console.SetCursorPosition(21, 12);
                    Console.Write("╚═══════╝");

                    ConsoleKey key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.P) {
                        pause = false;
                    }
                }

                if (!fblock.canPut(0, 0, 0, стакан)) {
                    Console.SetCursorPosition(17, 8);
                    Console.Write("╔════════════════╗");
                    Console.SetCursorPosition(17, 9);
                    Console.Write("║                ║");
                    Console.SetCursorPosition(17, 10);
                    Console.Write("║   GAME OVER!   ║");
                    Console.SetCursorPosition(17, 11);
                    Console.Write("║                ║");
                    Console.SetCursorPosition(17, 12);
                    Console.Write("║  Again? (y/n)  ║");
                    Console.SetCursorPosition(17, 13);
                    Console.Write("║                ║");
                    Console.SetCursorPosition(17, 14);
                    Console.Write("╚════════════════╝");

                    ConsoleKey key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Y) {
                        newGame();
                    }
                    else if (key == ConsoleKey.N) {
                        if (!startScreen())
                            return;
                    }
                }
            }
        }
    }
}
