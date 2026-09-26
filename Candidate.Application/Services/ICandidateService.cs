using Candidate.Application.DTOs;
using Candidate.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candidate.Application.Services
{
    public interface ICandidateService
    {
        Task<Candidate.Domain.Model.Candidate> GetCandidateProfileById(Guid userId);
        Task<StatusDTO> AddCandidateProfile(Candidate.Domain.Model.Candidate candidate);
        Task<StatusDTO> UpdateCandidateProfile(Candidate.Domain.Model.Candidate candidate);
        Task<StatusDTO> DeleteCandidateProfile(Guid userId);
    }
}
