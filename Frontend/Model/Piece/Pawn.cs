using System.Collections.Generic;
using System.Linq;

namespace Frontend.Model.ChessPiece
{
    public class Pawn : IPiece
    {
        public Set Set { get; private set; }
        public int Value => 1;

        public string Symbol => Set == Set.White ? "♙" : "♟";
        public string TextSymbol => SYMBOL;

        public Pawn(Set set)
        {
            Set = set;
        }

        public static readonly string SYMBOL = "P";
    }
}
