// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFluentNHibernateRepositoryCodeFile.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.RyansMappingEngine
{
    using System.Text;

    using Core;

    /// <summary>
    /// Provides an interface for types that are files, 
    /// pertaining to a Visual Studio project, containing Fluent NHibernate repository code. 
    /// </summary>
    public interface IFluentNHibernateRepositoryCodeFile : ICodeFile
    {
        /// <summary>
        /// Gets or sets the named queries section string of the code file.
        /// </summary>
        StringBuilder NamedQueriesSection { get; set; }
    }
}
