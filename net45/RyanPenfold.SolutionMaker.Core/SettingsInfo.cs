// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsInfo.cs" company="Inspire IT Ltd">
//     Copyright © Ryan Penfold. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    /// <summary>
    /// Provides settings information
    /// </summary>
    public class SettingsInfo : ISettingsInfo
    {
        /// <summary>
        /// Gets or sets the location of a POCO types file.
        /// </summary>
        public string AssemblyLocation { get; set; }

        /// <summary>
        /// Gets or sets a built-in type name.
        /// </summary>
        public string BuiltInType { get; set; }

        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the path to an IoC mappings config file
        /// </summary>
        public string IocMappingsConfigFilePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating in which language the code should be generated.
        /// </summary>
        public Language Language { get; set; }

        /// <summary>
        /// Gets or sets the namespace of the model project.
        /// </summary>
        public string ModelProjectNamespace { get; set; }

        /// <summary>
        /// Gets or sets the output directory path.
        /// </summary>
        public string OutputDirectory { get; set; }

        /// <summary>
        /// Gets or sets the namespace of the repository project.
        /// </summary>
        public string RepositoryProjectNamespace { get; set; }

        /// <summary>
        /// Gets or sets the root namespace.
        /// </summary>
        public string RootNamespace { get; set; }

        /// <summary>
        /// Gets or sets the namespace of the service project.
        /// </summary>
        public string ServiceProjectNamespace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether stored procedures should be queried.
        /// </summary>
        public bool StoredProcedures { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tables should be queried.
        /// </summary>
        public bool Tables { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether table-valued functions should be queried.
        /// </summary>
        public bool TableValuedFunctions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether project files should be updated.
        /// </summary>
        public bool UpdateProjectFiles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether views should be queried.
        /// </summary>
        public bool Views { get; set; }
    }
}
