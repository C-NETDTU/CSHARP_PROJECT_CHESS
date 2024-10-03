using System.Collections.Generic;
using System.Linq;

namespace Frontend.Model.ChessPiece
{
    public class Knight : IPiece
    {
        public Set Set { get; private set; }
        public int Value => 3; 

        public string Symbol => Set == Set.White ? "♘" : "♞";
        public string TextSymbol => SYMBOL;

        public Knight(Set set)
        {
            Set = set;
        }

        public static readonly string SYMBOL = "N";
    }
}
