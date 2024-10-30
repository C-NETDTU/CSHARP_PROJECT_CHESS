using System;
using Frontend.Model.ChessBoard;
using Frontend.Model.ChessPiece;

namespace Frontend.Model.ChessMove
{
    public class AppliedMove
    {
        public BoardMove BoardMove { get; set; }
        public MoveEffect? Effect { get; private set; }
        public IPrimaryMove Move => BoardMove.Move;
        public Position From => Move.From;
        public Position To => Move.To;
        public IPiece Piece => Move.Piece;
        public AppliedMove(BoardMove boardMove, MoveEffect? effect = null)
        {
            BoardMove = boardMove;
            Effect = effect ?? MoveEffect.None;
        }

        public override string ToString()
        {
            return ToString(true, true);
        }

        public string ToString(bool useFigurineNotation, bool includeResult)
        {
            string moveString = BoardMove.ToString(useFigurineNotation);

            string postFix = Effect switch
            {
                MoveEffect.Check => "+",
                MoveEffect.Checkmate => $"#  {(Piece.Set == Set.White ? "1-0" : "0-1")}",
                MoveEffect.Draw => "  ½ - ½",
                _ => ""
            };

            return $"{moveString}{postFix}";
        }
    }
}