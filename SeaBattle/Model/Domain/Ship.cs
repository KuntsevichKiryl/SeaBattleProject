using System.ComponentModel.DataAnnotations.Schema;

namespace SeaBattle.Model.Domain
{
    public class Ship
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong? Id { get; set; }
        public int Length { get; set; }
        public int XHead { get; set; }
        public int YHead { get; set; }
        public int XTail { get; set; }
        public int YTail { get; set; }
        public int Wounds { get; set; }

        public bool IsDestroyed()
        {
            return Length == Wounds;
        }

        public void MakeWound()
        {
            Wounds++;
        }
    }
}
