namespace SeaBattle.Model.Domain
{
    public class Shot
    {
        public ulong? Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsHit { get; set; }
    }
}
