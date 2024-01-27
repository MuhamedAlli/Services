using ServicesApp.Domain.DTOs;
using ServicesApp.Domain.DTOs.AccountDTOs;
using ServicesApp.Domain.DTOs.Dashboard;

namespace ServicesApp.Application.Interfaces
{
    public interface IDashService
    {
        public StatisticsDto GetStatistics();
        public Task<PagedResultDto<GetAppUserDto>> getAllClients(PagedInputsDto pagedInputsDto);
        public Task<PagedResultDto<GetAppUserDto>> getClientByNameOrPhone(PagedSearchInputsDto pagedSearchInputsDto);
        public Task<PagedResultDto<GetAppUserDto>> getAllProviders(PagedInputsDto pagedInputsDto);
        public Task<PagedResultDto<GetAppUserDto>> getProviderByNameOrPhone(PagedSearchInputsDto pagedSearchInputsDto);
        public Task<PagedResultDto<GetAppUserDto>> getAllAdmins(PagedInputsDto pagedInputsDto);
        public Task<List<string>> getAllRoles();
    }
}
