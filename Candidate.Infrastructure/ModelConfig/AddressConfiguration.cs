using Candidate.Domain.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candidate.Infrastructure.ModelConfig
{
    public class AddressConfiguration
    {
        public static Action<EntityTypeBuilder<Address>> Configuration
        {
            get
            {
                return entity =>
                {
                    entity.Property(e => e.CandidateId).IsRequired();
                };
            }
        }
    }
}
