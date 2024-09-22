using Microsoft.EntityFrameworkCore;

namespace WebApplication5.Models;

public class OrderStateDbContext : DbContext
{
    public OrderStateDbContext(DbContextOptions<OrderStateDbContext> options) : base(options) {}
}