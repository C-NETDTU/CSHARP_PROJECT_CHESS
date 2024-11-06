using System.Collections.Generic;
using System.Linq;

namespace Frontend.Model.ChessPiece
{
    public class Rook : IPiece
    {
        public Set Set { get; private set; }
        public int Value => 5;

        public string Symbol => Set == Set.White ? "♖" : "♜";
        public string TextSymbol => SYMBOL;

        public Rook(Set set)
        {
            Set = set;
        }

        public static readonly string SYMBOL = "R";
    }
}
