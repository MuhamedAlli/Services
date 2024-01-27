using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ServicesApp.Domain.DTOs;
using ServicesApp.Domain.DTOs.AccountDTOs;
using ServicesApp.Domain.DTOs.Dashboard;
using ServicesApp.Domain.Enums;
using ServicesApp.Infrastructure.Consts;

namespace ServicesApp.Infrastructure.Persistence
{
    public class DashService : IDashService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public DashService(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, IAccountService accountService, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _accountService = accountService;
            _mapper = mapper;
            _roleManager = roleManager;
        }
        public StatisticsDto GetStatistics()
        {
            var providersCount = _userManager.Users.Where(u => u.Type == UserType.Provider).Count();
            var clientsCount = _userManager.Users.Where(u => u.Type == UserType.Client).Count();
            var ServicesCount = _dbContext.Services.Count();
            var pendingOrdersCount = _dbContext.Orders.Where(o => o.State == OrderSatet.Pending).Count();
            var acceptedOrdersCount = _dbContext.Orders.Where(o => o.State == OrderSatet.Accepted).Count();
            var rejectedOrdersCount = _dbContext.Orders.Where(o => o.State == OrderSatet.Rejected).Count();
            var canceledOrdersCount = _dbContext.Orders.Where(o => o.State == OrderSatet.Canceled).Count();
            var completedOrdersCount = _dbContext.Orders.Where(o => o.State == OrderSatet.Completed).Count();
            var blockedOrdersCount = _dbContext.Orders.Where(o => o.State == OrderSatet.Blocked).Count();
            var statisticsDto = new StatisticsDto
            {
                ProvidersCount = providersCount,
                ClientsCount = clientsCount,
                ServicesCount = ServicesCount,
                PendingOrdersCount = pendingOrdersCount,
                AcceptedOrdersCount = acceptedOrdersCount,
                RejectedOrdersCount = rejectedOrdersCount,
                CanceledOrdersCount = canceledOrdersCount,
                CompletedOrdersCount = completedOrdersCount,
                BlockedOrdersCount = blockedOrdersCount
            };
            return statisticsDto;
        }

        public async Task<PagedResultDto<GetAppUserDto>> getAllAdmins(PagedInputsDto pagedInputsDto)
        {
            // Calculate the number of users to skip based on the page and pageSize
            int adminsToSkip = (pagedInputsDto.Page - 1) * pagedInputsDto.PageSize;

            // Retrieve users from UserManager with pagination
            var superAdmin = _userManager
                .GetUsersInRoleAsync(AppRoles.SuperAdmin).Result;

            var admins = _userManager
                .GetUsersInRoleAsync(AppRoles.Admin).Result;
            List<ApplicationUser> allAdmins = superAdmin.Concat(admins)
                 .OrderBy(u => u.Id) // Order by some property, adjust as needed
                 .Skip(adminsToSkip)
                 .Take(pagedInputsDto.PageSize)
                 .ToList();

            // Retrieve total count of users
            int totalCount = allAdmins.Count;
            //int n = _dbContext.Users.Where(u=>u.);
            int totalPages = totalCount % pagedInputsDto.PageSize == 0 ? (totalCount / pagedInputsDto.PageSize) : ((totalCount / pagedInputsDto.PageSize) + 1);


            // Map users to DTOs
            var adminsDto = _mapper.Map<IEnumerable<GetAppUserDto>>(allAdmins);
            foreach (var admin in adminsDto)
            {
                var user = await _userManager.FindByIdAsync(admin.Id);
                var roles = await _userManager.GetRolesAsync(user);
                admin.Roles = roles.ToList();
            }
            var pagedResultDto = new PagedResultDto<GetAppUserDto>
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Page = pagedInputsDto.Page,
                PageSize = pagedInputsDto.PageSize,
                Data = adminsDto
            };

            return pagedResultDto;
        }

        public async Task<PagedResultDto<GetAppUserDto>> getAllClients(PagedInputsDto pagedInputsDto)
        {
            var pagedClientsDto = await _accountService.getPagedUsers(UserType.Client, pagedInputsDto);
            return pagedClientsDto;

        }

        public async Task<PagedResultDto<GetAppUserDto>> getAllProviders(PagedInputsDto pagedInputsDto)
        {
            var pagedProvidersDto = await _accountService.getPagedUsers(UserType.Provider, pagedInputsDto);
            return pagedProvidersDto;
        }

        public async Task<List<string>> getAllRoles()
        {
            var roles = await _roleManager.Roles.Where(r => r.Name != "USER").Select(r => r.Name).ToListAsync();

            return roles;
        }
        public async Task<PagedResultDto<GetAppUserDto>> getClientByNameOrPhone(PagedSearchInputsDto pagedSearchInputsDto)
        {
            var clients = await getUserByNameOrPhone(UserType.Client, pagedSearchInputsDto);
            return clients;
        }
        public async Task<PagedResultDto<GetAppUserDto>> getProviderByNameOrPhone(PagedSearchInputsDto pagedSearchInputsDto)
        {
            var providers = await getUserByNameOrPhone(UserType.Provider, pagedSearchInputsDto);
            return providers;
        }
        private async Task<PagedResultDto<GetAppUserDto>> getUserByNameOrPhone(UserType userType, PagedSearchInputsDto pagedSearchInputsDto)
        {
            // Calculate the number of users to skip based on the page and pageSize
            int usersToSkip = (pagedSearchInputsDto.Page - 1) * pagedSearchInputsDto.PageSize;

            // Retrieve total count of users
            int totalCount = await _userManager.Users
                .Where(u => (u.UserName.Contains(pagedSearchInputsDto.SearchKey) || u.FullName.Contains(pagedSearchInputsDto.SearchKey)) && u.Type == userType)
                 .CountAsync();
            //int n = _dbContext.Users.Where(u=>u.);
            int totalPages = totalCount % pagedSearchInputsDto.PageSize == 0 ? (totalCount / pagedSearchInputsDto.PageSize) : ((totalCount / pagedSearchInputsDto.PageSize) + 1);

            var users = await _userManager.Users.Include(u => u.Service)
                .Where(u => (u.UserName.Contains(pagedSearchInputsDto.SearchKey) || u.FullName.Contains(pagedSearchInputsDto.SearchKey)) && u.Type == userType)
                .OrderBy(u => u.Id) // Order by some property, adjust as needed
                .Skip(usersToSkip)
                .Take(pagedSearchInputsDto.PageSize)
                .ToListAsync();

            var usersDto = _mapper.Map<List<GetAppUserDto>>(users);
            var pagedResultDto = new PagedResultDto<GetAppUserDto>
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Page = pagedSearchInputsDto.Page,
                PageSize = pagedSearchInputsDto.PageSize,
                Data = usersDto
            };

            return pagedResultDto;
        }
    }
}
