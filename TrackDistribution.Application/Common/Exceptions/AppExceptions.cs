namespace TrackDistribution.Application.Common.Exceptions;

/// <summary>Thrown when a requested entity does not exist. Mapped to 404 by the API middleware.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with id '{key}' was not found.")
    {
    }
}

/// <summary>Thrown for business-rule violations that aren't simple field validation. Mapped to 409/400.</summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}