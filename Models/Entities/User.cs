using System.ComponentModel.DataAnnotations;

namespace VersuriAPI.Models.Entities
{
    public class User
    {
        [Key]
        public required string Gmail { get; set; }

        public required string Name { get; set; }
    }
}
