using ApplicationServices.DTOs.Dashboard;

namespace ApplicationServices.Interfaces;

public interface IDashboardManager
{
    Task<DashboardResponse> GetAsync(Guid employeeId);
}
