using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using umbraco.cms.businesslogic.macro;
using uMigrate.Fluent;
using Umbraco.Core.Models;

namespace uMigrate.Internal.SyntaxImplementations {
    public class MacroSetSyntax : SetSyntaxBase<IMacro, IMacroSetSyntax, IMacroFilteredSetSyntax>, IMacroSetSyntax {
        public MacroSetSyntax([NotNull] IMigrationContext context, [CanBeNull] IReadOnlyList<IMacro> macros = null) 
            : base(context, () => macros ?? context.Services.MacroService.GetAll().AsReadOnlyList()) 
        {
        }

        public IMacroSetSyntax Add(string alias, params Action<IMacro>[] setups) {
            Argument.NotNullOrEmpty(nameof(alias), alias);
            Argument.NotNull(nameof(setups), setups);

            var macro = Context.Services.MacroService.GetByAlias(alias);
            var added = false;
            if (macro == null) {
                #pragma warning disable 618
                // http://our.umbraco.org/forum/developers/api-questions/59908-[Umbraco-714]-How-do-I-create-a-new-Macro-using-MacroService
                var legacyMacro = Macro.MakeNew(alias);
                #pragma warning restore 618
                added = true;
                macro = Context.Services.MacroService.GetById(legacyMacro.Id);
            }

            setups.InvokeAll(macro);
            Services.MacroService.Save(macro);
            Logger.Log($"Macro: {(added ? "added" : "updated")} '{alias}'.");
            return this;
        }

        public IMacroFilteredSetSyntax Change([NotNull] Action<IMacro> change) {
            return ChangeWithManualSave(m => {
                change(m);
                Services.MacroService.Save(m);
            });
        }

        public IMacroSetSyntax Delete(string alias) {
            Argument.NotNullOrEmpty(nameof(alias), alias);
            var macro = Services.MacroService.GetByAlias(alias);
            if (macro == null) {
                Logger.Log("Macro: '{0}' doesn't exist, no need to delete.", alias);
                return this;
            }

            NewSet(macro).Delete();
            return this;
        }

        public IMacroFilteredSetSyntax Delete() {
            return ChangeWithManualSave(m => {
                Services.MacroService.Delete(m);
                Logger.Log("Macro: '{0}' deleted.", m.Alias);
            });
        }

        protected override IMacroSetSyntax NewSet(IEnumerable<IMacro> items) 
            => new MacroSetSyntax(Context, items.ToArray());

        protected override string GetName(IMacro item) => item.Name;
    }
}
