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
    public class BillSplitReportController : ControllerBase
    {
        private readonly IBillSplitReportRepository _repository;
        private readonly ILogger<BillSplitReportController> _logger;

        public BillSplitReportController(IBillSplitReportRepository repository, ILogger<BillSplitReportController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/BillSplitReport
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BillSplitReport>>> GetAllReports()
        {
            try
            {
                var reports = await _repository.GetAllReportsAsync();
                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all bill split reports");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        // GET: api/BillSplitReport/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BillSplitReport>> GetReportById(int id)
        {
            try
            {
                var report = await _repository.GetReportByIdAsync(id);
                return Ok(report);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Report not found: {ReportId}", id);
                return NotFound($"Report with ID '{id}' not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving report by id: {ReportId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        // POST: api/BillSplitReport
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

                var createdReport = await _repository.AddReportAsync(report);
                
                return CreatedAtAction(
                    nameof(GetReportById), 
                    new { id = createdReport.Id }, 
                    createdReport);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating report");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating report");
            }
        }

        // PUT: api/BillSplitReport/{id}
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

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != report.Id)
                {
                    return BadRequest("ID in URL does not match the report ID in the body");
                }

                await _repository.UpdateReportAsync(report);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Report not found during update: {ReportId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating report: {ReportId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating report");
            }
        }

        // DELETE: api/BillSplitReport/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteReport(int id)
        {
            try
            {
                var success = await _repository.DeleteReportAsync(id);
                
                if (!success)
                {
                    return NotFound($"Report with ID '{id}' not found");
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting report: {ReportId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting report");
            }
        }
    }
} 