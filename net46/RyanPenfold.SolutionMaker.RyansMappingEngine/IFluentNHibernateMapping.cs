// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFluentNHibernateMapping.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.RyansMappingEngine
{
    using Core;

    /// <summary>
    /// Provides an interface for types that define NHibernate mappings between a source schema, 
    /// such as a database object, and a proposed CLR type. 
    /// </summary>
    public interface IFluentNHibernateMapping : IMapping
    {
        /// <summary>
        /// Gets or sets the create SQL object file.
        /// </summary>
        ICodeFile SqlCreateObjectFile { get; set; }

        /// <summary>
        /// Gets or sets the drop SQL object file.
        /// </summary>
        ICodeFile SqlDropObjectFile { get; set; }

        /// <summary>
        /// Gets or sets a model <see cref="ICodeFile"/>.
        /// </summary>
        ICodeFile ModelCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a model code unit tests <see cref="ICodeFile"/>.
        /// </summary>
        ITestsCodeFile ModelTestsCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a fluent NHibernate mapping <see cref="ICodeFile"/>.
        /// </summary>
        IFluentNHibernateMappingFile FluentNHibernateMappingCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a fluent NHibernate mapping tests <see cref="ICodeFile"/>.
        /// </summary>
        ITestsCodeFile FluentNHibernateMappingTestsCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a repository <see cref="ICodeFile"/>.
        /// </summary>
        IFluentNHibernateRepositoryCodeFile RepositoryCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a repository interface <see cref="ICodeFile"/>.
        /// </summary>
        IFluentNHibernateRepositoryCodeFile RepositoryInterfaceCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a repository tests <see cref="IFluentNHibernateRepositoryTestsCodeFile"/>.
        /// </summary>
        IFluentNHibernateRepositoryTestsCodeFile RepositoryTestsCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a service interface <see cref="ICodeFile"/>.
        /// </summary>
        ICodeFile ServiceInterfaceCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a service <see cref="ICodeFile"/>.
        /// </summary>
        ICodeFile ServiceCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a service tests <see cref="ICodeFile"/>.
        /// </summary>
        ICodeFile ServiceTestsCodeFile { get; set; }
    }
}
