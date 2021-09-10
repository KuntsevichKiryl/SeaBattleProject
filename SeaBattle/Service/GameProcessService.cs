using System;
using System.Collections.Generic;
using SeaBattle.Controller.View;
using SeaBattle.Logger;
using SeaBattle.Model;
using SeaBattle.Model.Domain;
using SeaBattle.Model.Dto;

namespace SeaBattle.Service
{
    class GameProcessService
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<GameProcessService>();
        private readonly BattleFieldService battleFieldService;
        private readonly GameService gameService;
        public record UnreadyField(BattleFieldDto BattleField, Dictionary<int, int> RestToPlace);

        public GameProcessService(BattleFieldService battleFieldService, GameService gameService)
        {
            this.battleFieldService = battleFieldService;
            this.gameService = gameService;
        }

        public GameDto LoadGame(ulong gameId)
        {
            log.Debug($"Loading game id:{gameId}");
            return gameService.GetGameById(gameId);
        }

        public void SaveGame(GameDto game)
        {
            log.Debug($"Saving game id:{game.Id}");
            gameService.SaveOrUpdateGame(game);
        }

        public GameDto CreateNewGame(UserDto gamer1, UserDto gamer2)
        {
            log.Debug("Creating new game");
            GameDto gameDto = new GameDto
            {
                StartTime = DateTime.Now,
                CurrentUser = (new[] { gamer1, gamer2 })[new Random().Next(2)],
                BattleFields = new List<BattleFieldDto>
                {
                    new()
                    {
                        Owner = gamer1,
                        Ships = new List<Ship>(),
                        Shots = new List<Shot>()
                    },
                    new()
                    {
                        Owner = gamer2,
                        Ships = new List<Ship>(),
                        Shots = new List<Shot>()
                    }
                }
            };
            return gameService.SaveOrUpdateGame(gameDto);
        }

        public bool IsGameReady(GameDto game, out List<UnreadyField> unready)
        {
            unready = new List<UnreadyField>();
            foreach (var bf in game.BattleFields)
            {
                bool isReady = IsBattleFieldReady(bf, out Dictionary<int, int> rest);
                if (isReady)
                {
                    continue;
                }
                unready.Add(new UnreadyField(bf, rest));
            }

            return unready.Count == 0;
        }

        public bool IsBattleFieldReady(BattleFieldDto battleField, out Dictionary<int, int> rest)
        {
            rest = battleFieldService.RestToFill(battleField);
            return rest.Count == 0;
        }

        public bool AddShip(BattleFieldDto battleField, ShipType type, int x, int y, ShipRotation rotation)
        {
            return battleFieldService.AddShip(battleField, x, y, type, rotation);
        }

        public ShotResult AddShot(GameDto game, int x, int y)
        {
            var enemyField = game.BattleFields
                .Find(bf => bf.Owner.Id != game.CurrentUser.Id);
            var shotResult = battleFieldService.AddShot(enemyField, x, y);
            switch (shotResult)
            {
                case ShotResult.Destroy:
                    if (battleFieldService.IsDefeated(enemyField))
                    {
                        game.Winner = game.CurrentUser;
                    }
                    break;
                case ShotResult.Hit:
                    //nothing to do here
                    break;
                case ShotResult.Miss:
                    game.CurrentUser = enemyField.Owner;
                    break;
            }

            return shotResult;
        }
    }
}
