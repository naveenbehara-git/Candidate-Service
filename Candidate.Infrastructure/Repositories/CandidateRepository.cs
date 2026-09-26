using Candidate.Infrastructure.Data;
using Candidate.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candidate.Infrastructure.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly IDatabaseService _databaseService; 
        public CandidateRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        public async Task<Candidate.Domain.Model.Candidate> GetCandidateProfileById(Guid userId)
        {
            try
            {
                var candidate = await _databaseService.Candidate
                    .Include(c => c.Educations)
                    .Include(c => c.Languages)
                    .Include(c => c.Skills)
                    .Include(c => c.Experiences)
                    .Include(c => c.Address)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (candidate == null)
                {
                    throw new InvalidOperationException($"Candidate with UserId {userId} not found.");
                }

                return candidate;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the candidate profile.", ex);
            }
        }

        public async Task<Candidate.Domain.Model.Candidate> InsertCandidate(Candidate.Domain.Model.Candidate candidate)
        {
            try
            {
                var existingCandidate = await _databaseService.Candidate
                    .FirstOrDefaultAsync(c => c.UserId == candidate.UserId);
                if (existingCandidate != null)
                {
                    throw new InvalidOperationException("A candidate with the same UserId already exists.");
                }

                var result = await _databaseService.Candidate.AddAsync(candidate);
                foreach (var address in candidate.Address)
                {
                    await _databaseService.Address.AddAsync(address);
                }
                foreach(var language in candidate.Languages)
                {
                    await _databaseService.Language.AddAsync(language);
                }
                foreach(var education in candidate.Educations)
                {
                    await _databaseService.Education.AddAsync(education);
                }
                foreach(var skill in candidate.Skills)
                {
                    await _databaseService.Skill.AddAsync(skill);
                }
                foreach(var experience in candidate.Experiences)
                {
                    await _databaseService.Experience.AddAsync(experience);
                }

                _databaseService.Save();  
                return result.Entity;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while inserting the candidate.", ex);
            }
        }

        public async Task<bool> DeleteCandidate(Guid userId)
        {
            try
            {
                // Load the candidate with all related entities
                var candidate = await _databaseService.Candidate
                    .Include(c => c.Address)
                    .Include(c => c.Languages)
                    .Include(c => c.Educations)
                    .Include(c => c.Skills)
                    .Include(c => c.Experiences)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (candidate == null)
                {
                    throw new InvalidOperationException($"Candidate with UserId {userId} not found.");
                }

                // Delete all related entities first (children before parent)
                // Delete Addresses
                if (candidate.Address != null && candidate.Address.Count > 0)
                {
                    _databaseService.Address.RemoveRange(candidate.Address);
                }

                // Delete Languages
                if (candidate.Languages != null && candidate.Languages.Count > 0)
                {
                    _databaseService.Language.RemoveRange(candidate.Languages);
                }

                // Delete Educations
                if (candidate.Educations != null && candidate.Educations.Count > 0)
                {
                    _databaseService.Education.RemoveRange(candidate.Educations);
                }

                // Delete Skills
                if (candidate.Skills != null && candidate.Skills.Count > 0)
                {
                    _databaseService.Skill.RemoveRange(candidate.Skills);
                }

                // Delete Experiences (WorkExperience)
                if (candidate.Experiences != null && candidate.Experiences.Count > 0)
                {
                    _databaseService.Experience.RemoveRange(candidate.Experiences);
                }

                // Finally, delete the candidate
                _databaseService.Candidate.Remove(candidate);

                // Save all changes in a single transaction
                _databaseService.Save();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the candidate.", ex);
            }
        }

        public async Task<Candidate.Domain.Model.Candidate> UpdateCandidate(Candidate.Domain.Model.Candidate candidateUpdate)
        {
            try
            {
                // Load the existing candidate with all related entities
                var existingCandidate = await _databaseService.Candidate
                    .Include(c => c.Address)
                    .Include(c => c.Languages)
                    .Include(c => c.Educations)
                    .Include(c => c.Skills)
                    .Include(c => c.Experiences)
                    .FirstOrDefaultAsync(c => c.CandidateId == candidateUpdate.CandidateId);

                if (existingCandidate == null)
                {
                    throw new InvalidOperationException($"Candidate with ID {candidateUpdate.CandidateId} not found.");
                }

                // Update basic candidate properties
                existingCandidate.FirstName = candidateUpdate.FirstName ?? existingCandidate.FirstName;
                existingCandidate.LastName = candidateUpdate.LastName ?? existingCandidate.LastName;
                existingCandidate.Email = candidateUpdate.Email ?? existingCandidate.Email;
                existingCandidate.PhoneNumber = candidateUpdate.PhoneNumber ?? existingCandidate.PhoneNumber;
                existingCandidate.Gender = candidateUpdate.Gender;
                existingCandidate.DOB = candidateUpdate.DOB != default ? candidateUpdate.DOB : existingCandidate.DOB;
                existingCandidate.MaritalStatus = candidateUpdate.MaritalStatus ?? existingCandidate.MaritalStatus;
                existingCandidate.Certifications = candidateUpdate.Certifications ?? existingCandidate.Certifications;
                existingCandidate.UpdatedAt = DateTime.UtcNow;

                // Update Addresses - Remove old and add new
                if (existingCandidate.Address != null && existingCandidate.Address.Count > 0)
                {
                    _databaseService.Address.RemoveRange(existingCandidate.Address);
                }
                if (candidateUpdate.Address != null && candidateUpdate.Address.Count > 0)
                {
                    foreach (var address in candidateUpdate.Address)
                    {
                        await _databaseService.Address.AddAsync(address);
                    }
                }

                // Update Languages - Remove old and add new
                if (existingCandidate.Languages != null && existingCandidate.Languages.Count > 0)
                {
                    _databaseService.Language.RemoveRange(existingCandidate.Languages);
                }
                if (candidateUpdate.Languages != null && candidateUpdate.Languages.Count > 0)
                {
                    foreach (var language in candidateUpdate.Languages)
                    {
                        await _databaseService.Language.AddAsync(language);
                    }
                }

                // Update Educations - Remove old and add new
                if (existingCandidate.Educations != null && existingCandidate.Educations.Count > 0)
                {
                    _databaseService.Education.RemoveRange(existingCandidate.Educations);
                }
                if (candidateUpdate.Educations != null && candidateUpdate.Educations.Count > 0)
                {
                    foreach (var education in candidateUpdate.Educations)
                    {
                        await _databaseService.Education.AddAsync(education);
                    }
                }

                // Update Skills - Remove old and add new
                if (existingCandidate.Skills != null && existingCandidate.Skills.Count > 0)
                {
                    _databaseService.Skill.RemoveRange(existingCandidate.Skills);
                }
                if (candidateUpdate.Skills != null && candidateUpdate.Skills.Count > 0)
                {
                    foreach (var skill in candidateUpdate.Skills)
                    {
                        await _databaseService.Skill.AddAsync(skill);
                    }
                }

                // Update Experiences (WorkExperience) - Remove old and add new
                if (existingCandidate.Experiences != null && existingCandidate.Experiences.Count > 0)
                {
                    _databaseService.Experience.RemoveRange(existingCandidate.Experiences);
                }
                if (candidateUpdate.Experiences != null && candidateUpdate.Experiences.Count > 0)
                {
                    foreach (var experience in candidateUpdate.Experiences)
                    {
                        await _databaseService.Experience.AddAsync(experience);
                    }
                }

                // Mark the candidate entity as modified
                _databaseService.Candidate.Update(existingCandidate);

                // Save all changes in a single transaction
                _databaseService.Save();

                return existingCandidate;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the candidate.", ex);
            }
        }
    }
}
