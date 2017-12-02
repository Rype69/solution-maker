// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMappedProperty.cs" company="Inspire IT Ltd">
//   Copyright © Inspire IT Ltd. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System;

    /// <summary>
    /// Describes a mapping between a POCO type property and a database column.
    /// </summary>
    public interface IMappedProperty
    {
        /// <summary>
        /// Gets or sets the .NET type for the mapping.
        /// </summary>
        Type ClrType { get; set; }

        /// <summary>
        /// Gets or sets the name of the database column.
        /// </summary>
        string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field can be null.
        /// </summary>
        bool? IsNullable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column is a primary key.
        /// </summary>
        bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets the length of the column.
        /// </summary>
        int? Length { get; set; }

        /// <summary>
        /// Gets or sets the name of the POCO type property.
        /// </summary>
        string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the SQL type name for the mapping.
        /// </summary>
        string SqlType { get; set; }
    }
}
