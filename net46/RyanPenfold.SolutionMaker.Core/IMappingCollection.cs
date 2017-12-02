// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMappingCollection.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface for types that provide access to a set of <see cref="Mapping"/>s
    /// </summary>
    public interface IMappingCollection
    {
        /// <summary>
        /// Gets the set of mappings
        /// </summary>
        IEnumerable<IMapping> Items { get; }

        /// <summary>
        /// Adds a <see cref="Mapping"/> to the collection.
        /// </summary>
        /// <param name="mapping">The <see cref="Mapping"/> to add.</param>
        void Add(IMapping mapping);

        /// <summary>
        /// Creates and adds a <see cref="Mapping"/> to the collection.
        /// </summary>
        /// <param name="proposedTypeName">
        /// The name of the proposed type, including the namespace.
        /// </param>
        /// <param name="source">
        /// The source of the mapping.
        /// </param>
        /// <param name="type">
        /// The type of mapping
        /// </param>
        /// <param name="projectRootNamespace">
        /// The root namespace of the project to which the type belongs.
        /// </param>
        /// <param name="mappedType">
        /// A <see cref="Type"/> associated with this mapping.
        /// </param>
        /// <param name="isForBuiltInType">
        /// Indicates whether the proposed type name is that of a built-in (system) type.
        /// </param>
        /// <returns>
        /// The newly created <see cref="IMapping"/>.
        /// </returns>
        IMapping Add(string proposedTypeName, string source, MappingType type, string projectRootNamespace, Type mappedType = null, bool isForBuiltInType = false);

        /// <summary>
        /// Clears the contents of the <see cref="MappingCollection"/>
        /// </summary>
        void Clear();
    }
}
