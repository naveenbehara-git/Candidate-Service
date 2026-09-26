using Candidate.Domain.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candidate.Infrastructure.ModelConfig
{
    public class CandidateConfiguration
    {
        public static Action<EntityTypeBuilder<Candidate.Domain.Model.Candidate>> Configuration
        {
            get
            {
                return entity =>
                {
                    entity.Property(e => e.CandidateId).IsRequired();
                    entity.Property(e => e.MaritalStatus).IsRequired();
                    entity.Property(e => e.CreatedAt).IsRequired();

                    // Foreign Keys
                    entity.Property(e => e.UserId).IsRequired();
                };
            }
        }
    }
}
