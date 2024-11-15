using System;
namespace Frontend.Model.ChessBoard
{
    public enum Position
    {
        a1, a2, a3, a4, a5, a6, a7, a8,
        b1, b2, b3, b4, b5, b6, b7, b8,
        c1, c2, c3, c4, c5, c6, c7, c8,
        d1, d2, d3, d4, d5, d6, d7, d8,
        e1, e2, e3, e4, e5, e6, e7, e8,
        f1, f2, f3, f4, f5, f6, f7, f8,
        g1, g2, g3, g4, g5, g6, g7, g8,
        h1, h2, h3, h4, h5, h6, h7, h8
    }

    public static class PositionMethods
    {
        // Gets the file as an integer 1 for 'a', 2 for 'b' ... 8 for 'h'
        public static int GetFile(this Position position)
        {
            return ((int)position / 8) + 1;
        }

        // Gets the rank as an integer 1 ... 8
        public static int GetRank(this Position position)
        {
            return ((int)position % 8) + 1;
        }

        // Gets the file as a char 'a' for 1, 'b' for 2 ... 'h' for 8
        public static char GetFileChar(this Position position)
        {
            return (char)('a' + ((int)position / 8));
        }

        // Get the Position from File and Rank enums
        public static Position Get(File file, Rank rank)
        {
            var position = (Position)(((int)file - 1) * 8 + ((int)rank - 1));
            return position;
        }

        // Get the Position from file and rank integers
        public static Position From(int file, int rank)
        {
            ValidateFileAndRank(file, rank);
            var position = (Position)((file - 1) * 8 + (rank - 1));
            return position;
        }

        public static Position FromString(string file, string rank)
        {
            if (file.Length != 1 || rank.Length != 1)
                throw new ArgumentException("File and rank must be single characters.");

            int fileIndex = file[0] - 'a' + 1;
            int rankIndex = rank[0] - '1' + 1;

            Console.WriteLine($"Converting {file}{rank} to fileIndex={fileIndex}, rankIndex={rankIndex}");
            return From(fileIndex, rankIndex);
        }

        // Check if position is a light square
        public static bool IsLightSquare(this Position position)
        {
            return ((int)position / 8 + (int)position % 8) % 2 == 0;
        }

        // Check if position is a dark square
        public static bool IsDarkSquare(this Position position)
        {
            return ((int)position / 8 + (int)position % 8) % 2 == 1;
        }

        // Private helper method to validate the file and rank
        private static void ValidateFileAndRank(int file, int rank)
        {
            if (file < 1 || file > 8 || rank < 1 || rank > 8)
                throw new ArgumentOutOfRangeException("File and rank must be between 1 and 8.");
        }

        public static string ToString(this Position position)
        {
            return $"{position.GetFileChar()}{position.GetRank()}";
        }
    }
}
