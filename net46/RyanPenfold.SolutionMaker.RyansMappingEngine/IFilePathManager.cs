// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFilePathManager.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.RyansMappingEngine
{
    /// <summary>
    /// Provides an interface for types that find and store the paths to files.
    /// </summary>
    public interface IFilePathManager
    {
        /// <summary>
        /// Gets the full path for the fluent NHibernate mapping template file.
        /// </summary>
        string FluentNHibernateMappingTemplateFilePath { get; }

        /// <summary>
        /// Gets the full path for the fluent NHibernate mapping tests template file.
        /// </summary>
        string FluentNHibernateMappingTestsTemplateFilePath { get; }

        /// <summary>
        /// Gets the full path for the model template file.
        /// </summary>
        string ModelTemplateFilePath { get; }

        /// <summary>
        /// Gets the full path for the model unit tests template file.
        /// </summary>
        string ModelUnitTestsTemplateFilePath { get; }

        /// <summary>
        /// Gets the full path for the repository interface template file.
        /// </summary>
        string RepositoryInterfaceTemplateFilePath { get; }

        /// <summary>
        /// Gets the full path for the repository template file.
        /// </summary>
        string RepositoryTemplateFilePath { get; }

        /// <summary>
        /// Gets the full path for the repository integration tests template file.
        /// </summary>
        string RepositoryIntegrationTestsTemplateFilePath { get; }

        /// <summary>
        /// Gets the full path for the repository integration tests table / view tests section template file.
        /// </summary>
        string RepositoryIntegrationTestsTableViewTestsSectionTemplateFilePath { get; }

        /// <summary>
        /// Gets the full path for the service interface template file.
        /// </summary>
        string ServiceInterfaceTemplateFilePath { get; }

        /// <summary>
        /// Gets the full path for the service template file.
        /// </summary>
        string ServiceTemplateFilePath { get; }

        /// <summary>
        /// Gets the full path for the service unit tests template file.
        /// </summary>
        string ServiceUnitTestsTemplateFilePath { get; }

        /// <summary>
        /// Gets the full path for the templates directory.
        /// </summary>
        string TemplatesDirectoryPath { get; }
    }
}