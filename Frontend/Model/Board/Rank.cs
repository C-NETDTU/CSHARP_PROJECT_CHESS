using System;
namespace Frontend.Model.ChessBoard
{
    public enum Rank
    {
        // The enum is 1-indexed to match the chess board
        r1 = 1, r2, r3, r4, r5, r6, r7, r8
    }

    public static class RankMethods
    {
        // Get a Position from int file and int rank. 
        // Example: Rank.r1.Get(File.a) returns Position.a1
        public static Position Get(this Rank rank, File file)
        {
            ValidateFile(file);
            return (Position)((int)file * 8 + (int)rank);
        }
        
        public static int GetRankInt(this Position position)
        {
            return ((int)position % 8) + 1;
        }

        // Private helper to validate the file
        private static void ValidateFile(File file)
        {
            if (file < File.a || file > File.h)
                throw new ArgumentOutOfRangeException("File must be between a and h.");
        }

    }

}