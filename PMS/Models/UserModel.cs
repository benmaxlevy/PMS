using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace PMS.Models
{
    internal class User
    {
        public int? ID { get; set; }
        [Required]
        public string? Username { get; set; }
        public string? HashedPassword { get; set; }
        public double? HourlyRate { get; set; }
        public virtual ICollection<DayIncome>? Incomes { get; set; }
    }

    internal class DayIncome
    {
        public int? ID { get; set; }
        public virtual User? User { get; set; }
        public virtual DateTime? Timestamp { get; set; }
        public double? Income { get; set; }
    }
}