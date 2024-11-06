using System;
namespace Frontend.Model.ChessBoard
{
    public enum File
    {
        // The enum is 1-indexed to match the chess board
        a = 1, b, c, d, e, f, g, h
    }

    public static class FileMethods

    {
        // Method to get a Position from int file and int rank. 
        // Example: File.a.Get(1) returns Position.a1
        public static Position Get(this File file, int rank)
        {
            ValidateRank(rank);
            return (Position)((int)file * 8 + (rank - 1));
        }
        // Method to get a File from a File and a Rank enum
        // Example: File.a.Get(Rank.r1) returns Position.a1
        public static Position Get(this File file, Rank rank)
        {
            return (Position)((int)file * 8 + (int)rank);
        }

        // Method to validate the rank
        private static void ValidateRank(int rank)
        {
            if (rank < 1 || rank > 8)
                throw new ArgumentOutOfRangeException("Rank must be between 1 and 8.");
        }

    }

}