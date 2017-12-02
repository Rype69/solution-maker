// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentNHibernateMapping.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.RyansMappingEngine
{
    using System;

    using Core;

    /// <summary>
    /// Provides an interface for types that define NHibernate mappings between a source schema, 
    /// such as a database object, and a proposed CLR type. 
    /// </summary>
    public class FluentNHibernateMapping : Mapping, IFluentNHibernateMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FluentNHibernateMapping"/> class.
        /// </summary>
        /// <param name="id">An identifier to assign to the <see cref="FluentNHibernateMapping"/>.</param>
        public FluentNHibernateMapping(Guid id) : base(id)
        {
            this.SqlCreateObjectFile = new CodeFile();
            this.SqlDropObjectFile = new CodeFile();
            this.ModelCodeFile = new CodeFile();
            this.ModelTestsCodeFile = new TestsCodeFile();
            this.RepositoryCodeFile = new FluentNHibernateRepositoryCodeFile();
            this.RepositoryInterfaceCodeFile = new FluentNHibernateRepositoryCodeFile();
            this.RepositoryTestsCodeFile = new FluentNHibernateRepositoryTestsCodeFile();
            this.ServiceCodeFile = new CodeFile();
            this.ServiceInterfaceCodeFile = new CodeFile();
            this.ServiceTestsCodeFile = new CodeFile();
            this.FluentNHibernateMappingCodeFile = new FluentNHibernateMappingFile();
            this.FluentNHibernateMappingTestsCodeFile = new TestsCodeFile();
        }

        /// <summary>
        /// Gets or sets the create SQL object file.
        /// </summary>
        public ICodeFile SqlCreateObjectFile { get; set; }

        /// <summary>
        /// Gets or sets the drop SQL object file.
        /// </summary>
        public ICodeFile SqlDropObjectFile { get; set; }

        /// <summary>
        /// Gets or sets a model <see cref="ICodeFile"/>.
        /// </summary>
        public ICodeFile ModelCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a model code unit tests <see cref="ICodeFile"/>.
        /// </summary>
        public ITestsCodeFile ModelTestsCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a fluent NHibernate mapping <see cref="ICodeFile"/>.
        /// </summary>
        public IFluentNHibernateMappingFile FluentNHibernateMappingCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a fluent NHibernate mapping tests <see cref="ICodeFile"/>.
        /// </summary>
        public ITestsCodeFile FluentNHibernateMappingTestsCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a repository <see cref="ICodeFile"/>.
        /// </summary>
        public IFluentNHibernateRepositoryCodeFile RepositoryCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a repository interface <see cref="ICodeFile"/>.
        /// </summary>
        public IFluentNHibernateRepositoryCodeFile RepositoryInterfaceCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a repository tests <see cref="IFluentNHibernateRepositoryTestsCodeFile"/>.
        /// </summary>
        public IFluentNHibernateRepositoryTestsCodeFile RepositoryTestsCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a service <see cref="ICodeFile"/>.
        /// </summary>
        public ICodeFile ServiceCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a service interface <see cref="ICodeFile"/>.
        /// </summary>
        public ICodeFile ServiceInterfaceCodeFile { get; set; }

        /// <summary>
        /// Gets or sets a service tests <see cref="ICodeFile"/>.
        /// </summary>
        public ICodeFile ServiceTestsCodeFile { get; set; }
    }
}
