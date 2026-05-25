using DOMAIN.ENUM;
using System.ComponentModel.DataAnnotations;

namespace APPLICATION.DTO
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? NoticeEndDate { get; set; }
    }

    public class CreateEmployeeDto
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public EmployeeRole Role { get; set; }
        public DateTime? NoticeEndDate { get; set; }
    }

    public class UpdateEmployeeDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public EmployeeRole? Role { get; set; }
        public DateTime? NoticeEndDate { get; set; }
    }
}
