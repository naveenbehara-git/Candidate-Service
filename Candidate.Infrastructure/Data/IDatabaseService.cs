using Candidate.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Candidate.Infrastructure.Data
{
    public interface IDatabaseService
    {
        public DbSet<Candidate.Domain.Model.Candidate> Candidate { get; set; }
        public DbSet<WorkExperience> Experience { get; set; }
        public DbSet<Education> Education { get; set; }
        public DbSet<Skill> Skill { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Language> Language { get; set; }

        void Save();
        Task SaveChangesAsync();
    }
}
