// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISettingsInfo.cs" company="Inspire IT Ltd">
//     Copyright © Ryan Penfold. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    /// <summary>
    /// Provides an interface for types that provide settings information.
    /// </summary>
    public interface ISettingsInfo
    {
        /// <summary>
        /// Gets or sets the location of a POCO types file.
        /// </summary>
        string AssemblyLocation { get; set; }

        /// <summary>
        /// Gets or sets a built-in type name.
        /// </summary>
        string BuiltInType { get; set; }

        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the path to an IoC mappings config file
        /// </summary>
        string IocMappingsConfigFilePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating in which language the code should be generated.
        /// </summary>
        Language Language { get; set; }

        /// <summary>
        /// Gets or sets the namespace of the model project.
        /// </summary>
        string ModelProjectNamespace { get; set; }

        /// <summary>
        /// Gets or sets the output directory path.
        /// </summary>
        string OutputDirectory { get; set; }

        /// <summary>
        /// Gets or sets the namespace of the repository project.
        /// </summary>
        string RepositoryProjectNamespace { get; set; }

        /// <summary>
        /// Gets or sets the root namespace.
        /// </summary>
        string RootNamespace { get; set; }

        /// <summary>
        /// Gets or sets the namespace of the service project.
        /// </summary>
        string ServiceProjectNamespace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether stored procedures should be queried.
        /// </summary>
        bool StoredProcedures { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tables should be queried.
        /// </summary>
        bool Tables { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether table-valued functions should be queried.
        /// </summary>
        bool TableValuedFunctions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether project files should be updated.
        /// </summary>
        bool UpdateProjectFiles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether views should be queried.
        /// </summary>
        bool Views { get; set; }
    }
}
