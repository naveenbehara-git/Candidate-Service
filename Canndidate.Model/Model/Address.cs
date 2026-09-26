using Candidate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candidate.Domain.Model
{
    public class Address
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Candidate.CandidateId))]
        public int CandidateId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public AddressType AddressType { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int PostalCode { get; set; }
    }
}
