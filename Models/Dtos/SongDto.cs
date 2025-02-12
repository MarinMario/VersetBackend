using VersuriAPI.Models.Entities;

namespace VersuriAPI.Models.Dtos
{
    public class SongDto
    {
        public required string Name { get; set; }
        public required string Lyrics { get; set; }
        public required string UserGmail { get; set; }
    }
}
