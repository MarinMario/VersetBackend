using VersuriAPI.Models.Entities;

namespace VersuriAPI.Models.Dtos
{
    public class SongDto
    {
        public required string Name { get; set; }
        public required string Lyrics { get; set; }
        public required string Description { get; set; }
        public required TAccessFor AccessFor { get; set; }
    }
}
