using Microsoft.AspNetCore.Mvc;
//using StockApp.Models;
using BankApi.Models;
using StockApp.Repositories;
using BankApi.Repositories;
using System.Threading.Tasks;
using Src.Model;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatReportController : ControllerBase
    {
        private readonly IChatReportRepository _repository;

        public ChatReportController(IChatReportRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetReports()
        {
            var reports = _repository.GetAllChatReportsAsync();
            return Ok(reports);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var report = await _repository.GetChatReportByIdAsync(id);
            if (report == null)
                return NotFound();

            return Ok(report);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _repository.DeleteChatReportAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChatReport report)
        {
            if (report == null || string.IsNullOrEmpty(report.ReportedUserCnp) || string.IsNullOrEmpty(report.ReportedMessage))
            {
                return BadRequest("Invalid report data.");
            }

            await _repository.AddChatReportAsync(report);
            return CreatedAtAction(nameof(GetReports), new { id = report.Id }, report);
        }
    }
}
