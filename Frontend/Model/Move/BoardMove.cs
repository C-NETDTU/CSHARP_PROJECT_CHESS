using Frontend.Model.ChessBoard;
using Frontend.Model.ChessPiece;

using System;

namespace Frontend.Model.ChessMove
{
    public class BoardMove
    {
        public IPrimaryMove Move { get; private set; }
        public IPreMove? PreMove { get; private set; }
        public IConsequence? Consequence { get; private set; }

        public string SanNotation { get; set; }

        public Position From => Move.From;
        public Position To => Move.To;
        public IPiece Piece => Move.Piece;


        public BoardMove(IPrimaryMove move, IPreMove? preMove = null, IConsequence? consequence = null, string sanNotation = "")
        {
            Move = move;
            PreMove = preMove;
            Consequence = consequence;
            SanNotation = sanNotation;
        }

        public Board ApplyOn(Board board)
        {
            var updatedBoard = PreMove?.ApplyOn(board) ?? board;
            updatedBoard = Move.ApplyOn(updatedBoard);
            updatedBoard = Consequence?.ApplyOn(updatedBoard) ?? updatedBoard;
            return updatedBoard;
        }
        public override string ToString()
        {
            return ToString(true);
        }

        public string ToString(bool useFigurineNotation)
        {
            if (!useFigurineNotation)
            {
                return SanNotation;
            }

            return ConvertToFigurineNotation(SanNotation);
        }

        private string ConvertToFigurineNotation(string san)
        {
            if (Move.Piece is IPiece piece)
            {
                string figurineSymbol = piece.Symbol;
                string textSymbol = piece.TextSymbol;
                san = san.Replace(textSymbol, figurineSymbol);
            }
            return san;
        }
    }
}

