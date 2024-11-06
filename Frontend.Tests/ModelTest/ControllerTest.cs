using System.Collections.Generic;
using Xunit;
using Frontend.Model.ChessBoard;
using Frontend.Model.ChessPiece;
using Frontend.Model.ChessMove;
using Frontend.Model.Game;

public class GameManagerTests
{
    [Fact]
    public void GameManager_InitializesCorrectly()
    {
        // Arrange
        var gameManager = new GameManager();

        // Act & Assert
        Assert.NotNull(gameManager.CurrentBoard);               // Ensure board is initialized
        Assert.NotNull(gameManager.chessGame);                 // Ensure chessGame is initialized
        Assert.Empty(gameManager.gameHistory);         // No moves should be in history initially
        Assert.Equal(Set.White, gameManager.CurrentTurn);       // White should start first
    }

    [Fact]
    public void GameController_GetValidMoves_ReturnsListOfBoardMoves()
    {
        // Arrange
        var gameManager = new GameManager();

        // The board is now be in initial position, we'll check valid moves for pawn on e2.

        var validMoves = gameManager.GetLegalMoves(Position.e2);

        // Act & Assert
        Assert.NotEmpty(validMoves);                            // Ensure valid moves are returned
        Assert.Equal(2, validMoves.Count);                       // There should be 2 valid moves for a pawn on e2
        Assert.All(validMoves, move => Assert.IsType<BoardMove>(move)); // Ensure all moves are of type BoardMove

        // Check if the moves are correct
        Assert.Contains(validMoves, move => move.To == Position.e3); // Pawn can move to e3
        Assert.Contains(validMoves, move => move.To == Position.e4); // Pawn can move to e4

        //we'll also check for the black pawn on e7 - this should return an empty list

        validMoves = gameManager.GetLegalMoves(Position.e7);
        Assert.Empty(validMoves);
    }

    [Fact]
    public void GameManager_ApplyOn_UpdateBoardCorrectly()
    {
        // Arrange
        var gameManager = new GameManager();

        // Act
        var validMoves = gameManager.GetLegalMoves(Position.e2);
        var move = validMoves.Find(m => m.To == Position.e4);
        gameManager.ApplyMove(move);

        // Assert
        Assert.Equal(Set.Black, gameManager.CurrentTurn); // Turn should change to black
        Assert.Null(gameManager.CurrentBoard[Position.e2].Piece); // e2 should now be empty
        Assert.NotNull(gameManager.CurrentBoard[Position.e4].Piece); // e4 should now have a piece

        Assert.Equal(move.Piece, gameManager.CurrentBoard[Position.e4].Piece); // The piece on e4 should be the same as the one moved

        // we'll also check if the move is added to the history
        Assert.Single(gameManager.gameHistory); // Only one move should be in history
    }

    [Fact]
    public void GameManager_ApplyMove_CaptureIsHandled()
    {
        // Arrange
        var gameManager = new GameManager();

        // Act
        var validMoves = gameManager.GetLegalMoves(Position.e2);
        var move = validMoves.Find(m => m.To == Position.e4);
        gameManager.ApplyMove(move);

        validMoves = gameManager.GetLegalMoves(Position.d7);
        move = validMoves.Find(m => m.To == Position.d5);
        gameManager.ApplyMove(move);

        validMoves = gameManager.GetLegalMoves(Position.e4);
        move = validMoves.Find(m => m.To == Position.d5);
        gameManager.ApplyMove(move);

        // Assert
        Assert.Equal(Set.Black, gameManager.CurrentTurn); // Turn should change to black
        Assert.Null(gameManager.CurrentBoard[Position.e4].Piece); // e4 should now be empty
        Assert.NotNull(gameManager.CurrentBoard[Position.d5].Piece); // d5 should now have a piece

        Assert.Equal(move.Piece, gameManager.CurrentBoard[Position.d5].Piece); // The piece on d5 should be the same as the one moved

        // we'll also check if the move is added to the history
        Assert.Equal(3, gameManager.gameHistory.Count); // 3 moves should be in history
    }

