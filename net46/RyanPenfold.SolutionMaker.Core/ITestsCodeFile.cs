// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITestsCodeFile.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System.Text;

    /// <summary>
    /// A file containing tests code, pertaining to a Visual Studio project.
    /// </summary>
    public interface ITestsCodeFile : ICodeFile
    {
        /// <summary>
        /// Gets or sets the tests section string of the code file.
        /// </summary>
        StringBuilder TestsSection { get; set; }
    }
}
