namespace Domain.Common;

public static class EffectivePermissionResolver
{
    public static IReadOnlySet<string> Resolve(
        IEnumerable<string> roleGrants,
        IEnumerable<string> deniedPermissions)
    {
        var result = roleGrants.ToHashSet(StringComparer.Ordinal);
        result.ExceptWith(deniedPermissions);
        return result;
    }
}
