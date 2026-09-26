using Candidate.Application.DTOs;
using Candidate.Infrastructure.Repositories;
using Candidate.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candidate.Application.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;

        public CandidateService(ICandidateRepository candidateRepository)
        {
            _candidateRepository = candidateRepository;
        }
        public async Task<Candidate.Domain.Model.Candidate> GetCandidateProfileById(Guid userId)
        {
            return await _candidateRepository.GetCandidateProfileById(userId);
        }

        public async Task<StatusDTO> AddCandidateProfile(Candidate.Domain.Model.Candidate candidate)
        {
            await _candidateRepository.InsertCandidate(candidate);
            return new StatusDTO { StatusCode = 200, StatusMessage = "Candidate profile added successfully." };
        }

        public async Task<StatusDTO> DeleteCandidateProfile(Guid userId)
        {
            await _candidateRepository.DeleteCandidate(userId);
            return new StatusDTO { StatusCode = 200, StatusMessage = "Candidate profile deleted successfully." };
        }

        public async Task<StatusDTO> UpdateCandidateProfile(Candidate.Domain.Model.Candidate candidate)
        {
            await _candidateRepository.UpdateCandidate(candidate);
            return new StatusDTO { StatusCode = 200, StatusMessage = "Candidate profile updated successfully." };
        }
    }
}
