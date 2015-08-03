using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using umbraco.cms.businesslogic.packager;
using uMigrate.Fluent;
using Umbraco.Core.IO;

namespace uMigrate.Internal.SyntaxImplementations {
    public class PackageSetSyntax : IPackageSetSyntax {
        private readonly IMigrationContext _context;

        public PackageSetSyntax([NotNull] IMigrationContext context) {
            Argument.NotNull("context", context);
            if (context.Logger == null)
                throw new InvalidOperationException("Context must have a logger when given to a SetSyntax.");

            _context = context;
        }

        public void Install(string packagePath, PackageInstallSettings settings = null) {
            packagePath = ValidateAndNormalizePackagePath(packagePath);
            settings = settings ?? new PackageInstallSettings();

            var rootFileSystem = _context.GetFileSystem("~/");
            Ensure.That(rootFileSystem.FileExists(packagePath), "Package file '{0}' was not found.", rootFileSystem.GetFullPath(packagePath));

            var dataFileSystem = _context.GetFileSystem(SystemDirectories.Data);
            var dataTempPath = Path.Combine("uMigrate", "Packages");
            var packageCopyPath = Path.Combine(dataTempPath, Path.GetFileName(packagePath));
            if (!packageCopyPath.EndsWith(".umb", StringComparison.InvariantCultureIgnoreCase))
                packageCopyPath += ".umb";

            string packageName = null;
            try {
                using (var stream = rootFileSystem.OpenFile(packagePath)) {
                    dataFileSystem.AddFile(packageCopyPath, stream, true);
                }
                
                var installer = new Installer();
                var unpackedPath = installer.Import(packageCopyPath);
                ValidatePackage(installer, settings, packagePath);

                packageName = installer.Name;
                var packageId = installer.CreateManifest(unpackedPath, "", "");
                installer.InstallFiles(packageId, unpackedPath);
                installer.InstallBusinessLogic(packageId, unpackedPath);
                installer.InstallCleanUp(packageId, unpackedPath);
            }
            finally {
                try {
                    dataFileSystem.DeleteFile(packageCopyPath);
                }
                catch (Exception ex) {
                    var message = string.Format(
                        "Package: '{0}', failed to delete file '{0}': {1}.",
                        packageName ?? rootFileSystem.GetFileName(packagePath),
                        ex.Message
                    );
                    // ReSharper disable once PossibleNullReferenceException
                    _context.Logger.Log(message);
                    _context.Logger.System.Error(message, ex);
                }
            }

            // ReSharper disable once PossibleNullReferenceException
            _context.Logger.Log("Package: '{0}' installed.", packageName);
        }

        private static string ValidateAndNormalizePackagePath(string packagePath) {
            if (packagePath.StartsWith("~/"))
                return packagePath.Substring(2);

            if (!Path.IsPathRooted(packagePath))
                throw new NotSupportedException("Package path must be either absolute or relative to the site (starting with ~/).");
            
            return packagePath;
        }

        private void ValidatePackage([NotNull] Installer installer, [NotNull] PackageInstallSettings settings, [NotNull] string packagePath) {
            var messageBuilder = new StringBuilder().AppendFormat("Package '{0}' (at '{1}') contains:", installer.Name, packagePath).AppendLine();
            if (installer.ContainsUnsecureFiles) {
                messageBuilder.Remove(messageBuilder.Length - 1, 1)
                    .Append(" files that might cause AppDomain restart. This is not supported since uMigrate cannot resume incomplete migrations after restart.");
                AppendValidationErrors(messageBuilder, installer.UnsecureFiles);
                throw new NotSupportedException(messageBuilder.ToString());
            }

            var hasErrors = false;
            if (installer.ContainsBinaryFileErrors && !settings.IgnoreBinaryFileErrors) {
                messageBuilder.AppendFormat("  * .NET binaries that might not be compatible with this version of Umbraco (set IgnoreBinaryFileErrors to ignore this error).");
                AppendValidationErrors(messageBuilder, installer.BinaryFileErrors);
                hasErrors = true;
            }

            if (installer.ContainsLegacyPropertyEditors && !settings.AllowLegacyPropertyEditors) {
                messageBuilder.AppendFormat("  * Legacy property editors which are not compatible with Umbraco 7 (set AllowLegacyPropertyEditors to ignore this error).");
                hasErrors = true;
            }

            if (installer.ContainsMacroConflict && !settings.OverwriteMacros) {
                messageBuilder.AppendFormat("  * Macros that have same aliases as existing macros (set OverwriteMacros to force overwrite).");
                AppendValidationErrors(messageBuilder, installer.ConflictingMacroAliases.Values);
                hasErrors = true;
            }

            if (installer.ContainsStyleSheeConflicts && !settings.OverwriteStyleSheets) {
                messageBuilder.AppendFormat("  * Stylesheets that have same aliases as existing stylesheets (set OverwriteStyleSheets to force overwrite).");
                AppendValidationErrors(messageBuilder, installer.ConflictingStyleSheetNames.Values);
                hasErrors = true;
            }

            if (installer.ContainsTemplateConflicts && !settings.OverwriteTemplates) {
                messageBuilder.AppendFormat("  * Templates that have same aliases as existing stylesheets (set OverwriteTemplates to force overwrite).");
                AppendValidationErrors(messageBuilder, installer.ConflictingTemplateAliases.Values);
                hasErrors = true;
            }

            if (hasErrors)
                throw new UmbracoMigrationException(messageBuilder.ToString());
        }

        private static void AppendValidationErrors(StringBuilder builder, IEnumerable<string> errors) {
            var errorIndex = 0;
            foreach (var error in errors) {
                errorIndex += 1;
                builder.AppendLine().AppendFormat("    {0}. {1}", errorIndex, error);
            }
        }
    }
}