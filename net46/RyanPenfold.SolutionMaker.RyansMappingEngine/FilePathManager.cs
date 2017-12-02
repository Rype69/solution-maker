// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileFinder.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.RyansMappingEngine
{
    using System;
    using System.IO;

    using Core;

    /// <summary>
    /// Finds and stores the paths to files
    /// </summary>
    public class FilePathManager : IFilePathManager
    {
        /// <summary>
        /// A code factory instance.
        /// </summary>
        private readonly ICodeFactory codeFactory;

        /// <summary>
        /// The full path for the fluent NHibernate mapping template file.
        /// </summary>
        private string fluentNHibernateMappingTemplateFilePath;

        /// <summary>
        /// The full path for the fluent NHibernate mapping tests template file.
        /// </summary>
        private string fluentNHibernateMappingTestsTemplateFilePath;

        /// <summary>
        /// The full path for the model template file.
        /// </summary>
        private string modelTemplateFilePath;

        /// <summary>
        /// The full path for the model unit tests template file.
        /// </summary>
        private string modelUnitTestsTemplateFilePath;

        /// <summary>
        /// The full path for the repository interface template file.
        /// </summary>
        private string repositoryInterfaceTemplateFilePath;

        /// <summary>
        /// The full path for the repository template file.
        /// </summary>
        private string repositoryTemplateFilePath;

        /// <summary>
        /// The full path for the repository integration tests template file.
        /// </summary>
        private string repositoryIntegrationTestsTemplateFilePath;

        /// <summary>
        /// The full path for the repository integration tests table/view tests section template file.
        /// </summary>
        private string repositoryIntegrationTestsTableViewTestsSectionTemplateFilePath;

        /// <summary>
        /// The full path for the service interface template file.
        /// </summary>
        private string serviceInterfaceTemplateFilePath;

        /// <summary>
        /// The full path for the service template file.
        /// </summary>
        private string serviceTemplateFilePath;

        /// <summary>
        /// The full path for the service unit tests template file.
        /// </summary>
        private string serviceUnitTestsTemplateFilePath;

        /// <summary>
        /// The full path for the templates directory.
        /// </summary>
        private string templatesDirectoryPath;

        /// <summary>
        /// Initialises a new instance of the <see cref="FilePathManager"/> class.
        /// </summary>
        public FilePathManager(ICodeFactory codeFactory)
        {
            if (codeFactory == null)
            {
                throw new ArgumentNullException(nameof(codeFactory));
            }

            this.codeFactory = codeFactory;
        }

        /// <summary>
        /// Gets the full path for the fluent NHibernate mapping template file.
        /// </summary>
        public virtual string FluentNHibernateMappingTemplateFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.fluentNHibernateMappingTemplateFilePath))
                {
                    this.fluentNHibernateMappingTemplateFilePath = Utilities.IO.File.Find($"Map.{this.codeFactory.CodeFileExtension}.txt", this.TemplatesDirectoryPath);
                }

                return this.fluentNHibernateMappingTemplateFilePath;
            }
        }

        /// <summary>
        /// Gets the full path for the fluent NHibernate mapping tests template file.
        /// </summary>
        public virtual string FluentNHibernateMappingTestsTemplateFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.fluentNHibernateMappingTestsTemplateFilePath))
                {
                    this.fluentNHibernateMappingTestsTemplateFilePath = Utilities.IO.File.Find($"MapTests.{this.codeFactory.CodeFileExtension}.txt", this.TemplatesDirectoryPath);
                }

                return this.fluentNHibernateMappingTestsTemplateFilePath;
            }
        }

        /// <summary>
        /// Gets the full path for the model template file.
        /// </summary>
        public virtual string ModelTemplateFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.modelTemplateFilePath))
                {
                    this.modelTemplateFilePath = Utilities.IO.File.Find($"Model.{this.codeFactory.CodeFileExtension}.txt", this.TemplatesDirectoryPath);
                }

                return this.modelTemplateFilePath;
            }
        }

        /// <summary>
        /// Gets the full path for the model unit tests template file.
        /// </summary>
        public virtual string ModelUnitTestsTemplateFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.modelUnitTestsTemplateFilePath))
                {
                    this.modelUnitTestsTemplateFilePath = Utilities.IO.File.Find($"ModelTests.{this.codeFactory.CodeFileExtension}.txt", this.TemplatesDirectoryPath);
                }

                return this.modelUnitTestsTemplateFilePath;
            }
        }

        /// <summary>
        /// Gets the full path for the repository interface template file.
        /// </summary>
        public virtual string RepositoryInterfaceTemplateFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.repositoryInterfaceTemplateFilePath))
                {
                    this.repositoryInterfaceTemplateFilePath = Utilities.IO.File.Find($"IRepository.{this.codeFactory.CodeFileExtension}.txt", this.TemplatesDirectoryPath);
                }

                return this.repositoryInterfaceTemplateFilePath;
            }
        }

        /// <summary>
        /// Gets the full path for the repository template file.
        /// </summary>
        public virtual string RepositoryTemplateFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.repositoryTemplateFilePath))
                {
                    this.repositoryTemplateFilePath = Utilities.IO.File.Find($"Repository.{this.codeFactory.CodeFileExtension}.txt", this.TemplatesDirectoryPath);
                }

                return this.repositoryTemplateFilePath;
            }
        }

        /// <summary>
        /// Gets the full path for the repository integration tests template file.
        /// </summary>
        public virtual string RepositoryIntegrationTestsTemplateFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.repositoryIntegrationTestsTemplateFilePath))
                {
                    this.repositoryIntegrationTestsTemplateFilePath = Utilities.IO.File.Find($"RepositoryTests.{this.codeFactory.CodeFileExtension}.txt", this.TemplatesDirectoryPath);
                }

                return this.repositoryIntegrationTestsTemplateFilePath;
            }
        }

        /// <summary>
        /// Gets the full path for the repository integration tests table/view tests section template file.
        /// </summary>
        public virtual string RepositoryIntegrationTestsTableViewTestsSectionTemplateFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.repositoryIntegrationTestsTableViewTestsSectionTemplateFilePath))
                {
                    this.repositoryIntegrationTestsTableViewTestsSectionTemplateFilePath = Utilities.IO.File.Find($"RepositoryTests_TableViewTestsSection.{this.codeFactory.CodeFileExtension}.txt", this.TemplatesDirectoryPath);
                }

                return this.repositoryIntegrationTestsTableViewTestsSectionTemplateFilePath;
            }
        }

        /// <summary>
        /// Gets the full path for the service interface template file.
        /// </summary>
        public virtual string ServiceInterfaceTemplateFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.serviceInterfaceTemplateFilePath))
                {
                    this.serviceInterfaceTemplateFilePath = Utilities.IO.File.Find($"IService.{this.codeFactory.CodeFileExtension}.txt", this.TemplatesDirectoryPath);
                }

                return this.serviceInterfaceTemplateFilePath;
            }
        }

        /// <summary>
        /// Gets the full path for the service template file.
        /// </summary>
        public virtual string ServiceTemplateFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.serviceTemplateFilePath))
                {
                    this.serviceTemplateFilePath = Utilities.IO.File.Find($"Service.{this.codeFactory.CodeFileExtension}.txt", this.TemplatesDirectoryPath);
                }

                return this.serviceTemplateFilePath;
            }
        }

        /// <summary>
        /// Gets the full path for the service unit tests template file.
        /// </summary>
        public virtual string ServiceUnitTestsTemplateFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.serviceUnitTestsTemplateFilePath))
                {
                    this.serviceUnitTestsTemplateFilePath = Utilities.IO.File.Find($"ServiceTests.{this.codeFactory.CodeFileExtension}.txt", this.TemplatesDirectoryPath);
                }

                return this.serviceUnitTestsTemplateFilePath;
            }
        }

        /// <summary>
        /// Gets the full path for the templates directory.
        /// </summary>
        public virtual string TemplatesDirectoryPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.templatesDirectoryPath))
                {
                    this.templatesDirectoryPath = $"{Directory.GetCurrentDirectory()}\\Templates";
                }

                return this.templatesDirectoryPath;
            }
        }
    }
}
