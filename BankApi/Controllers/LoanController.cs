using BankApi.Data;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoanController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public LoanController(ApiDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public async Task<ActionResult<List<Loan>>> GetLoans()
        {
            var loans = await _context.Loans.ToListAsync();
            return Ok(loans);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Loan>> GetLoanById(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound($"Loan with ID {id} not found.");
            }
            return Ok(loan);
        }

        [HttpPost]
        public async Task<ActionResult<Loan>> CreateLoan([FromBody] Loan loan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLoanById), new { id = loan.Id }, loan);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateLoan(int id, [FromBody] Loan loan)
        {
            if (id != loan.Id)
            {
                return BadRequest("Loan ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingLoan = await _context.Loans.FindAsync(id);
            if (existingLoan == null)
            {
                return NotFound($"Loan with ID {id} not found.");
            }

            _context.Entry(existingLoan).CurrentValues.SetValues(loan);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLoan(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound($"Loan with ID {id} not found.");
            }

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
