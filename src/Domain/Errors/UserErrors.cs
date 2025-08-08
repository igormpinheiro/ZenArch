using ErrorOr;
using SharedKernel.Resources;

namespace Domain.Errors;

public static class UserErrors
{
    // Erros de validação
    public static Error NameEmpty => Error.Validation(
        code: "User.NameEmpty",
        description: ResourceMessages.NAME_EMPTY);

    public static Error NameTooLong => Error.Validation(
        code: "User.NameTooLong",
        description: ResourceMessages.FIELD_TOO_LONG);

    public static Error EmailEmpty => Error.Validation(
        code: "User.EmailEmpty",
        description: ResourceMessages.EMAIL_EMPTY);

    public static Error EmailInvalid => Error.Validation(
        code: "User.EmailInvalid",
        description: ResourceMessages.EMAIL_INVALID);

    // Erros de conflito
    public static Error NotFound => Error.NotFound(
        code: "User.NotFound",
        description: ResourceMessages.USER_NOT_FOUND); // Usando o novo recurso

    public static Error EmailAlreadyExists => Error.Conflict(
        code: "User.EmailAlreadyExists",
        description: ResourceMessages.EMAIL_ALREADY_REGISTERED);

    public static Error UserAlreadyExists => Error.Conflict(
        code: "User.UserAlreadyExists",
        description: ResourceMessages.USER_ALREADY_EXISTS);

    // Erros de autorização
    public static Error Unauthorized => Error.Forbidden(
        code: "User.Unauthorized",
        description: ResourceMessages.USER_WITHOUT_PERMISSION_ACCESS_RESOURCE);

    public static Error InsufficientPermissions => Error.Forbidden(
        code: "User.InsufficientPermissions",
        description: ResourceMessages.INSUFFICIENT_PERMISSIONS);

    // Erros de negócio
    public static Error UserInactive => Error.Failure(
        code: "User.Inactive",
        description: ResourceMessages.USER_INACTIVE);

    public static Error UserBlocked => Error.Failure(
        code: "User.Blocked",
        description: ResourceMessages.USER_BLOCKED);

    // Erros de senha
    public static Error InvalidPassword => Error.Validation(
        code: "User.InvalidPassword",
        description: ResourceMessages.INVALID_PASSWORD);

    public static Error PasswordEmpty => Error.Validation(
        code: "User.PasswordEmpty",
        description: ResourceMessages.PASSWORD_EMPTY);

    public static Error PasswordDifferentFromCurrent => Error.Validation(
        code: "User.PasswordDifferentFromCurrent",
        description: ResourceMessages.PASSWORD_DIFFERENT_CURRENT_PASSWORD);

    public static Error EmailOrPasswordInvalid => Error.Validation(
        code: "User.EmailOrPasswordInvalid",
        description: ResourceMessages.EMAIL_OR_PASSWORD_INVALID);

    // Métodos para erros com parâmetros dinâmicos
    public static Error InvalidAge(int age) => Error.Validation(
        code: "User.InvalidAge",
        description: $"{ResourceMessages.INVALID_AGE} Age provided: {age}");

    public static Error DuplicateRole(string roleName) => Error.Conflict(
        code: "User.DuplicateRole",
        description: $"{ResourceMessages.DUPLICATE_ROLE} Role: '{roleName}'");

    public static Error RoleNotFound(string roleName) => Error.NotFound(
        code: "User.RoleNotFound",
        description: $"{ResourceMessages.ROLE_NOT_FOUND} Role: '{roleName}'");
}
