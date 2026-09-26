using Candidate.Infrastructure.ModelConfig;
using Candidate.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Candidate.Infrastructure.Data
{
    public class DatabaseService : DbContext, IDatabaseService
    {
        public string DefaultUserName { get; set; } = "System";

        public DatabaseService(DbContextOptions<DatabaseService> options) : base(options)
        {
        }

        public DbSet<Candidate.Domain.Model.Candidate> Candidate { get; set; }
        public DbSet<WorkExperience> Experience { get; set; }
        public DbSet<Education> Education { get; set; }
        public DbSet<Skill> Skill { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Language> Language { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //custom model configuration for CandidateProfile entity
            modelBuilder.Entity(CandidateConfiguration.Configuration);
            modelBuilder.Entity(WorkExpConfigueation.Configuration);
            modelBuilder.Entity(EducationConfiguration.Configuration);
            modelBuilder.Entity(SkillConfiguration.Configuration);
            modelBuilder.Entity(AddressConfiguration.Configuration);
            modelBuilder.Entity(LanguageConfiguration.Configuration);

        }

        public void Save()
        {
            SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await SaveChangesAsync();
        }
    }
}
