using System.Collections.Generic;
using System.Linq;

namespace Frontend.Model.ChessPiece
{
    public class Queen : IPiece
    {
        public Set Set { get; private set; }
        public int Value => 9;

        public string Symbol => Set == Set.White ? "♕" : "♛";
        public string TextSymbol => SYMBOL;

        public Queen(Set set)
        {
            Set = set;
        }

        public static readonly string SYMBOL = "Q";
    }
}
