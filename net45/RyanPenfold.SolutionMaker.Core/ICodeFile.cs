// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICodeFile.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A file containing code, pertaining to a Visual Studio project.
    /// </summary>
    public interface ICodeFile : IProjectItemFile
    {
        /// <summary>
        /// Gets or sets the contents of the file.
        /// </summary>
        StringBuilder Contents { get; set; }

        /// <summary>
        /// Gets or sets a set of imported namespaces for the code file.
        /// </summary>
        IList<string> ImportedNamespaces { get; set; }

        /// <summary>
        /// Gets or sets the namespace of the code file.
        /// </summary>
        string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the properties section string of the code file.
        /// </summary>
        StringBuilder PropertiesSection { get; set; }

        /// <summary>
        /// Gets or sets the name of the type in the code file, excluding the namespace.
        /// </summary>
        string TypeName { get; set; }

        /// <summary>
        /// Gets the name of the type in the code file, including the namespace.
        /// </summary>
        string TypeFullName { get; }
    }
}
