// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingCollection.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides access to a set of <see cref="Mapping"/>s
    /// </summary>
    public class MappingCollection : IMappingCollection
    {
        /// <summary>
        /// A set of <see cref="Mapping"/>s.
        /// </summary>
        private IList<IMapping> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingCollection"/> class.
        /// </summary>
        public MappingCollection()
        {
            this.items = new List<IMapping>();
        }

        /// <summary>
        /// Gets the set of mappings, sorted by <see cref="Mapping.Source"/>
        /// </summary>
        public IEnumerable<IMapping> Items
        {
            get
            {
                return this.items.OrderBy(m => m.ProposedTypeFullName);
            }
        }

        /// <summary>
        /// Adds a <see cref="Mapping"/> to the collection.
        /// </summary>
        /// <param name="mapping">The <see cref="Mapping"/> to add.</param>
        public void Add(IMapping mapping)
        {
            this.items.Add(mapping);
        }

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
        public virtual IMapping Add(string proposedTypeName, string source, MappingType type, string projectRootNamespace, Type mappedType = null, bool isForBuiltInType = false)
        {
            Guid id;
            do
            {
                id = Guid.NewGuid();
            }
            while (this.items.Any(m => m.Id == id));

            var mapping = new Mapping(id)
                              {
                                  IsForBuiltInType = isForBuiltInType,
                                  ProposedTypeFullName = proposedTypeName,
                                  Source = source,
                                  ProjectRootNamespace = projectRootNamespace,
                                  Type = type
                              };

            this.Add(mapping);

            return mapping;
        }

        /// <summary>
        /// Clears the contents of the <see cref="MappingCollection"/>
        /// </summary>
        public void Clear()
        {
            this.items = new List<IMapping>();
        }
    }
}
