using System.Collections.Generic;
using System.Linq;

namespace Frontend.Model.ChessPiece
{
    public class King : IPiece
    {
        public Set Set { get; private set; }
        public int Value => int.MaxValue;

        public string Symbol => Set == Set.White ? "♔" : "♚";
        public string TextSymbol => SYMBOL;

        public King(Set set)
        {
            Set = set;
        }
        public static readonly string SYMBOL = "K";
    }
}