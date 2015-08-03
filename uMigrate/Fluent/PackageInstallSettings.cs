namespace uMigrate.Fluent {
    public class PackageInstallSettings {
        public bool IgnoreBinaryFileErrors { get; set; }
        public bool AllowLegacyPropertyEditors { get; set; }
        public bool OverwriteMacros { get; set; }
        public bool OverwriteStyleSheets { get; set; }
        public bool OverwriteTemplates { get; set; }
        public bool IgnoreUnsecureFiles { get; set; }
    }
}