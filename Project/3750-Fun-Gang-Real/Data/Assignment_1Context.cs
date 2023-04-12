using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Assignment_1.Models;

namespace Assignment_1.Data
{
    public class Assignment_1Context : DbContext
    {
        public Assignment_1Context(DbContextOptions<Assignment_1Context> options)
            : base(options)
        {
        }

        public DbSet<Assignment_1.Models.User> User { get; set; } = default!;
        public DbSet<Assignment_1.Models.Class> Class { get; set; } = default!; // allows us to get the context for the class table
        public DbSet<Assignment_1.Models.Registrations> Registrations { get; set; } = default!;
        public DbSet<Assignment_1.Models.ClassAssignments> ClassAssignments { get; set; } = default!;
        public DbSet<Assignment_1.Models.AssignmentSubmissions> AssignmentSubmissions { get; set; } = default!;
        public DbSet<Assignment_1.Models.Payment> Payment { get; set; } = default!;
    }
}
