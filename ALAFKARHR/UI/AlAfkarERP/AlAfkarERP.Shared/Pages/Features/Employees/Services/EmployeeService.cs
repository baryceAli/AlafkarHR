using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Employees.Dtos;
using SharedWithUI.HRCore.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Employees.Services;

public class EmployeeService :BaseApiService, IEmployeeService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;
    public EmployeeService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _apiConfig = apiConfig;
        _path = $"api/{_apiConfig.Version}/Employee/Employees";
    }

    public async Task<ApiResult<EmployeeDto>> CreateAsync(EmployeeDto employee)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}")
        {
            Content = JsonContent.Create(new
            {
                Employee = employee
            })
        };
        return await SendAsync<EmployeeDto>(request, "createdEmployee");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> ChangePositionAsync(ChangePositionDto changePosition)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/ChangePosition")
        {
            Content = JsonContent.Create(new
            {
                ChangePosition=changePosition
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
        //var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}");
        //return await SendAsync<UpdateDeleteResponseDto>(request, null);

    }

    public async Task<ApiResult<PaginatedResult<EmployeeDto>>> GetAsync(int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<EmployeeDto>>(request, "employeeList");
    }

    public async Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByAdministrationAsync(Guid administrationId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/administration/{administrationId}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<EmployeeDto>>(request, "employeeList");
    }

    public async Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByBranchAsync(Guid branchId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/branch/{branchId}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<EmployeeDto>>(request, "employeeList");
    }

    public async Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByCompanyIdAsync(Guid companyId, int pageIndex, int pageSize, string? searchText = "")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, 
                    $"{_path}/company/{companyId}?pageIndex={pageIndex}&pageSize={pageSize}&searchText={searchText}");
        return await SendAsync<PaginatedResult<EmployeeDto>>(request, "employeeList");
    }

    public async Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByDepartmentAsync(Guid departmentId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/department/{departmentId}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<EmployeeDto>>(request, "employeeList");
    }

    public async Task<ApiResult<EmployeeDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}");
        return await SendAsync<EmployeeDto>(request, "employee");
    }

    public async Task<ApiResult<PublicEmployeeViewDto>> GetPublicViewAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/public-view/{id}");
        return await SendAsync<PublicEmployeeViewDto>(request, "employee");
    }

    public async Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByPositionAsync(Guid positionId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/position/{positionId}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<EmployeeDto>>(request, "employeeList");
    }

    public async Task<ApiResult<List<HrLifecycleEventDto>>> GetLifecycleEventsAsync(Guid employeeId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{employeeId}/Lifecycle");
        return await SendAsync<List<HrLifecycleEventDto>>(request, "events");
    }

    public async Task<ApiResult<HrLifecycleEventDto>> CreateLifecycleEventAsync(Guid employeeId, HrLifecycleEventDto lifecycleEvent)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{employeeId}/Lifecycle")
        {
            Content = JsonContent.Create(new { Event = lifecycleEvent })
        };
        return await SendAsync<HrLifecycleEventDto>(request, "event");
    }

    public async Task<ApiResult<HrLifecycleEventDto>> UpdateLifecycleEventAsync(Guid employeeId, HrLifecycleEventDto lifecycleEvent)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/{employeeId}/Lifecycle/{lifecycleEvent.Id}")
        {
            Content = JsonContent.Create(new { Event = lifecycleEvent })
        };
        return await SendAsync<HrLifecycleEventDto>(request, "event");
    }

    public async Task<ApiResult<HrLifecycleEventDto>> TransitionLifecycleEventAsync(Guid employeeId, Guid eventId, string transition)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{employeeId}/Lifecycle/{eventId}/{transition}");
        return await SendAsync<HrLifecycleEventDto>(request, "event");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteLifecycleEventAsync(Guid employeeId, Guid eventId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{employeeId}/Lifecycle/{eventId}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<List<EmployeeEmergencyContactDto>>> GetEmergencyContactsAsync(Guid employeeId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{employeeId}/EmergencyContacts");
        return await SendAsync<List<EmployeeEmergencyContactDto>>(request, "contacts");
    }

    public async Task<ApiResult<EmployeeEmergencyContactDto>> CreateEmergencyContactAsync(Guid employeeId, EmployeeEmergencyContactDto contact)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{employeeId}/EmergencyContacts")
        {
            Content = JsonContent.Create(new { Contact = contact })
        };
        return await SendAsync<EmployeeEmergencyContactDto>(request, "contact");
    }

    public async Task<ApiResult<EmployeeEmergencyContactDto>> UpdateEmergencyContactAsync(Guid employeeId, EmployeeEmergencyContactDto contact)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/{employeeId}/EmergencyContacts/{contact.Id}")
        {
            Content = JsonContent.Create(new { Contact = contact })
        };
        return await SendAsync<EmployeeEmergencyContactDto>(request, "contact");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteEmergencyContactAsync(Guid employeeId, Guid contactId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{employeeId}/EmergencyContacts/{contactId}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<List<EmployeeDocumentLinkDto>>> GetDocumentLinksAsync(Guid employeeId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{employeeId}/Documents");
        return await SendAsync<List<EmployeeDocumentLinkDto>>(request, "documents");
    }

    public async Task<ApiResult<EmployeeDocumentLinkDto>> CreateDocumentLinkAsync(Guid employeeId, EmployeeDocumentLinkDto document)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{employeeId}/Documents")
        {
            Content = JsonContent.Create(new { Document = document })
        };
        return await SendAsync<EmployeeDocumentLinkDto>(request, "document");
    }

    public async Task<ApiResult<EmployeeDocumentLinkDto>> UpdateDocumentLinkAsync(Guid employeeId, EmployeeDocumentLinkDto document)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/{employeeId}/Documents/{document.Id}")
        {
            Content = JsonContent.Create(new { Document = document })
        };
        return await SendAsync<EmployeeDocumentLinkDto>(request, "document");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteDocumentLinkAsync(Guid employeeId, Guid documentId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{employeeId}/Documents/{documentId}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<List<EmployeeSkillDto>>> GetSkillsAsync(Guid employeeId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{employeeId}/Skills");
        return await SendAsync<List<EmployeeSkillDto>>(request, "skills");
    }

    public async Task<ApiResult<EmployeeSkillDto>> CreateSkillAsync(Guid employeeId, EmployeeSkillDto skill)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{employeeId}/Skills")
        {
            Content = JsonContent.Create(new { Skill = skill })
        };
        return await SendAsync<EmployeeSkillDto>(request, "skill");
    }

    public async Task<ApiResult<EmployeeSkillDto>> UpdateSkillAsync(Guid employeeId, EmployeeSkillDto skill)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/{employeeId}/Skills/{skill.Id}")
        {
            Content = JsonContent.Create(new { Skill = skill })
        };
        return await SendAsync<EmployeeSkillDto>(request, "skill");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteSkillAsync(Guid employeeId, Guid skillId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{employeeId}/Skills/{skillId}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<List<EmployeeCertificationDto>>> GetCertificationsAsync(Guid employeeId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{employeeId}/Certifications");
        return await SendAsync<List<EmployeeCertificationDto>>(request, "certifications");
    }

    public async Task<ApiResult<EmployeeCertificationDto>> CreateCertificationAsync(Guid employeeId, EmployeeCertificationDto certification)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{employeeId}/Certifications")
        {
            Content = JsonContent.Create(new { Certification = certification })
        };
        return await SendAsync<EmployeeCertificationDto>(request, "certification");
    }

    public async Task<ApiResult<EmployeeCertificationDto>> UpdateCertificationAsync(Guid employeeId, EmployeeCertificationDto certification)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/{employeeId}/Certifications/{certification.Id}")
        {
            Content = JsonContent.Create(new { Certification = certification })
        };
        return await SendAsync<EmployeeCertificationDto>(request, "certification");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteCertificationAsync(Guid employeeId, Guid certificationId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{employeeId}/Certifications/{certificationId}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    
    public async Task<ApiResult<UpdateDeleteResponseDto>> TerminateEmployeeAsync(TerminateEmployeeDto terminateEmployee)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/Terminate")
        {
            Content = JsonContent.Create(new
            {
                ChangePosition = terminateEmployee
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> TransferDepartmentAsync(TransferDepartmentDto transferDepartment)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/Transfer")
        {
            Content = JsonContent.Create(new
            {
                ChangePosition = transferDepartment
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(EmployeeDto employee)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}")
        {
            Content = JsonContent.Create(new
            {
                Employee = employee
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
