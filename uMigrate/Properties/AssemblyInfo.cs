using System.Reflection;
using System.Runtime.InteropServices;
using uMigrate.Properties;

[assembly: AssemblyTitle("uMigrate")]
[assembly: AssemblyDescription("Migration framework for Umbraco (Core).")]
[assembly: AssemblyCompany("Affinity ID")]
[assembly: AssemblyProduct("uMigrate")]

[assembly: ComVisible(false)]
[assembly: Guid("4031cffe-8654-4705-b79c-9a9c1c2a92c9")]

[assembly: AssemblyVersion(AssemblyInfo.VersionString)]
[assembly: AssemblyFileVersion(AssemblyInfo.VersionString)]
[assembly: AssemblyInformationalVersion(AssemblyInfo.InformationalVersionString)]

namespace uMigrate.Properties {
    public static class AssemblyInfo {
        // please follow SemVer here:
        public const string VersionString = "0.10.1";
        public const string InformationalVersionString = VersionString;
    }
}