namespace MountainGoatsBikes.Models
{
    public class Staff
    {
        public int StaffId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }     
        public byte Active { get; set; }
        public int StoreId { get; set; }
        public int? ManagerId { get; set; }
    }
}
