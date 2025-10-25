using Library.Domain.Repositories;
using Library.Domain.Aggregates.Loan;
using Library.Domain.Common.CQRS;
using MediatR;

namespace Library.Application.Loans.Commands;

public record ExtendLoanDueDateCommand : ICommand
{
    public Guid LoanId { get; init; }
    public int AdditionalDays { get; init; }
}

public class ExtendLoanDueDateCommandHandler(
    IRepository<Loan> loanRepository
    ) : IRequestHandler<ExtendLoanDueDateCommand>
{
    public async Task Handle(ExtendLoanDueDateCommand request, CancellationToken cancellationToken)
    {
        var loan = await loanRepository.GetByIdAsync(request.LoanId) ??
            throw new InvalidOperationException($"Loan with ID {request.LoanId} not found");

        loan.ExtendDueDate(request.AdditionalDays);
    }
}