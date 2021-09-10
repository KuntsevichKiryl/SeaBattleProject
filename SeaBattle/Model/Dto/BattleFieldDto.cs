using System.Collections.Generic;
using SeaBattle.Model.Domain;

namespace SeaBattle.Model.Dto
{
    class BattleFieldDto
    {
        public ulong? Id { get; set; }
        public List<Ship> Ships { get; set; }
        public List<Shot> Shots { get; set; }
        public UserDto Owner { get; set; }
    }
}
