using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Umbraco.Core.Services;

namespace uMigrate.Internal {
    public class ServiceContextWrapper : IServiceContext {
        private readonly ServiceContext _services;

        public ServiceContextWrapper([NotNull] ServiceContext services) {
            _services = Argument.NotNull("services", services);
        }

        public IApplicationTreeService ApplicationTreeService {
            get { return _services.ApplicationTreeService; }
        }

        public IContentService ContentService {
            get { return _services.ContentService; }
        }

        public IContentTypeService ContentTypeService {
            get { return _services.ContentTypeService; }
        }

        public IDataTypeService DataTypeService {
            get { return _services.DataTypeService; }
        }

        public IEntityService EntityService {
            get { return _services.EntityService; }
        }

        public IFileService FileService {
            get { return _services.FileService; }
        }

        public ILocalizationService LocalizationService {
            get { return _services.LocalizationService; }
        }

        public IMacroService MacroService {
            get { return _services.MacroService; }
        }

        public IMediaService MediaService {
            get { return _services.MediaService; }
        }

        public IMemberGroupService MemberGroupService {
            get { return _services.MemberGroupService; }
        }

        public IMemberService MemberService {
            get { return _services.MemberService; }
        }

        public IMemberTypeService MemberTypeService {
            get { return _services.MemberTypeService; }
        }
        
        public INotificationService NotificationService {
            get { return _services.NotificationService; }
        }

        public IPackagingService PackagingService {
            get { return _services.PackagingService; }
        }

        public IRelationService RelationService {
            get { return _services.RelationService; }
        }

        public ISectionService SectionService {
            get { return _services.SectionService; }
        }

        public ITagService TagService {
            get { return _services.TagService; }
        }

        public IUserService UserService {
            get { return _services.UserService; }
        }
    }
}