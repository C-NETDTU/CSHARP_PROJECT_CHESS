namespace Frontend.Model.ChessPiece
{

    public enum Set
    {
        White,
        Black
    }

    public static class SetExtensions
    {
        public static Set Opposite(this Set set)
        {
            return set == Set.White ? Set.Black : Set.White;
        }
    }
}
