namespace NShave
{
    public interface IScope
    {
        bool IsDefault();
        string Current();
        int Nesting();
        string AsJsonPath();
    }
}