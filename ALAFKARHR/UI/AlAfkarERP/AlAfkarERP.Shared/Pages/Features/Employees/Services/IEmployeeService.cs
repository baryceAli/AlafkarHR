using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Employees.Dtos;
using SharedWithUI.HRCore.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Employees.Services;

public interface IEmployeeService
{
    public Task<ApiResult<EmployeeDto>> CreateAsync(EmployeeDto employee);
    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(EmployeeDto employee);
    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);

    public Task<ApiResult<UpdateDeleteResponseDto>> ChangePositionAsync(ChangePositionDto changePosition);
    public Task<ApiResult<UpdateDeleteResponseDto>> TerminateEmployeeAsync(TerminateEmployeeDto terminateEmployee);
    public Task<ApiResult<UpdateDeleteResponseDto>> TransferDepartmentAsync(TransferDepartmentDto transferDepartment);

    public Task<ApiResult<EmployeeDto>> GetByIdAsync(Guid id);
    public Task<ApiResult<PublicEmployeeViewDto>> GetPublicViewAsync(Guid id);
    public Task<ApiResult<PaginatedResult<EmployeeDto>>> GetAsync(int pageIndex, int pageSize);
    public Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByCompanyIdAsync(Guid companyId, int pageIndex, int pageSize,string? searchText="");
    public Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByBranchAsync(Guid branchId, int pageIndex, int pageSize);
    public Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByAdministrationAsync(Guid administrationId, int pageIndex, int pageSize);
    public Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByDepartmentAsync(Guid departmentId, int pageIndex, int pageSize);
    public Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByPositionAsync(Guid positionId, int pageIndex, int pageSize);

    public Task<ApiResult<List<HrLifecycleEventDto>>> GetLifecycleEventsAsync(Guid employeeId);
    public Task<ApiResult<HrLifecycleEventDto>> CreateLifecycleEventAsync(Guid employeeId, HrLifecycleEventDto lifecycleEvent);
    public Task<ApiResult<HrLifecycleEventDto>> UpdateLifecycleEventAsync(Guid employeeId, HrLifecycleEventDto lifecycleEvent);
    public Task<ApiResult<HrLifecycleEventDto>> TransitionLifecycleEventAsync(Guid employeeId, Guid eventId, string transition);
    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteLifecycleEventAsync(Guid employeeId, Guid eventId);

    public Task<ApiResult<List<EmployeeEmergencyContactDto>>> GetEmergencyContactsAsync(Guid employeeId);
    public Task<ApiResult<EmployeeEmergencyContactDto>> CreateEmergencyContactAsync(Guid employeeId, EmployeeEmergencyContactDto contact);
    public Task<ApiResult<EmployeeEmergencyContactDto>> UpdateEmergencyContactAsync(Guid employeeId, EmployeeEmergencyContactDto contact);
    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteEmergencyContactAsync(Guid employeeId, Guid contactId);

    public Task<ApiResult<List<EmployeeDocumentLinkDto>>> GetDocumentLinksAsync(Guid employeeId);
    public Task<ApiResult<EmployeeDocumentLinkDto>> CreateDocumentLinkAsync(Guid employeeId, EmployeeDocumentLinkDto document);
    public Task<ApiResult<EmployeeDocumentLinkDto>> UpdateDocumentLinkAsync(Guid employeeId, EmployeeDocumentLinkDto document);
    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteDocumentLinkAsync(Guid employeeId, Guid documentId);

    public Task<ApiResult<List<EmployeeSkillDto>>> GetSkillsAsync(Guid employeeId);
    public Task<ApiResult<EmployeeSkillDto>> CreateSkillAsync(Guid employeeId, EmployeeSkillDto skill);
    public Task<ApiResult<EmployeeSkillDto>> UpdateSkillAsync(Guid employeeId, EmployeeSkillDto skill);
    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteSkillAsync(Guid employeeId, Guid skillId);

    public Task<ApiResult<List<EmployeeCertificationDto>>> GetCertificationsAsync(Guid employeeId);
    public Task<ApiResult<EmployeeCertificationDto>> CreateCertificationAsync(Guid employeeId, EmployeeCertificationDto certification);
    public Task<ApiResult<EmployeeCertificationDto>> UpdateCertificationAsync(Guid employeeId, EmployeeCertificationDto certification);
    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteCertificationAsync(Guid employeeId, Guid certificationId);

}
