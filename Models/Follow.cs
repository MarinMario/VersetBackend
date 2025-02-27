using System.ComponentModel.DataAnnotations;

namespace VersuriAPI.Models
{

    public enum FollowStatusType
    {
        None,
        Requested,
        Following,

    }

    public class Follow
    {
        public required Guid Id { get; set; }
        public required User User { get; set; }
        public required User Follows { get; set; }
        public required FollowStatusType FollowStatus { get; set; }
    }

    public class DtoFollowPublic
    {
        public required Guid Id { get; set; }
        public required DtoUserPublic User { get; set; }
        public required DtoUserPublic Follows { get; set; }
        public required FollowStatusType FollowStatus { get; set; }
    }

    public class DtoFollowStatus
    {
        public required FollowStatusType FollowStatus { get; set; } 
    }
}
