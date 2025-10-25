using Library.Domain.Repositories;
using Library.Domain.Aggregates;
using Library.Domain.Common.CQRS;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Library.Application.Auth.Queries;

public class GetAllUsersQuery : IQuery<List<UserDto>>
{
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime MembershipDate { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class GetAllUsersQueryHandler(
    IRepository<ApplicationUser> userRepository,
    UserManager<ApplicationUser> userManager
    )
    : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync();

        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MembershipDate = user.MembershipDate,
                Roles = roles.ToList()
            });
        }
        return userDtos;
    }
}