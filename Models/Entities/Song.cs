using System.ComponentModel.DataAnnotations.Schema;

namespace VersuriAPI.Models.Entities
{
    public enum TAccessFor { Public, Followers, Private }

    public class Song
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Lyrics { get; set; }
        public required string Description { get; set; }
        public required DateTime CreationDate { get; set; }
        public required DateTime LastUpdateDate { get; set; }
        public required TAccessFor AccessFor { get; set; }
        public required User User { get; set; }
    }
}
