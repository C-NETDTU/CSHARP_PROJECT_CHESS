using System;
using System.Collections.Generic;
using System.Linq;
using Frontend.Model.ChessPiece;
using Frontend.Model.ChessMove;

namespace Frontend.Model.ChessBoard
{
    public class Board
    {
        public Dictionary<Position, IPiece> Pieces { get; set; }
        public Dictionary<Position, Square> Squares { get; set; }

        public AppliedMove? LastMove { get; set; }

        public Set Turn => LastMove?.Move.Piece.Set == Set.White ? Set.Black : Set.White;

        // This default constructor initializes the board with the initial pieces (see bottom of file)
        public Board() : this(InitialPieces) { }

        // This constructor initializes the board with the given pieces
        public Board(Dictionary<Position, IPiece> pieces, AppliedMove? lastMove = null, Set? turn = Set.White)
        {
            Pieces = new Dictionary<Position, IPiece>(pieces);
            Squares = Enum.GetValues(typeof(Position))
                          .Cast<Position>()
                          .ToDictionary(
                              position => position,
                              position => new Square(position, Pieces.ContainsKey(position) ? Pieces[position] : null)
                          );
        }

        // Access square by position
        public Square this[Position position] => Squares[position];

        // Access square by file and rank (enum version)
        public Square this[File file, Rank rank] => Squares[PositionMethods.Get(file, rank)];

        // Access square by file and rank (int version)
        public Square? Get(int file, int rank)
        {
            try
            {
                var position = PositionMethods.From(file, rank);
                return Squares[position];
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        // Find square containing given piece
        public Square? Find(IPiece piece)
        {
            return Squares.Values.FirstOrDefault(square => square.Piece == piece);
        }

        // Find all squares containing a specific type of piece and set (like WHITE or BLACK)
        public List<Square> Find<T>(Set set) where T : IPiece
        {
            return Squares.Values.Where(square => square.Piece != null && square.Piece.GetType() == typeof(T) && square.Piece.Set == set).ToList();
        }

        // Get all pieces belonging to a specific set (like WHITE or BLACK)
        public Dictionary<Position, IPiece> GetPieces(Set set)
        {
            return Pieces.Where(pair => pair.Value.Set == set).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
        /*
        // Should be implemented later. This method applies a piece effect on the board
        public Board Apply(PieceEffect effect)
        {
            return effect != null ? effect.ApplyOn(this) : this;
        }
        */

        private static readonly Dictionary<Position, IPiece> InitialPieces = new()
        {
            { Position.a8, new Rook(Set.Black) },
            { Position.b8, new Knight(Set.Black) },
            { Position.c8, new Bishop(Set.Black) },
            { Position.d8, new Queen(Set.Black) },
            { Position.e8, new King(Set.Black) },
            { Position.f8, new Bishop(Set.Black) },
            { Position.g8, new Knight(Set.Black) },
            { Position.h8, new Rook(Set.Black) },

            { Position.a7, new Pawn(Set.Black) },
            { Position.b7, new Pawn(Set.Black) },
            { Position.c7, new Pawn(Set.Black) },
            { Position.d7, new Pawn(Set.Black) },
            { Position.e7, new Pawn(Set.Black) },
            { Position.f7, new Pawn(Set.Black) },
            { Position.g7, new Pawn(Set.Black) },
            { Position.h7, new Pawn(Set.Black) },

            { Position.a2, new Pawn(Set.White) },
            { Position.b2, new Pawn(Set.White) },
            { Position.c2, new Pawn(Set.White) },
            { Position.d2, new Pawn(Set.White) },
            { Position.e2, new Pawn(Set.White) },
            { Position.f2, new Pawn(Set.White) },
            { Position.g2, new Pawn(Set.White) },
            { Position.h2, new Pawn(Set.White) },

            { Position.a1, new Rook(Set.White) },
            { Position.b1, new Knight(Set.White) },
            { Position.c1, new Bishop(Set.White) },
            { Position.d1, new Queen(Set.White) },
            { Position.e1, new King(Set.White) },
            { Position.f1, new Bishop(Set.White) },
            { Position.g1, new Knight(Set.White) },
            { Position.h1, new Rook(Set.White) },
    };
    }
}