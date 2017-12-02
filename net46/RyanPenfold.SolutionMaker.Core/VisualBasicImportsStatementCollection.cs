// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualBasicImportsStatementCollection.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Utilities.Text;

    /// <summary>
    /// A VB.NET language-specific collection of imports statements.
    /// </summary>
    public class VisualBasicImportsStatementCollection : IUsingDirectiveCollection
    {
        /// <summary>
        /// A set of namespaces.
        /// </summary>
        private readonly List<string> namespaces;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualBasicImportsStatementCollection"/> class. 
        /// </summary>
        public VisualBasicImportsStatementCollection()
        {            
            this.namespaces = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualBasicImportsStatementCollection"/> class.
        /// </summary>
        /// <param name="namespaces">
        /// A set of namespaces to add to the collection.
        /// </param>
        public VisualBasicImportsStatementCollection(IEnumerable<string> namespaces) : this()
        {
            // NULL-check the parameter
            if (namespaces == null)
            {
                throw new ArgumentNullException(nameof(namespaces));
            }

            // Set the field value
            foreach (var @namespace in namespaces)
            {
                this.Add(@namespace);
            }
        }

        /// <summary>
        /// Adds a namespace to the collection.
        /// </summary>
        /// <param name="namespace">The namespace to add.</param>
        public void Add(string @namespace)
        {
            if (this.namespaces.Any(n => n == @namespace))
            {
                return;
            }

            this.namespaces.Add(@namespace);
        }

        /// <summary>
        /// Renders the using directive collection as a string in a specific language.
        /// </summary>
        /// <param name="indent">An preceding indent for the using directives.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public virtual string ToString(string indent = null)
        {
            var builder = new System.Text.StringBuilder();
            string currentRootNamespace;
            var previousRootNamespace = string.Empty;

            foreach (var @namespace in this.namespaces.Where(n => n != null && (n == "System" || n.StartsWith("System."))).OrderBy(s => s))
            {
                currentRootNamespace = @namespace.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).First();
                if (!string.IsNullOrWhiteSpace(previousRootNamespace) && currentRootNamespace != previousRootNamespace)
                {
                    builder.AppendWithDelimiter(string.Empty, "\r\n");
                }

                builder.AppendWithDelimiter($"Imports {@namespace}", "\r\n");
                previousRootNamespace = @namespace.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).First();
            }

            foreach (var @namespace in this.namespaces.Where(n => n != null && (n != "System" && !n.StartsWith("System."))).OrderBy(s => s))
            {
                currentRootNamespace = @namespace.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).First();
                if (!string.IsNullOrWhiteSpace(previousRootNamespace) && currentRootNamespace != previousRootNamespace)
                {
                    builder.AppendWithDelimiter(string.Empty, "\r\n");
                }

                builder.AppendWithDelimiter($"Imports {@namespace}", "\r\n");
                previousRootNamespace = @namespace.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).First();
            }

            builder.AppendWithDelimiter(string.Empty, "\r\n");

            return builder.ToString();
        }
    }
}
