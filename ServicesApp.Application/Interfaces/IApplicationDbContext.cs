using Microsoft.EntityFrameworkCore;
using ServicesApp.Domain.Entities;

namespace ServicesApp.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<Service> Services { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Otp> Otps { get; set; }
    }
}
