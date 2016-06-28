using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace uMigrate.Internal {
    public static class InternalExtensions {
        public static void MigrateEach<T>(this IEnumerable<T> items, Action<T> action, Func<T, string> describeItem) {
            foreach (var item in items) {
                Migrate(item, action, describeItem);
            }
        }

        public static void MigrateEach(this IEnumerable<IContent> contents, Action<IContent> action) {
            contents.MigrateEach(action, c => string.Format("content '{0}' (id {1})", c.Id, c.Name));
        }

        public static void MigrateEach(this IEnumerable<IContentType> contentTypes, Action<IContentType> action) {
            contentTypes.MigrateEach(action, t => string.Format("content type '{0}' (alias '{1}')", t.Name, t.Alias));
        }

        public static void MigrateEach(this IEnumerable<PropertyType> propertyTypes, Action<PropertyType> action) {
            propertyTypes.MigrateEach(action, p => string.Format("property '{0}'", p.Name));
        }

        public static void Migrate<T>(T item, Action<T> action, Func<T, string> describeItem) {
            try {
                action(item);
            }
            catch (Exception ex) {
                throw new UmbracoMigrationException(
                    string.Format("Failed to migrate {0}. {1}", describeItem(item), ex.Message), ex
                );
            }
        }

        // internal to avoid conflicts with other libraries
        internal static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
            TValue value;
            var found = dictionary.TryGetValue(key, out value);
            return found ? value : default(TValue);
        }

        internal static ICollection<T> AsWriteableCollection<T>(this IEnumerable<T> items) {
            var collection = items as ICollection<T>;
            if (collection != null && !collection.IsReadOnly)
                return collection;

            return items.ToList();
        }

        internal static IReadOnlyList<T> AsReadOnlyList<T>(this IEnumerable<T> items) {
            return (items as IReadOnlyList<T>)
                ?? new ReadOnlyCollection<T>((items as IList<T> ?? items.ToList()));
        }

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
