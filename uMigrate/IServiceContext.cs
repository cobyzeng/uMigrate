using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Umbraco.Core.Services;

namespace uMigrate {
    [PublicAPI]
    public interface IServiceContext {
        [PublicAPI] [NotNull] IApplicationTreeService ApplicationTreeService { get; }
        [PublicAPI] [NotNull] IContentService ContentService { get; }
        [PublicAPI] [NotNull] IContentTypeService ContentTypeService { get; }
        [PublicAPI] [NotNull] IDataTypeService DataTypeService { get; }
        [PublicAPI] [NotNull] IEntityService EntityService { get; }
        [PublicAPI] [NotNull] IFileService FileService { get; }
        [PublicAPI] [NotNull] ILocalizationService LocalizationService { get; }
        [PublicAPI] [NotNull] IMacroService MacroService { get; }
        [PublicAPI] [NotNull] IMediaService MediaService { get; }
        [PublicAPI] [NotNull] IMemberGroupService MemberGroupService { get; }
        [PublicAPI] [NotNull] IMemberService MemberService { get; }
        [PublicAPI] [NotNull] IMemberTypeService MemberTypeService { get; }
        [PublicAPI] [NotNull] INotificationService NotificationService { get; }
        [PublicAPI] [NotNull] IPackagingService PackagingService { get; }
        [PublicAPI] [NotNull] IRelationService RelationService { get; }
        [PublicAPI] [NotNull] ISectionService SectionService { get; }
        [PublicAPI] [NotNull] ITagService TagService { get; }
        [PublicAPI] [NotNull] IUserService UserService { get; }
    }
}
