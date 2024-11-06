using ChessDotNet;
using ChessDotNet.Pieces;
using Frontend.Model.ChessBoard;
using Frontend.Model.ChessMove;
using Frontend.Model.ChessPiece;
using System.Collections.Generic;
using ChessPosition = Frontend.Model.ChessBoard.Position;

namespace Frontend.Model.Game
{
    public class GameManager
    {
        public ChessGame chessGame { get; private set; }
        public Stack<Board> gameHistory { get; private set; }

        public Board CurrentBoard { get; private set; }

        public Set CurrentTurn => CurrentBoard.Turn;

        //This constructor will make a chessGame with the initial board position
        public GameManager()
        {

            chessGame = new ChessGame();
            gameHistory = new Stack<Board>();
            CurrentBoard = new Board();

        }

        //TODO: We need a alternative constructor that makes a game from custom position
        public GameManager(string fen, AppliedMove? lastMove = null, Set? turn = Set.White)
        {
            chessGame = new ChessGame(fen);
            gameHistory = new Stack<Board>();
            CurrentBoard = new Board(FenToDictionary(fen), lastMove, turn);
        }

        public List<BoardMove> GetLegalMoves(ChessPosition customPosition)
        {
            var chessDotNetPosition = ToChessDotNetPosition(customPosition);
            var validMoves = chessGame.GetValidMoves(chessDotNetPosition);

            List<BoardMove> customMoves = new();

            foreach (var chessMove in validMoves)
            {
                var boardMove = ConvertToBoardMove(chessMove);
                customMoves.Add(boardMove);

            }

            return customMoves;
        }

        public void ApplyMove(BoardMove move)
        {
            var chessMove = ConvertToChessDotNetMove(move);
            MoveType type = chessGame.MakeMove(chessMove, true);
            var newBoard = move.ApplyOn(CurrentBoard);
            newBoard.LastMove = new AppliedMove(move);
            gameHistory.Push(newBoard);
            CurrentBoard = gameHistory.Peek();
        }

        private ChessDotNet.Move ConvertToChessDotNetMove(BoardMove move)
        {
            var from = move.From.ToString();
            var to = move.To.ToString();
            var player = chessGame.WhoseTurn;
            var promotionChar = move.Consequence is Promotion promotion
                    ? promotion.Piece.TextSymbol[0]
                    : (char?)null;
            var chessDotNetMove = new ChessDotNet.Move(from, to, player, promotionChar);

            return chessDotNetMove;
        }

        private BoardMove ConvertToBoardMove(ChessDotNet.Move chessMove)
        {
            var from = FromChessDotNetPosition(chessMove.OriginalPosition);
            var to = FromChessDotNetPosition(chessMove.NewPosition);
            var piece = CurrentBoard.Pieces[from];

            IPrimaryMove primaryMove = new ChessMove.Move(piece, from, to);
            IPreMove? preMove = null;
            IConsequence? consequence = null;

            if (piece is ChessPiece.King && Math.Abs(chessMove.OriginalPosition.File - chessMove.NewPosition.File) == 2)
            {
                if (chessMove.NewPosition.File == ChessDotNet.File.G)
                {
                    primaryMove = new KingSideCastle(piece, from, to);
                }
                if (chessMove.NewPosition.File == ChessDotNet.File.C)
                {
                    primaryMove = new QueenSideCastle(piece, from, to);
                }
            }
            else if (piece is ChessPiece.Pawn && (chessMove.NewPosition.Rank == 1 || chessMove.NewPosition.Rank == 8))
            {
                if (chessMove.Promotion.HasValue)
                {
                    System.Console.WriteLine("The promotion value is: " + chessMove.Promotion.Value);
                    consequence = new Promotion(GetPromotedPiece(chessMove.Promotion.Value, piece.Set), to);
                    System.Console.WriteLine("The promotion piece is: " + consequence.Piece.GetType());
                }
            }
            else if (piece is ChessPiece.Pawn)
            {
                if (Math.Abs(to.GetFile() - from.GetFile()) == 1 && CurrentBoard[to].IsEmpty)
                {
                    var enPassantPosition = PositionMethods.From(to.GetFile(), from.GetRank());
                    var capturedPiece = CurrentBoard[enPassantPosition].Piece;

                    if (capturedPiece is ChessPiece.Pawn && capturedPiece.Set != piece.Set)
                    {
                        preMove = new Capture(piece, enPassantPosition);
                    }
                }
                else if (CurrentBoard[to].IsNotEmpty)
                {
                    preMove = new Capture(piece, to);
                }
            }
            else if (chessGame.GetPieceAt(chessMove.NewPosition) != null)
            {
                preMove = new Capture(piece, to);
            }

            string sanNotation;
            if (chessMove.Promotion.HasValue)
            {
                sanNotation = $"{from}{to}{chessMove.Promotion.Value.ToString().ToLower()}";
            }
            else
            {
                sanNotation = $"{from}{to}";
            }

            return new BoardMove(primaryMove, preMove, consequence, sanNotation);
        }

        private static IPiece GetPromotedPiece(char promotionPieceChar, Set pieceSet)
        {
            switch (promotionPieceChar)
            {
                case 'Q':
                    return new ChessPiece.Queen(pieceSet);
                case 'R':
                    return new ChessPiece.Rook(pieceSet);
                case 'B':
                    return new ChessPiece.Bishop(pieceSet);
                case 'N':
                    return new ChessPiece.Knight(pieceSet);
                default:
                    throw new ArgumentException($"Invalid promotion piece: {promotionPieceChar}");
            }
        }

        public static Dictionary<ChessPosition, IPiece> FenToDictionary(string fen)
        {
            var pieces = new Dictionary<ChessPosition, IPiece>();
            var ranks = fen.Split(' ')[0].Split('/');

            for (int i = 0; i < ranks.Length; i++)
            {
                int file = 1;

                foreach (char symbol in ranks[i])
                {
                    if (char.IsDigit(symbol))
                    {
                        file += (int)char.GetNumericValue(symbol);
                    }
                    else
                    {
                        var piece = CreatePieceFromFenSymbol(symbol);
                        var position = PositionMethods.From(file, 8 - i);
                        pieces[position] = piece;
                        //System.Console.WriteLine($"Added {piece.GetType().Name} at {position}");
                        file++;
                    }
                }
            }

            return pieces;
        }

        private static IPiece CreatePieceFromFenSymbol(char symbol)
        {
            bool isWhite = char.IsUpper(symbol);
            var set = isWhite ? Set.White : Set.Black;

            return symbol.ToString().ToLower() switch
            {
                "p" => new ChessPiece.Pawn(set),
                "r" => new ChessPiece.Rook(set),
                "n" => new ChessPiece.Knight(set),
                "b" => new ChessPiece.Bishop(set),
                "q" => new ChessPiece.Queen(set),
                "k" => new ChessPiece.King(set),
                _ => throw new ArgumentException($"Invalid FEN symbol: {symbol}")
            };
        }

        private static ChessDotNet.Position ToChessDotNetPosition(ChessPosition customPosition)
        {
            return new ChessDotNet.Position(customPosition.ToString().ToLower());
        }

        private static ChessPosition FromChessDotNetPosition(ChessDotNet.Position chessNetPosition)
        {
            return (ChessPosition)Enum.Parse(typeof(ChessPosition), chessNetPosition.ToString().ToLower());
        }
    }
}
