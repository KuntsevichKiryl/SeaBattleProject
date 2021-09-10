using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeaBattle.Model.Domain
{
    public class BattleField
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong? Id { get; set; }
        public List<Ship> Ships { get; set; }
        public List<Shot> Shots { get; set; }
        public ulong OwnerId { get; set; }
    }
}
