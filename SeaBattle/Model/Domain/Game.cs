using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace SeaBattle.Model.Domain
{
    public class Game
    {       
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong? Id { get; set; }
        public List<BattleField> BattleFields { get; set; }
        public ulong? WinnerId { get; set; }
        public ulong? CurrentTurnUserId { get; set; }
        public DateTime StartTime { get; set; }
    }
}
