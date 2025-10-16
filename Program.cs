using System;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        while (true)
        {
            PlayGame();
            Console.Write("\nSpil igen? (y/n): ");
            var again = Console.ReadLine()?.Trim().ToLower();
            if (again != "y") break;
            Console.Clear();
        }
    }

    static void PlayGame()
    {
        const int size = 5;          // brætstørrelse (stretch: gør dynamisk)
        int attempts = 6;            // antal forsøg (stretch: sværhedsgrad)
        char[,] board = InitBoard(size);

        // Placér skat
        var rnd = new Random();
        int treasureRow = rnd.Next(0, size);
        int treasureCol = rnd.Next(0, size);

        Console.WriteLine("=== Treasure Hunt ===");
        Console.WriteLine($"Find skatten på et {size}x{size}-bræt. Du har {attempts} forsøg.");
        PrintBoard(board);

        while (attempts > 0)
        {
            // Læs gæt
            var (row, col, ok) = ReadGuess(size);
            if (!ok) { continue; } // ugyldigt input → prøv igen uden at miste forsøg

            // Tjek hit
            if (row == treasureRow && col == treasureCol)
            {
                Console.WriteLine("\n🎉 Du fandt skatten! Godt gået!");
                // (stretch) gem high score
                // Vis bræt med skat markeret:
                board[row, col] = '*';
                PrintBoard(board, showTreasure: false);
                return;
            }

            // Miss → markér og giv hint
            if (board[row, col] == 'O')
            {
                Console.WriteLine("Du har allerede gættet der. Vælg et andet felt.");
            }
            else
            {
                board[row, col] = 'O';
                int distance = Manhattan(row, col, treasureRow, treasureCol);
                Console.WriteLine($"Miss. Afstand til skatten: {distance}");
                // (stretch) Hot/Cold baseret på distance
                attempts--;
            }

            Console.WriteLine($"Forsøg tilbage: {attempts}");
            PrintBoard(board);
        }

        Console.WriteLine("\nØv! Forsøg brugt. Du fandt ikke skatten.");
        // (valgfrit) afslør skatten
        // Console.WriteLine($"Skatten lå på: {treasureRow} {treasureCol}");
    }

    static char[,] InitBoard(int size)
    {
        var board = new char[size, size];
        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                board[r, c] = '.';
        return board;
    }

    static void PrintBoard(char[,] board, bool showTreasure = false)
    {
        int size = board.GetLength(0);
        Console.WriteLine();
        // overskrift med kolonneindeks
        Console.Write("   ");
        for (int c = 0; c < size; c++) Console.Write(c + " ");
        Console.WriteLine();
        for (int r = 0; r < size; r++)
        {
            Console.Write(r + "  ");
            for (int c = 0; c < size; c++)
            {
                Console.Write(board[r, c] + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    static (int row, int col, bool ok) ReadGuess(int size)
    {
        Console.Write("Indtast gæt (række kolonne): ");
        var line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return (-1, -1, false);

        var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2) { Console.WriteLine("Skriv to tal adskilt af mellemrum."); return (-1, -1, false); }

        bool okR = int.TryParse(parts[0], out int r);
        bool okC = int.TryParse(parts[1], out int c);
        if (!okR || !okC) { Console.WriteLine("Kun heltal, tak."); return (-1, -1, false); }
        if (r < 0 || r >= size || c < 0 || c >= size)
        {
            Console.WriteLine($"Koordinater skal være mellem 0 og {size - 1}.");
            return (-1, -1, false);
        }
        return (r, c, true);
    }

    static int Manhattan(int r1, int c1, int r2, int c2)
        => Math.Abs(r1 - r2) + Math.Abs(c1 - c2);
}
