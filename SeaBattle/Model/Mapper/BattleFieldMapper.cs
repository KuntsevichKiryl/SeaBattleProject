using SeaBattle.Model.Domain;
using SeaBattle.Model.Dto;

namespace SeaBattle.Model.Mapper
{
    class BattleFieldMapper
    {
        public BattleFieldDto ToDto(BattleField battleField, UserDto userDto)
        {
            var dto = new BattleFieldDto
            {
                Id = battleField.Id,
                Ships = battleField.Ships,
                Shots = battleField.Shots,
                Owner = userDto
            };
            return dto;
        }

        public BattleField ToEntity(BattleFieldDto battleFieldDto)
        {
            BattleField battleField = new BattleField
            {
                Id = battleFieldDto.Id,
                Ships = battleFieldDto.Ships,
                Shots = battleFieldDto.Shots,
                OwnerId = battleFieldDto.Owner.Id.Value
            };
            return battleField;
        }
    }
}
