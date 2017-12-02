// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingEngine.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;

    using IO;

    using Microsoft.Build.Evaluation;

    using StringBuilder = System.Text.StringBuilder;

    using Utilities.Text;

    /// <summary>
    /// An abstract base type for types that 
    /// provide the core functionality for producing mapping code.
    /// </summary>
    public abstract class MappingEngine : IMappingEngine
    {
        /// <summary>
        /// A language-specific code factory
        /// </summary>
        protected ICodeFactory CodeFactory;

        /// <summary>
        /// The original set of mappings, before being converted to NHibernate mappings.
        /// </summary>
        protected readonly IMappingCollection MappingCollection;

        /// <summary>
        /// Gets the project file collection.
        /// </summary>
        protected ProjectCollection ProjectCollection;

        /// <summary>
        /// App settings
        /// </summary>
        protected readonly ISettingsFile SettingsFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingEngine"/> class. 
        /// </summary>
        /// <param name="mappingCollection">
        /// A set of mappings to process.
        /// </param>
        /// <param name="settingsFile">
        /// Information pertaining to app settings.
        /// </param>
        protected MappingEngine(IMappingCollection mappingCollection, ISettingsFile settingsFile)

        {
            if (mappingCollection == null)
            {
                throw new ArgumentNullException(nameof(mappingCollection));
            }

            if (settingsFile == null)
            {
                throw new ArgumentNullException(nameof(settingsFile));
            }

            // Set the field values
            this.MappingCollection = mappingCollection;
            this.SettingsFile = settingsFile;
        }

        /// <summary>
        /// Finds any project-wide imported namespaces in the first 
        /// project file found in a given directory path.
        /// </summary>
        /// <param name="directoryPath">
        /// The full path to the directory containing the project file.
        /// </param>
        /// <returns>
        /// Any project-wide imported namespaces in the first 
        /// project file found in a given directory path.
        /// </returns>
        protected virtual IEnumerable<string> FindImportedNamespaces(string directoryPath)
        {
            if (!this.SettingsFile.Data.UpdateProjectFiles)
            {
                return new List<string>().AsEnumerable();
            }

            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Directory {directoryPath} not found.");
            }

            var projectFilePath = this.FindProjectFile(directoryPath);
            if (projectFilePath == null)
            {
                throw new FileNotFoundException($"{this.CodeFactory.ProjectFileExtension} file not found in directory {directoryPath}.");
            }

            return this.ProjectCollection.LoadProject(projectFilePath)
                    .Items.Where(p => p?.ItemType == "Import").Select(p => p.EvaluatedInclude);
        }

        /// <summary>
        /// Attempts to find a Visual Studio project file in a specified directory.
        /// </summary>
        /// <returns>The path to a project file, if found, otherwise null.</returns>
        protected virtual string FindProjectFile(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Directory {directoryPath} not found.");
            }

            // Find a project file
            var projectFilePath = Path.Combine(directoryPath, $"{new DirectoryInfo(directoryPath).Name}.{this.CodeFactory.ProjectFileExtension}");
            if (!File.Exists(projectFilePath))
            {
                projectFilePath = Directory.GetFiles(directoryPath, $"*.{this.CodeFactory.ProjectFileExtension}").FirstOrDefault();
            }

            return projectFilePath;
        }

        /// <summary>
        /// Attempts to load a type from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <returns>A <see cref="Type"/> if loaded successfully, otherwise null.</returns>
        protected virtual Type LoadTypeFromMapping(IMapping mapping)
        {
            // NULL-check the parameter
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            Type type;
            if (mapping.IsForBuiltInType)
            {
                type = Type.GetType(mapping.ProposedTypeFullName);
                if (type == null)
                {
                    throw new TypeLoadException($"Cannot load type {mapping.ProposedTypeFullName}");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(this.SettingsFile.Data.AssemblyLocation))
                {
                    throw new ArgumentNullException(nameof(this.SettingsFile.Data.AssemblyLocation));
                }

                if (!File.Exists(this.SettingsFile.Data.AssemblyLocation))
                {
                    throw new FileNotFoundException($"File {this.SettingsFile.Data.AssemblyLocation} not found.");
                }

                var assembly = System.Reflection.Assembly.LoadFrom(this.SettingsFile.Data.AssemblyLocation);
                if (assembly == null)
                {
                    throw new ApplicationException(
                        $"Cannot open assembly {this.SettingsFile.Data.AssemblyLocation}.");
                }

                type = assembly.GetTypes().FirstOrDefault(t => t.FullName == mapping.ProposedTypeFullName);
                if (type == null)
                {
                    throw new TypeAccessException(
                        $"Cannot load type {mapping.ProposedTypeName} from assembly {this.SettingsFile.Data.AssemblyLocation}.");
                }
            }

            return type;
        }

        /// <summary>
        /// Produces mapping code.
        /// </summary>
        public abstract void Process();

        /// <summary>
        /// Produces mapping code.
        /// </summary>
        /// <param name="backgroundWorker">
        /// A background worker to report progress on.
        /// </param>
        public abstract void Process(BackgroundWorker backgroundWorker);

        /// <summary>
        /// Sets the project item settings for a code file.
        /// </summary>
        /// <param name="codeFile">The code file to set project items settings on.</param>
        /// <param name="directoryPath">The file path to a prospective project directory.</param>
        /// <param name="relativeFilePathBuilder">The relative path to the file.</param>
        /// <param name="fileName">The name of the file including extension.</param>
        /// <param name="projectItemType">The type of the project item.</param>
        protected virtual void SetProjectItemSettings(
            IProjectItemFile codeFile,
            string directoryPath,
            StringBuilder relativeFilePathBuilder,
            string fileName,
            ProjectItemType projectItemType)
        {
            if (codeFile == null)
            {
                throw new ArgumentNullException(nameof(codeFile));
            }

            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            if (relativeFilePathBuilder == null)
            {
                throw new ArgumentNullException(nameof(relativeFilePathBuilder));
            }

            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            codeFile.ProjectFilePath = this.FindProjectFile(directoryPath);
            if (codeFile.ProjectFilePath == null)
            {
                throw new FileNotFoundException($"{this.CodeFactory.ProjectFileExtension} file not found in directory {directoryPath}.");
            }

            codeFile.ProjectItemType = projectItemType;
            relativeFilePathBuilder.AppendWithDelimiter(fileName, "\\");
            codeFile.UnevaluatedInclude = relativeFilePathBuilder.ToString();
        }

        /// <summary>
        /// Updates a Visual Studio project file with a load of file items.
        /// </summary>
        /// <param name="projectFilePath">The path to the Visual Studio project file on disk.</param>
        /// <param name="files">The files to add.</param>
        public virtual void UpdateProjectFile(string projectFilePath, IEnumerable<IProjectItemFile> files)
        {
            if (projectFilePath == null)
            {
                throw new ArgumentNullException(nameof(projectFilePath));
            }

            if (!File.Exists(projectFilePath))
            {
                throw new FileNotFoundException($"File {projectFilePath} not found.");
            }

            var project = this.ProjectCollection.LoadProject(projectFilePath);

            foreach (var file in files)
            {
                if (file.ProjectItemType == ProjectItemType.Invalid)
                {
                    throw new InvalidOperationException("Invalid ProjectItemType specified.");
                }

                if (string.IsNullOrWhiteSpace(file.UnevaluatedInclude))
                {
                    throw new ArgumentNullException(nameof(file.UnevaluatedInclude));
                }

                if (!project.Items.Any(
                        p =>
                        p.ItemType == file.ProjectItemType.ToString()
                        && p.UnevaluatedInclude == file.UnevaluatedInclude))
                {
                    project.AddItem(file.ProjectItemType.ToString(), file.UnevaluatedInclude);
                }
            }

            project.Save(projectFilePath);
        }
    }
}
