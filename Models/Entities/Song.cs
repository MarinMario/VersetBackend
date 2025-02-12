using System.ComponentModel.DataAnnotations.Schema;

namespace VersuriAPI.Models.Entities
{
    public class Song
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Lyrics { get; set; }
        public required User User { get; set; }
    }
}
