// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentNHibernateRepositoryCodeFile.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.RyansMappingEngine
{
    using System.Text;

    using Core;

    /// <summary>
    /// A file, pertaining to a Visual Studio project, containing Fluent NHibernate repository code. 
    /// </summary>
    public class FluentNHibernateRepositoryCodeFile : CodeFile, IFluentNHibernateRepositoryCodeFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FluentNHibernateRepositoryCodeFile"/> class. 
        /// </summary>
        public FluentNHibernateRepositoryCodeFile()
        {
            this.NamedQueriesSection = new StringBuilder();
        }

        /// <summary>
        /// Gets or sets the named queries section string of the code file.
        /// </summary>
        public StringBuilder NamedQueriesSection { get; set; }
    }
}
