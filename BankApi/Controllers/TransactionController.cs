using BankApi.Models;
using BankApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    /// <summary>
    /// API Controller for managing transactions.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionController"/> class.
        /// </summary>
        /// <param name="transactionRepository">The transaction repository.</param>
        public TransactionController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        }

        /// <summary>
        /// Retrieves all transactions.
        /// </summary>
        /// <returns>A list of all transactions.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            try
            {
                var transactions = await _transactionRepository.getAllTransactions();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves transactions based on filter criteria.
        /// </summary>
        /// <param name="criteria">The filter criteria.</param>
        /// <returns>A list of transactions matching the criteria.</returns>
        [HttpPost("filter")]
        public async Task<IActionResult> GetTransactionsByFilter([FromBody] TransactionFilterCriteria criteria)
        {
            if (criteria == null)
            {
                return BadRequest("Filter criteria cannot be null.");
            }

            try
            {
                var transactions = await _transactionRepository.GetByFilterCriteria(criteria);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a new transaction.
        /// </summary>
        /// <param name="transaction">The transaction to add.</param>
        /// <returns>The created transaction.</returns>
        [HttpPost]
        public async Task<IActionResult> AddTransaction([FromBody] TransactionLogTransaction transaction)
        {
            if (transaction == null)
            {
                return BadRequest("Transaction cannot be null.");
            }

            try
            {
                await _transactionRepository.AddTransaction(transaction);
                return CreatedAtAction(nameof(GetAllTransactions), new { id = transaction.StockSymbol }, transaction);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

