using Microsoft.Extensions.Options;

using MiniValidation;

namespace ChunkerService.Utils.Configuration;

public class MiniValidationValidateOptions<TOptions>(string? name) : IValidateOptions<TOptions>
    where TOptions : class
{
    public string? Name { get; } = name;

    public ValidateOptionsResult Validate(
        string? name,
        TOptions options
    )
    {
        if (Name != null && Name != name)
        {
            return ValidateOptionsResult.Skip;
        }

        ArgumentNullException.ThrowIfNull(options);

        if (MiniValidator.TryValidate(options, out var validationErrors))
        {
            return ValidateOptionsResult.Success;
        }

        var errors = new List<string>();
        foreach (var (member, memberErrors) in validationErrors)
        {
            errors.Add($"DataAnnotation validation failed for '{Name}' member: '{member}' with errors: '{string.Join("', '", memberErrors)}'.");
        }

        return ValidateOptionsResult.Fail(errors);
    }
}
