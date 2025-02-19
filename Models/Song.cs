using System.ComponentModel.DataAnnotations.Schema;

namespace VersuriAPI.Models
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

    public class DtoSongAdd
    {
        public required string Name { get; set; }
        public required string Lyrics { get; set; }
        public required string Description { get; set; }
        public required TAccessFor AccessFor { get; set; }
    }

    public class DtoSongUpdate
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Lyrics { get; set; }
        public required string Description { get; set; }
        public required TAccessFor AccessFor { get; set; }
    }


    public class DtoSongPublic
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Lyrics { get; set; }
        public required string Description { get; set; }
        public required TAccessFor AccessFor { get; set; }
        public required DateTime CreationDate { get; set; }
        public required DateTime LastUpdateDate { get; set; }
        public required int Likes { get; set; }
        public required int Dislikes { get; set; }
        public required int Comments { get; set; }
        public required DtoUserPublic User { get; set; }
    }
}
