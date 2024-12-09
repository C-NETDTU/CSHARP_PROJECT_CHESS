using System.Collections.Generic;

namespace Frontend.Model.ChessPiece
{
    // Interface definition for a chess piece
    public interface IPiece
    {
        Set Set { get; }
        // Asset - Needs to be implemented
        //int? Asset { get; }
        string Symbol { get; }
        string TextSymbol { get; }
        int Value { get; }

    }
}
