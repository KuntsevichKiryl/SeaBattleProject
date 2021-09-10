using System;
using System.Collections.Generic;

namespace SeaBattle.Model.Dto
{
    class GameDto
    {
        public ulong? Id { get; set; }
        public DateTime StartTime { get; set; }
        public List<BattleFieldDto> BattleFields { get; set; }
        public UserDto CurrentUser { get; set; }
        public UserDto Winner { get; set; }
    }
}
