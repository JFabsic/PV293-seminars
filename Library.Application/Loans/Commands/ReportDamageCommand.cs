using Library.Domain.Repositories;
using Library.Domain.Aggregates.Loan;
using Library.Domain.Common.CQRS;
using Library.Domain.ValueObjects;
using MediatR;

namespace Library.Application.Loans.Commands;

public record ReportDamageCommand : ICommand
{
    public Guid LoanId { get; init; }
    public required string DamageDescription { get; init; }
    public required Money DamageCost { get; init; }
}

public class ReportDamageCommandHandler(
    IRepository<Loan> loanRepository
    ) : IRequestHandler<ReportDamageCommand>
{
    public async Task Handle(ReportDamageCommand request, CancellationToken cancellationToken)
    {
        var loan = await loanRepository.GetByIdAsync(request.LoanId) ??
            throw new InvalidOperationException($"Loan with ID {request.LoanId} not found");

        loan.ReportDamage(request.DamageDescription, request.DamageCost);
    }
}