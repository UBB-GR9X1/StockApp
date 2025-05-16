using BankApi.Repositories;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoanRequestController : ControllerBase
    {
        private readonly ILoanRequestRepository _loanRequestRepository;

        public LoanRequestController(ILoanRequestRepository loanRequestRepository)
        {
            _loanRequestRepository = loanRequestRepository ?? throw new ArgumentNullException(nameof(loanRequestRepository));
        }

        [HttpGet]
        public async Task<ActionResult<List<LoanRequest>>> GetLoanRequests()
        {
            var loanRequests = await _loanRequestRepository.GetLoanRequestsAsync();
            return Ok(loanRequests);
        }

        [HttpGet("unsolved")]
        public async Task<ActionResult<List<LoanRequest>>> GetUnsolvedLoanRequests()
        {
            var unsolvedLoanRequests = await _loanRequestRepository.GetUnsolvedLoanRequestsAsync();
            return Ok(unsolvedLoanRequests);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLoanRequest(int id)
        {
            try
            {
                await _loanRequestRepository.DeleteLoanRequestAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Loan request with ID {id} not found.");
            }
        }

        [HttpPatch("{id}/solve")]
        public async Task<ActionResult> SolveLoanRequest(int id)
        {
            try
            {
                await _loanRequestRepository.SolveLoanRequestAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Loan request with ID {id} not found.");
            }
        }
    }
}
