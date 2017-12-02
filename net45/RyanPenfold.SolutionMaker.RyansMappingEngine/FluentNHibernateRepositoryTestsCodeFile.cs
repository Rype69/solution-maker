// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentNHibernateRepositoryTestsCodeFile.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.RyansMappingEngine
{
    using System.Text;

    using Core;

    using Utilities.Collections.Generic;

    /// <summary>
    /// A repository tests code file.
    /// </summary>
    public class FluentNHibernateRepositoryTestsCodeFile : CodeFile, IFluentNHibernateRepositoryTestsCodeFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FluentNHibernateRepositoryTestsCodeFile"/> class. 
        /// </summary>
        public FluentNHibernateRepositoryTestsCodeFile()
        {
            this.AssertEqualPropertyValuesMethodSection = new StringBuilder();
            this.ConnectionStringNameConstant = new StringBuilder();
            this.InitialiseInstanceMethodSection = new StringBuilder();
            this.InsertStatementColumnsSection = new StringBuilder();
            this.InsertStatementValuesSection = new StringBuilder();
            this.TableViewTestsSection = new StringBuilder();
            this.StoredProcedureFunctionTestsSection = new StringBuilder();
            this.ImportedNamespaces.UniqueAdd("Microsoft.VisualStudio.TestTools.UnitTesting");
        }

        /// <summary>
        /// Gets or sets the section of code containing an assert equal property values method.
        /// </summary>
        public StringBuilder AssertEqualPropertyValuesMethodSection { get; set; }

        /// <summary>
        /// Gets or sets the section of code that contains a string constant for a connection string name.
        /// </summary>
        public StringBuilder ConnectionStringNameConstant { get; set; }

        /// <summary>
        /// Gets or sets the section of code containing an initialize instance method.
        /// </summary>
        public StringBuilder InitialiseInstanceMethodSection { get; set; }

        /// <summary>
        /// Gets or sets the section of code containing the columns portion of an insert statement method.
        /// </summary>
        public StringBuilder InsertStatementColumnsSection { get; set; }

        /// <summary>
        /// Gets or sets the section of code containing the values portion of an insert statement method.
        /// </summary>
        public StringBuilder InsertStatementValuesSection { get; set; }

        /// <summary>
        /// Gets or sets the section of code containing integration tests for SQL tables and / or views.
        /// </summary>
        public StringBuilder TableViewTestsSection { get; set; }

        /// <summary>
        /// Gets or sets the section of code containing integration tests for SQL stored procedures and / or table-valued functions.
        /// </summary>
        public StringBuilder StoredProcedureFunctionTestsSection { get; set; }
    }
}
