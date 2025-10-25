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
/// Specification to check if a borrower has an active loan for a specific book
/// </summary>
public sealed class ActiveLoanByBorrowerAndBookSpec : Specification<Loan>
{
    public ActiveLoanByBorrowerAndBookSpec(Guid borrowerId, Guid bookId)
    {
        Query.Where(l => l.BorrowerId == borrowerId
                      && l.BookId == bookId
                      && l.Status == LoanStatus.Active);
    }
}
