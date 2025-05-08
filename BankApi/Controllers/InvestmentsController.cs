using BankApi.Models;
using BankApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvestmentsController : ControllerBase
    {
        private readonly IInvestmentsRepository _investmentsRepository;

        public InvestmentsController(IInvestmentsRepository investmentsRepository)
        {
            _investmentsRepository = investmentsRepository ?? throw new ArgumentNullException(nameof(investmentsRepository));
        }

        [HttpGet]
        public ActionResult<List<Investment>> GetInvestmentsHistory()
        {
            try
            {
                var investments = _investmentsRepository.GetInvestmentsHistory();
                return Ok(investments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public ActionResult AddInvestment(Investment investment)
        {
            if (investment == null)
                return BadRequest("Investment cannot be null.");

            try
            {
                _investmentsRepository.AddInvestment(investment);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{investmentId}")]
        public ActionResult UpdateInvestment(int investmentId, [FromQuery] string investorCNP, [FromQuery] decimal amountReturned)
        {
            if (investmentId <= 0)
                return BadRequest("Invalid investment ID.");

            if (string.IsNullOrWhiteSpace(investorCNP))
                return BadRequest("Investor CNP cannot be empty.");

            try
            {
                _investmentsRepository.UpdateInvestment(investmentId, investorCNP, amountReturned);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}