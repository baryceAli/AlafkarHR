using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Auth.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Auth.Services;

public interface IAuthDashboardService
{
    Task<ApiResult<AuthDashboardDto>> GetDashboardAsync(Guid companyId);
}
