using System.Security.Claims;
using Library.Domain.Aggregates.Loan;

namespace Library.Domain.Services;

/// <summary>
/// Interface for loan-related domain service operations
/// Use this for dependency injection and mocking in tests
/// </summary>
public interface ILoanService
{
    /// <summary>
    /// Creates a new loan with business validation
    /// </summary>
    /// <param name="bookId">The ID of the book to be loaned</param>
    /// <param name="requestedBorrowerId">Optional specific borrower ID (for admin/librarian use)</param>
    /// <param name="user">The current user making the request</param>
    /// <param name="loanDurationDays">Duration of the loan in days</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A new Loan entity</returns>
    /// <exception cref="InvalidOperationException">When business rules are violated</exception>
    Task<Loan> CheckOutBookAsync(
        Guid bookId,
        Guid? requestedBorrowerId,
        ClaimsPrincipal user,
        int loanDurationDays,
        CancellationToken cancellationToken);
}
