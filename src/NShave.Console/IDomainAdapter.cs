namespace NShave.Console
{
    public interface IDomainAdapter
    {
        string MustacheTemplate { get; set; }
        string Data { get; set; }
    }
}