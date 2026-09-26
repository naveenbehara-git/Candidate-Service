using Candidate.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candidate.Infrastructure.Repositories
{
    public interface ICandidateRepository
    {
        Task<Candidate.Domain.Model.Candidate> GetCandidateProfileById(Guid UserId);
        Task<Candidate.Domain.Model.Candidate> InsertCandidate(Candidate.Domain.Model.Candidate candidate);
        Task<Candidate.Domain.Model.Candidate> UpdateCandidate(Candidate.Domain.Model.Candidate candidate);
        Task<bool> DeleteCandidate(Guid userId);
    }
}
