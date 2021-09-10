using System.Collections.Generic;
using System.Linq;
using SeaBattle.Model.Domain;
using SeaBattle.Model.Dto;

namespace SeaBattle.Model.Mapper
{
    class GameMapper
    {
        private readonly BattleFieldMapper battleFieldMapper;

        public GameMapper(BattleFieldMapper battleFieldMapper)
        {
            this.battleFieldMapper = battleFieldMapper;
        }

        public GameDto ToDto(Game game, List<UserDto> userDtos)
        {
            GameDto gameDto = new()
            {
                Id = game.Id,
                StartTime = game.StartTime
            };
            Dictionary<ulong?, UserDto> users = userDtos.ToDictionary(user => user.Id, user => user);
            List<BattleFieldDto> bfs = new List<BattleFieldDto>();

            foreach (var bf in game.BattleFields)
            {
                var bfDto = battleFieldMapper.ToDto(bf, users.GetValueOrDefault(bf.OwnerId));
                bfs.Add(bfDto);
            }

            gameDto.BattleFields = bfs;
            gameDto.CurrentUser = users.GetValueOrDefault(game.CurrentTurnUserId);
            gameDto.Winner = game.WinnerId is not null ? users.GetValueOrDefault(game.WinnerId) : null;
            return gameDto;
        }

        public Game ToEntity(GameDto gameDto)
        {
            var bfs = new List<BattleField>();

            foreach (var battleFieldDto in gameDto.BattleFields)
            {
                var bf = battleFieldMapper.ToEntity(battleFieldDto);
                bfs.Add(bf);
            }

            Game game = new Game
            {
                Id = gameDto.Id,
                StartTime = gameDto.StartTime,
                BattleFields = bfs,
                CurrentTurnUserId = gameDto.CurrentUser.Id,
                WinnerId = gameDto.Winner?.Id
            };
            return game;
        }
    }
}
