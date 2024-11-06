using System;
using Frontend.Model.ChessPiece;
using Frontend.Model.ChessBoard;
namespace Frontend.Model.ChessMove
{
    // This class represents a different types of effects that a move can have on the board
    public interface IPieceEffect
    {
        IPiece Piece { get; }
        Board ApplyOn(Board board);

    }
    public interface IPreMove : IPieceEffect
    {
    }

    public interface IPrimaryMove : IPieceEffect
    {
        Position From { get; }
        Position To { get; }
    }

    public interface IConsequence : IPieceEffect
    {
    }
}
