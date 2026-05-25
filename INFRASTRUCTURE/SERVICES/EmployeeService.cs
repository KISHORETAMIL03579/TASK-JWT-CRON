using APPLICATION.DTO;
using APPLICATION.INTERFACE;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DOMAIN.ENITITES;
using DOMAIN.ENUM;
using INFRASTRUCTURE.CACHES;
using INFRASTRUCTURE.DATA;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace INFRASTRUCTURE.SERVICES;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;
    private readonly IMapper _mapper;

    private const string CacheKey = CacheKeys.Employees;

    public EmployeeService(AppDbContext db, IMemoryCache cache, IMapper mapper)
    {
        _db = db;
        _cache = cache;
        _mapper = mapper;
    }

    // ---------------- GET ALL ----------------
    public async Task<List<EmployeeDto>> GetAll()
    {
        return await _cache.GetOrCreateAsync<List<EmployeeDto>>(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            entry.SlidingExpiration = TimeSpan.FromSeconds(60);

            return await _db.Employees
                .AsNoTracking()
                .Where(e => e.Status == EmployeeStatus.Active)
                .ProjectTo<EmployeeDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }) ?? new List<EmployeeDto>();
    }

    // ---------------- GET BY ID ----------------
    public async Task<EmployeeDto?> GetById(Guid id)
    {
        var cacheKey = CacheKeys.EmployeeById(id);

        return await _cache.GetOrCreateAsync<EmployeeDto?>(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            entry.SlidingExpiration = TimeSpan.FromMinutes(1);

            var emp = await _db.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == EmployeeStatus.Active);

            return emp == null ? null : _mapper.Map<EmployeeDto>(emp);
        });
    }

    // ---------------- CREATE ----------------
    public async Task Create(CreateEmployeeDto dto)
    {
        var employee = _mapper.Map<Employee>(dto);

        employee.Id = Guid.NewGuid();
        employee.Status = EmployeeStatus.Active;

        await _db.Employees.AddAsync(employee);
        await _db.SaveChangesAsync();

        ClearCache(employee.Id);
    }

    // ---------------- UPDATE ----------------
    public async Task Update(Guid id, UpdateEmployeeDto dto)
    {
        var emp = await _db.Employees.FirstOrDefaultAsync(x => x.Id == id);

        if (emp == null) return;

        _mapper.Map(dto, emp);

        await _db.SaveChangesAsync();

        ClearCache(id);
    }

    // ---------------- DELETE (SOFT DELETE) ----------------
    public async Task Delete(Guid id)
    {
        var emp = await _db.Employees.FirstOrDefaultAsync(x => x.Id == id);

        if (emp == null) return;

        emp.Status = EmployeeStatus.Archived;

        await _db.SaveChangesAsync();

        ClearCache(id);
    }

    // ---------------- CACHE INVALIDATION ----------------
    private void ClearCache(Guid? id = null)
    {
        _cache.Remove(CacheKey);

        if (id.HasValue)
            _cache.Remove(CacheKeys.EmployeeById(id.Value));
    }
}