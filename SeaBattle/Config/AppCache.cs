using SeaBattle.Model.Dto;

namespace SeaBattle.Config
{
    class AppCache
    {
        public ulong? NextGameId { get; set; }
        public UserDto Admin { get; set; }
        public UserDto Opponent { get; set; }
        public GameDto CurrentGame { get; set; }
    }
}
