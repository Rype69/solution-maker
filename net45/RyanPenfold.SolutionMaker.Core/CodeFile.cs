// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeFile.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A code file pertaining to a Visual Studio project.
    /// </summary>
    public class CodeFile : ProjectItemFile, ICodeFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFile"/> class.
        /// </summary>
        public CodeFile()
        {
            this.Contents = new StringBuilder();
            this.ImportedNamespaces = new List<string>();
            this.PropertiesSection = new StringBuilder();
        }

        /// <summary>
        /// Gets or sets the contents of the file.
        /// </summary>
        public StringBuilder Contents { get; set; }

        /// <summary>
        /// Gets or sets a set of imported namespaces for the code file.
        /// </summary>
        public IList<string> ImportedNamespaces { get; set; }

        /// <summary>
        /// Gets or sets the namespace of the code file.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the properties section string of the code file.
        /// </summary>
        public StringBuilder PropertiesSection { get; set; }

        /// <summary>
        /// Gets or sets the name of the type in the code file, excluding the namespace.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets the name of the type in the code file, including the namespace.
        /// </summary>
        public string TypeFullName => $"{this.Namespace}.{this.TypeName}";
    }
}
