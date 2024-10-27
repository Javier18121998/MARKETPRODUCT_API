using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Market.Models
{
    public class MarketDbContext : DbContext
    {
        public DbSet<ProductDto> Products { get; set; }
        public DbSet<OrderDto> Orders { get; set; }

        public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options)
        {
        }
    }
}
