// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFluentNHibernateMappingFile.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.RyansMappingEngine
{
    using System.Text;

    using Core;

    /// <summary>
    /// A file containing Fluent NHibernate mapping code.
    /// </summary>
    public interface IFluentNHibernateMappingFile : ICodeFile
    {
        /// <summary>
        /// Gets or sets the column mappings section string of the code file.
        /// This code is located in the constructor.
        /// </summary>
        StringBuilder ColumnMappingsSection { get; set; }

        /// <summary>
        /// Gets or sets the primary key column mappings section string of the code file.
        /// This code is located in the constructor.
        /// </summary>
        StringBuilder PrimaryKeyColumnMappingsSection { get; set; }
    }
}
