using SeaBattle.Model.Domain;
using SeaBattle.Model.Dto;

namespace SeaBattle.Model.Mapper
{
    public class UserMapper
    {
        public UserDto ToDto(User user)
        {
            UserDto userDto = new()
            {
                Id = user.Id,
                Name = user.Name
            };
            return userDto;
        }

        public User ToEntity(UserDto newUser)
        {
            User user = new()
            {
                Id = newUser.Id,
                Name = newUser.Name,
                Login = newUser.Login,
                Password = newUser.Password
            };
            return user;
        }
    }
}
