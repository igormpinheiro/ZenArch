namespace Web.API.Contracts.Responses;

internal sealed record ValidationError(string PropertyName, string Message)
{
    public static ValidationError Create(string propertyName, string message)
    {
        return new ValidationError(
            string.IsNullOrWhiteSpace(propertyName) ? "Unknown" : propertyName,
            string.IsNullOrWhiteSpace(message) ? "Erro de validação" : message
        );
    }
}
