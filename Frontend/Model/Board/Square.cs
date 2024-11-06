#nullable enable
using System;
using Frontend.Model.ChessPiece;
namespace Frontend.Model.ChessBoard
{
    public class Square{
        public Position Position { get; }
        public IPiece? Piece { get; }

        // Get file and rank of square from position
        public int File => Position.GetFile();
        public int Rank => Position.GetRank();

        public bool IsDark => Position.IsDarkSquare();
        public bool IsEmpty => Piece == null;
        public bool IsNotEmpty => !IsEmpty;

        public bool HasPiece(Set set) => Piece?.Set == set;
        public bool HasWhitePiece => Piece?.Set == Set.White;
        public bool HasBlackPiece => Piece?.Set == Set.Black;

        public Square(Position position, IPiece? piece = null)
        {
            Position = position;
            Piece = piece;
        }
        public override string ToString()
        {
            return $"{Position.GetFileChar()}{Rank}";
        }
    }
}