namespace GoldenPagesUz.Exceptions;

public class CategoryNotFoundException : Exception
{
    public CategoryNotFoundException(int id) : base($"Category Not Found. Id: {id}")
    { }
    public CategoryNotFoundException(string message) : base($"Category Not Found. Message: {message}")
    { }
}