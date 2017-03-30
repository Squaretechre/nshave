namespace NShave.Scope
{
    public interface IScope
    {
        int Nesting();
        bool IsDefault();
        string AsJsonPath();
        ScopeType Current();
    }
}