using System;
using ChessDotNet;
using ChessDotNet.Pieces;
using Frontend.Model.ChessBoard;
using Frontend.Model.ChessMove;
using Frontend.Model.ChessPiece;
using System.Collections.Generic;
using ChessPosition = Frontend.Model.ChessBoard.Position;

namespace Frontend.Controller
{
    public class GameManager
    {
        public ChessGame chessGame { get; private set; }

        private Queue<BoardMove>? solutionMoves;
        public Stack<Board> gameHistory { get; private set; }
        
        public Stack<BoardMove> poppedMoves { get; set; }

        public Board CurrentBoard { get; private set; }

        public Set CurrentTurn => CurrentBoard.Turn;
        
        public event Action<bool>? OnPuzzleCompleted;
        public event Action? OnUserMoveApplied;

        //This constructor will make a chessGame with the initial board position
        public GameManager(string fen, List<string> solutionMoves)
        {

            chessGame = new ChessGame(fen);
            gameHistory = new Stack<Board>();
            poppedMoves = new Stack<BoardMove>();
            var (pieces, turn) = FenToDictionary(fen);
            CurrentBoard = new Board(pieces, turn);
            gameHistory.Push(CurrentBoard);
            System.Console.WriteLine("Game initialized with pieces: ");
            foreach (var piece in CurrentBoard.Pieces)
            {
                Console.WriteLine($"Piece: {piece.Value.GetType().Name} at {piece.Key}");
            }

            var tempMoves = ParseSolutionMoves(solutionMoves);
            this.solutionMoves = tempMoves;
            //debug
            Console.WriteLine($"Parsed solution moves: {solutionMoves.Count}");
            foreach (var move in this.solutionMoves)
            {
                Console.WriteLine(move);
            }

        }
        

        public void PerformNextMove()
        {
            if (solutionMoves != null && solutionMoves.Count > 0)
            {
                var nextMove = solutionMoves.Dequeue();
                ApplyMove(nextMove);
            }
        }

        public void OnUserMove(BoardMove userMove)
        {
            if (solutionMoves == null || solutionMoves.Count == 0)
            {
                throw new InvalidOperationException("No solution moves available.");
            }
            Console.WriteLine($"User moved: {userMove}. Expected: {solutionMoves.Peek()}.");
            ApplyMove(userMove);
            
            var expectedMove = solutionMoves.Peek();
            Console.WriteLine(userMove.ToString().Equals(expectedMove.ToString()));
            if (userMove.From == expectedMove.From && userMove.To == expectedMove.To)
            {
                Console.WriteLine("User move is correct.");
                solutionMoves.Dequeue();

                if (solutionMoves.Count == 0)
                {
                    Console.WriteLine("No solution moves available - puzzle completed");
                    OnPuzzleCompleted?.Invoke(true);
                }
                OnUserMoveApplied?.Invoke();
            }  
            else
            {
                Console.WriteLine("User move is incorrect.");
                OnPuzzleCompleted?.Invoke(false); 
            }
        }

        public void GoBackOneMove()
        {
            Console.WriteLine("Before pop, gamehistory: " + gameHistory.Count);
            var poppedGame = gameHistory.Pop();
            Console.WriteLine("Going back one move.");
            Console.WriteLine("No last move - start of game: " + (poppedGame.LastMove?.BoardMove == null));
            if (poppedGame.LastMove?.BoardMove != null) poppedMoves.Push(poppedGame.LastMove.BoardMove);
            chessGame.Undo(1);
            CurrentBoard = gameHistory.Peek();
            Console.WriteLine("Undo performed, last applied move: " + CurrentBoard.LastMove?.ToString());
        }

        public void GoForwardOneMove()
        {
            Console.WriteLine("Number of popped moves: " + poppedMoves.Count);
            var lastPoppedMove = poppedMoves.Pop();
            var chessDotNetMove = ConvertToChessDotNetMove(lastPoppedMove);
            chessGame.MakeMove(chessDotNetMove, true);
            var newBoard = lastPoppedMove.ApplyOn(CurrentBoard);

            var moveEffect = ApplyMoveEffect();
            newBoard.LastMove = new AppliedMove(lastPoppedMove, moveEffect);
            newBoard.Turn = lastPoppedMove.Piece.Set.Opposite();
            gameHistory.Push(newBoard);
            CurrentBoard = gameHistory.Peek();
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

            var moveEffect = ApplyMoveEffect();
            newBoard.LastMove = new AppliedMove(move, moveEffect);
            newBoard.Turn = move.Piece.Set.Opposite();

            Console.WriteLine($"ApplyMove: Move applied - {move}. Turn before push: {newBoard.Turn}");

            gameHistory.Push(newBoard);
            CurrentBoard = gameHistory.Peek();
        }

