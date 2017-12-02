// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestsCodeFile.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System.Text;

    /// <summary>
    /// A file containing tests code, pertaining to a Visual Studio project.
    /// </summary>
    public class TestsCodeFile : CodeFile, ITestsCodeFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestsCodeFile"/> class.
        /// </summary>
        public TestsCodeFile()
        {
            this.TestsSection = new StringBuilder();
        }

        /// <summary>
        /// Gets or sets the tests section string of the code file.
        /// </summary>
        public StringBuilder TestsSection { get; set; }
    }
}
