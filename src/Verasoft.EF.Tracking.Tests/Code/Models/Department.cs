using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF.Tracking.Tests
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual Project Project { get; set; }
    }
}
