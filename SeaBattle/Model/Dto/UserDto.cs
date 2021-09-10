namespace SeaBattle.Model.Dto
{
    public class UserDto
    {
        public ulong? Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public UserDto()
        {
        }

        public UserDto(string name, string login, string password)
        {
            Name = name;
            Login = login;
            Password = password;
        }
    }
}
