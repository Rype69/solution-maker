// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUsingDirectiveCollection.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    /// <summary>
    /// A CLR language-specific collection of imports statements.
    /// </summary>
    public interface IUsingDirectiveCollection
    {
        /// <summary>
        /// Adds a namespace to the collection.
        /// </summary>
        /// <param name="namespace">The namespace to add.</param>
        void Add(string @namespace);

        /// <summary>
        /// Renders the using directive collection as a string in a specific language.
        /// </summary>
        /// <param name="indent">An preceding indent for the using directives.</param>
        /// <returns>A <see cref="string"/>.</returns>
        string ToString(string indent = null);
    }
}
