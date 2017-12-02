// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mapping.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines a mapping between a source schema, such as a database object, 
    /// and a proposed CLR type
    /// </summary>.
    public class Mapping : IMapping
    {
        /// <summary>
        /// The root namespace of the source assembly, if this is a POCO mapping.
        /// </summary>
        private string sourceAssemblyRootNamespace;

        /// <summary>
        /// The name of the source object of the mapping.
        /// </summary>
        private string sourceObjectName;

        /// <summary>
        /// The schema of the source object of the mapping.
        /// </summary>
        private string sourceObjectSchema;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mapping"/> class.
        /// </summary>
        /// <param name="id">An identifier to assign to the <see cref="Mapping"/>.</param>
        public Mapping(Guid id)
        {
            this.Id = id;
            this.CodeFileStringReplacements = new Dictionary<string, string>();
            this.MappedProperties = new List<IMappedProperty>();
            this.SqlParameters = new SqlParameterCollection();
            this.PrimaryKeyNames = new List<string>();
        }

        #region Properties relevant to the UI

        /// <summary>
        /// Gets the identifier of the <see cref="Mapping"/>.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets or sets a value denoting the type of mapping.
        /// </summary>
        public virtual MappingType Type { get; set; }

        /// <summary>
        /// Gets or sets the source of the <see cref="Mapping"/>
        /// </summary>
        public virtual string Source { get; set; }

        /// <summary>
        /// Gets or sets the fully qualified name of the proposed type, including the namespace.
        /// </summary>
        public virtual string ProposedTypeFullName { get; set; }

        /// <summary>
        /// Gets or sets any parameters for a stored procedure / function mapping.
        /// </summary>
        public virtual string Parameters => this.SqlParameters.ToString();

        #endregion

        /// <summary>
        /// Gets or sets the set of string replacements to make 
        /// when producing files from templates.
        /// </summary>
        public IDictionary<string, string> CodeFileStringReplacements { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this type is a built-in (System) type.
        /// </summary>
        public virtual bool IsForBuiltInType { get; set; }

        /// <summary>
        /// Gets or sets the set of properties to map.
        /// </summary>
        public IList<IMappedProperty> MappedProperties { get; set; }

        /// <summary>
        /// Gets or sets the set of primary key property names.
        /// </summary>
        public IList<string> PrimaryKeyNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the mapping has been processed.
        /// </summary>
        public virtual bool Processed { get; set; }

        /// <summary>
        /// Gets or sets the namespace of the project to which the POCO type of this mapping belongs.
        /// </summary>
        public virtual string ProjectRootNamespace { get; set; }

        /// <summary>
        /// Gets the name of the proposed type, derived from the <see cref="ProposedTypeFullName"/>.
        /// </summary>
        public virtual string ProposedTypeName => this.ProposedTypeFullName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Last();

        /// <summary>
        /// Gets the name of the proposed type, including the namespace, derived from the <see cref="ProposedTypeFullName"/>.
        /// </summary>
        public virtual string ProposedTypeNamespace => !string.IsNullOrWhiteSpace(this.ProposedTypeFullName) && this.ProposedTypeFullName.Contains(".")
                                                   ? this.ProposedTypeFullName.Substring(0, this.ProposedTypeFullName.LastIndexOf(".", StringComparison.Ordinal))
                                                   : string.Empty;

        /// <summary>
        /// Gets the root namespace of the source assembly, if this is a POCO mapping.
        /// </summary>
        public virtual string SourceAssemblyRootNamespace
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.sourceAssemblyRootNamespace))
                {
                    if (string.IsNullOrWhiteSpace(this.Source) || !this.Source.Contains(" "))
                    {
                        return string.Empty;
                    }

                    var assemblyFileName = this.Source.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
                    if (assemblyFileName == null)
                    {
                        return string.Empty;
                    }

                    if (!assemblyFileName.EndsWith(".dll") && !assemblyFileName.EndsWith(".exe"))
                    {
                        this.sourceAssemblyRootNamespace = assemblyFileName;
                    }
                    else
                    {
                        this.sourceAssemblyRootNamespace = assemblyFileName.Substring(0, assemblyFileName.Length - 4);
                    }
                }

                return this.sourceAssemblyRootNamespace;
            }
        }

        /// <summary>
        /// Gets the name of the source object of the mapping.
        /// </summary>
        public virtual string SourceObjectName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.sourceObjectName))
                {
                    if (this.Type == MappingType.DbStoredProcedure || this.Type == MappingType.DbTable || this.Type == MappingType.DbTableValuedFunction || this.Type == MappingType.DbView)
                    {
                        if (string.IsNullOrWhiteSpace(this.Source) || !this.Source.Contains(" "))
                        {
                            return string.Empty;
                        }

                        var objectSchemaAndName = this.Source.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
                        if (objectSchemaAndName == null || !objectSchemaAndName.Contains("."))
                        {
                            return string.Empty;
                        }

                        this.sourceObjectName = objectSchemaAndName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).Last();
                    }
                    else if (this.Type == MappingType.Poco)
                    {
                        this.sourceObjectName = this.ProposedTypeName;
                    }
                }

                return this.sourceObjectName;
            }
        }

        /// <summary>
        /// Gets the schema of the source object of the mapping.
        /// </summary>
        public virtual string SourceObjectSchema
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.sourceObjectSchema))
                {
                    if (this.Type == MappingType.DbStoredProcedure || this.Type == MappingType.DbTable || this.Type == MappingType.DbTableValuedFunction || this.Type == MappingType.DbView)
                    {
                        if (string.IsNullOrWhiteSpace(this.Source) || !this.Source.Contains(" "))
                        {
                            return null;
                        }

                        var objectSchemaAndName = this.Source.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
                        if (objectSchemaAndName == null || !objectSchemaAndName.Contains("."))
                        {
                            return null;
                        }

                        this.sourceObjectSchema = objectSchemaAndName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).First();
                    }
                    else if (this.Type == MappingType.Poco)
                    {
                        this.sourceObjectSchema = this.ProposedTypeNamespace.StartsWith($"{this.SourceAssemblyRootNamespace}.") 
                            ? this.ProposedTypeNamespace.Substring($"{this.SourceAssemblyRootNamespace}.".Length).Split(new [] {'.'}, StringSplitOptions.RemoveEmptyEntries).First() 
                            : "dbo";
                    }
                }

                return this.sourceObjectSchema;
            }
        }

        /// <summary>
        /// Gets or sets any parameters for a stored procedure / function mapping.
        /// </summary>
        public ISqlParameterCollection SqlParameters { get; set; }
    }
}
