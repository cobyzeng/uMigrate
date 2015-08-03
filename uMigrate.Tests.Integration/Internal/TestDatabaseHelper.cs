using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using ReflectionMagic;
using SQLCE4Umbraco;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;

namespace uMigrate.Tests.Integration.Internal {
    public class TestDatabaseHelper {
        private const string ConnectionStringName = "umbracoDbDSN";

        public void Create() {
            var file = GetFile();
            var cachedEmptyPath = file.FullName + ".empty";
            if (File.Exists(cachedEmptyPath)) {
                File.Copy(cachedEmptyPath, file.FullName);
                return;
            }

            new SqlCeEngine(ConnectionString).CreateDatabase();
            SqlSyntaxContext.SqlSyntaxProvider = new SqlCeSyntaxProvider();
            new UmbracoDatabase(ConnectionStringName).CreateDatabaseSchema(false);
            File.Copy(file.FullName, cachedEmptyPath);
        }

        public void Drop() {
            // not sure why Umbraco never closes this
            typeof (SqlCeContextGuardian)
                .AsDynamicType()
                .CloseBackgroundConnection();

            var file = GetFile();
            if (file.Exists)
                file.Delete();
        }

        private FileInfo GetFile() {
            return new FileInfo(new SqlCeConnectionStringBuilder(ConnectionString).DataSource);
        }

        private static string ConnectionString {
            get { return ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString; }
        }
    }
}
