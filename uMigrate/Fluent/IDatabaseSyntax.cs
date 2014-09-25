using JetBrains.Annotations;

namespace uMigrate.Fluent {
    public interface IDatabaseSyntax {
        [PublicAPI, NotNull] IDatabaseSyntax ExecuteEmbeddedScripts(params string[] resourceNames);
        [PublicAPI, NotNull] IDatabaseSyntax ExecuteEmbeddedScript([NotNull] string resourceName);
    }
}