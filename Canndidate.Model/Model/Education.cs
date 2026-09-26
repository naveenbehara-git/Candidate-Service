using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Candidate.Domain.Model
{
    public class Education : DatePeriod
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Candidate.CandidateId))]
        public int CandidateId { get; set; }    
        public string InstitutionName { get; set; }
        public string Specification { get; set; }
        public decimal CGPA { get; set; }
        public string Address { get; set; }
    }
}
