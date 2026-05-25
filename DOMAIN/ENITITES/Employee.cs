using DOMAIN.ENUM;

public class Employee
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public EmployeeRole Role { get; set; }
    public EmployeeStatus Status { get; set; }
    public DateTime? NoticeEndDate { get; set; }
}