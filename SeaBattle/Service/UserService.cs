using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SeaBattle.Exceptions;
using SeaBattle.Model.Domain;
using SeaBattle.Model.Dto;
using SeaBattle.Repository;
using System.Linq;
using SeaBattle.Controller.View;
using SeaBattle.Logger;
using SeaBattle.Model.Mapper;


namespace SeaBattle.Service
{
    class UserService
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<UserService>();
        private readonly ApplicationDbContext context;
        private readonly UserMapper userMapper;
        private readonly DbSet<User> users;

        public UserService(ApplicationDbContext context, UserMapper userMapper)
        {
            this.context = context;
            this.userMapper = userMapper;
            users = context.Users;
        }

        public List<UserDto> GetByIds(List<ulong> ids)
        {
            if (ids.Count == 0)
            {
                return new List<UserDto>();
            }
            var result = from user in users
                         where ids.Any(id => user.Id.GetValueOrDefault().Equals(id))
                         select user;

            return result.Select(user => userMapper.ToDto(user)).ToList();
        }

        public UserDto GetUserByLoginAndPassword(string login, string password)
        {
            log.Debug("Getting user by login and password");
            User user = (from u in users
                         where u.Login.Equals(login) && u.Password.Equals(password)
                         select u).FirstOrDefault();

            if (user is not null)
            {
                return userMapper.ToDto(user);
            }

            return null;
        }

        public UserDto SaveUser(UserDto user)
        {
            log.Debug($"Save user id:{user.Id}");
            User newUser = userMapper.ToEntity(user);
            users.Add(newUser);
            try
            {
                context.SaveChanges();
                return userMapper.ToDto(newUser);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("UNIQUE constraint failed: UsersIds.Login") ?? false)
                {
                    throw new UserAlreadyExists($"User with login: '{newUser.Login}' already exists.");
                }
                throw;
            }
        }

        public bool ExistsByLogin(string login)
        {
            var user = users.FirstOrDefault(u => u.Login.Equals(login));
            return user is not null;
        }
    }
}