        private MoveEffect ApplyMoveEffect()
        {
            var isCheck = chessGame.IsInCheck(chessGame.WhoseTurn);
            var isCheckMate = chessGame.IsCheckmated(chessGame.WhoseTurn);

            if (isCheck)
            {
                return MoveEffect.check;
            }

            if (isCheckMate)
            {
                return MoveEffect.checkmate;
            }
            
            return MoveEffect.none;
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

        private Queue<BoardMove> ParseSolutionMoves(List<string> solutionMoves)
        {
            var tempBoard = new Board(CurrentBoard.Pieces, CurrentBoard.Turn);
            Queue<BoardMove> parsedMoves = new();
            foreach (var move in solutionMoves)
            {
                var boardMove = ConvertSolutionMoveToBoardMove(move, tempBoard);
                tempBoard = boardMove.ApplyOn(tempBoard);
                parsedMoves.Enqueue(boardMove);
            }
            return parsedMoves;
        }

        private BoardMove ConvertSolutionMoveToBoardMove(string move, Board board)
        {
            if (move.Length != 4)
            {
                throw new ArgumentException("Invalid move string");
            }

            string fromPositon = move.Substring(0, 2);
            string toPosition = move.Substring(2, 2);

            var from = PositionMethods.FromString(fromPositon[0].ToString(), fromPositon[1].ToString());
            var to = PositionMethods.FromString(toPosition[0].ToString(), toPosition[1].ToString());

            var piece = board.Pieces[from];

            IPrimaryMove primaryMove = new Model.ChessMove.Move(piece, from, to);
            IPreMove? preMove = null;
            IConsequence? consequence = null;

            if (piece is Model.ChessPiece.King && Math.Abs(from.GetFile() - to.GetFile()) == 2)
            {
                primaryMove = to.GetFile() > from.GetFile()
                    ? new KingSideCastle(piece, from, to)
                    : new QueenSideCastle(piece, from, to);
            }
            else if (piece is Model.ChessPiece.Pawn && (to.GetRank() == 1 || to.GetRank() == 8))
            {
                consequence = new Promotion(new Model.ChessPiece.Queen(piece.Set), to);
            }
            else if (piece is Model.ChessPiece.Pawn)
            {
                if (Math.Abs(to.GetFile() - from.GetFile()) == 1 && CurrentBoard[to].IsEmpty)
                {
                    var enPassantPosition = PositionMethods.From(to.GetFile(), from.GetRank());
                    var capturedPiece = board[enPassantPosition].Piece;

                    if (capturedPiece is Model.ChessPiece.Pawn && capturedPiece.Set != piece.Set)
                    {
                        preMove = new Capture(piece, enPassantPosition);
                    }
                }
                else if (board[to].IsNotEmpty)
                {
                    preMove = new Capture(piece, to);
                }
            }
            else if (board[to].IsNotEmpty)
            {
                preMove = new Capture(piece, to);
            }
            string sanNotation = $"{from}{to}";

            return new BoardMove(primaryMove, preMove, consequence, sanNotation);
        }

        private BoardMove ConvertToBoardMove(ChessDotNet.Move chessMove)
        {
            var from = FromChessDotNetPosition(chessMove.OriginalPosition);
            var to = FromChessDotNetPosition(chessMove.NewPosition);
            
            var piece = CurrentBoard.Pieces[from];
            

            IPrimaryMove primaryMove = new Model.ChessMove.Move(piece, from, to);
            IPreMove? preMove = null;
            IConsequence? consequence = null;

            if (piece is Model.ChessPiece.King && Math.Abs(chessMove.OriginalPosition.File - chessMove.NewPosition.File) == 2)
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
            else if (piece is Model.ChessPiece.Pawn && (chessMove.NewPosition.Rank == 1 || chessMove.NewPosition.Rank == 8))
            {
                if (chessMove.Promotion.HasValue)
                {
                    System.Console.WriteLine("The promotion value is: " + chessMove.Promotion.Value);
                    consequence = new Promotion(GetPromotedPiece(chessMove.Promotion.Value, piece.Set), to);
                    System.Console.WriteLine("The promotion piece is: " + consequence.Piece.GetType());
                }
            }
            else if (piece is Model.ChessPiece.Pawn)
            {
                if (Math.Abs(to.GetFile() - from.GetFile()) == 1 && CurrentBoard[to].IsEmpty)
                {
                    var enPassantPosition = PositionMethods.From(to.GetFile(), from.GetRank());
                    var capturedPiece = CurrentBoard[enPassantPosition].Piece;

                    if (capturedPiece is Model.ChessPiece.Pawn && capturedPiece.Set != piece.Set)
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
                    return new Model.ChessPiece.Queen(pieceSet);
                case 'R':
                    return new Model.ChessPiece.Rook(pieceSet);
                case 'B':
                    return new Model.ChessPiece.Bishop(pieceSet);
                case 'N':
                    return new Model.ChessPiece.Knight(pieceSet);
                default:
                    throw new ArgumentException($"Invalid promotion piece: {promotionPieceChar}");
            }
        }

        public static (Dictionary<ChessPosition, IPiece> pieces, Set turn) FenToDictionary(string fen)
        {
            var pieces = new Dictionary<ChessPosition, IPiece>();
            var fenParts = fen.Split(' ');

            var ranks = fenParts[0].Split('/');
            Set turn = fenParts[1] == "w" ? Set.White : Set.Black; // Determine the turn from FEN

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
                        file++;
                    }
                }
            }

            return (pieces, turn);
        }


        public static Set GetTurnFromFen(string fen)
        {
            var parts = fen.Split(' ');
            if (parts.Length > 1)
            {
                return parts[1] == "w" ? Set.White : Set.Black;
            }
            throw new ArgumentException("Invalid FEN string");
        }

        private static IPiece CreatePieceFromFenSymbol(char symbol)
        {
            bool isWhite = char.IsUpper(symbol);
            var set = isWhite ? Set.White : Set.Black;

            return symbol.ToString().ToLower() switch
            {
                "p" => new Model.ChessPiece.Pawn(set),
                "r" => new Model.ChessPiece.Rook(set),
                "n" => new Model.ChessPiece.Knight(set),
                "b" => new Model.ChessPiece.Bishop(set),
                "q" => new Model.ChessPiece.Queen(set),
                "k" => new Model.ChessPiece.King(set),
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