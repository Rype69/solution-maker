// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectItemType.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    /// <summary>
    /// Denotes a type of project item file
    /// </summary>
    public enum ProjectItemType
    {
        /// <summary>
        /// Invalid, default value.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// Denotes an embedded resource.
        /// </summary>
        EmbeddedResource = 1,

        /// <summary>
        /// Denotes a compiled project item.
        /// </summary>
        Compile = 2
    }
}
