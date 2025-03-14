namespace MountainGoatsBikes.Models
{
	public class Customer
	{
		public int CustomerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string? Phone { get; set; }
		public string Email { get; set; } = string.Empty;
		public string? Street { get; set; }
		public string? City { get; set; }
		public string? State { get; set; }
		public string? ZipCode { get; set; }
	}
}
