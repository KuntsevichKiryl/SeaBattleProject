using SeaBattle.Model.Domain;
using SeaBattle.Model.Dto;
using System.Collections.Generic;
using System.Linq;
using SeaBattle.Repository;
using Microsoft.EntityFrameworkCore;
using SeaBattle.Controller.View;
using SeaBattle.Logger;
using SeaBattle.Model.Mapper;

namespace SeaBattle.Service
{
    class GameService
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<GameService>();
        private readonly ApplicationDbContext context;
        private readonly GameMapper gameMapper;
        private readonly UserService userService;
        private readonly DbSet<Game> games;

        public GameService(ApplicationDbContext context, GameMapper gameMapper, UserService userService)
        {
            this.context = context;
            this.gameMapper = gameMapper;
            this.userService = userService;
            games = context.Games;
        }

        public List<GameDto> GetGamesByUserId(ulong userId, int limit)
        {
            log.Debug($"Getting list of user id:{userId} games");
            var result = from game in games.AsNoTracking()
                         where game.BattleFields.Any(bf => bf.OwnerId == userId)
                         orderby game.StartTime descending
                         select game;
            var data = limit > 0 ? result.Take(limit) : result;
            return Retrieve(data);
        }

        public GameDto GetGameById(ulong gameId)
        {
            log.Debug($"Getting game id:{gameId} by id");
            return Retrieve(games.AsNoTracking().Where(g => g.Id == gameId)).FirstOrDefault(); ;
        }

        public GameDto SaveOrUpdateGame(GameDto gameDto)
        {
            log.Debug($"Saving/updating game id:{gameDto.Id}");
            var game = gameMapper.ToEntity(gameDto);
            if (game.Id is null)
            {
                games.Add(game);
            }
            else
            {
                game.BattleFields.ForEach(bf =>
                {
                    bf.Ships.ForEach(ship =>
                    {
                        if (ship.Id is null)
                        {
                            context.Ships.Add(ship);
                            return;
                        }

                        var extShip = context.Ships.FirstOrDefault(s => s.Id.Equals(ship.Id));
                        if (extShip is not null)
                        {
                            context.Entry(extShip).State = EntityState.Detached;
                        }
                        context.Entry(ship).State = EntityState.Modified;
                    });
                    bf.Shots.ForEach(shot =>
                    {
                        if (shot.Id is null)
                        {
                            context.Shots.Add(shot);
                        }
                    });
                    var extBf = context.BattleFields.Local.FirstOrDefault(b => b.Id.Equals(bf.Id));
                    if (extBf is not null)
                    {
                        context.Entry(extBf).State = EntityState.Detached;
                    }
                    context.Entry(bf).State = EntityState.Modified;
                });
                var extGame = games.Local.FirstOrDefault(g => g.Id.Equals(game.Id));
                if (extGame is not null)
                {
                    context.Entry(extGame).State = EntityState.Detached;
                }
                context.Entry(game).State = EntityState.Modified;
            }
            context.SaveChanges();
            return gameMapper.ToDto(game, gameDto.BattleFields.Select(bf => bf.Owner).ToList());
        }

        private List<GameDto> Retrieve(IQueryable<Game> queryResult)
        {
            return queryResult
                .Include(u => u.BattleFields)
                .ThenInclude(b => b.Ships)
                .Include(u => u.BattleFields)
                .ThenInclude(b => b.Shots)
                .Select(game => gameMapper.ToDto(game, userService.GetByIds(game.BattleFields.Select(bf => bf.OwnerId).ToList())))
                .ToList();
        }

        public bool CheckLoginForGameId(ulong gameId, string login)
        {
            var user = context.Users.FirstOrDefault(u => u.Login.Equals(login));
            if (user is null)
            {
                return false;
            }

            var found = games
                .Count(g => g.Id.Equals(gameId)
                            && g.BattleFields.Any(bf => bf.OwnerId.Equals(user.Id)));
            return found > 0;
        }

        public bool CheckGameByUserId(ulong gameId, ulong userId)
        {
            var userField = GetGameById(gameId)?.BattleFields.Count(bf => bf.Owner.Id.Equals(userId));
            return userField > 0;
        }
    }
}
