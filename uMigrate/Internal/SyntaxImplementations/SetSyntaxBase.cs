using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Umbraco.Core;
using uMigrate.Fluent;

namespace uMigrate.Internal.SyntaxImplementations {
    public abstract class SetSyntaxBase<TItem, TSetSyntax, TFilteredSetSyntax> : ISetSyntax<TItem, TSetSyntax, TFilteredSetSyntax>
        where TFilteredSetSyntax : IFilteredSetSyntax<TItem, TFilteredSetSyntax>
        where TSetSyntax : TFilteredSetSyntax 
    {
        private readonly Lazy<IReadOnlyList<TItem>> _items;

        protected SetSyntaxBase([NotNull] IMigrationContext context, [NotNull] Func<IReadOnlyList<TItem>> items) {
            if (context.Logger == null)
                throw new InvalidOperationException("Context must have a logger when given to a SetSyntax.");

            Context = context;
            _items = new Lazy<IReadOnlyList<TItem>>(items);
        }

        [NotNull] protected IMigrationContext Context { get; private set; }
        [NotNull]
        protected IMigrationLogger Logger {
            // ReSharper disable once AssignNullToNotNullAttribute
            get { return Context.Logger; }
        }

        [NotNull] protected abstract TSetSyntax NewSet(IEnumerable<TItem> items);
        [CanBeNull] protected abstract string GetName(TItem item);

        [NotNull]
        protected TSetSyntax NewSet([NotNull] params TItem[] items) {
            return NewSet((IEnumerable<TItem>)items);
        }

        public TFilteredSetSyntax From([NotNull] params TItem[] items) {
            return NewSet((IEnumerable<TItem>)items);
        }

        public TFilteredSetSyntax From([NotNull] IEnumerable<TItem> items) {
            return NewSet(items);
        }

        public TFilteredSetSyntax Where(Func<TItem, bool> predicate) {
            return NewSet(Objects.Where(predicate));
        }

        public IReadOnlyList<TItem> Objects {
            get { return _items.Value; }
        }

        public TItem Object {
            get {
                Ensure.That(Objects.Count != 0, "No items found in the current {0}.", GetType().Name);
                Ensure.That(
                    Objects.Count == 1,
                    "There is more than one item in the current {0}: {1}.",
                    GetType().Name, string.Join(",", Objects.Select(GetName))
                );

                return Objects[0];
            }
        }

        [NotNull]
        protected TSetSyntax ChangeWithManualSave([NotNull, InstantHandle] Action<TItem> change) {
            Objects.MigrateEach(
                change,
                item => string.Format("{0} '{1}'", GetFriendlyItemTypeName(), GetName(item))
            );
            return (TSetSyntax)(object)this;
        }

        private string GetFriendlyItemTypeName() {
            var name = typeof(TItem).Name;
            return Regex.Replace(name, @"^I(?=\p{Lu})", "")
                        .SplitPascalCasing()
                        .ToLowerInvariant();
        }
    }
}
