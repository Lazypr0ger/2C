
namespace Contracts.Exceptions;

public class ElementNotFoundException(string value) : Exception($"Element not found at value = {value}")
{
    public string Value { get; private set; } = value;
}
