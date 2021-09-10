namespace SeaBattle.Model
{
    public record ShipType(int Length, int MaxCount)
    {
        public static readonly ShipType OneDecker = new(1, 4);
        public static readonly ShipType TwoDecker = new(2, 3);
        public static readonly ShipType ThreeDecker = new(3, 2);
        public static readonly ShipType FourDecker = new(4, 1);
    }
}
