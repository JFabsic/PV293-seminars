using Ardalis.Specification;
using Library.Domain.Aggregates.Loan;

namespace Library.Domain.Specifications;

/// <summary>
/// Specification to find an active loan for a specific book
/// </summary>
public sealed class ActiveLoanByBookSpec : Specification<Loan>, ISingleResultSpecification<Loan>
{
    public ActiveLoanByBookSpec(Guid bookId)
    {
        Query.Where(l => l.BookId == bookId && l.Status == LoanStatus.Active);
    }
}


/// <summary>
/// Specification to find overdue loans that require attention
/// Complex query for monitoring and notification purposes
/// Use case: Identify loans that need follow-up actions (reminders, fees, etc.)
/// </summary>
public sealed class OverdueLoansSpec : Specification<Loan>
{
    public OverdueLoansSpec(int? minimumDaysOverdue = null, bool includeReturned = false)
    {
        var now = DateTime.UtcNow;

        // Active loans that are past due date
        Query.Where(l => l.Status == LoanStatus.Active && l.DueDate < now);

        if (minimumDaysOverdue.HasValue && minimumDaysOverdue.Value > 0)
        {
            var cutoffDate = now.AddDays(-minimumDaysOverdue.Value);
            Query.Where(l => l.DueDate <= cutoffDate);
        }

        if (includeReturned)
        {
            // Include returned loans that had late returns (have late fees)
            // This requires checking if the loan has fines
            // Note: In a real scenario, you might need to use AsNoTracking or separate queries
            // for performance with large datasets
        }

        // Order by most overdue first
        Query.OrderBy(l => l.DueDate);
    }
}
