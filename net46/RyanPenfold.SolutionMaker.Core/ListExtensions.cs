// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListExtensions.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides extension methods for <see cref="IList{T}"/> instances.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Determines whether a set of mappings has a mapping to a POCO type with the specified name.
        /// </summary>
        /// <typeparam name="T">
        /// The type of mappings.
        /// </typeparam>
        /// <param name="list">
        /// A set of mappings.
        /// </param>
        /// <param name="proposedTypeName">
        /// The proposed name of a type.
        /// </param>
        /// <returns>
        /// A <see cref="bool"/> indicating whether the set of mappings has a mapping to a POCO type with the specified name.
        /// </returns>
        public static bool HasPocoMappingForProposedTypeName<T>(this IList<T> list, string proposedTypeName) where T : IMapping
        {
            return list.Any(m => m.ProposedTypeName == proposedTypeName && m.Type == MappingType.Poco);
        }

        /// <summary>
        /// Determines whether a set of mappings has a mapping to a POCO type with the specified name.
        /// </summary>
        /// <typeparam name="T">
        /// The type of mappings.
        /// </typeparam>
        /// <param name="list">
        /// A set of mappings.
        /// </param>
        /// <param name="proposedTypeName">
        /// The proposed name of a type.
        /// </param>
        /// <returns>
        /// A <see cref="bool"/> indicating whether the set of mappings has a mapping to a POCO type with the specified name.
        /// </returns>
        public static bool HasTableViewMappingForProposedTypeName<T>(this IList<T> list, string proposedTypeName) where T : IMapping
        {
            return list.Any(m => m.ProposedTypeName == proposedTypeName && (m.Type == MappingType.DbTable || m.Type == MappingType.DbView));
        }
    }
}
