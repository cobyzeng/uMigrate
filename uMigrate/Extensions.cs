using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace uMigrate {
    public static class Extensions {
        public static byte[] ReadAllBytes([NotNull] this IFileSystem fileSystem, string path) {
            var memory = new MemoryStream();
            using (var stream = fileSystem.OpenFile(path)) {
                stream.CopyTo(memory);
            }
            return memory.ToArray();
        }

        public static string ReadAllText([NotNull] this IFileSystem fileSystem, string path) {
            using (var stream = fileSystem.OpenFile(path))
            using (var reader = new StreamReader(stream)) {
                return reader.ReadToEnd();
            }
        }

        public static void SaveThenPublishIfPublished(this IContentService contentService, IContent content) {
            if (content.Published) {
                contentService.SaveAndPublish(content);
            }
            else {
                contentService.Save(content);
            }
        }

        public static PropertyType CloneUsing(this PropertyType property, IDataTypeService dataTypeService) {
            var dataType = dataTypeService.GetDataTypeDefinitionById(property.DataTypeDefinitionId);
            return new PropertyType(dataType) {
                Alias = property.Alias,
                CreateDate = property.CreateDate,
                Description = property.Description,
                Mandatory = property.Mandatory,
                Name = property.Name,
                PropertyEditorAlias = property.PropertyEditorAlias,
                SortOrder = property.SortOrder,
                ValidationRegExp = property.ValidationRegExp
            };
        }
    }
}
