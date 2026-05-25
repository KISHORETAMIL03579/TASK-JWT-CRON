
namespace INFRASTRUCTURE.CACHES
{
    public static class CacheKeys
    {
        public const string Employees = "employees";

        // Optional: future-proof keys (recommended)
        public static string EmployeeById(Guid id) => $"employee_{id}";
    }
}