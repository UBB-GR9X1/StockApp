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

        [HttpPost("punish")]
        public async Task<IActionResult> PunishUser([FromBody] ChatReport chatReport)
        {
            if (chatReport == null || string.IsNullOrEmpty(chatReport.ReportedUserCnp))
            {
                return BadRequest("Invalid report.");
            }

            var user = await _userRepository.GetUserByCnpAsync(chatReport.ReportedUserCnp);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            const int BASE_PENALTY = 15;
            int noOffenses = user.NumberOfOffenses;
            int penalty = noOffenses >= 3 ? BASE_PENALTY * noOffenses : BASE_PENALTY;

            await _userRepository.PenalizeUserAsync(user.CNP, penalty);
            await _userRepository.IncrementOffensesCountAsync(user.CNP);

            _chatReportRepository.DeleteChatReportAsync(chatReport.Id);
            _chatReportRepository.UpdateActivityLogAsync(user.CNP, penalty);

            var tipsService = new TipsService(new TipsRepository(_db)); //?
            tipsService.GiveTipToUser(user.CNP);

            int tipCount = _chatReportRepository.GetNumberOfGivenTipsForUserAsync(user.CNP);
            if (tipCount % 3 == 0)
            {
                var messageService = new MessagesService(new MessagesRepository(_db)); //?
                messageService.GiveMessageToUser(user.CNP);
            }

            return Ok(new { success = true, penalty });
        }

        [HttpPost("do-not-punish")]
        public async void DoNotPunishUser([FromBody] ChatReport chatReport)
        {
            _chatReportRepository.DeleteChatReportAsync(chatReport.Id);
        }

    }
}
