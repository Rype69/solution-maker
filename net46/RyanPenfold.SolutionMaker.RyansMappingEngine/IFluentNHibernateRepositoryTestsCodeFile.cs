// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFluentNHibernateRepositoryTestsCodeFile.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.RyansMappingEngine
{
    using System.Text;

    using Core;

    /// <summary>
    /// Provides an interface for a repository tests code file.
    /// </summary>
    public interface IFluentNHibernateRepositoryTestsCodeFile : ICodeFile
    {
        /// <summary>
        /// Gets or sets the section of code containing an assert equal property values method.
        /// </summary>
        StringBuilder AssertEqualPropertyValuesMethodSection { get; set; }

        /// <summary>
        /// Gets or sets the section of code that contains a string constant for a connection string name.
        /// </summary>
        StringBuilder ConnectionStringNameConstant { get; set; }

        /// <summary>
        /// Gets or sets the section of code containing an initialize instance method.
        /// </summary>
        StringBuilder InitialiseInstanceMethodSection { get; set; }

        /// <summary>
        /// Gets or sets the section of code containing the columns portion of an insert statement method.
        /// </summary>
        StringBuilder InsertStatementColumnsSection { get; set; }

        /// <summary>
        /// Gets or sets the section of code containing the values portion of an insert statement method.
        /// </summary>
        StringBuilder InsertStatementValuesSection { get; set; }

        /// <summary>
        /// Gets or sets the section of code containing integration tests for SQL tables and / or views.
        /// </summary>
        StringBuilder TableViewTestsSection { get; set; }

        /// <summary>
        /// Gets or sets the section of code containing integration tests for SQL stored procedures and / or table-valued functions.
        /// </summary>
        StringBuilder StoredProcedureFunctionTestsSection { get; set; }
    }
}
