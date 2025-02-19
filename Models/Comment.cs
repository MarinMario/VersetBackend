namespace VersuriAPI.Models
{
    public class Comment
    {
        public required Guid Id { get; set; }
        public required string Content { get; set; }
        public required DateTime CreationDate { get; set; }
        public required bool Edited { get; set; }
        public required User User { get; set; }
        public required Song Song { get; set; }
    }

    public class DtoCommentAdd
    {
        public required string Content { get; set; }
        public required Guid SongId { get; set; }
    }

    public class DtoCommentPublic
    {
        public required Guid Id { get; set; }
        public required string Content { get; set; }
        public required DateTime CreationDate { get; set; }
        public required bool Edited { get; set; }
        public required DtoUserPublic User { get; set; }
        public required Guid SongId { get; set; }
    }
}
