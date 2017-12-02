// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMapping.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface for types that define mappings between a source schema, 
    /// such as a database object, and a proposed CLR type.
    /// </summary>
    public interface IMapping
    {
        #region Properties relevant to the UI

        /// <summary>
        /// Gets the identifier of the mapping.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets or sets a value denoting the type of mapping.
        /// </summary>
        MappingType Type { get; set; }

        /// <summary>
        /// Gets or sets the source of the <see cref="Mapping"/>
        /// </summary>
        string Source { get; set; }

        /// <summary>
        /// Gets or sets the fully qualified name of the proposed type, including the namespace.
        /// </summary>
        string ProposedTypeFullName { get; set; }

        /// <summary>
        /// Gets any parameters for a stored procedure / function mapping.
        /// </summary>
        string Parameters { get; }

        #endregion

        /// <summary>
        /// Gets or sets the set of string replacements to make 
        /// when producing files from templates.
        /// </summary>
        IDictionary<string, string> CodeFileStringReplacements { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this type is a built-in (System) type.
        /// </summary>
        bool IsForBuiltInType { get; set; }

        /// <summary>
        /// Gets or sets the set of properties to map.
        /// </summary>
        IList<IMappedProperty> MappedProperties { get; set; }

        /// <summary>
        /// Gets or sets the set of primary key property names.
        /// </summary>
        IList<string> PrimaryKeyNames { get; set; }

        /// <summary>
        /// Gets or sets the namespace of the project to which the POCO type of this mapping belongs.
        /// </summary>
        string ProjectRootNamespace { get; set; }

        /// <summary>
        /// Gets the name of the proposed type, derived from the <see cref="ProposedTypeFullName"/>.
        /// </summary>
        string ProposedTypeName { get; }

        /// <summary>
        /// Gets the name of the proposed type, including the namespace, derived from the <see cref="ProposedTypeFullName"/>.
        /// </summary>
        string ProposedTypeNamespace { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the mapping has been processed.
        /// </summary>
        bool Processed { get; set; }

        /// <summary>
        /// Gets or sets any parameters for a stored procedure / function mapping.
        /// </summary>
        ISqlParameterCollection SqlParameters { get; set; }

        /// <summary>
        /// Gets the root namespace of the source assembly, if this is a POCO mapping.
        /// </summary>
        string SourceAssemblyRootNamespace { get; }

        /// <summary>
        /// Gets the name of the source object of the mapping.
        /// </summary>
        string SourceObjectName { get; }

        /// <summary>
        /// Gets the schema of the source object of the mapping.
        /// </summary>
        string SourceObjectSchema { get; }
    }
}
