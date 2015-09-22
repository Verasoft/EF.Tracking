using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verasoft.EF;

namespace Verasoft.EF.Tracking.Tests
{
    public class EFTrackingContext : TrackingDbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Department> Departments { get; set; }

        public EFTrackingContext()
            : base("EFTracking")
        {

        }
    }
}
