namespace GoldenPagesUz.Exceptions;

public class CategoryNotFoundException : Exception
{
    public CategoryNotFoundException(int id) : base($"Category Not Found. Id: {id}")
    { }
}