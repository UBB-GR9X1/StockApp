using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankApi.Models;
using BankApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillSplitReportsController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly ILogger<BillSplitReportsController> _logger;

        public BillSplitReportsController(IRepository repository, ILogger<BillSplitReportsController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/BillSplitReports
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<BillSplitReport>>> GetAllReports()
        {
            try
            {
                var reports = await _repository.GetAllBillSplitReportsAsync();
                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all bill split reports");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        // GET: api/BillSplitReports/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BillSplitReport>> GetReportById(int id)
        {
            try
            {
                var report = await _repository.GetBillSplitReportByIdAsync(id);
                return Ok(report);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Bill split report not found: {ReportId}", id);
                return NotFound($"Bill split report with ID '{id}' not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bill split report by id: {ReportId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        // POST: api/BillSplitReports
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BillSplitReport>> CreateReport([FromBody] BillSplitReport report)
        {
            try
            {
                if (report == null)
                {
                    return BadRequest("Report data is null");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdReport = await _repository.AddBillSplitReportAsync(report);
                
                return CreatedAtAction(nameof(GetReportById), 
                    new { id = createdReport.Id }, 
                    createdReport);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bill split report");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating bill split report");
            }
        }

        // PUT: api/BillSplitReports/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateReport(int id, [FromBody] BillSplitReport report)
        {
            try
            {
                if (report == null)
                {
                    return BadRequest("Report data is null");
                }

                if (id != report.Id)
                {
                    return BadRequest("ID in URL does not match ID in request body");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _repository.UpdateBillSplitReportAsync(report);
                
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Bill split report not found during update: {ReportId}", id);
                return NotFound($"Bill split report with ID '{id}' not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bill split report: {ReportId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating bill split report");
            }
        }

        // DELETE: api/BillSplitReports/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteReport(int id)
        {
            try
            {
                var result = await _repository.DeleteBillSplitReportAsync(id);
                
                if (!result)
                {
                    return NotFound($"Bill split report with ID '{id}' not found");
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bill split report: {ReportId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting bill split report");
            }
        }

        // GET: api/BillSplitReports/balance/{userCnp}
        [HttpGet("balance/{userCnp}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetCurrentBalance(string userCnp)
        {
            try
            {
                var balance = await _repository.GetCurrentBalanceAsync(userCnp);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving balance for user: {UserCnp}", userCnp);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving balance");
            }
        }

        // GET: api/BillSplitReports/creditScore/{userCnp}
        [HttpGet("creditScore/{userCnp}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetCreditScore(string userCnp)
        {
            try
            {
                var creditScore = await _repository.GetCurrentCreditScoreAsync(userCnp);
                return Ok(creditScore);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving credit score for user: {UserCnp}", userCnp);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving credit score");
            }
        }

        // PUT: api/BillSplitReports/creditScore/{userCnp}
        [HttpPut("creditScore/{userCnp}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCreditScore(string userCnp, [FromBody] int newCreditScore)
        {
            try
            {
                await _repository.UpdateCreditScoreAsync(userCnp, newCreditScore);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating credit score for user: {UserCnp}", userCnp);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating credit score");
            }
        }
    }
} 