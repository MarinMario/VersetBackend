using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;

namespace VersuriAPI.Models.Entities
{
    public class User
    {
        [Key]
        public required string Email { get; set; }

        public required string Name { get; set; }
        public required bool Public { get; set; }
        public required DateTime CreationDate {  get; set; }
    }
}
