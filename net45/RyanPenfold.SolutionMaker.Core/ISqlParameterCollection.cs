// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqlParameterCollection.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface for types that provide access to a set of <see cref="ISqlParameter"/>s.
    /// </summary>
    public interface ISqlParameterCollection
    {
        /// <summary>
        /// Gets the set of stored procedure parameters.
        /// </summary>
        IList<ISqlParameter> Items { get; }

        /// <summary>
        /// Adds a <see cref="ISqlParameter"/> to the collection.
        /// </summary>
        /// <param name="parameter">The <see cref="ISqlParameter"/> to add.</param>
        void Add(ISqlParameter parameter);

        /// <summary>
        /// Clears the contents of the <see cref="MappingCollection"/>
        /// </summary>
        void Clear();

        /// <summary>
        /// Produces a string.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        string ToString();
    }
}
