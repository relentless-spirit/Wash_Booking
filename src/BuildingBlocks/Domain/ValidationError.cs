namespace BuildingBlocks.Domain;

public sealed record ValidationError(IReadOnlyCollection<Error> Errors) : Error(
    "Validation.General",
    "One or more validation errors occurred",
    ErrorType.Validation)
{
    public static ValidationError FromResults(IEnumerable<Result> results)
    {
        var errors = results
            .Where(r => r.IsError) 
            .Select(r => r.Error)
            .ToList(); 

        return new(errors);
    }
}
