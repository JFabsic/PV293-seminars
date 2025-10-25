using System.Security.Claims;
using Library.Domain.Repositories;
using Library.Domain.Aggregates.Loan;
using Library.Domain.Common.CQRS;
using Library.Domain.Services;
using MediatR;

namespace Library.Application.Loans.Commands;

public record CheckOutBookCommand : ICommand<Guid>
{
    public Guid BookId { get; init; }
    public Guid? BorrowerId { get; init; } // Optional - if not provided, use current user
    public int LoanDurationDays { get; init; } = 14;
    public ClaimsPrincipal? User { get; init; } // User context for authorization
}

public class CheckOutBookCommandHandler(
    IRepository<Loan> loanRepository,
    ILoanService loanService)
    : IRequestHandler<CheckOutBookCommand, Guid>
{
    public async Task<Guid> Handle(CheckOutBookCommand command, CancellationToken cancellationToken)
    {
        if (command.User == null)
            throw new InvalidOperationException("User context is required");

        // Delegate to domain service (which handles fetching data and business logic)
        var loan = await loanService.CheckOutBookAsync(
            command.BookId,
            command.BorrowerId,
            command.User,
            command.LoanDurationDays,
            cancellationToken
        );

        // Persist the loan (Book will be updated via domain event)
        loanRepository.Add(loan);

        return loan.Id;
    }
}