    [Fact]
    public void GameManager_ApplyMove_CastlingIsHandled()
    {
        // Arrange
        var gameManager = new GameManager();

        // Act
        var validMoves = gameManager.GetLegalMoves(Position.g1);
        var move = validMoves.Find(m => m.To == Position.f3);
        gameManager.ApplyMove(move);

        validMoves = gameManager.GetLegalMoves(Position.g8);
        move = validMoves.Find(m => m.To == Position.f6);
        gameManager.ApplyMove(move);

        validMoves = gameManager.GetLegalMoves(Position.g2);
        move = validMoves.Find(m => m.To == Position.g3);
        gameManager.ApplyMove(move);

        validMoves = gameManager.GetLegalMoves(Position.g7);
        move = validMoves.Find(m => m.To == Position.g6);
        gameManager.ApplyMove(move);

        validMoves = gameManager.GetLegalMoves(Position.f1);
        move = validMoves.Find(m => m.To == Position.g2);
        gameManager.ApplyMove(move);

        validMoves = gameManager.GetLegalMoves(Position.f8);
        move = validMoves.Find(m => m.To == Position.g7);
        gameManager.ApplyMove(move);

        validMoves = gameManager.GetLegalMoves(Position.e1);
        move = validMoves.Find(m => m.To == Position.g1);
        gameManager.ApplyMove(move);

        // Assert
        Assert.Equal(Set.Black, gameManager.CurrentTurn); // Turn should change to black
        Assert.Null(gameManager.CurrentBoard[Position.e1].Piece); // e1 should now be empty
        Assert.Null(gameManager.CurrentBoard[Position.h1].Piece); // h1 should now be empty

        Assert.NotNull(gameManager.CurrentBoard[Position.f1].Piece); // f1 should now have a piece
        Assert.NotNull(gameManager.CurrentBoard[Position.g1].Piece); // g1 should now have a piece

        Assert.Equal(move.Piece, gameManager.CurrentBoard[Position.g1].Piece); // The piece on g1 should be the same as the one moved
        Assert.IsType<Rook>(gameManager.CurrentBoard[Position.f1].Piece); // The piece on f1 should be a rook

        // we'll also check if the move is added to the history
        Assert.Equal(7, gameManager.gameHistory.Count); // 5 moves should be in history

        validMoves = gameManager.GetLegalMoves(Position.e8);
        move = validMoves.Find(m => m.To == Position.g8);
        gameManager.ApplyMove(move);

        // Assert
        Assert.Equal(Set.White, gameManager.CurrentTurn); // Turn should change to white
        Assert.Null(gameManager.CurrentBoard[Position.e8].Piece); // e8 should now be empty
        Assert.Null(gameManager.CurrentBoard[Position.h8].Piece); // h8 should now be empty

        Assert.NotNull(gameManager.CurrentBoard[Position.f8].Piece); // f8 should now have a piece
        Assert.NotNull(gameManager.CurrentBoard[Position.g8].Piece); // g8 should now have a piece

        Assert.Equal(move.Piece, gameManager.CurrentBoard[Position.g8].Piece); // The piece on g8 should be the same as the one moved
        Assert.IsType<Rook>(gameManager.CurrentBoard[Position.f8].Piece); // The piece on f8 should be a rook

        // we'll also check if the move is added to the history
        Assert.Equal(8, gameManager.gameHistory.Count); // 8 moves should be in history
    }

    [Fact]
    public void GameManager_ApplyMove_PromotionIsHandled()
    {
        // Arrange
        // we'll set up a custom board with a pawn on a7 and a pawn on b2

        var gameManager = new GameManager("8/PK6/8/8/8/8/1pk5/8 w - - 0 1");
        Assert.Equal(Set.White, gameManager.CurrentTurn); // White should start first

        var validMoves = gameManager.GetLegalMoves(Position.a7);
        Assert.NotEmpty(validMoves); // Should not be empty
        foreach (var move in validMoves)
        {
            System.Console.WriteLine(move);
        }
        Assert.Equal(4, validMoves.Count); // should contain 4 moves

        // Act
        gameManager.ApplyMove(validMoves[0]);

        // Assert
        Assert.Equal(Set.Black, gameManager.CurrentTurn); // Turn should change to black
        Assert.Null(gameManager.CurrentBoard[Position.a7].Piece); // a7 should now be empty
        Assert.NotNull(gameManager.CurrentBoard[Position.a8].Piece); // a8 should now have a piece
        Assert.IsType<Queen>(gameManager.CurrentBoard[Position.a8].Piece); // The piece on a8 should be a queen
    }

    [Fact]
    public void GameManager_TranslateFENtoDictionary()
    {
        // Arrange
        var fen = "8/PK6/8/8/8/8/1pk5/8 w - - 0 1";

        // Act
        var board = GameManager.FenToDictionary(fen);

        // Assert
        Assert.Equal(4, board.Count); // 4 positions should be in the dictionary
        Assert.IsType<Pawn>(board[Position.a7]); // a7 should have a pawn
        Assert.IsType<King>(board[Position.b7]); // b7 should have a king
        Assert.IsType<Pawn>(board[Position.b2]); // b2 should have a pawn
        Assert.IsType<King>(board[Position.c2]); // c2 should have a king
    }

    [Fact]
    public void GameManager_EnPassentIsHandled()
    {

        var fen = "4k3/8/8/8/3p4/8/4P3/4K3 w - - 0 1";
        var gameManager = new GameManager(fen);

        var validMoves = gameManager.GetLegalMoves(Position.e2);
        var move = validMoves.Find(m => m.To == Position.e4);
        gameManager.ApplyMove(move);

        Assert.Equal(Set.Black, gameManager.CurrentTurn); // Turn should change to black
        validMoves = gameManager.GetLegalMoves(Position.d4);

        Assert.NotEmpty(validMoves); // Should not be empty
        Assert.Equal(2, validMoves.Count); // should contain 2 moves
        Assert.Contains(validMoves, move => move.To == Position.e3); // Pawn can capture with move to e3

        move = validMoves.Find(m => m.To == Position.e3);
        gameManager.ApplyMove(move);

        System.Console.WriteLine("move made: " + move);

        Assert.Equal(Set.White, gameManager.CurrentTurn); // Turn should change to white
        Assert.Null(gameManager.CurrentBoard[Position.e4].Piece); // e4 should now be empty
        Assert.Null(gameManager.CurrentBoard[Position.d4].Piece); // d4 should now be empty
        Assert.NotNull(gameManager.CurrentBoard[Position.e3].Piece); // e3 should now have a piece
        Assert.IsType<Pawn>(gameManager.CurrentBoard[Position.e3].Piece); // The piece on e3 should be a pawn
        Assert.Equal(move.Piece.Set, gameManager.CurrentBoard[Position.e3].Piece.Set); // The piece on e3 should be the same color

    }
}
