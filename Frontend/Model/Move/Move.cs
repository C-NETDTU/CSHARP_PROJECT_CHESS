using System.Linq;
using Frontend.Model.ChessPiece;
using Frontend.Model.ChessBoard;

namespace Frontend.Model.ChessMove
{
    // The diffenrent types of moves, implemented as classes
    public class Move : IPrimaryMove, IConsequence
    {
        public IPiece Piece { get; private set; }
        public Position From { get; private set; }
        public Position To { get; private set; }

        // Constructor
        public Move(IPiece piece, Position from, Position to)
        {
            Piece = piece;
            From = from;
            To = to;
        }
        public Board ApplyOn(Board board)
        {
            Console.WriteLine($"Moving piece from {From} to {To}");
            // Create a new Board instance with updated pieces
            var updatedPieces = board.Pieces
                .Where(kvp => kvp.Key != From)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);


            if (!updatedPieces.ContainsKey(From))
            {
                Console.WriteLine($"Piece successfully removed from {From}");
            }
            else
            {
                Console.WriteLine($"Piece was not removed from {From}");
            }

            updatedPieces[To] = Piece;

            if(updatedPieces.ContainsKey(To))
            {
                Console.WriteLine($"Piece successfully added to {To}");
            }
            else
            {
                Console.WriteLine($"Piece was not added to {To}");
            }

            return new Board(updatedPieces);
        }
    }

    public class KingSideCastle : IPrimaryMove
    {
        public IPiece Piece { get; private set; }
        public Position From { get; private set; }
        public Position To { get; private set; }

        // Constructor
        public KingSideCastle(IPiece piece, Position from, Position to)
        {
            Piece = piece;
            From = from;
            To = to;
        }

        // Applies the king-side castle move on the board
        public Board ApplyOn(Board board)
        {

            Position rookInitialPosition = Piece.Set == Set.White ? Position.h1 : Position.h8;  
            Position rookFinalPosition = Piece.Set == Set.White ? Position.f1 : Position.f8;   

            var updatedPieces = board.Pieces
                .Where(kvp => kvp.Key != From && kvp.Key != rookInitialPosition)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            updatedPieces[To] = Piece;

            if (board.Pieces.TryGetValue(rookInitialPosition, out IPiece? rook) && rook != null)
            {
                updatedPieces[rookFinalPosition] = rook;
            }

            return new Board(updatedPieces);
        }
    }

    public class QueenSideCastle : IPrimaryMove
    {
        public IPiece Piece { get; private set; }
        public Position From { get; private set; }
        public Position To { get; private set; }

        // Constructor
        public QueenSideCastle(IPiece piece, Position from, Position to)
        {
            Piece = piece;
            From = from;
            To = to;
        }

        // Applies the queen-side castle move on the board
        public Board ApplyOn(Board board)
        {
            Position rookInitialPosition = Piece.Set == Set.White ? Position.a1 : Position.a8;  
            Position rookFinalPosition = Piece.Set == Set.White ? Position.d1 : Position.d8;   

            var updatedPieces = board.Pieces
                .Where(kvp => kvp.Key != From && kvp.Key != rookInitialPosition)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            updatedPieces[To] = Piece;

            if (board.Pieces.TryGetValue(rookInitialPosition, out IPiece? rook) && rook != null)
            {
            updatedPieces[rookFinalPosition] = rook; 
            }

            return new Board(updatedPieces);

        }
    }

    public class Capture : IPreMove
    {
        public IPiece Piece { get; private set; }
        public Position Position { get; private set; }

        // Constructor
        public Capture(IPiece piece, Position position)
        {
            Piece = piece;
            Position = position;
        }

        // Applies the capture move on the board
        public Board ApplyOn(Board board)
        {
            var updatedPieces = board.Pieces
                .Where(kvp => kvp.Key != Position)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return new Board(updatedPieces);
        }
    }

    public class Promotion : IConsequence
    {
        public IPiece Piece { get; private set; }
        public Position Position { get; private set; }

        // Constructor
        public Promotion(IPiece piece, Position position)
        {
            Piece = piece;
            Position = position;
        }

        // Applies the promotion on the board
        public Board ApplyOn(Board board)
        {
            var updatedPieces = board.Pieces
                .Where(kvp => kvp.Key != Position)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            updatedPieces[Position] = Piece;

            return new Board(updatedPieces);
        }
    }
}