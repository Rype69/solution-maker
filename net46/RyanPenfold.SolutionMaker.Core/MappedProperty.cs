// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappedProperty.cs" company="Inspire IT Ltd">
//   Copyright © Inspire IT Ltd. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System;

    /// <summary>
    /// Describes a mapping between a POCO type property and a database column.
    /// </summary>
    public class MappedProperty : IMappedProperty
    {
        /// <summary>
        /// Gets or sets the .NET type for the mapping.
        /// </summary>
        public Type ClrType { get; set; }

        /// <summary>
        /// Gets or sets the name of the database column.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field can be null.
        /// </summary>
        public bool? IsNullable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column is a primary key.
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets the length of the column.
        /// </summary>
        public int? Length { get; set; }

        /// <summary>
        /// Gets or sets the name of the POCO type property.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the SQL type name for the mapping.
        /// </summary>
        public string SqlType { get; set; }
    }
}
