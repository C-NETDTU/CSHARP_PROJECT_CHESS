using System.Text.Json.Serialization;

namespace Shared.DTO
{
	public class PuzzleDTO
	{
		[JsonPropertyName("id")]
		public string? Id {  get; set; }
		[JsonPropertyName("puzzleId")]
		public string? PuzzleId { get; set; }
		[JsonPropertyName("fen")]
		public string? FEN { get; set; }
		[JsonPropertyName("moves")]
		public string? Moves { get; set; }
		[JsonPropertyName("rating")]
        public int Rating { get; set; }
		[JsonPropertyName("themes")]
        public string? Themes { get; set; }


        /// <summary>
        /// Parameterless constructor for PuzzleDTO class
        /// </summary>
        public PuzzleDTO() { }

		/// <summary>
		/// Parameterized constructor for PuzzleDTO class
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