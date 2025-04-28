using System.Globalization;
using Application.Features.Users.Commands;
using Application.Models;
using Domain.Entities;

namespace Application.Mappings;

public static class UserExtensions
{
    public static UserViewModel ToViewModel(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new UserViewModel
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedBy = user.CreatedBy,
            CreatedAt = user.CreatedAt.ToString(CultureInfo.CurrentCulture),
            UpdatedBy = user.UpdatedBy,
            UpdatedAt = user.UpdatedAt.ToString()
        };
    }
    
    public static IEnumerable<UserViewModel> ToViewModel(this IEnumerable<User> users)
    {
        ArgumentNullException.ThrowIfNull(users);

        return users.Select(ToViewModel).ToList();
    }

    public static User ToEntity(this UserViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        return new User(id: viewModel.Id, email: viewModel.Email, name: viewModel.Name);
    }

    public static CreateUserCommand ToCreateCommand(this UserViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        
        return new CreateUserCommand(Email: viewModel.Email, Name: viewModel.Name);
    }
}
