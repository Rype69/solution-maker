// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectItemFile.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    /// <summary>
    /// A file pertaining to a Visual Studio project.
    /// </summary>
    public interface IProjectItemFile
    {
        /// <summary>
        /// Gets or sets a value indicating whether the file is ready to be written.
        /// </summary>
        bool IsReady { get; set; }

        /// <summary>
        /// Gets or sets the path to the file on disk.
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Gets or sets the path to the project file on disk.
        /// </summary>
        string ProjectFilePath { get; set; }

        /// <summary>
        /// Gets or sets the type of project item file.
        /// </summary>
        ProjectItemType ProjectItemType { get; set; }

        /// <summary>
        /// Gets or sets the name and/or relative file path of the item to add to the project file.
        /// For example: "Class1.cs", "Test1\\Test2\\Class2.cs", "Test1\\Test2\\Mapping.xml", "..\\Test1\\Class1.cs".
        /// </summary>
        string UnevaluatedInclude { get; set; }
    }
}
