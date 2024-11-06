using Xunit;
using Frontend.Model.ChessBoard;
using Frontend.Model.ChessPiece;
using Frontend.Model.ChessMove;
using System.Collections.Generic;

namespace Frontend.Tests
{
    public class MoveTests
    {
        [Fact]
        public void ApplyMove_ShouldUpdateBoardState()
        {
            var pieces = new Dictionary<Position, IPiece>
            {
                { Position.e2, new Pawn(Set.White) },
            };

            var board = new Board(pieces);
            
            var move = new Move(pieces[Position.e2], Position.e2, Position.e4);
            var updatedBoard = move.ApplyOn(board);

            Assert.False(updatedBoard.Pieces.ContainsKey(Position.e2));
            Assert.True(updatedBoard.Pieces.ContainsKey(Position.e4)); 
            Assert.IsType<Pawn>(updatedBoard.Pieces[Position.e4]); 
            Assert.Equal(Set.White, updatedBoard.Pieces[Position.e4].Set);
        }
        
        [Fact]
        public void ApplyMove_ShouldRemoveOpponentsPiece()
        {
            var pieces = new Dictionary<Position, IPiece>
            {
                { Position.e4, new Pawn(Set.White) },
                { Position.d5, new Pawn(Set.Black) }
            };

            var board = new Board(pieces);

            var captureMove = new Capture(pieces[Position.e4], Position.d5);
            var updatedBoard = captureMove.ApplyOn(board);

            Assert.False(updatedBoard.Pieces.ContainsKey(Position.d5));
            Assert.True(updatedBoard.Pieces.ContainsKey(Position.e4));
        }

        
        [Fact]
        public void ApplyWhiteKingSideCastle_ShouldMoveKingAndRook() 
        {
            var pieces = new Dictionary<Position, IPiece>
            {
                { Position.e1, new King(Set.White) },
                { Position.h1, new Rook(Set.White) }
            };

            var board = new Board(pieces);
            
            var castleMove = new KingSideCastle(pieces[Position.e1], Position.e1, Position.g1);
            var updatedBoard = castleMove.ApplyOn(board);

            Assert.False(updatedBoard.Pieces.ContainsKey(Position.e1));
            Assert.False(updatedBoard.Pieces.ContainsKey(Position.h1));
            Assert.IsType<King>(updatedBoard.Pieces[Position.g1]);
            Assert.Equal(Set.White, updatedBoard.Pieces[Position.g1].Set);
            Assert.IsType<Rook>(updatedBoard.Pieces[Position.f1]);
            Assert.Equal(Set.White, updatedBoard.Pieces[Position.f1].Set);
        }

        [Fact]
        public void ApplyBlackKingSideCastle_ShouldMoveKingAndRook()
        {
            var pieces = new Dictionary<Position, IPiece>
            {
                { Position.e8, new King(Set.Black) },
                { Position.h8, new Rook(Set.Black) }
            };

            var board = new Board(pieces);

            var castleMove = new KingSideCastle(pieces[Position.e8], Position.e8, Position.g8);
            var updatedBoard = castleMove.ApplyOn(board);

            Assert.False(updatedBoard.Pieces.ContainsKey(Position.e8));
            Assert.False(updatedBoard.Pieces.ContainsKey(Position.h8));
            Assert.IsType<King>(updatedBoard.Pieces[Position.g8]);
            Assert.IsType<Rook>(updatedBoard.Pieces[Position.f8]);
            Assert.Equal(Set.Black, updatedBoard.Pieces[Position.g8].Set);
            Assert.Equal(Set.Black, updatedBoard.Pieces[Position.f8].Set);
        }

        [Fact]
        public void ApplyWhiteQueenSideCastle_ShouldMoveKingAndRook() 
        {
            var pieces = new Dictionary<Position, IPiece>
            {
                { Position.e1, new King(Set.White) },
                { Position.a1, new Rook(Set.White) }
            };

            var board = new Board(pieces);

            var castleMove = new QueenSideCastle(pieces[Position.e1], Position.e1, Position.c1);
            var updatedBoard = castleMove.ApplyOn(board);

            Assert.False(updatedBoard.Pieces.ContainsKey(Position.e1));
            Assert.False(updatedBoard.Pieces.ContainsKey(Position.a1));
            Assert.IsType<King>(updatedBoard.Pieces[Position.c1]);
            Assert.IsType<Rook>(updatedBoard.Pieces[Position.d1]);
            Assert.Equal(Set.White, updatedBoard.Pieces[Position.c1].Set);
            Assert.Equal(Set.White, updatedBoard.Pieces[Position.d1].Set);
        }

        [Fact]
        public void ApplyBlackQueenSideCastle_ShouldMoveKingAndRook()
        {
            var pieces = new Dictionary<Position, IPiece>
            {
                { Position.e8, new King(Set.Black) },
                { Position.a8, new Rook(Set.Black) }
            };

            var board = new Board(pieces);

            var castleMove = new QueenSideCastle(pieces[Position.e8], Position.e8, Position.c8);
            var updatedBoard = castleMove.ApplyOn(board);

            Assert.False(updatedBoard.Pieces.ContainsKey(Position.e8));
            Assert.False(updatedBoard.Pieces.ContainsKey(Position.a8));
            Assert.IsType<King>(updatedBoard.Pieces[Position.c8]);
            Assert.IsType<Rook>(updatedBoard.Pieces[Position.d8]);
            Assert.Equal(Set.Black, updatedBoard.Pieces[Position.c8].Set);
            Assert.Equal(Set.Black, updatedBoard.Pieces[Position.d8].Set);
        }

        [Fact]
        public void ApplyPromotionForWhite_ShouldSpawnQueen()
        {
            var pieces = new Dictionary<Position, IPiece>
            {

            };
            var board = new Board(pieces);

            var promotionMove = new Promotion(new Queen(Set.White), Position.e8);
            var updatedBoard = promotionMove.ApplyOn(board);

            Assert.True(updatedBoard.Pieces.ContainsKey(Position.e8));
            Assert.IsType<Queen>(updatedBoard.Pieces[Position.e8]);
            Assert.Equal(Set.White, updatedBoard.Pieces[Position.e8].Set);
        }

        [Fact]
        public void ApplyPromotionForBlack_ShouldSpawnQueen()
        {

            var pieces = new Dictionary<Position, IPiece>
            {

            };
            var board = new Board(pieces);

            var promotionMove = new Promotion(new Queen(Set.White), Position.e1);
            var updatedBoard = promotionMove.ApplyOn(board);

            Assert.True(updatedBoard.Pieces.ContainsKey(Position.e1));
            Assert.IsType<Queen>(updatedBoard.Pieces[Position.e1]);
            Assert.Equal(Set.White, updatedBoard.Pieces[Position.e1].Set);
        }
    }
}