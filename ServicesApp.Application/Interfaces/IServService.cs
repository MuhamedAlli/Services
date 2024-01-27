using ServicesApp.Domain.DTOs;
using ServicesApp.Domain.DTOs.ServiceDTOs;

namespace ServicesApp.Application.Interfaces
{
    public interface IServService
    {
        public Task<ServiceResponseDto> CreateService(string imageName, CreateServiceDto createServiceDto);
        public Task<ServiceResponseDto> EditService(string imageName, EditServiceDto editServiceDto);
        public string? getExitedImageName(EditServiceDto editServiceDto);
        public Task<bool> DeleteService(int id);
        public ServiceResponseDto GetServiceById(int id);
        public Task<List<ServiceResponseDto>> GetServicesByTitle(string title);
        public Task<PagedResultDto<ServiceResponseDto>> GetAllServices(PagedInputsDto pagedInputsDto);

    }
}
