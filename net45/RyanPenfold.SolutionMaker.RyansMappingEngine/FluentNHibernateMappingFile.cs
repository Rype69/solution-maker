// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentNHibernateMappingFile.cs" company="Inspire IT Ltd">
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
    public class FluentNHibernateMappingFile : CodeFile, IFluentNHibernateMappingFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FluentNHibernateMappingFile"/> class.
        /// </summary>
        public FluentNHibernateMappingFile()
        {
            this.ColumnMappingsSection = new StringBuilder();
            this.PrimaryKeyColumnMappingsSection = new StringBuilder();
        }

        /// <summary>
        /// Gets or sets the column mappings section string of the code file.
        /// This code is located in the constructor.
        /// </summary>
        public StringBuilder ColumnMappingsSection { get; set; }

        /// <summary>
        /// Gets or sets the primary key column mappings section string of the code file.
        /// This code is located in the constructor.
        /// </summary>
        public StringBuilder PrimaryKeyColumnMappingsSection { get; set; }
    }
}
