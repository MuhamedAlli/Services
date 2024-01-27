using AutoMapper;
using ServicesApp.Domain.DTOs;
using ServicesApp.Domain.DTOs.ServiceDTOs;
namespace ServicesApp.Infrastructure.Persistence
{
    public class ServService : IServService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        public ServService(IMapper mapper, ApplicationDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<ServiceResponseDto> CreateService(string imageName, CreateServiceDto createServiceDto)
        {
            var service = new Service
            {
                Title = createServiceDto.Title,
                Description = createServiceDto.Description,
                Image = imageName,
                CreateOn = DateTime.Now,
                CreatedById = createServiceDto.CreatedById
            };
            _dbContext.Services.Add(service);
            var result = await _dbContext.SaveChangesAsync();
            if (result <= 0)
                return null;

            var serviceDto = _mapper.Map<ServiceResponseDto>(service);

            return serviceDto;
        }

        public async Task<ServiceResponseDto> EditService(string imageName, EditServiceDto editServiceDto)
        {
            var service = _dbContext.Services.Find(editServiceDto.Id);

            if (service is null)
                return null;

            service.Title = editServiceDto.Title;
            service.Description = editServiceDto.Description;
            service.Image = imageName;
            service.UpdatedById = editServiceDto.UpdatedById;
            service.UpdatedOn = DateTime.Now;

            var result = await _dbContext.SaveChangesAsync();
            if (result <= 0)
                return null;

            var serviceDto = _mapper.Map<ServiceResponseDto>(service);

            return serviceDto;
        }
        public string? getExitedImageName(EditServiceDto editServiceDto)
        {
            var user = _dbContext.Services.Find(editServiceDto.Id);
            if (user is null || user.Image is null)
                return null;

            return user.Image;
        }
        public async Task<bool> DeleteService(int id)
        {
            var service = _dbContext.Services.Find(id);

            if (service is null) return false;

            _dbContext.Services.Remove(service);
            var result = await _dbContext.SaveChangesAsync();
            if (result <= 0) return false;

            return true;
        }

        public ServiceResponseDto GetServiceById(int id)
        {
            var service = _dbContext.Services.Find(id);

            if (service is null) return null;

            var serviceDto = _mapper.Map<ServiceResponseDto>(service);

            return serviceDto;
        }
        public async Task<List<ServiceResponseDto>> GetServicesByTitle(string title)
        {
            var service = await _dbContext.Services.Where(s => s.Title.Contains(title)).ToListAsync();

            var serviceDto = _mapper.Map<List<ServiceResponseDto>>(service);

            return serviceDto;
        }
        public async Task<PagedResultDto<ServiceResponseDto>> GetAllServices(PagedInputsDto pagedInputsDto)
        {
            // Calculate the number of users to skip based on the page and pageSize
            int servicesToSkip = (pagedInputsDto.Page - 1) * pagedInputsDto.PageSize;
            // Retrieve total count of users
            int totalCount = await _dbContext.Services.CountAsync();
            int totalPages = totalCount % pagedInputsDto.PageSize == 0 ? (totalCount / pagedInputsDto.PageSize) : ((totalCount / pagedInputsDto.PageSize) + 1);

            var services = await _dbContext.Services.
                OrderBy(s => s.Id)
                .Skip(servicesToSkip)
                .Take(pagedInputsDto.PageSize)
                .ToListAsync();
            var servicesDto = _mapper.Map<IEnumerable<ServiceResponseDto>>(services);

            var pagedResultDto = new PagedResultDto<ServiceResponseDto>
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Page = pagedInputsDto.Page,
                PageSize = pagedInputsDto.PageSize,
                Data = servicesDto
            };

            return pagedResultDto;
        }
    }
}
