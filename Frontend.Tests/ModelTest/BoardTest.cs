using System.Collections.Generic;
using Xunit;
using Frontend.Model.ChessBoard;
using ChessFile = Frontend.Model.ChessBoard.File;
using Frontend.Model.ChessPiece;

namespace Frontend.Tests
{
    public class BoardTest
    {
        [Fact]
        public void Board_InitializeWithDefaultPieces()
        {
            Board board = new Board();
            Assert.Equal(32, board.Pieces.Count);
        }

        [Fact]
        public void Board_InitializeWithGivenPieces()
        {
            var pieces = new Dictionary<Position, IPiece>
            {
                { Position.e1, new King(Set.White) },
                { Position.h1, new Rook(Set.White) }
            };

            Board board = new Board(pieces);

            Assert.Equal(2, board.Pieces.Count);
            Assert.Equal(pieces, board.Pieces);
        }

        [Fact]
        public void Square_Access_ByPosition_Works()
        {
            var pieces = new Dictionary<Position, IPiece>
            {
                { Position.e1, new King(Set.White) },
                { Position.h1, new Rook(Set.White) }
            };
            var board = new Board(pieces);

            var square = board[Position.e1];

            Assert.Equal(Position.e1, square.Position);
            Assert.IsType<King>(square.Piece);
        }

        [Fact]
        public void Square_Access_ByFileAndRank_Works()
        {
            var pieces = new Dictionary<Position, IPiece>
            {
                { Position.e1, new King(Set.White) },
                { Position.h1, new Rook(Set.White) }
            };

            var board = new Board(pieces);

            var square = board[ChessFile.e, Rank.r1];

            Assert.Equal(Position.e1, square.Position); 
            Assert.IsType<King>(square.Piece);
        }


        [Fact]
        public void Square_Access_ByIntegers_Works()
        {
            var pieces = new Dictionary<Position, IPiece>
            {
                { Position.e1, new King(Set.White) },
                { Position.h1, new Rook(Set.White) }
            };
            var board = new Board(pieces);
            
            var square = board.Get(5, 1);

            Assert.Equal(Position.e1, square.Position);
            Assert.IsType<King>(square.Piece);
        }

        [Fact]
        public void Get_ShouldReturnNullForInvalidFileOrRank()
        {
            var board = new Board();

            var square = board.Get(9, 1);

            Assert.Null(square);
        }

        [Fact]
        public void Find_ShouldReturnCorrectSquareForPiece()
        {
            var board = new Board();
            var king = board.Pieces[Position.e1];

            var kingSquare = board.Find(king);

            Assert.Equal(Position.e1, kingSquare.Position);
        }

        [Fact]
        public void Find_ShouldReturnNullIfPieceIsNotOnBoard()
        {
            var board = new Board();

            var square = board.Find(new King(Set.Black));

            Assert.Null(square);
        }

        [Fact]
        public void Find_ShouldReturnAllSquaresWithPawnsForSpecificSet()
        {
            var board = new Board();

            var whitePawns = board.Find<Pawn>(Set.White);

            Assert.Equal(8, whitePawns.Count);
        }

        [Fact]
        public void GetPieces_ShouldReturnAllPiecesForSpecificSet()
        {
            var board = new Board();

            var whitePieces = board.GetPieces(Set.White);

            Assert.Equal(16, whitePieces.Count);
        }

    }
}

