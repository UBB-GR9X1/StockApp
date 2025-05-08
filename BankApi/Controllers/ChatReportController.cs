﻿using BankApi.Models;
using BankApi.Repositories;
using Microsoft.AspNetCore.Mvc;
//using StockApp.Models;
using StockApp.Repositories;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatReportController : ControllerBase
    {
        private readonly IChatReportRepository _chatReportRepository;
        private readonly IUserRepository _userRepository;

        public ChatReportController(IChatReportRepository repository, IUserRepository userRepository)
        {
            _chatReportRepository = repository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetReportsAsync()
        {
            var reports = await _chatReportRepository.GetAllChatReportsAsync();
            return Ok(reports);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var report = await _chatReportRepository.GetChatReportByIdAsync(id);
            if (report == null)
                return NotFound();

            return Ok(report);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _chatReportRepository.DeleteChatReportAsync(id);
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

            await _chatReportRepository.AddChatReportAsync(report);
            return CreatedAtAction(nameof(GetReportsAsync), new { id = report.Id }, report);
        }

    }
}
