namespace NShave
{
    public interface IScope
    {
        bool IsDefault();
        ScopeType Current();
        int Nesting();
        string AsJsonPath();
    }
}