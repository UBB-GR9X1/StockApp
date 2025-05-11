using BankApi.Models;
using BankApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessagesRepository _messagesRepository;
        public MessageController(IMessagesRepository messagesRepository)
        {
            _messagesRepository = messagesRepository ?? throw new ArgumentNullException(nameof(messagesRepository));
        }

        [HttpGet("{cnp}")]
        public async Task<ActionResult<List<Message>>> GetMessagesForUser(string cnp)
        {
            try
            {
                var messages = await _messagesRepository.GetMessagesForGivenUserAsync(cnp);
                if (messages == null || !messages.Any())
                {
                    return NotFound($"No messages found for user with CNP {cnp}.");
                }
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{cnp}/random")]
        public async Task<ActionResult> GiveRandomMessage(string cnp)
        {
            try
            {
                await _messagesRepository.GiveUserRandomMessageAsync(cnp);
                return Ok("Random congratulatory message assigned to the user.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{cnp}/roast")]
        public async Task<ActionResult> GiveRandomRoastMessage(string cnp)
        {
            try
            {
                await _messagesRepository.GiveUserRandomRoastMessageAsync(cnp);
                return Ok("Random roast message assigned to the user.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
