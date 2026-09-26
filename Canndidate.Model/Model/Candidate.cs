using Candidate.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Candidate.Domain.Model
{
    public class Candidate
    {
        [Key]
        public int CandidateId { get; set; }
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Gender Gender { get; set; }
        public DateTime DOB { get; set; }
        public List<Address> Address { get; set; }
        public string MaritalStatus { get; set; }
        public List<Language> Languages { get; set; } = new List<Language>();
        public List<Education> Educations { get; set; }
        public List<WorkExperience> Experiences { get; set; }
        public List<Skill> Skills { get; set; }
        public string Certifications { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
