using System.Collections.Generic;
using System.Linq;

namespace Frontend.Model.ChessPiece
{
    public class Bishop : IPiece
    {
        public Set Set { get; private set; }
        public int Value => 3;

        public string Symbol => Set == Set.White ? "♗" : "♝";
        public string TextSymbol => SYMBOL;

        public Bishop(Set set)
        {
            Set = set;
        }

        public static readonly string SYMBOL = "B";
    }
}
