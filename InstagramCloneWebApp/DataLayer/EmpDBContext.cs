using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InstagramCloneWebApp.Entities;
using System.Diagnostics.CodeAnalysis;

namespace InstagramCloneWebApp.DataLayer
{
    public class EmpDBContext : DbContext
    {
        public EmpDBContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
        }

        public DbSet<ImageEntity> ImagesDetails { get; set; }
    }
}
