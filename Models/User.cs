using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;

namespace VersuriAPI.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required bool Public { get; set; }
        public required DateTime CreationDate { get; set; }
        public required List<Guid> LikedSongs { get; set; }
        public required List<Guid> DislikedSongs { get; set; }
    }

    public class DtoUserUpdate
    {
        public required string Name { get; set; }
        public required bool Public { get; set; }
    }

    public class DtoUserPublic
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required bool Public { get; set; }
        public required DateTime CreationDate { get; set; }

    }
}
