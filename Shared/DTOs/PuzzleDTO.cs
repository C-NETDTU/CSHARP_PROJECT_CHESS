namespace Shared.DTO
{
	public class PuzzleDTO
	{
		public string? Id {  get; set; }
		public string? PuzzleId { get; set; }
		public string? FEN { get; set; }
		public int Rating { get; set; }
		public string? Moves { get; set; }
		public string? Themes { get; set; }


        /// <summary>
        /// Parameterless constructor for Puzzle class
        /// </summary>
        public PuzzleDTO() { }

		/// <summary>
		/// Parameterized constructor for Puzzle class
		/// </summary>
		public PuzzleDTO(string id, string? puzzleId, string? fen, string? moves, int rating, string themes)
		{
			Id = id;
			PuzzleId = puzzleId;
			FEN = fen;
			Moves = moves;
			Rating = rating;
			Themes = themes;
		}

		

    }
}