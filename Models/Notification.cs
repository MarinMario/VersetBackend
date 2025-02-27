namespace VersuriAPI.Models
{
    public class Notification
    {
        public required Guid Id { get; set; }
        public required DateTime Date { get; set; }
        public required User User { get; set; }
    }
}
