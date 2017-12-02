// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentNHibernateMappingEngine.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// TODO: generate the following: from JobMap's constructor
/*
    this.EnsureTableExists(this.SchemaName, this.TableName);
 */

// TODO: Add to source control
// TODO: Format document (VB.NET) indent content inside namespace if there is a namespace.
// TODO: Determine primary keys at MappingEngine.Process method - use RyanPenfold.Utilities.SqlClient.SQL.SelectPrimaryKeys
// TODO: Nested mapped objects - traverse SQL fk constraints etc 
//      see http://stackoverflow.com/questions/3691366/fluent-nhibernate-how-to-map-a-foreign-key-column-in-the-mapping-class
//      and RyanPenfold.Utilities.SqlClient.SQL.SelectForeignKeys
//      ClassMap (BaseClassMap?) types should include lines similar to: this.References(x => x.Organisation).Column("OrganisationId").Not.LazyLoad()
// TODO: When the "open" button is pressed, and there is already a file explorer window open, the window should come in to focus.
// TODO: Add relevant content to app.config in repository integration tests
// TODO: Test a stored procedure with parameters
// TODO: Test a stored procedure without any parameters
// TODO: Test with a table with a single column of each type
// TODO: Test single stored procedures with tables
// TODO: Test multiple stored procedures with tables
// TODO: Test single stored procedures without any tables
// TODO: Test multiple stored procedures without any tables
// TODO: Test table without any columns
// TODO: Test table with only timestamp column
// TODO: Test with all non-nullable columns
// TODO: Test with lots of sub namespaces
// TODO: Unit tests for this app's code
// TODO: Include name in Stylecop dictionary
// TODO: Find code issues

namespace RyanPenfold.SolutionMaker.RyansMappingEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using AutoMapper;

    using Core;
    using Core.IO;

    using Microsoft.Build.Evaluation;

    using Utilities.Collections.Generic;
    using Utilities.Data.SqlClient;
    using Utilities.Text;

    using MappingType = Core.MappingType;
    using SqlCommand = System.Data.SqlClient.SqlCommand;
    using StringBuilder = System.Text.StringBuilder;

    /// <summary>
    /// Provides the core functionality for producing mapping code.
    /// </summary>
    public class FluentNHibernateMappingEngine : Core.MappingEngine
    {
        /// <summary>
        /// A file to contain SQL to create database objects.
        /// </summary>
        protected ICodeFile CreateDatabaseObjectsFile;

        /// <summary>
        /// A set of mappings to process.
        /// </summary>
        protected IList<IFluentNHibernateMapping> Mappings;

        /// <summary>
        /// A file path manager instance
        /// </summary>
        protected IFilePathManager FilePathManager;

        /// <summary>
        /// Gets or sets the set of string replacements to make when producing any file from a template.
        /// </summary>
        protected IDictionary<string, string> GlobalFileStringReplacements;

        /// <summary>
        /// Indicates whether a property mapping iteration is the first in the set.
        /// </summary>
        protected bool IsFirstPropertyMapping;

        /// <summary>
        /// IoC container config document.
        /// </summary>
        protected XDocument IocContainerConfigDocument;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentNHibernateMappingEngine"/> class. 
        /// </summary>
        public FluentNHibernateMappingEngine() : this(
            IocContainer.Resolver.Resolve<IMappingCollection>(),
            IocContainer.Resolver.Resolve<ISettingsFile>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentNHibernateMappingEngine"/> class. 
        /// </summary>
        /// <param name="mappingCollection">
        /// A set of mappings to process.
        /// </param>
        /// <param name="settingsFile">
        /// Information pertaining to app settings.
        /// </param>
        public FluentNHibernateMappingEngine(IMappingCollection mappingCollection, ISettingsFile settingsFile)
            : base(mappingCollection,
                   settingsFile)
        {
            // NULL-check the parameters
            if (mappingCollection == null)
            {
                throw new ArgumentNullException(nameof(mappingCollection));
            }

            if (settingsFile == null)
            {
                throw new ArgumentNullException(nameof(settingsFile));
            }

            Mapper.CreateMap(typeof(IMapping), typeof(IFluentNHibernateMapping));
        }

        /// <summary>
        /// Attempts to add repository and service type mappings to an IoC config file.
        /// </summary>
        /// <param name="mapping">
        /// A mapping.
        /// </param>
        protected virtual void AddRepositoryServiceAndMapToIoCConfig(IFluentNHibernateMapping mapping)
        {
            // NULL-check the mapping.
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            // If XML document is null, create nodes
            if (this.IocContainerConfigDocument == null)
            {
                this.IocContainerConfigDocument = new XDocument(new XDeclaration("1.0", "utf-8", null));
            }

            // Attempt to find the Autofac element in the config file.
            var configurationElement = this.IocContainerConfigDocument.Elements("configuration").FirstOrDefault();

            // NULL-check configurationElement
            if (configurationElement == null)
            {
                configurationElement = new XElement("configuration");
                this.IocContainerConfigDocument.Add(configurationElement);
            }

            // Attempt to find the configSections element
            var configSectionsElement = configurationElement.Elements("configSections").FirstOrDefault();

            // NULL-check configSectionsElement
            if (configSectionsElement == null)
            {
                configSectionsElement = new XElement("configSections");
                configurationElement.FirstNode.AddBeforeSelf(configSectionsElement);
            }

            // Attempt to find the autofac section element
            var autofacSectionElement = configSectionsElement.Elements("section").FirstOrDefault(e => e.Attribute("name") != null && string.Equals(e.Attribute("name").Value, "Autofac", StringComparison.InvariantCultureIgnoreCase));

            // NULL-check the autofacSectionElement
            if (autofacSectionElement == null)
            {
                autofacSectionElement = new XElement("section", new XAttribute("name", "autofac"), new XAttribute("type", "Autofac.Configuration.SectionHandler, Autofac.Configuration"));
                configSectionsElement.Add(autofacSectionElement);
            }

            // Attempt to find the Autofac element in the config file.
            var autofacElement = configurationElement.Elements("autofac").FirstOrDefault();

            // NULL-check autofacElement
            if (autofacElement == null)
            {
                autofacElement = new XElement("autofac");
                configurationElement.Add(autofacElement);
            }

            // Attempt to find the components element in the config file.
            var componentsElement = autofacElement.Elements("components").FirstOrDefault();

            // NULL-check componentsElement
            if (componentsElement == null)
            {
                componentsElement = new XElement("components");
                autofacElement.Add(componentsElement);
            }

            // Loop through each of the mappings in the set.
            if (!componentsElement.Elements("component").Any(e => e.Attribute("service") != null && 
                string.Equals(e.Attribute("service").Value, $"{mapping.RepositoryInterfaceCodeFile.TypeFullName}, {this.SettingsFile.Data.ModelProjectNamespace}", StringComparison.InvariantCultureIgnoreCase)))
            {
                componentsElement.Add(new XElement("component",
                    new XAttribute("instance-scope", "single-instance"),
                    new XAttribute("service", $"{mapping.RepositoryInterfaceCodeFile.TypeFullName}, {this.SettingsFile.Data.ModelProjectNamespace}"),
                    new XAttribute("type", $"{mapping.RepositoryCodeFile.TypeFullName}, {this.SettingsFile.Data.RepositoryProjectNamespace}")));
            }

            if (!string.IsNullOrWhiteSpace(this.SettingsFile.Data.ServiceProjectNamespace) &&
                !componentsElement.Elements("component").Any(e => e.Attribute("service") != null &&
                string.Equals(e.Attribute("service").Value, $"{mapping.ServiceInterfaceCodeFile.TypeFullName}, {this.SettingsFile.Data.ModelProjectNamespace}", StringComparison.InvariantCultureIgnoreCase)))
            {
                componentsElement.Add(new XElement("component",
                    new XAttribute("instance-scope", "single-instance"),
                    new XAttribute("service", $"{mapping.ServiceInterfaceCodeFile.TypeFullName}, {this.SettingsFile.Data.ModelProjectNamespace}"),
                    new XAttribute("type", $"{mapping.ServiceCodeFile.TypeFullName}, {this.SettingsFile.Data.ServiceProjectNamespace}")));
            }

            if (!componentsElement.Elements("component").Any(e => e.Attribute("service") != null &&
                string.Equals(e.Attribute("type").Value, $"{mapping.FluentNHibernateMappingCodeFile.TypeFullName}, {this.SettingsFile.Data.RepositoryProjectNamespace}", StringComparison.InvariantCultureIgnoreCase)))
            {
                componentsElement.Add(new XElement("component",
                    new XAttribute("instance-scope", "single-instance"),
                    new XAttribute("type", $"{mapping.FluentNHibernateMappingCodeFile.TypeFullName}, {this.SettingsFile.Data.RepositoryProjectNamespace}")));
            }

            var elementsWithServiceAttribute = componentsElement.Elements().Where(e => e.Attributes().Any(g => string.Equals(g.Name.LocalName, "service", StringComparison.InvariantCultureIgnoreCase)));
            var elementsWithoutServiceAttribute = componentsElement.Elements().Where(e => !e.Attributes().Any(g => string.Equals(g.Name.LocalName, "service", StringComparison.InvariantCultureIgnoreCase)));

            // Sort the components elements
            var sortedElementsWithServiceAttribute = elementsWithServiceAttribute.OrderBy(e => e.Attribute("service").Value).ToList();
            var sortedElementsWithoutServiceAttribute = elementsWithoutServiceAttribute.OrderBy(e => e.Attribute("type").Value).ToList();

            componentsElement.Elements().Remove();
            componentsElement.Add(sortedElementsWithServiceAttribute);
            componentsElement.Add(sortedElementsWithoutServiceAttribute);
        }

        /// <summary>
        /// Builds a set of mapped properties from a datareader.
        /// </summary>
        /// <returns>A set of <see cref="IMappedProperty"/> instances.</returns>
        protected virtual IEnumerable<IMappedProperty> BuildMappedProperties(IDataReader reader, IFluentNHibernateMapping mapping)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (reader.IsClosed)
            {
                throw new DataException("Datareader is closed.");
            }

            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            var schema = reader.GetSchemaTable();

            for (var fieldId = 0; fieldId < reader.FieldCount; fieldId++)
            {
                var mappedProperty = new MappedProperty
                {
                    ClrType = reader.GetFieldType(fieldId),
                    ColumnName = reader.GetName(fieldId),
                    IsNullable = schema == null ? null : new bool?(Convert.ToBoolean(schema.Rows[fieldId]["AllowDBNull"])),
                    IsPrimaryKey = false,
                    Length = schema == null ? null : new int?(Convert.ToInt32(schema.Rows[fieldId]["ColumnSize"])),
                    PropertyName = this.CodeFactory.ToPropertyName(reader.GetName(fieldId)),
                    SqlType = schema?.Rows[fieldId]["DataTypeName"].ToString(),
                };

                this.ProcessPropertyMapping(mapping, mappedProperty);

                yield return mappedProperty;
            }
        }

        /// <summary>
        /// Determines a namespace for a new type corresponding to a model type 
        /// from a model type name, the model project namespace, and the target project namespace.
        /// </summary>
        /// <param name="mapping">A mapping containing the namespace of a model type.</param>
        /// <param name="modelProjectRootNamespace">The root namespace of the model project.</param>
        /// <param name="targetProjectRootNamespace">The root namespace of the target project, in which it's intended to put a new type.</param>
        /// <returns>A <see cref="string"/></returns>
        protected virtual string DetermineCorrespondingNamespace(IMapping mapping, string modelProjectRootNamespace, string targetProjectRootNamespace)
        {
            // NULL-check the mapping parameter
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            // Ensure the namespace is present
            if (string.IsNullOrWhiteSpace(mapping.ProposedTypeNamespace))
            {
                throw new ArgumentNullException(nameof(mapping.ProposedTypeNamespace));
            }

            // NULL-check the modelProjectRootNamespace parameter
            if (string.IsNullOrWhiteSpace(modelProjectRootNamespace))
            {
                throw new ArgumentNullException(nameof(modelProjectRootNamespace));
            }

            // NULL-check the targetProjectRootNamespace parameter
            if (string.IsNullOrWhiteSpace(targetProjectRootNamespace))
            {
                throw new ArgumentNullException(nameof(targetProjectRootNamespace));
            }

            // If the model type name is equal to the model project namespace
            if (mapping.ProposedTypeNamespace == modelProjectRootNamespace)
            {
                return targetProjectRootNamespace;
            }

            // If the model type name starts with the model project namespace
            if (mapping.ProposedTypeNamespace.StartsWith($"{modelProjectRootNamespace}."))
            {
                return $"{targetProjectRootNamespace}.{mapping.ProposedTypeNamespace.Substring($"{modelProjectRootNamespace}.".Length)}";
            }

            // If the proposed type name is from already existing POCO type from an assembly
            if (this.Mappings.HasPocoMappingForProposedTypeName(mapping.ProposedTypeName))
            {
                if (mapping.ProposedTypeNamespace != mapping.SourceAssemblyRootNamespace
                    && mapping.ProposedTypeNamespace.StartsWith($"{mapping.SourceAssemblyRootNamespace}."))
                {
                    return $"{targetProjectRootNamespace}.{mapping.ProposedTypeNamespace.Substring($"{mapping.SourceAssemblyRootNamespace}.".Length)}";
                }

                return targetProjectRootNamespace;
            }

            return $"{targetProjectRootNamespace}.{mapping.ProposedTypeNamespace}";
        }

        /// <summary>
        /// Determines the output directory path for a given code file.
        /// </summary>
        /// <param name="mapping">
        /// The mapping to determine the output directory for.
        /// </param>
        /// <param name="projectNamespace">
        /// The namespace for the project that the file belongs in.
        /// </param>
        /// <param name="outputDirectoryBuilder">
        /// A <see cref="System.Text.StringBuilder"/> that, upon completion, shall contain the full path to the output directory.
        /// </param>
        /// <param name="projectDirectoryBuilder">
        /// A <see cref="System.Text.StringBuilder"/> that, upon completion, shall contain the full path to the project directory. 
        /// </param>
        /// <param name="relativeFilePathBuilder">
        /// A <see cref="System.Text.StringBuilder"/> that, upon completion, shall contain the relative path to the file.
        /// </param>
        /// <param name="subNamespace">
        /// A sub namespace, if specified, appends the necessary directory to the paths.
        /// </param>
        protected virtual void DetermineOutputDirectory(
                        IMapping mapping,
                        string projectNamespace,
                        StringBuilder outputDirectoryBuilder,
                        StringBuilder projectDirectoryBuilder,
                        StringBuilder relativeFilePathBuilder,
                        string subNamespace = null)
        {
            if (outputDirectoryBuilder == null)
            {
                outputDirectoryBuilder = new StringBuilder();
            }

            if (projectDirectoryBuilder == null)
            {
                projectDirectoryBuilder = new StringBuilder();
            }

            if (relativeFilePathBuilder == null)
            {
                relativeFilePathBuilder = new StringBuilder();
            }

            outputDirectoryBuilder.Clear();
            projectDirectoryBuilder.Clear();
            relativeFilePathBuilder.Clear();

            outputDirectoryBuilder.Append($"{this.SettingsFile.Data.OutputDirectory}");
            projectDirectoryBuilder.Append(Path.Combine(this.SettingsFile.Data.OutputDirectory, projectNamespace));

            if (mapping.ProposedTypeNamespace == this.SettingsFile.Data.ModelProjectNamespace)
            {
                // The file should be placed directly in the project root directory.
                outputDirectoryBuilder.AppendWithDelimiter(projectNamespace, "\\");
            }
            else if (mapping.ProposedTypeNamespace.StartsWith($"{this.SettingsFile.Data.ModelProjectNamespace}."))
            {
                // The file should be placed in a sub directory of the project root directory.
                outputDirectoryBuilder.AppendWithDelimiter(projectNamespace, "\\");

                var subNamespacePortions = mapping.ProposedTypeNamespace.Substring(this.SettingsFile.Data.ModelProjectNamespace.Length).Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var subNamespacePortion in subNamespacePortions)
                {
                    outputDirectoryBuilder.AppendWithDelimiter(subNamespacePortion, "\\");
                    relativeFilePathBuilder.AppendWithDelimiter(subNamespacePortion, "\\");
                }
            }
            else if (mapping.Type == MappingType.Poco)
            {
                // The file should be placed directly or in a sub directory of the project root directory.
                outputDirectoryBuilder.AppendWithDelimiter(projectNamespace, "\\");
                var @namespace = this.DetermineCorrespondingNamespace(mapping, this.SettingsFile.Data.ModelProjectNamespace, projectNamespace);
                if (@namespace.StartsWith($"{projectNamespace}."))
                {
                    foreach (var subNamespacePortion in @namespace.Substring(projectNamespace.Length + 1).Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        outputDirectoryBuilder.AppendWithDelimiter(subNamespacePortion, "\\");
                        relativeFilePathBuilder.AppendWithDelimiter(subNamespacePortion, "\\");
                    }
                }
            }
            else
            {
                // The file should be placed outside of the project root directory.
                var determiningSubNamespaceBuilder = new StringBuilder();
                var subNamespaceBuilder = new StringBuilder();
                relativeFilePathBuilder.Append("..");
                foreach (var subNamespacePortion in mapping.ProposedTypeNamespace.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    determiningSubNamespaceBuilder.AppendWithDelimiter(subNamespacePortion, ".");
                    if (projectNamespace.StartsWith($"{determiningSubNamespaceBuilder}."))
                    {
                        subNamespaceBuilder.AppendWithDelimiter(subNamespacePortion, ".");
                    }
                    else
                    {
                        outputDirectoryBuilder.AppendWithDelimiter(subNamespacePortion, "\\");
                        relativeFilePathBuilder.AppendWithDelimiter(subNamespacePortion, "\\");
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(subNamespace))
            {
                outputDirectoryBuilder.AppendWithDelimiter(subNamespace, "\\");
                relativeFilePathBuilder.AppendWithDelimiter(subNamespace, "\\");
            }
        }

        /// <summary>
        /// Makes this instance ready for processing
        /// </summary>
        protected virtual void Initialise()
        {
            this.Mappings = new List<IFluentNHibernateMapping>();
            this.ProjectCollection = new ProjectCollection();

            // Determine which type of code factory should be created
            switch (this.SettingsFile.Data.Language)
            {
                case Language.CSharp:
                    this.CodeFactory = new CSharpCodeFactory();
                    break;
                case Language.VisualBasic:
                    this.CodeFactory = new VisualBasicCodeFactory();
                    break;
                default:
                    throw new InvalidOperationException("Invalid language specified.");
            }

            // Create new file path manager based on the code factory
            this.FilePathManager = new FilePathManager(this.CodeFactory);

            // Set the global string replacement values
            this.GlobalFileStringReplacements = new Dictionary<string, string>
            {
                ["{companyName}"] = this.SettingsFile.Data.CompanyName
            };

            // Create new create database objects SQL file.
            this.CreateDatabaseObjectsFile = new CodeFile();
        }

        /// <summary>
        /// Loops through each of the mappings and produces code.
        /// </summary>
        public override void Process()
        {
            this.Process(null);
        }

        /// <summary>
        /// Loops through each of the mappings and produces code.
        /// </summary>
        /// <param name="backgroundWorker">
        /// A background worker to report progress on.
        /// </param>
        public override void Process(BackgroundWorker backgroundWorker)
        {
            this.Initialise();

            backgroundWorker.ReportProgress(0);

            // Initialise and populate the list of NHibernate mappings
            this.Mappings = new List<IFluentNHibernateMapping>();
            foreach (var mapping in this.MappingCollection.Items)
            {
                this.Mappings.Add(Mapper.Map(mapping, new FluentNHibernateMapping(mapping.Id), typeof(IMapping), typeof(IFluentNHibernateMapping)) as IFluentNHibernateMapping);
            }

            var totalMappingsCount = this.Mappings.Count;
            var mappingsProcessedCount = 0;

            // Attempt to load the IoC container config file
            if (!string.IsNullOrWhiteSpace(this.SettingsFile.Data.IocMappingsConfigFilePath)
                && File.Exists(this.SettingsFile.Data.IocMappingsConfigFilePath))
            {
                this.IocContainerConfigDocument = XDocument.Load(this.SettingsFile.Data.IocMappingsConfigFilePath);
            }

            foreach (var databaseMapping in this.Mappings.Where(m => m != null &&
                        (m.Type == MappingType.DbStoredProcedure ||
                         m.Type == MappingType.DbTable ||
                         m.Type == MappingType.DbTableValuedFunction ||
                         m.Type == MappingType.DbView)))
            {
                this.ProcessTypeMapping(databaseMapping);
                mappingsProcessedCount++;
                backgroundWorker.ReportProgress(Convert.ToInt32(((mappingsProcessedCount / (double)totalMappingsCount) * (this.SettingsFile.Data.UpdateProjectFiles ? 50 : 100))));
            }

            foreach (var pocoMapping in this.Mappings.Where(m => m != null && m.Type == MappingType.Poco))
            {
                this.ProcessTypeMapping(pocoMapping);
                mappingsProcessedCount++;
                backgroundWorker.ReportProgress(Convert.ToInt32(((mappingsProcessedCount / (double)totalMappingsCount) * (this.SettingsFile.Data.UpdateProjectFiles ? 50 : 100))));
            }

            // Write the CreateDatabaseObjectsFile
            if (this.CreateDatabaseObjectsFile.Contents.Length > 0)
            {
                File.WriteAllText(this.CreateDatabaseObjectsFile.Path, this.CreateDatabaseObjectsFile.Contents.ToString());
            }

            // Write to the IoC container config file
            if (this.IocContainerConfigDocument != null && !string.IsNullOrWhiteSpace(this.SettingsFile.Data.IocMappingsConfigFilePath))
            {
                // If file exists - remove
                if (File.Exists(this.SettingsFile.Data.IocMappingsConfigFilePath))
                {
                    File.Delete(this.SettingsFile.Data.IocMappingsConfigFilePath);
                }

                // Format the document
                var mappingFileContentBuilder = new StringBuilder();
                using (var writer = new Utilities.IO.StringWriter(mappingFileContentBuilder, System.Text.Encoding.UTF8))
                {
                    this.IocContainerConfigDocument.Save(writer);
                }

                // Write the file
                File.WriteAllText(this.SettingsFile.Data.IocMappingsConfigFilePath, mappingFileContentBuilder.ToString());
            }

            // Update project files
            if (!this.SettingsFile.Data.UpdateProjectFiles)
            {
                return;
            }

            var fileSets = new List<IEnumerable<ICodeFile>> {
                                   this.Mappings.Select(p => p.SqlCreateObjectFile),
                                   this.Mappings.Select(p => p.SqlDropObjectFile),
                                   this.Mappings.Select(p => p.ModelCodeFile),
                                   this.Mappings.Select(p => p.ModelTestsCodeFile),
                                   this.Mappings.Select(p => p.FluentNHibernateMappingCodeFile),
                                   this.Mappings.Select(p => p.RepositoryCodeFile),
                                   this.Mappings.Select(p => p.RepositoryInterfaceCodeFile),
                                   this.Mappings.Select(p => p.RepositoryTestsCodeFile),
                                   !string.IsNullOrWhiteSpace(this.SettingsFile.Data.ServiceProjectNamespace) ? this.Mappings.Select(p => p.ServiceCodeFile) : null,
                                   !string.IsNullOrWhiteSpace(this.SettingsFile.Data.ServiceProjectNamespace) ? this.Mappings.Select(p => p.ServiceInterfaceCodeFile) : null,
                                   !string.IsNullOrWhiteSpace(this.SettingsFile.Data.ServiceProjectNamespace) ? this.Mappings.Select(p => p.ServiceTestsCodeFile) : null,
                                   this.Mappings.Select(p => p.FluentNHibernateMappingTestsCodeFile)
                               };

            var totalProjectItemCount = fileSets.Where(p => p != null).Sum(f => f.Count());
            var projectItemsProcessedCount = 0;

            foreach (var fileSet in fileSets.Where(p => p != null))
            {
                foreach (var files in fileSet.GroupBy(p => p.ProjectFilePath).Where(p => p != null))
                {
                    if (files.All(p => p == null))
                    {
                        continue;
                    }

                    var projectFilePath = files.First().ProjectFilePath;

                    if (string.IsNullOrWhiteSpace(projectFilePath))
                    {
                        projectItemsProcessedCount += files.Count(f => f != null);
                        backgroundWorker.ReportProgress(Convert.ToInt32(50 + ((projectItemsProcessedCount / (double)totalProjectItemCount) * 50)));
                        continue;
                    }

                    this.UpdateProjectFile(projectFilePath, files);
                    projectItemsProcessedCount += files.Count(f => f != null);
                    backgroundWorker.ReportProgress(Convert.ToInt32(50 + ((projectItemsProcessedCount / (double)totalProjectItemCount) * 50)));
                }
            }
        }

        /// <summary>
        /// Produces fragments of code files for a specific property / column mapping.
        /// </summary>
        protected virtual void ProcessPropertyMapping(IFluentNHibernateMapping mapping, IMappedProperty mappedProperty)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            if (mappedProperty == null)
            {
                throw new ArgumentNullException(nameof(mappedProperty));
            }

            // Model file
            mapping.ModelCodeFile.PropertiesSection.AppendWithDelimiter(
                this.CodeFactory.GeneratePropertyDeclarationString(mappedProperty),
                $"\r\n{this.CodeFactory.PropertyDeclarationDelimiter}");

            // Model tests file
            mapping.ModelTestsCodeFile.TestsSection.AppendWithDelimiter(
                this.CodeFactory.GeneratePropertyUnitTestMethod(mapping, mappedProperty, mapping.ModelTestsCodeFile.ImportedNamespaces),
                "\r\n\r\n");

            // Fluent mapping file
            if ((mapping.PrimaryKeyNames.Any(p => p == mappedProperty.PropertyName) && mapping.PrimaryKeyNames.Count == 1) ||
               (mapping.PrimaryKeyNames.All(p => p == null) && string.Equals(mappedProperty.ColumnName, "id", StringComparison.InvariantCultureIgnoreCase)))
            {
                mapping.FluentNHibernateMappingCodeFile.PrimaryKeyColumnMappingsSection.AppendWithDelimiter(
                    this.CodeFactory.GenerateFluentNHibernatePrimaryKeyMappingCode(
                        mapping,
                        mappedProperty),
                    "\r\n");
            }
            else if (mapping.PrimaryKeyNames.Any(p => p == mappedProperty.PropertyName) && mapping.PrimaryKeyNames.Count > 1)
            {
                if (mapping.FluentNHibernateMappingCodeFile.PrimaryKeyColumnMappingsSection.Length == 0)
                {
                    mapping.FluentNHibernateMappingCodeFile.PrimaryKeyColumnMappingsSection.Append(
                        this.CodeFactory.GenerateFluentNHibernateCompositeIdMappingCode(mapping, mappedProperty));
                }

                mapping.FluentNHibernateMappingCodeFile.PrimaryKeyColumnMappingsSection.AppendWithDelimiter(
                    this.CodeFactory.GenerateFluentNHibernateKeyPropertyMappingCode(
                        mapping,
                        mappedProperty),
                    $"{this.CodeFactory.LineContinuationOperator}\r\n");
            }
            else
            {
                mapping.FluentNHibernateMappingCodeFile.PropertiesSection.AppendWithDelimiter(
                    this.CodeFactory.GenerateFluentNHibernatePropertyMappingCode(
                        mapping,
                        mappedProperty),
                    "\r\n");
            }

            // Fluent mapping tests file
            var propertyTest = this.CodeFactory.GenerateFluentNHibernatePropertyMappingTestsCode(
                mapping,
                mappedProperty,
                mapping.FluentNHibernateMappingTestsCodeFile.ImportedNamespaces);

            if (!string.IsNullOrWhiteSpace(propertyTest))
            {
                mapping.FluentNHibernateMappingTestsCodeFile.TestsSection.AppendWithDelimiter(propertyTest, "\r\n");
            }

            // Repository integration tests file
            if (mapping.Type == MappingType.DbTable || mapping.Type == MappingType.DbView || (mapping.Type == MappingType.Poco && !this.Mappings.HasTableViewMappingForProposedTypeName(mapping.ProposedTypeName)))
            {
                this.CodeFactory.AppendAssertAreEqualStatement(mappedProperty, mapping.RepositoryTestsCodeFile.AssertEqualPropertyValuesMethodSection, mapping.RepositoryTestsCodeFile.ImportedNamespaces);

                if (!string.Equals(mappedProperty.PropertyName, "id", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (mapping.RepositoryTestsCodeFile.InitialiseInstanceMethodSection.Length == 0)
                    {
                        mapping.RepositoryTestsCodeFile.InitialiseInstanceMethodSection.Append("\r\n");
                    }

                    mapping.RepositoryTestsCodeFile.InitialiseInstanceMethodSection.AppendLine(
                        this.CodeFactory.GenerateInitialisePropertyStatement(mapping, mappedProperty, mapping.RepositoryTestsCodeFile.ImportedNamespaces));
                }

                mapping.RepositoryTestsCodeFile.InsertStatementColumnsSection.AppendLine(
                    this.CodeFactory.GenerateInsertStatementColumnPortion(mapping, mappedProperty, !this.IsFirstPropertyMapping));

                mapping.RepositoryTestsCodeFile.InsertStatementValuesSection.AppendLine(
                    this.CodeFactory.GenerateInsertStatementValuePortion(mappedProperty, !this.IsFirstPropertyMapping));
            }

            if (this.IsFirstPropertyMapping)
            {
                this.IsFirstPropertyMapping = false;
            }
        }

        /// <summary>
        /// Produces code files for a specific type mapping.
        /// </summary>
        /// <param name="mapping">
        /// Mapping settings
        /// </param>
        protected virtual void ProcessTypeMapping(IFluentNHibernateMapping mapping)
        {
            // NULL-check the mapping parameter
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            this.IsFirstPropertyMapping = true;

            // Determine the mapping type
            switch (mapping.Type)
            {
                case MappingType.DbStoredProcedure:

                    using (var connection = new SqlConnection(this.SettingsFile.Data.ConnectionString))
                    using (var execStoredProcedureCommand = new SqlCommand($"[{mapping.SourceObjectSchema}].[{mapping.SourceObjectName}]", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        execStoredProcedureCommand.Connection.Open();
                        SqlCommandBuilder.DeriveParameters(execStoredProcedureCommand);
                        foreach (var mappingParameter in mapping.SqlParameters.Items.Where(s => s != null && s.Name != "@RETURN_VALUE"))
                        {
                            execStoredProcedureCommand.Parameters[mappingParameter.Name].Value = mappingParameter.PassNullValue || mappingParameter.Value == null ? DBNull.Value : Convert.ChangeType(mappingParameter.Value, mappingParameter.Type.ToClrType());
                        }

                        using (var reader = execStoredProcedureCommand.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            try
                            {
                                // ReSharper disable once PossibleNullReferenceException
                                (mapping.MappedProperties as List<IMappedProperty>).AddRange(this.BuildMappedProperties(reader, mapping));
                            }
                            finally
                            {
                                execStoredProcedureCommand.Cancel();
                            }
                        }
                    }

                    break;

                case MappingType.DbTableValuedFunction:

                    var parameters = Utilities.Data.SqlClient.SqlCommand.DeriveParameters(
                        $"{mapping.SourceObjectSchema}.{mapping.SourceObjectName}",
                        this.SettingsFile.Data.ConnectionString);

                    using (var connection = new SqlConnection(this.SettingsFile.Data.ConnectionString))
                    using (var selectFromTableValuedFunctionCommand = new SqlCommand(string.Empty, connection))
                    {
                        var parametersBuilder = new StringBuilder();

                        foreach (var parameter in parameters)
                        {
                            selectFromTableValuedFunctionCommand.Parameters.Add(parameter);

                            var mappingParameter = mapping.SqlParameters.Items.FirstOrDefault(s => s != null && s.Name == parameter.ParameterName);
                            if (mappingParameter == null)
                            {
                                continue;
                            }

                            var value = mappingParameter.PassNullValue || mappingParameter.Value == null ? DBNull.Value : Convert.ChangeType(mappingParameter.Value, mappingParameter.Type.ToClrType());
                            selectFromTableValuedFunctionCommand.Parameters[parameter.ParameterName].Value = value;

                            if (parameter.ParameterName == "@TABLE_RETURN_VALUE")
                            {
                                continue;
                            }

                            parametersBuilder.AppendWithDelimiter(parameter.ParameterName, ",");
                        }

                        selectFromTableValuedFunctionCommand.CommandText = $"SELECT * FROM [{mapping.SourceObjectSchema}].[{mapping.SourceObjectName}]({parametersBuilder});";
                        selectFromTableValuedFunctionCommand.Connection.Open();
                        using (var reader = selectFromTableValuedFunctionCommand.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            try
                            {
                                // ReSharper disable once PossibleNullReferenceException
                                (mapping.MappedProperties as List<IMappedProperty>).AddRange(this.BuildMappedProperties(reader, mapping));
                            }
                            finally
                            {
                                selectFromTableValuedFunctionCommand.Cancel();
                            }
                        }
                    }

                    break;

                case MappingType.DbTable:
                case MappingType.DbView:

                    // Determine the primary key columns
                    foreach (var primaryKeyColumnName in SqlTable.GetPrimaryKeyColumnNames(this.SettingsFile.Data.ConnectionString, mapping.SourceObjectName, mapping.SourceObjectSchema))
                    {
                        mapping.PrimaryKeyNames.UniqueAdd(this.CodeFactory.ToPropertyName(primaryKeyColumnName));
                    }

                    using (var connection = new SqlConnection(this.SettingsFile.Data.ConnectionString))
                    using (var selectFromTableCommand = new SqlCommand($"SELECT * FROM [{mapping.SourceObjectSchema}].[{mapping.SourceObjectName}];", connection))
                    {
                        selectFromTableCommand.Connection.Open();
                        using (var reader = selectFromTableCommand.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            try
                            {
                                // ReSharper disable once PossibleNullReferenceException
                                (mapping.MappedProperties as List<IMappedProperty>).AddRange(this.BuildMappedProperties(reader, mapping));
                            }
                            finally
                            {
                                selectFromTableCommand.Cancel();
                            }
                        }
                    }

                    break;
                case MappingType.Poco:

                    var type = this.LoadTypeFromMapping(mapping);
                    if (type == null)
                    {
                        throw new TypeLoadException($"Unable to load type {mapping.ProposedTypeFullName}");
                    }

                    var propertiesForType = type.GetProperties();
                    var hasPropertyNamedId = propertiesForType.Any(p => string.Equals(p.Name, "id", StringComparison.InvariantCultureIgnoreCase));
                    var isFirstIteration = true;
                    foreach (var property in propertiesForType)
                    {
                        var mappedProperty = new MappedProperty
                        {
                            ClrType = property.PropertyType,
                            ColumnName = property.Name,
                            PropertyName = property.Name,
                            IsPrimaryKey = (hasPropertyNamedId && string.Equals(property.Name, "id", StringComparison.InvariantCultureIgnoreCase))
                                            || (!hasPropertyNamedId && isFirstIteration),
                            SqlType = property.PropertyType.FromClrType().ToString()
                        };

                        this.ProcessPropertyMapping(mapping, mappedProperty);

                        mapping.MappedProperties.Add(mappedProperty);

                        isFirstIteration = false;
                    }

                    break;

                default:
                    throw new InvalidOperationException("Invalid mapping type specified.");
            }

            // Build the replacements dictionary
            mapping.CodeFileStringReplacements["{className}"] = this.CodeFactory.SanitiseKeyword(mapping.ProposedTypeName);
            mapping.CodeFileStringReplacements["{classNameLowerCaseFirstLetter}"] = Utilities.String.ToLowerCamelCase(mapping.ProposedTypeName);
            foreach (var globalFileStringReplacement in this.GlobalFileStringReplacements)
            {
                mapping.CodeFileStringReplacements.Add(globalFileStringReplacement);
            }

            // Set the namespaces of the mapping's code files
            this.SetNamespaces(mapping);

            // Produce code files
            if (mapping.Type != MappingType.Poco)
            {
                var mappingsForProposedTypeName = this.Mappings.Where(m => m.ProposedTypeFullName == mapping.ProposedTypeFullName).ToList();

                // If this isn't a table/view mapping, and there is a table/view mapping for this proposed type name.
                if (mapping.Type == MappingType.DbTable || mapping.Type == MappingType.DbView
                    || !mappingsForProposedTypeName.Any(m => m.Type == MappingType.DbTable || m.Type == MappingType.DbView))
                {
                    this.ProducePocoCodeFile(mapping);
                    this.ProducePocoUnitTestsCodeFile(mapping);
                }
            }

            this.ProduceRepositoryInterfaceCodeFile(mapping);
            this.ProduceRepositoryCodeFile(mapping);

            if (!string.IsNullOrWhiteSpace(this.SettingsFile.Data.ServiceProjectNamespace))
            {
                this.ProduceServiceInterfaceCodeFile(mapping);
                this.ProduceServiceCodeFile(mapping);
                this.ProduceServiceUnitTestsCodeFile(mapping);
            }

            if (mapping.Type == MappingType.DbTable || mapping.Type == MappingType.DbView
                || (mapping.Type == MappingType.Poco && !this.Mappings.HasTableViewMappingForProposedTypeName(mapping.ProposedTypeName)))
            {
                this.ProduceFluentNHibernateMappingFile(mapping);
                this.ProduceFluentNHibernateMappingTestsFile(mapping);
                this.ProduceRepositoryIntegrationTestsCodeFile(mapping);
            }
            else if (mapping.Type == MappingType.DbStoredProcedure || mapping.Type == MappingType.DbTableValuedFunction)
            {
                this.ProduceRepositoryIntegrationTestsCodeFile(mapping);
            }

            this.ProduceCreateSqlObjectScript(mapping);
            this.ProduceDropSqlObjectScript(mapping);

            if (this.IocContainerConfigDocument != null)
            {
                this.AddRepositoryServiceAndMapToIoCConfig(mapping);
            }

            mapping.Processed = true;

            // Determine the set of files for this type 
            var files = new IProjectItemFile[]
                            {
                                mapping.SqlCreateObjectFile,
                                mapping.SqlDropObjectFile,
                                mapping.ModelCodeFile,
                                mapping.ModelTestsCodeFile,
                                mapping.FluentNHibernateMappingCodeFile,
                                mapping.FluentNHibernateMappingTestsCodeFile,
                                mapping.RepositoryCodeFile,
                                mapping.RepositoryInterfaceCodeFile,
                                mapping.RepositoryTestsCodeFile,
                                !string.IsNullOrWhiteSpace(this.SettingsFile.Data.ServiceProjectNamespace) ? mapping.ServiceCodeFile : null,
                                !string.IsNullOrWhiteSpace(this.SettingsFile.Data.ServiceProjectNamespace) ? mapping.ServiceInterfaceCodeFile : null,
                                !string.IsNullOrWhiteSpace(this.SettingsFile.Data.ServiceProjectNamespace) ? mapping.ServiceTestsCodeFile : null
                            }.Where(p => p != null);

            // Write files
            foreach (var file in files.Where(f => !string.IsNullOrWhiteSpace(f.Path)))
            {
                // Attempt to get the directory path
                var directoryPath = Path.GetDirectoryName(file.Path);
                if (directoryPath == null)
                {
                    throw new InvalidOperationException($"Cannot determine directory path for file {file.Path}");
                }

                // Create the output directory if it doesn't already exist
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                if (file is IFluentNHibernateRepositoryCodeFile)
                {
                    (file as ICodeFile).Contents.Replace("{methods}", $"{(file as IFluentNHibernateRepositoryCodeFile).NamedQueriesSection}");
                }

                if (file is ICodeFile)
                {
                    // Write the file
                    File.WriteAllText(file.Path, (file as ICodeFile).Contents.ToString());
                }
            }
        }

        /// <summary>
        /// Produces a create object sql script file according to some mapping settings.
        /// </summary>
        /// <param name="mapping">A mapping to produce a create object sql script file for.</param>
        protected virtual void ProduceCreateSqlObjectScript(IFluentNHibernateMapping mapping)
        {
            // NULL-check the parameter
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            // If a create sql object file for this mapping has already been prepared, return.
            if (mapping.SqlCreateObjectFile.IsReady)
            {
                return;
            }

            // File name variable
            string fileName;

            // Determine what type of mapping this is.
            if (mapping.Type == MappingType.DbStoredProcedure)
            {
                if (!string.Equals(mapping.SourceObjectSchema, "dbo", StringComparison.InvariantCultureIgnoreCase))
                {
                    mapping.SqlCreateObjectFile.Contents.AppendWithDelimiter(ScriptGenerator.GenerateCreateSchemaScript(mapping.SourceObjectSchema), "\r\n");
                }

                mapping.SqlCreateObjectFile.Contents.AppendWithDelimiter(ScriptGenerator.GenerateCreateObjectScript(this.SettingsFile.Data.ConnectionString, mapping.SourceObjectName, mapping.SourceObjectSchema), "\r\n");
                fileName = $"CREATE PROCEDURE [{mapping.SourceObjectSchema}].[{mapping.SourceObjectName}].sql";
            }
            else if (mapping.Type == MappingType.DbTable)
            {
                if (!string.Equals(mapping.SourceObjectSchema, "dbo", StringComparison.InvariantCultureIgnoreCase))
                {
                    mapping.SqlCreateObjectFile.Contents.AppendWithDelimiter(ScriptGenerator.GenerateCreateSchemaScript(mapping.SourceObjectSchema), "\r\n");
                }

                mapping.SqlCreateObjectFile.Contents.AppendWithDelimiter(ScriptGenerator.GenerateCreateTableScript(this.SettingsFile.Data.ConnectionString, mapping.SourceObjectName, mapping.SourceObjectSchema), "\r\n");
                fileName = $"CREATE TABLE [{mapping.SourceObjectSchema}].[{mapping.SourceObjectName}].sql";
            }
            else if (mapping.Type == MappingType.DbTableValuedFunction)
            {
                if (!string.Equals(mapping.SourceObjectSchema, "dbo", StringComparison.InvariantCultureIgnoreCase))
                {
                    mapping.SqlCreateObjectFile.Contents.AppendWithDelimiter(ScriptGenerator.GenerateCreateSchemaScript(mapping.SourceObjectSchema), "\r\n");
                }

                mapping.SqlCreateObjectFile.Contents.AppendWithDelimiter(ScriptGenerator.GenerateCreateObjectScript(this.SettingsFile.Data.ConnectionString, mapping.SourceObjectName, mapping.SourceObjectSchema), "\r\n");
                fileName = $"CREATE FUNCTION [{mapping.SourceObjectSchema}].[{mapping.SourceObjectName}].sql";
            }
            else if (mapping.Type == MappingType.DbView)
            {
                if (!string.Equals(mapping.SourceObjectSchema, "dbo", StringComparison.InvariantCultureIgnoreCase))
                {
                    mapping.SqlCreateObjectFile.Contents.AppendWithDelimiter(ScriptGenerator.GenerateCreateSchemaScript(mapping.SourceObjectSchema), "\r\n");
                }

                mapping.SqlCreateObjectFile.Contents.AppendWithDelimiter(ScriptGenerator.GenerateCreateObjectScript(this.SettingsFile.Data.ConnectionString, mapping.SourceObjectName, mapping.SourceObjectSchema), "\r\n");
                fileName = $"CREATE VIEW [{mapping.SourceObjectSchema}].[{mapping.SourceObjectName}].sql";
            }
            else if (mapping.Type == MappingType.Poco)
            {
                var type = this.LoadTypeFromMapping(mapping);
                if (type == null)
                {
                    throw new TypeLoadException($"Unable to load type {mapping.ProposedTypeFullName}");
                }

                // If this poco mapping has got another mapping to a SQL object
                if (this.Mappings.Any(m => m != mapping && m.ProposedTypeName == mapping.ProposedTypeName && m.Type != MappingType.Poco))
                {
                    return;
                }

                if (!string.Equals(mapping.SourceObjectSchema, "dbo", StringComparison.InvariantCultureIgnoreCase))
                {
                    mapping.SqlCreateObjectFile.Contents.AppendWithDelimiter(ScriptGenerator.GenerateCreateSchemaScript(mapping.SourceObjectSchema), "\r\n");
                }

                mapping.SqlCreateObjectFile.Contents.AppendWithDelimiter(ScriptGenerator.GenerateCreateTableScript(type, mapping.ProposedTypeName, mapping.SourceObjectSchema, false), "\r\n");
                fileName = $"CREATE TABLE [{mapping.SourceObjectSchema}].[{mapping.ProposedTypeName}].sql";

                this.CreateDatabaseObjectsFile.Contents.AppendWithDelimiter(ScriptGenerator.GenerateCreateTableScript(type, mapping.ProposedTypeName, mapping.SourceObjectSchema), "\r\n");

                if (string.IsNullOrWhiteSpace(this.CreateDatabaseObjectsFile.Path))
                {
                    this.CreateDatabaseObjectsFile.Path = Path.Combine(this.SettingsFile.Data.OutputDirectory, "CREATE DATABASE.sql");
                }
            }
            else
            {
                return;
            }

            // Determine the output directory
            var outputDirectoryBuilder = new StringBuilder();
            var projectDirectoryBuilder = new StringBuilder();
            var relativeFilePathBuilder = new StringBuilder();
            this.DetermineOutputDirectory(mapping,
                $"{this.SettingsFile.Data.RepositoryProjectNamespace}.Tests.Integration",
                outputDirectoryBuilder,
                projectDirectoryBuilder,
                relativeFilePathBuilder);

            outputDirectoryBuilder.AppendWithDelimiter("SQL", "\\");

            // Store the file path in the mapping
            mapping.SqlCreateObjectFile.Path = Path.Combine(outputDirectoryBuilder.ToString(), fileName);

            // If the setting "UpdateProjectFiles" is set to true, update the project files.
            if (this.SettingsFile.Data.UpdateProjectFiles)
            {
                relativeFilePathBuilder.AppendWithDelimiter("SQL", "\\");
                this.SetProjectItemSettings(mapping.SqlCreateObjectFile, projectDirectoryBuilder.ToString(), relativeFilePathBuilder, fileName, ProjectItemType.EmbeddedResource);
            }

            // Indicate that the sql create object file has been prepared for this mapping.
            mapping.SqlCreateObjectFile.IsReady = true;
        }

        /// <summary>
        /// Produces a drop object sql script file according to some mapping settings.
        /// </summary>
        /// <param name="mapping">A mapping to produce a drop object sql script file for.</param>
        protected virtual void ProduceDropSqlObjectScript(IFluentNHibernateMapping mapping)
        {
            // NULL-check the parameter
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            // If a drop object sql file for this mapping has already been prepared, return.
            if (mapping.SqlDropObjectFile.IsReady)
            {
                return;
            }

            // File name variable
            string fileName;

            // Determine what type of mapping this is.
            if (mapping.Type == MappingType.DbStoredProcedure)
            {
                mapping.SqlDropObjectFile.Contents.Append(ScriptGenerator.GenerateDropObjectScript(SqlObjectType.StoredProcedure, mapping.SourceObjectName, mapping.SourceObjectSchema));
                fileName = $"DROP PROCEDURE [{mapping.SourceObjectSchema}].[{mapping.SourceObjectName}].sql";
            }
            else if (mapping.Type == MappingType.DbTable)
            {
                mapping.SqlDropObjectFile.Contents.Append(ScriptGenerator.GenerateDropObjectScript(SqlObjectType.Table, mapping.SourceObjectName, mapping.SourceObjectSchema));
                fileName = $"DROP TABLE [{mapping.SourceObjectSchema}].[{mapping.SourceObjectName}].sql";
            }
            else if (mapping.Type == MappingType.DbTableValuedFunction)
            {
                mapping.SqlDropObjectFile.Contents.Append(ScriptGenerator.GenerateDropObjectScript(SqlObjectType.TableValuedFunction, mapping.SourceObjectName, mapping.SourceObjectSchema));
                fileName = $"DROP FUNCTION [{mapping.SourceObjectSchema}].[{mapping.SourceObjectName}].sql";
            }
            else if (mapping.Type == MappingType.DbView)
            {
                mapping.SqlDropObjectFile.Contents.Append(ScriptGenerator.GenerateDropObjectScript(SqlObjectType.View, mapping.SourceObjectName, mapping.SourceObjectSchema));
                fileName = $"DROP VIEW [{mapping.SourceObjectSchema}].[{mapping.SourceObjectName}].sql";
            }
            else if (mapping.Type == MappingType.Poco)
            {
                var type = this.LoadTypeFromMapping(mapping);
                if (type == null)
                {
                    throw new TypeLoadException($"Unable to load type {mapping.ProposedTypeFullName}");
                }

                // If this poco mapping has got another mapping to a SQL object
                if (this.Mappings.Any(m => m != mapping && m.ProposedTypeName == mapping.ProposedTypeName && m.Type != MappingType.Poco))
                {
                    return;
                }

                mapping.SqlDropObjectFile.Contents.Append(ScriptGenerator.GenerateDropObjectScript(SqlObjectType.Table, mapping.SourceObjectName, mapping.SourceObjectSchema));
                fileName = $"DROP TABLE [{mapping.SourceObjectSchema}].[{mapping.SourceObjectName}].sql";
            }
            else
            {
                return;
            }

            // Determine the output directory
            var outputDirectoryBuilder = new StringBuilder();
            var projectDirectoryBuilder = new StringBuilder();
            var relativeFilePathBuilder = new StringBuilder();
            this.DetermineOutputDirectory(mapping,
                $"{this.SettingsFile.Data.RepositoryProjectNamespace}.Tests.Integration",
                outputDirectoryBuilder,
                projectDirectoryBuilder,
                relativeFilePathBuilder);

            outputDirectoryBuilder.AppendWithDelimiter("SQL", "\\");

            // Store the file path in the mapping
            mapping.SqlDropObjectFile.Path = Path.Combine(outputDirectoryBuilder.ToString(), fileName);

            // If the setting "UpdateProjectFiles" is set to true, update the project files.
            if (this.SettingsFile.Data.UpdateProjectFiles)
            {
                relativeFilePathBuilder.AppendWithDelimiter("SQL", "\\");
                this.SetProjectItemSettings(mapping.SqlDropObjectFile, projectDirectoryBuilder.ToString(), relativeFilePathBuilder, fileName, ProjectItemType.EmbeddedResource);
            }

            // Indicate that the sql create object file has been prepared for each mapping for this proposed type name.
            mapping.SqlDropObjectFile.IsReady = true;
        }

        /// <summary>
        /// Produces a fluent nhibernate mapping file according to some mapping settings.
        /// </summary>
        /// <param name="mapping">A mapping to produce a fluent nhibernate mapping file for.</param>
        protected virtual void ProduceFluentNHibernateMappingFile(IFluentNHibernateMapping mapping)
        {
            // NULL-check the parameter
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            // If a fluent nhibernate mapping file for this mapping has already been prepared, return.
            if (mapping.FluentNHibernateMappingCodeFile.IsReady)
            {
                return;
            }

            // The file is the same instance for each mapping for the same proposed type name.
            foreach (var mappingForSameTypeName in this.Mappings.Where(m => m.ProposedTypeFullName == mapping.ProposedTypeFullName))
            {
                mappingForSameTypeName.FluentNHibernateMappingCodeFile = mapping.FluentNHibernateMappingCodeFile;
            }

            // NULL-check FluentNHibernateMappingTemplateFilePath
            if (string.IsNullOrWhiteSpace(this.FilePathManager.FluentNHibernateMappingTemplateFilePath))
            {
                throw new ArgumentNullException(nameof(this.FilePathManager.FluentNHibernateMappingTemplateFilePath));
            }

            // Ensure the model template file exists
            if (!File.Exists(this.FilePathManager.FluentNHibernateMappingTemplateFilePath))
            {
                throw new FileNotFoundException($"File {this.FilePathManager.FluentNHibernateMappingTemplateFilePath} not found.");
            }

            // Read the file
            mapping.FluentNHibernateMappingCodeFile.Contents = new StringBuilder(File.ReadAllText(this.FilePathManager.FluentNHibernateMappingTemplateFilePath));

            // Determine the output directory
            var outputDirectoryBuilder = new StringBuilder();
            var projectDirectoryBuilder = new StringBuilder();
            var relativeFilePathBuilder = new StringBuilder();
            this.DetermineOutputDirectory(mapping,
                this.SettingsFile.Data.RepositoryProjectNamespace,
                outputDirectoryBuilder,
                projectDirectoryBuilder,
                relativeFilePathBuilder);

            // Determine specific replacement values specific to the fluent nhibernate file
            var fileName = $"{mapping.ProposedTypeName}Map.{this.CodeFactory.CodeFileExtension}";
            var codeFileNamespace = this.CodeFactory.DetermineCodeFileNamespace(mapping.FluentNHibernateMappingCodeFile.Namespace, this.SettingsFile.Data.RepositoryProjectNamespace);

            // Only search the project files for imported namespaces *if* the 
            // UpdateProjectFiles settings is set to true
            mapping.FluentNHibernateMappingCodeFile.ImportedNamespaces = new List<string> { "RyanPenfold.NHibernateRepository", "RyanPenfold.Utilities.Collections.Generic", $"{mapping.ProposedTypeNamespace}" };

            // Build fluent mappings
            var fluentMappingsStringBuilder = new StringBuilder();
            fluentMappingsStringBuilder.Append(this.CodeFactory.GenerateFluentNHibernateTypeMappingCode(mapping));

            if (mapping.FluentNHibernateMappingCodeFile.PrimaryKeyColumnMappingsSection.Length > 0 &&
                !mapping.FluentNHibernateMappingCodeFile.PrimaryKeyColumnMappingsSection.ToString().EndsWith(this.CodeFactory.StatementDelimiter))
            {
                mapping.FluentNHibernateMappingCodeFile.PrimaryKeyColumnMappingsSection.Append(this.CodeFactory.StatementDelimiter);
            }

            if (mapping.FluentNHibernateMappingCodeFile.PrimaryKeyColumnMappingsSection.Length > 0)
            {
                fluentMappingsStringBuilder.Append(mapping.FluentNHibernateMappingCodeFile.PrimaryKeyColumnMappingsSection);
            }

            fluentMappingsStringBuilder.AppendWithDelimiter(mapping.FluentNHibernateMappingCodeFile.PropertiesSection.ToString(), "\r\n");

            // Make replacements
            mapping.FluentNHibernateMappingCodeFile.Contents.Replace("{endNamespace}", this.CodeFactory.GenerateEndNamespaceStatement(codeFileNamespace));
            mapping.FluentNHibernateMappingCodeFile.Contents.Replace("{fileName}", fileName);
            mapping.FluentNHibernateMappingCodeFile.Contents.Replace("{fluentMappings}", fluentMappingsStringBuilder.ToString());
            mapping.FluentNHibernateMappingCodeFile.Contents.Replace("{startNamespace}", this.CodeFactory.GenerateStartNamespaceStatement(codeFileNamespace));
            mapping.FluentNHibernateMappingCodeFile.ImportedNamespaces = mapping.FluentNHibernateMappingCodeFile.ImportedNamespaces.Where(a => this.SettingsFile.Data.UpdateProjectFiles && this.FindImportedNamespaces(projectDirectoryBuilder.ToString()).All(b => b != a)).ToList();
            var usings = $"{this.CodeFactory.GenerateUsingStatements(mapping.FluentNHibernateMappingCodeFile.ImportedNamespaces, codeFileNamespace)}";
            mapping.FluentNHibernateMappingCodeFile.Contents.Replace("{usings}", string.IsNullOrWhiteSpace(usings) ? string.Empty : $"\r\n{usings}");

            // Make string replacements in the file from mapping settings
            foreach (var replacement in mapping.CodeFileStringReplacements)
            {
                mapping.FluentNHibernateMappingCodeFile.Contents.Replace(replacement.Key, replacement.Value);
            }

            outputDirectoryBuilder.AppendWithDelimiter("FluentMappings", "\\");

            // Store the file path in the mapping
            mapping.FluentNHibernateMappingCodeFile.Path = Path.Combine(outputDirectoryBuilder.ToString(), fileName);

            // If the setting "UpdateProjectFiles" is set to true, update the project files.
            if (this.SettingsFile.Data.UpdateProjectFiles)
            {
                relativeFilePathBuilder.AppendWithDelimiter("FluentMappings", "\\");
                this.SetProjectItemSettings(mapping.FluentNHibernateMappingCodeFile, projectDirectoryBuilder.ToString(), relativeFilePathBuilder, fileName, ProjectItemType.Compile);
            }

            // Indicate that the fluent nhibernate mapping file has been prepared.
            mapping.FluentNHibernateMappingCodeFile.IsReady = true;
        }

        /// <summary>
        /// Produces a fluent nhibernate mapping tests file according to some mapping settings.
        /// </summary>
        /// <param name="mapping">A mapping to produce a fluent nhibernate mapping tests file for.</param>
        protected virtual void ProduceFluentNHibernateMappingTestsFile(IFluentNHibernateMapping mapping)
        {
            // NULL-check the parameter
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            // If a fluent nhibernate mapping tests code file for this mapping has already been prepared, return.
            if (mapping.FluentNHibernateMappingTestsCodeFile.IsReady)
            {
                return;
            }

            // The file is the same instance for each mapping for the same proposed type name.
            foreach (var mappingForSameTypeName in this.Mappings.Where(m => m.ProposedTypeFullName == mapping.ProposedTypeFullName))
            {
                mappingForSameTypeName.FluentNHibernateMappingTestsCodeFile = mapping.FluentNHibernateMappingTestsCodeFile;
            }

            // NULL-check FluentNHibernateMappingTestsTemplateFilePath
            if (string.IsNullOrWhiteSpace(this.FilePathManager.FluentNHibernateMappingTestsTemplateFilePath))
            {
                throw new ArgumentNullException(nameof(this.FilePathManager.FluentNHibernateMappingTestsTemplateFilePath));
            }

            // Ensure the fluent nhibernate mapping tests template file exists
            if (!File.Exists(this.FilePathManager.FluentNHibernateMappingTestsTemplateFilePath))
            {
                throw new FileNotFoundException($"File {this.FilePathManager.FluentNHibernateMappingTestsTemplateFilePath} not found.");
            }

            // Read the file
            mapping.FluentNHibernateMappingTestsCodeFile.Contents = new StringBuilder(File.ReadAllText(this.FilePathManager.FluentNHibernateMappingTestsTemplateFilePath));

            // Determine the project namespace
            var projectNamespace = $"{this.SettingsFile.Data.RepositoryProjectNamespace}.Tests.Integration";

            // Determine the output directory
            var outputDirectoryBuilder = new StringBuilder();
            var projectDirectoryBuilder = new StringBuilder();
            var relativeFilePathBuilder = new StringBuilder();
            this.DetermineOutputDirectory(mapping,
                projectNamespace,
                outputDirectoryBuilder,
                projectDirectoryBuilder,
                relativeFilePathBuilder,
                "FluentMappings");

            // Determine replacement values specific to the fluent nhibernate mapping tests
            var fileName = $"{mapping.ProposedTypeName}MapTests.{this.CodeFactory.CodeFileExtension}";
            var codeFileNamespace = this.CodeFactory.DetermineCodeFileNamespace(
                    mapping.FluentNHibernateMappingTestsCodeFile.Namespace,
                    projectNamespace);

            // Add the relevant imported namespaces
            mapping.FluentNHibernateMappingTestsCodeFile.ImportedNamespaces.UniqueAdd("FluentNHibernate.Testing");
            mapping.FluentNHibernateMappingTestsCodeFile.ImportedNamespaces.UniqueAdd("Microsoft.VisualStudio.TestTools.UnitTesting");
            mapping.FluentNHibernateMappingTestsCodeFile.ImportedNamespaces.UniqueAdd(mapping.ProposedTypeNamespace);
            mapping.FluentNHibernateMappingTestsCodeFile.ImportedNamespaces.UniqueAdd($"{this.SettingsFile.Data.RootNamespace}.NHibernateRepository");

            // Only search the project files for imported namespaces *if* the 
            // UpdateProjectFiles settings is set to true
            mapping.FluentNHibernateMappingTestsCodeFile.ImportedNamespaces = mapping.FluentNHibernateMappingTestsCodeFile.ImportedNamespaces.Where(a => this.SettingsFile.Data.UpdateProjectFiles && this.FindImportedNamespaces(projectDirectoryBuilder.ToString()).All(b => b != a)).ToList();
            mapping.FluentNHibernateMappingTestsCodeFile.ImportedNamespaces = mapping.FluentNHibernateMappingTestsCodeFile.ImportedNamespaces.Where(a => a != codeFileNamespace && !(codeFileNamespace.StartsWith($"{a}.")) && a != projectNamespace && !(projectNamespace.StartsWith($"{a}."))).ToList();
            var usings = $"{this.CodeFactory.GenerateUsingStatements(mapping.FluentNHibernateMappingTestsCodeFile.ImportedNamespaces, codeFileNamespace)}";

            string tableOrView;
            if (mapping.Type == MappingType.DbTable || mapping.Type == MappingType.Poco)
            {
                tableOrView = "TABLE";
            }
            else if (mapping.Type == MappingType.DbView)
            {
                tableOrView = "VIEW";
            }
            else
            {
                throw new InvalidOperationException("Fluent mapping file can only pertain to a table or a view.");
            }

            var indexOfSubnamespace = mapping.FluentNHibernateMappingCodeFile.Namespace.IndexOf(".Repository.", StringComparison.Ordinal) + 1;

            // Make replacements
            mapping.FluentNHibernateMappingTestsCodeFile.Contents.Replace("{tests}", mapping.FluentNHibernateMappingTestsCodeFile.TestsSection.ToString());
            mapping.FluentNHibernateMappingTestsCodeFile.Contents.Replace("{endNamespace}", this.CodeFactory.GenerateEndNamespaceStatement(codeFileNamespace));
            mapping.FluentNHibernateMappingTestsCodeFile.Contents.Replace("{fileName}", fileName);
            mapping.FluentNHibernateMappingTestsCodeFile.Contents.Replace("{fluentMappingsNamespace}", mapping.FluentNHibernateMappingCodeFile.Namespace.Substring(indexOfSubnamespace, mapping.FluentNHibernateMappingCodeFile.Namespace.Length - indexOfSubnamespace));
            mapping.FluentNHibernateMappingTestsCodeFile.Contents.Replace("{sourceObjectName}", mapping.Type == MappingType.Poco ? mapping.ProposedTypeName : mapping.SourceObjectName);
            mapping.FluentNHibernateMappingTestsCodeFile.Contents.Replace("{sourceObjectSchema}", mapping.SourceObjectSchema);
            mapping.FluentNHibernateMappingTestsCodeFile.Contents.Replace("{startNamespace}", this.CodeFactory.GenerateStartNamespaceStatement(codeFileNamespace));
            mapping.FluentNHibernateMappingTestsCodeFile.Contents.Replace("{tableOrViewToLower}", tableOrView.ToLower());
            mapping.FluentNHibernateMappingTestsCodeFile.Contents.Replace("{tableOrViewToUpper}", tableOrView.ToUpper());
            mapping.FluentNHibernateMappingTestsCodeFile.Contents.Replace("{usings}", string.IsNullOrWhiteSpace(usings) ? string.Empty : $"\r\n{usings}");

            // Make string replacements in the file from mapping settings
            foreach (var replacement in mapping.CodeFileStringReplacements)
            {
                mapping.FluentNHibernateMappingTestsCodeFile.Contents.Replace(replacement.Key, replacement.Value);
            }

            // Store the file path in the mapping
            mapping.FluentNHibernateMappingTestsCodeFile.Path = Path.Combine(outputDirectoryBuilder.ToString(), fileName);

            // Set the project file path and the item relative file path for the project file
            if (this.SettingsFile.Data.UpdateProjectFiles)
            {
                this.SetProjectItemSettings(mapping.FluentNHibernateMappingTestsCodeFile, projectDirectoryBuilder.ToString(), relativeFilePathBuilder, fileName, ProjectItemType.Compile);
            }

            // Indicate that the fluent nhibernate mapping tests code file has been prepared.
            mapping.FluentNHibernateMappingTestsCodeFile.IsReady = true;
        }

        /// <summary>
        /// Produces a POCO type code file according to some mapping settings.
        /// </summary>
        /// <param name="mapping">A mapping to produce a POCO code file for.</param>
        protected virtual void ProducePocoCodeFile(IFluentNHibernateMapping mapping)
        {
            // NULL-check the parameter
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            // If a POCO code file for this mapping has already been prepared / written, return.
            if (mapping.ModelCodeFile.IsReady)
            {
                return;
            }

            // The file is the same instance for each mapping for the same proposed type name.
            foreach (var mappingForSameTypeName in this.Mappings.Where(m => m.ProposedTypeFullName == mapping.ProposedTypeFullName))
            {
                mappingForSameTypeName.ModelCodeFile = mapping.ModelCodeFile;
            }

            // NULL-check ModelTemplateFilePath
            if (string.IsNullOrWhiteSpace(this.FilePathManager.ModelTemplateFilePath))
            {
                throw new ArgumentNullException(nameof(this.FilePathManager.ModelTemplateFilePath));
            }

            // Ensure the model template file exists
            if (!File.Exists(this.FilePathManager.ModelTemplateFilePath))
            {
                throw new FileNotFoundException($"File {this.FilePathManager.ModelTemplateFilePath} not found.");
            }

            // Read the file
            mapping.ModelCodeFile.Contents = new StringBuilder(File.ReadAllText(this.FilePathManager.ModelTemplateFilePath));

            // Determine the output directory
            var outputDirectoryBuilder = new StringBuilder();
            var projectDirectoryBuilder = new StringBuilder();
            var relativeFilePathBuilder = new StringBuilder();
            this.DetermineOutputDirectory(mapping,
                this.SettingsFile.Data.ModelProjectNamespace,
                outputDirectoryBuilder,
                projectDirectoryBuilder,
                relativeFilePathBuilder);

            // Determine specific replacement values specific to the poco file
            var fileName = $"{mapping.ProposedTypeName}.{this.CodeFactory.CodeFileExtension}";
            var codeFileNamespace = this.CodeFactory.DetermineCodeFileNamespace(mapping.ProposedTypeNamespace, this.SettingsFile.Data.ModelProjectNamespace);

            // Make replacements
            mapping.ModelCodeFile.Contents.Replace("{endNamespace}", this.CodeFactory.GenerateEndNamespaceStatement(codeFileNamespace));
            mapping.ModelCodeFile.Contents.Replace("{fileName}", fileName);
            mapping.ModelCodeFile.Contents.Replace("{objectName}", mapping.SourceObjectName);
            mapping.ModelCodeFile.Contents.Replace("{properties}", mapping.ModelCodeFile.PropertiesSection.ToString());
            mapping.ModelCodeFile.Contents.Replace("{schema}", mapping.SourceObjectSchema);
            mapping.ModelCodeFile.Contents.Replace("{startNamespace}", this.CodeFactory.GenerateStartNamespaceStatement(codeFileNamespace));
            mapping.ModelCodeFile.Contents.Replace("{usings}", string.Empty);

            // Make string replacements in the file from mapping settings
            foreach (var replacement in mapping.CodeFileStringReplacements)
            {
                mapping.ModelCodeFile.Contents.Replace(replacement.Key, replacement.Value);
            }

            // Store the file path in the mapping
            mapping.ModelCodeFile.Path = Path.Combine(outputDirectoryBuilder.ToString(), fileName);

            // Set the project file path and the item relative file path for the project file
            if (this.SettingsFile.Data.UpdateProjectFiles)
            {
                this.SetProjectItemSettings(mapping.ModelCodeFile, projectDirectoryBuilder.ToString(), relativeFilePathBuilder, fileName, ProjectItemType.Compile);
            }

            // Indicate that the POCO code file has been prepared.
            mapping.ModelCodeFile.IsReady = true;
        }

        /// <summary>
        /// Produces a repository code file according to some mapping settings.
        /// </summary>
        /// <param name="mapping">A mapping to produce a repository code file for.</param>
        protected virtual void ProducePocoUnitTestsCodeFile(IFluentNHibernateMapping mapping)
        {
            // NULL-check the parameter
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            // If a unit tests code file for this mapping has already been prepared, return.
            if (mapping.ModelTestsCodeFile.IsReady)
            {
                return;
            }

            // The file is the same instance for each mapping for the same proposed type name.
            foreach (var mappingForSameTypeName in this.Mappings.Where(m => m.ProposedTypeFullName == mapping.ProposedTypeFullName))
            {
                mappingForSameTypeName.ModelTestsCodeFile = mapping.ModelTestsCodeFile;
            }

            // NULL-check ModelUnitTestsTemplateFilePath
            if (string.IsNullOrWhiteSpace(this.FilePathManager.ModelUnitTestsTemplateFilePath))
            {
                throw new ArgumentNullException(nameof(this.FilePathManager.ModelUnitTestsTemplateFilePath));
            }

            // Ensure the model unit tests template file exists
            if (!File.Exists(this.FilePathManager.ModelUnitTestsTemplateFilePath))
            {
                throw new FileNotFoundException($"File {this.FilePathManager.ModelUnitTestsTemplateFilePath} not found.");
            }

            // Read the file
            mapping.ModelTestsCodeFile.Contents = new StringBuilder(File.ReadAllText(this.FilePathManager.ModelUnitTestsTemplateFilePath));

            // Determine the project namespace
            var projectNamespace = $"{this.SettingsFile.Data.ModelProjectNamespace}.Tests.Unit";

            // Determine the output directory
            var outputDirectoryBuilder = new StringBuilder();
            var projectDirectoryBuilder = new StringBuilder();
            var relativeFilePathBuilder = new StringBuilder();
            this.DetermineOutputDirectory(mapping,
                projectNamespace,
                outputDirectoryBuilder,
                projectDirectoryBuilder,
                relativeFilePathBuilder);

            // Determine replacement values specific to the model unit tests
            var fileName = $"{mapping.ProposedTypeName}Tests.{this.CodeFactory.CodeFileExtension}";
            var codeFileNamespace = this.CodeFactory.DetermineCodeFileNamespace(
                this.DetermineCorrespondingNamespace(
                    mapping,
                    this.SettingsFile.Data.ModelProjectNamespace,
                    projectNamespace),
                    projectNamespace);

            // Only search the project files for imported namespaces *if* the 
            // UpdateProjectFiles settings is set to true
            mapping.ModelTestsCodeFile.ImportedNamespaces = mapping.ModelTestsCodeFile.ImportedNamespaces.Where(a => this.SettingsFile.Data.UpdateProjectFiles && this.FindImportedNamespaces(projectDirectoryBuilder.ToString()).All(b => b != a)).ToList();
            mapping.ModelTestsCodeFile.ImportedNamespaces = mapping.ModelTestsCodeFile.ImportedNamespaces.Where(a => a != codeFileNamespace && !(codeFileNamespace.StartsWith($"{a}.")) && a != projectNamespace && !(projectNamespace.StartsWith($"{a}."))).ToList();
            var usings = $"{this.CodeFactory.GenerateUsingStatements(mapping.ModelTestsCodeFile.ImportedNamespaces, codeFileNamespace)}";

            // Make replacements
            mapping.ModelTestsCodeFile.Contents.Replace("{tests}", mapping.ModelTestsCodeFile.TestsSection.Length == 0 ? string.Empty : $"\r\n{mapping.ModelTestsCodeFile.TestsSection}");
            mapping.ModelTestsCodeFile.Contents.Replace("{endNamespace}", this.CodeFactory.GenerateEndNamespaceStatement(codeFileNamespace));
            mapping.ModelTestsCodeFile.Contents.Replace("{fileName}", fileName);
            mapping.ModelTestsCodeFile.Contents.Replace("{startNamespace}", this.CodeFactory.GenerateStartNamespaceStatement(codeFileNamespace));
            mapping.ModelTestsCodeFile.Contents.Replace("{usings}", string.IsNullOrWhiteSpace(usings) ? string.Empty : $"\r\n{usings}");

            // Make string replacements in the file from mapping settings
            foreach (var replacement in mapping.CodeFileStringReplacements)
            {
                mapping.ModelTestsCodeFile.Contents.Replace(replacement.Key, replacement.Value);
            }

            // Store the file path in the mapping
            mapping.ModelTestsCodeFile.Path = Path.Combine(outputDirectoryBuilder.ToString(), fileName);

            // Set the project file path and the item relative file path for the project file
            if (this.SettingsFile.Data.UpdateProjectFiles)
            {
                this.SetProjectItemSettings(mapping.ModelTestsCodeFile, projectDirectoryBuilder.ToString(), relativeFilePathBuilder, fileName, ProjectItemType.Compile);
            }

            // Indicate that the model unit tests code file has been prepared.
            mapping.ModelTestsCodeFile.IsReady = true;
        }

        /// <summary>
        /// Produces a repository code file according to some mapping settings.
        /// </summary>
        /// <param name="mapping">A mapping to produce a repository code file for.</param>
        protected virtual void ProduceRepositoryCodeFile(IFluentNHibernateMapping mapping)
        {
            // NULL-check the parameter
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            // The repository code file is the same instance for each mapping for the same proposed type name.
            foreach (var mappingForSameTypeName in this.Mappings.Where(m => m.ProposedTypeFullName == mapping.ProposedTypeFullName))
            {
                mappingForSameTypeName.RepositoryCodeFile = mapping.RepositoryCodeFile;
            }

            // Determine the output directory
            var outputDirectoryBuilder = new StringBuilder();
            var projectDirectoryBuilder = new StringBuilder();
            var relativeFilePathBuilder = new StringBuilder();
            this.DetermineOutputDirectory(mapping,
                this.SettingsFile.Data.RepositoryProjectNamespace,
                outputDirectoryBuilder,
                projectDirectoryBuilder,
                relativeFilePathBuilder);

            // If this is a stored procedure or a table-valued function mapping
            if (mapping.Type == MappingType.DbStoredProcedure || mapping.Type == MappingType.DbTableValuedFunction)
            {
                foreach (var alreadyImportedNamespace in this.FindImportedNamespaces(projectDirectoryBuilder.ToString()))
                {
                    mapping.RepositoryCodeFile.ImportedNamespaces.UniqueAdd(alreadyImportedNamespace);
                }

                mapping.RepositoryCodeFile.NamedQueriesSection.AppendWithDelimiter($"\r\n{this.CodeFactory.GenerateFluentNHibernateGetNamedQueryCode(mapping, mapping.RepositoryCodeFile.ImportedNamespaces)}", "\r\n");
            }

            // If there are any other mappings yet to do that map to this proposed type name, 
            //  return and run the rest of the code in this method on the last map, 
            //  when all the imported namespaces have been accumulated.
            if (this.Mappings.Count(m => m.ProposedTypeName == mapping.ProposedTypeName && !m.Processed) > 1)
            {
                return;
            }

            // NULL-check RepositoryTemplateFilePath
            if (string.IsNullOrWhiteSpace(this.FilePathManager.RepositoryTemplateFilePath))
            {
                throw new ArgumentNullException(nameof(this.FilePathManager.RepositoryTemplateFilePath));
            }

            // Ensure the repository template file exists
            if (!File.Exists(this.FilePathManager.RepositoryTemplateFilePath))
            {
                throw new FileNotFoundException($"File {this.FilePathManager.RepositoryTemplateFilePath} not found.");
            }

            // Read the file
            if (mapping.RepositoryCodeFile.Contents.Length == 0)
            {
                mapping.RepositoryCodeFile.Contents = new StringBuilder(File.ReadAllText(this.FilePathManager.RepositoryTemplateFilePath));
            }

            // Determine string replacement values specific to the repository
            var fileName = $"{mapping.ProposedTypeName}Repository.{this.CodeFactory.CodeFileExtension}";
            var @namespace = this.CodeFactory.DetermineCodeFileNamespace(
                mapping.RepositoryCodeFile.Namespace,
                this.SettingsFile.Data.RepositoryProjectNamespace);

            // Only search the project files for imported namespaces *if* the 
            // UpdateProjectFiles settings is set to true
            mapping.RepositoryCodeFile.ImportedNamespaces.UniqueAdd(mapping.ProposedTypeNamespace);
            mapping.RepositoryCodeFile.ImportedNamespaces.UniqueAdd(mapping.RepositoryInterfaceCodeFile.Namespace);
            mapping.RepositoryCodeFile.ImportedNamespaces.UniqueAdd($"{this.SettingsFile.Data.RootNamespace}.NHibernateRepository");
            mapping.RepositoryCodeFile.ImportedNamespaces.UniqueAdd("FluentMappings");

            mapping.RepositoryCodeFile.ImportedNamespaces =
                mapping.RepositoryCodeFile.ImportedNamespaces.Where(a => this.SettingsFile.Data.UpdateProjectFiles && this.FindImportedNamespaces(projectDirectoryBuilder.ToString()).All(b => b != a)).ToList();
            var usings = $"{this.CodeFactory.GenerateUsingStatements(mapping.RepositoryCodeFile.ImportedNamespaces, @namespace)}";

            // Make replacements
            mapping.RepositoryCodeFile.Contents.Replace("{endNamespace}", this.CodeFactory.GenerateEndNamespaceStatement(@namespace));
            mapping.RepositoryCodeFile.Contents.Replace("{fileName}", fileName);
            mapping.RepositoryCodeFile.Contents.Replace("{startNamespace}", this.CodeFactory.GenerateStartNamespaceStatement(@namespace));
            mapping.RepositoryCodeFile.Contents.Replace("{usings}", string.IsNullOrWhiteSpace(usings) ? string.Empty : $"\r\n{usings}");

            // Make string replacements in the file from mapping settings
            foreach (var replacement in mapping.CodeFileStringReplacements)
            {
                mapping.RepositoryCodeFile.Contents.Replace(replacement.Key, replacement.Value);
            }

            // Store the file path in the mapping
            mapping.RepositoryCodeFile.Path = Path.Combine(outputDirectoryBuilder.ToString(), fileName);

            // If the setting "UpdateProjectFiles" is set to true, update the project files.
            if (this.SettingsFile.Data.UpdateProjectFiles)
            {
                this.SetProjectItemSettings(mapping.RepositoryCodeFile, projectDirectoryBuilder.ToString(), relativeFilePathBuilder, fileName, ProjectItemType.Compile);
            }

            // Indicate that the repository code file has been prepared.
            mapping.RepositoryCodeFile.IsReady = true;
        }

        /// <summary>
        /// Produces a repository tests code file according to some mapping settings.
        /// </summary>
        /// <param name="mapping">A mapping to produce a repository tests code file for.</param>
        protected virtual void ProduceRepositoryIntegrationTestsCodeFile(IFluentNHibernateMapping mapping)
        {
            // NULL-check the parameter
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            // The file is the same instance for each mapping for the same proposed type name.
            var mappingsForProposedTypeName = this.Mappings.Where(m => m.ProposedTypeFullName == mapping.ProposedTypeFullName).ToList();
            foreach (var mappingForSameTypeName in mappingsForProposedTypeName)
            {
                mappingForSameTypeName.RepositoryTestsCodeFile = mapping.RepositoryTestsCodeFile;
            }

            // NULL-check RepositoryIntegrationTestsTemplateFilePath
            if (string.IsNullOrWhiteSpace(this.FilePathManager.RepositoryIntegrationTestsTemplateFilePath))
            {
                throw new ArgumentNullException(nameof(this.FilePathManager.RepositoryIntegrationTestsTemplateFilePath));
            }

            // Ensure the repository tests template file exists
            if (!File.Exists(this.FilePathManager.RepositoryIntegrationTestsTemplateFilePath))
            {
                throw new FileNotFoundException($"File {this.FilePathManager.RepositoryIntegrationTestsTemplateFilePath} not found.");
            }

            // Read the file
            if (mapping.RepositoryTestsCodeFile.Contents.Length == 0)
            {
                mapping.RepositoryTestsCodeFile.Contents = new StringBuilder(File.ReadAllText(this.FilePathManager.RepositoryIntegrationTestsTemplateFilePath));
            }

            // Determine the project namespace
            var projectNamespace = $"{this.SettingsFile.Data.RepositoryProjectNamespace}.Tests.Integration";

            // Determine the output directory
            var outputDirectoryBuilder = new StringBuilder();
            var projectDirectoryBuilder = new StringBuilder();
            var relativeFilePathBuilder = new StringBuilder();
            this.DetermineOutputDirectory(mapping,
                projectNamespace,
                outputDirectoryBuilder,
                projectDirectoryBuilder,
                relativeFilePathBuilder);

            // Determine replacement values specific to the fluent nhibernate mapping tests
            var fileName = $"{mapping.ProposedTypeName}RepositoryTests.{this.CodeFactory.CodeFileExtension}";
            var codeFileNamespace = this.CodeFactory.DetermineCodeFileNamespace(
                mapping.RepositoryTestsCodeFile.Namespace,
                    projectNamespace);

            // Add the relevant imported namespaces
            mapping.RepositoryTestsCodeFile.ImportedNamespaces.UniqueAdd("Microsoft.VisualStudio.TestTools.UnitTesting");

            if (((mapping.Type == MappingType.DbTable || mapping.Type == MappingType.DbView) &&
               (mappingsForProposedTypeName.Count(m => (m.Type == MappingType.DbTable || m.Type == MappingType.DbView) && !m.Processed) == 1))
               || mapping.Type == MappingType.Poco && !this.Mappings.HasTableViewMappingForProposedTypeName(mapping.ProposedTypeName))
            {
                // Ensure the repository tests table/view section template file exists
                if (!File.Exists(this.FilePathManager.RepositoryIntegrationTestsTableViewTestsSectionTemplateFilePath))
                {
                    throw new FileNotFoundException($"File {this.FilePathManager.RepositoryIntegrationTestsTableViewTestsSectionTemplateFilePath} not found.");
                }

                mapping.RepositoryTestsCodeFile.TableViewTestsSection = new StringBuilder(File.ReadAllText(this.FilePathManager.RepositoryIntegrationTestsTableViewTestsSectionTemplateFilePath));

                // Add the relevant imported namespaces for the table / view tests
                mapping.RepositoryTestsCodeFile.ImportedNamespaces.UniqueAdd("System");
                mapping.RepositoryTestsCodeFile.ImportedNamespaces.UniqueAdd("System.Linq");
                mapping.RepositoryTestsCodeFile.ImportedNamespaces.UniqueAdd("System.Collections.Generic");
                mapping.RepositoryTestsCodeFile.ImportedNamespaces.UniqueAdd("RyanPenfold.Utilities.Text");
                mapping.RepositoryTestsCodeFile.ImportedNamespaces.UniqueAdd(mapping.ProposedTypeNamespace);
                mapping.RepositoryTestsCodeFile.ImportedNamespaces.UniqueAdd(mapping.RepositoryCodeFile.Namespace);

                var tableOrView = string.Empty;
                if (mapping.Type == MappingType.DbTable || mapping.Type == MappingType.Poco)
                {
                    tableOrView = "TABLE";
                }
                else if (mapping.Type == MappingType.DbView)
                {
                    tableOrView = "VIEW";
                }

                // Make replacements
                if (!string.IsNullOrWhiteSpace(tableOrView))
                {
                    mapping.RepositoryTestsCodeFile.Contents.Replace("{tableViewTests}", $"\r\n{mapping.RepositoryTestsCodeFile.TableViewTestsSection}");
                    mapping.RepositoryTestsCodeFile.Contents.Replace("{assertEqualPropertyValuesMethodBody}", mapping.RepositoryTestsCodeFile.AssertEqualPropertyValuesMethodSection.ToString().TrimEnd("\r\n".ToCharArray()));
                    mapping.RepositoryTestsCodeFile.Contents.Replace("{initialiseInstanceMethodBody}", mapping.RepositoryTestsCodeFile.InitialiseInstanceMethodSection.ToString());
                    mapping.RepositoryTestsCodeFile.Contents.Replace("{insertStatementColumns}", mapping.RepositoryTestsCodeFile.InsertStatementColumnsSection.ToString().TrimEnd("\r\n".ToCharArray()));
                    mapping.RepositoryTestsCodeFile.Contents.Replace("{insertStatementValues}", mapping.RepositoryTestsCodeFile.InsertStatementValuesSection.ToString().TrimEnd("\r\n".ToCharArray()));
                    mapping.RepositoryTestsCodeFile.Contents.Replace("{tableOrViewToLower}", tableOrView.ToLower());
                    mapping.RepositoryTestsCodeFile.Contents.Replace("{sourceObjectName}", mapping.SourceObjectName);
                    mapping.RepositoryTestsCodeFile.Contents.Replace("{sourceObjectSchema}", mapping.SourceObjectSchema);
                }
            }

            if (mapping.Type == MappingType.DbStoredProcedure || mapping.Type == MappingType.DbTableValuedFunction)
            {
                mapping.RepositoryTestsCodeFile.StoredProcedureFunctionTestsSection.AppendWithDelimiter(
                    this.CodeFactory.GenerateFluentNHibernateGetNamedQueryTestsCode(mapping, mapping.RepositoryTestsCodeFile.ImportedNamespaces), "\r\n");
            }

            if ((mapping.Type == MappingType.DbStoredProcedure || mapping.Type == MappingType.DbTableValuedFunction) &&
               (mappingsForProposedTypeName.Count(m => (m.Type == MappingType.DbStoredProcedure || m.Type == MappingType.DbTableValuedFunction) && !m.Processed) == 1))
            {
                mapping.RepositoryTestsCodeFile.StoredProcedureFunctionTestsSection.Replace("{sourceObjectSchema}", mapping.SourceObjectSchema);
                mapping.RepositoryTestsCodeFile.StoredProcedureFunctionTestsSection.Replace("{sourceObjectName}", mapping.SourceObjectName);
                mapping.RepositoryTestsCodeFile.Contents.Replace("{storedProcedureFunctionTests}", mapping.RepositoryTestsCodeFile.StoredProcedureFunctionTestsSection.ToString());
            }

            if (!mappingsForProposedTypeName.Any(m => m.Type == MappingType.DbStoredProcedure || m.Type == MappingType.DbTableValuedFunction))
            {
                mapping.RepositoryTestsCodeFile.Contents.Replace("{storedProcedureFunctionTests}", string.Empty);
            }

            if (!mappingsForProposedTypeName.Any(m => m.Type == MappingType.DbTable || m.Type == MappingType.DbView))
            {
                mapping.RepositoryTestsCodeFile.Contents.Replace("{tableViewTests}", string.Empty);
            }

            if (string.IsNullOrWhiteSpace(mapping.RepositoryTestsCodeFile.Path))
            {
                // Store the file path in the mapping
                mapping.RepositoryTestsCodeFile.Path = Path.Combine(outputDirectoryBuilder.ToString(), fileName);
            }

            // If this is a POCO mapping, include the namespace.
            if (this.Mappings.HasPocoMappingForProposedTypeName(mapping.ProposedTypeName))
            {
                mapping.RepositoryCodeFile.ImportedNamespaces.UniqueAdd(mapping.ProposedTypeNamespace);
            }

            // If this is the last mapping for the proposed type name
            if (mappingsForProposedTypeName.Count(m => !m.Processed) == 1)
            {
                // Only search the project files for imported namespaces *if* the 
                // UpdateProjectFiles settings is set to true
                mapping.RepositoryTestsCodeFile.ImportedNamespaces = mapping.RepositoryTestsCodeFile.ImportedNamespaces.Where(a => this.SettingsFile.Data.UpdateProjectFiles && this.FindImportedNamespaces(projectDirectoryBuilder.ToString()).All(b => b != a)).ToList();
                mapping.RepositoryTestsCodeFile.ImportedNamespaces = mapping.RepositoryTestsCodeFile.ImportedNamespaces.Where(a => a != codeFileNamespace && !(codeFileNamespace.StartsWith($"{a}.")) && a != projectNamespace && !(projectNamespace.StartsWith($"{a}."))).ToList();
                var usings = $"{this.CodeFactory.GenerateUsingStatements(mapping.RepositoryTestsCodeFile.ImportedNamespaces, codeFileNamespace)}";

                mapping.RepositoryTestsCodeFile.Contents.Replace("{endNamespace}", this.CodeFactory.GenerateEndNamespaceStatement(codeFileNamespace));
                mapping.RepositoryTestsCodeFile.Contents.Replace("{fileName}", fileName);
                mapping.RepositoryTestsCodeFile.Contents.Replace("{startNamespace}", this.CodeFactory.GenerateStartNamespaceStatement(codeFileNamespace));
                mapping.RepositoryTestsCodeFile.Contents.Replace("{usings}", string.IsNullOrWhiteSpace(usings) ? string.Empty : $"\r\n{usings}");

                // Make string replacements in the file from mapping settings
                foreach (var replacement in mapping.CodeFileStringReplacements)
                {
                    mapping.RepositoryTestsCodeFile.Contents.Replace(replacement.Key, replacement.Value);
                }

                // Set the project file path and the item relative file path for the project file
                if (this.SettingsFile.Data.UpdateProjectFiles)
                {
                    this.SetProjectItemSettings(mapping.RepositoryTestsCodeFile, projectDirectoryBuilder.ToString(), relativeFilePathBuilder, fileName, ProjectItemType.Compile);
                }

                // Indicate that the fluent nhibernate mapping tests code file has been prepared.
                mapping.RepositoryTestsCodeFile.IsReady = true;
            }
        }

        /// <summary>
        /// Produces a repository interface code file according to some mapping settings.
        /// </summary>
        /// <param name="mapping">A mapping to produce a repository interface code file for.</param>
        protected virtual void ProduceRepositoryInterfaceCodeFile(IFluentNHibernateMapping mapping)
        {
            // NULL-check the parameter
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            // The file is the same instance for each mapping for the same proposed type name.
            foreach (var mappingForSameTypeName in this.Mappings.Where(m => m.ProposedTypeFullName == mapping.ProposedTypeFullName))
            {
                mappingForSameTypeName.RepositoryInterfaceCodeFile = mapping.RepositoryInterfaceCodeFile;
            }

            // Determine the output directory
            var outputDirectoryBuilder = new StringBuilder();
            var projectDirectoryBuilder = new StringBuilder();
            var relativeFilePathBuilder = new StringBuilder();
            this.DetermineOutputDirectory(mapping,
                this.SettingsFile.Data.ModelProjectNamespace,
                outputDirectoryBuilder,
                projectDirectoryBuilder,
                relativeFilePathBuilder);

            // If this is a stored procedure or a table-valued function mapping
            if (mapping.Type == MappingType.DbStoredProcedure || mapping.Type == MappingType.DbTableValuedFunction)
            {
                foreach (var alreadyImportedNamespace in this.FindImportedNamespaces(projectDirectoryBuilder.ToString()))
                {
                    mapping.RepositoryInterfaceCodeFile.ImportedNamespaces.UniqueAdd(alreadyImportedNamespace);
                }

                mapping.RepositoryInterfaceCodeFile.NamedQueriesSection.AppendWithDelimiter($"\r\n{this.CodeFactory.GenerateFluentNHibernateGetNamedQueryInterfaceCode(mapping, mapping.RepositoryInterfaceCodeFile.ImportedNamespaces)}", "\r\n");
            }

            // If this is a POCO mapping, include the namespace.
            if (this.Mappings.HasPocoMappingForProposedTypeName(mapping.ProposedTypeName))
            {
                mapping.RepositoryInterfaceCodeFile.ImportedNamespaces.UniqueAdd(mapping.ProposedTypeNamespace);
            }

            // If there are any other mappings yet to do that map to this proposed type name, 
            //  return and run the rest of the code in this method on the last map, 
            //  when all the imported namespaces have been accumulated.
            if (this.Mappings.Count(m => m.ProposedTypeName == mapping.ProposedTypeName && !m.Processed) > 1)
            {
                return;
            }

            // NULL-check RepositoryInterfaceTemplateFilePath
            if (string.IsNullOrWhiteSpace(this.FilePathManager.RepositoryInterfaceTemplateFilePath))
            {
                throw new ArgumentNullException(nameof(this.FilePathManager.RepositoryInterfaceTemplateFilePath));
            }

            // Ensure the repository interface template file exists
            if (!File.Exists(this.FilePathManager.RepositoryInterfaceTemplateFilePath))
            {
                throw new FileNotFoundException($"File {this.FilePathManager.RepositoryInterfaceTemplateFilePath} not found.");
            }

            // Read the file
            mapping.RepositoryInterfaceCodeFile.Contents = new StringBuilder(File.ReadAllText(this.FilePathManager.RepositoryInterfaceTemplateFilePath));

            // Determine string replacement values specific to the repository interface
            var fileName = $"I{mapping.ProposedTypeName}Repository.{this.CodeFactory.CodeFileExtension}";
            var @namespace = this.CodeFactory.DetermineCodeFileNamespace(mapping.RepositoryInterfaceCodeFile.Namespace, this.SettingsFile.Data.ModelProjectNamespace);
            mapping.RepositoryInterfaceCodeFile.ImportedNamespaces.UniqueAdd($"{this.SettingsFile.Data.RootNamespace}.BusinessBase.Infrastructure");
            mapping.RepositoryInterfaceCodeFile.ImportedNamespaces =
                mapping.RepositoryInterfaceCodeFile.ImportedNamespaces.Where(a =>
                            a != this.SettingsFile.Data.ModelProjectNamespace
                            && !a.StartsWith($"{this.SettingsFile.Data.ModelProjectNamespace}.")
                            && (!this.SettingsFile.Data.UpdateProjectFiles
                                ||
                                (this.SettingsFile.Data.UpdateProjectFiles
                                    && this.FindImportedNamespaces(projectDirectoryBuilder.ToString()).All(b => b != a)))).ToList();
            var usings = $"{this.CodeFactory.GenerateUsingStatements(mapping.RepositoryInterfaceCodeFile.ImportedNamespaces, @namespace)}";

            // Make replacements
            mapping.RepositoryInterfaceCodeFile.Contents.Replace("{endNamespace}", this.CodeFactory.GenerateEndNamespaceStatement(@namespace));
            mapping.RepositoryInterfaceCodeFile.Contents.Replace("{fileName}", fileName);
            mapping.RepositoryInterfaceCodeFile.Contents.Replace("{startNamespace}", this.CodeFactory.GenerateStartNamespaceStatement(@namespace));
            mapping.RepositoryInterfaceCodeFile.Contents.Replace("{usings}", string.IsNullOrWhiteSpace(usings) ? string.Empty : $"\r\n{usings}");

            // Make string replacements in the file from mapping settings
            foreach (var replacement in mapping.CodeFileStringReplacements)
            {
                mapping.RepositoryInterfaceCodeFile.Contents.Replace(replacement.Key, replacement.Value);
            }

            outputDirectoryBuilder.AppendWithDelimiter("RepositoryInterfaces", "\\");

            mapping.RepositoryInterfaceCodeFile.Path = Path.Combine(outputDirectoryBuilder.ToString(), fileName);

            // If the setting "UpdateProjectFiles" is set to true, update the project files.
            if (this.SettingsFile.Data.UpdateProjectFiles)
            {
                relativeFilePathBuilder.AppendWithDelimiter("RepositoryInterfaces", "\\");
                this.SetProjectItemSettings(mapping.RepositoryInterfaceCodeFile, projectDirectoryBuilder.ToString(), relativeFilePathBuilder, fileName, ProjectItemType.Compile);
            }

            // Indicate that the repository interface code file has been prepared.
            mapping.RepositoryInterfaceCodeFile.IsReady = true;
        }

        /// <summary>
        /// Produces a service code file according to some mapping settings.
        /// </summary>
        /// <param name="mapping">A mapping to produce a service code file for.</param>
        protected virtual void ProduceServiceCodeFile(IFluentNHibernateMapping mapping)
        {
            // NULL-check the parameter
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            // If a service code file for this mapping has already been prepared, return.
            if (mapping.ServiceCodeFile.IsReady)
            {
                return;
            }

            // The file is the same instance for each mapping for the same proposed type name.
            foreach (var mappingForSameTypeName in this.Mappings.Where(m => m.ProposedTypeFullName == mapping.ProposedTypeFullName))
            {
                mappingForSameTypeName.ServiceCodeFile = mapping.ServiceCodeFile;
            }

            // If this is a POCO mapping, include the namespace.
            if (this.Mappings.HasPocoMappingForProposedTypeName(mapping.ProposedTypeName))
            {
                mapping.ServiceCodeFile.ImportedNamespaces.UniqueAdd(mapping.ProposedTypeNamespace);
            }

            // NULL-check ServiceTemplateFilePath
            if (string.IsNullOrWhiteSpace(this.FilePathManager.ServiceTemplateFilePath))
            {
                throw new ArgumentNullException(nameof(this.FilePathManager.ServiceTemplateFilePath));
            }

            // Ensure the service template file exists
            if (!File.Exists(this.FilePathManager.ServiceTemplateFilePath))
            {
                throw new FileNotFoundException($"File {this.FilePathManager.ServiceTemplateFilePath} not found.");
            }

            // Read the file
            mapping.ServiceCodeFile.Contents = new StringBuilder(File.ReadAllText(this.FilePathManager.ServiceTemplateFilePath));

            // Determine the output directory
            var outputDirectoryBuilder = new StringBuilder();
            var projectDirectoryBuilder = new StringBuilder();
            var relativeFilePathBuilder = new StringBuilder();
            this.DetermineOutputDirectory(mapping,
                this.SettingsFile.Data.ServiceProjectNamespace,
                outputDirectoryBuilder,
                projectDirectoryBuilder,
                relativeFilePathBuilder);

            // Determine string replacement values specific to the service
            var fileName = $"{mapping.ProposedTypeName}Service.{this.CodeFactory.CodeFileExtension}";
            var @namespace = this.CodeFactory.DetermineCodeFileNamespace(
                mapping.ServiceCodeFile.Namespace,
                this.SettingsFile.Data.ServiceProjectNamespace);

            // Only search the project files for imported namespaces *if* the 
            // UpdateProjectFiles settings is set to true
            mapping.ServiceCodeFile.ImportedNamespaces = new[]
            {
                "System", "System.Collections.Generic", "System.Linq",
                $"{this.SettingsFile.Data.RootNamespace}.BusinessBase.Infrastructure",
                $"{this.SettingsFile.Data.RootNamespace}.IocContainer",
                $"{mapping.ProposedTypeNamespace}",
                mapping.RepositoryInterfaceCodeFile.Namespace,
                mapping.ServiceInterfaceCodeFile.Namespace
            }.Where(a => this.SettingsFile.Data.UpdateProjectFiles && this.FindImportedNamespaces(projectDirectoryBuilder.ToString()).All(b => b != a)).ToList();
            var usings = $"{this.CodeFactory.GenerateUsingStatements(mapping.ServiceCodeFile.ImportedNamespaces, @namespace)}";

            // Make replacements
            mapping.ServiceCodeFile.Contents.Replace("{endNamespace}", this.CodeFactory.GenerateEndNamespaceStatement(@namespace));
            mapping.ServiceCodeFile.Contents.Replace("{fileName}", fileName);
            mapping.ServiceCodeFile.Contents.Replace("{startNamespace}", this.CodeFactory.GenerateStartNamespaceStatement(@namespace));
            mapping.ServiceCodeFile.Contents.Replace("{usings}", string.IsNullOrWhiteSpace(usings) ? string.Empty : $"\r\n{usings}");

            // Make string replacements in the file from mapping settings
            foreach (var replacement in mapping.CodeFileStringReplacements)
            {
                mapping.ServiceCodeFile.Contents.Replace(replacement.Key, replacement.Value);
            }

            // Create the output directory if it doesn't already exist
            if (!Directory.Exists(outputDirectoryBuilder.ToString()))
            {
                Directory.CreateDirectory(outputDirectoryBuilder.ToString());
            }

            // Write the file
            mapping.ServiceCodeFile.Path = Path.Combine(outputDirectoryBuilder.ToString(), fileName);

            // If the setting "UpdateProjectFiles" is set to true, update the project files.
            if (this.SettingsFile.Data.UpdateProjectFiles)
            {
                this.SetProjectItemSettings(mapping.ServiceCodeFile, projectDirectoryBuilder.ToString(), relativeFilePathBuilder, fileName, ProjectItemType.Compile);
            }

            // Indicate that the service code file has been prepared.
            mapping.ServiceCodeFile.IsReady = true;
        }

        /// <summary>
        /// Produces a service interface code file according to some mapping settings.
        /// </summary>
        /// <param name="mapping">A mapping to produce a service interface code file for.</param>
        protected virtual void ProduceServiceInterfaceCodeFile(IFluentNHibernateMapping mapping)
        {
            // NULL-check the parameter
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            // If a service interface code file for this mapping has already been prepared, return.
            if (mapping.ServiceInterfaceCodeFile.IsReady)
            {
                return;
            }

            // If this is a POCO mapping, include the namespace.
            if (this.Mappings.HasPocoMappingForProposedTypeName(mapping.ProposedTypeName))
            {
                mapping.ServiceInterfaceCodeFile.ImportedNamespaces.UniqueAdd(mapping.ProposedTypeNamespace);
            }

            foreach (var mappingForSameTypeName in this.Mappings.Where(m => m.ProposedTypeFullName == mapping.ProposedTypeFullName))
            {
                mappingForSameTypeName.ServiceInterfaceCodeFile = mapping.ServiceInterfaceCodeFile;
            }

            // NULL-check ServiceInterfaceTemplateFilePath
            if (string.IsNullOrWhiteSpace(this.FilePathManager.ServiceInterfaceTemplateFilePath))
            {
                throw new ArgumentNullException(nameof(this.FilePathManager.ServiceInterfaceTemplateFilePath));
            }

            // Ensure the service interface template file exists
            if (!File.Exists(this.FilePathManager.ServiceInterfaceTemplateFilePath))
            {
                throw new FileNotFoundException($"File {this.FilePathManager.ServiceInterfaceTemplateFilePath} not found.");
            }

            // Read the file
            mapping.ServiceInterfaceCodeFile.Contents = new StringBuilder(File.ReadAllText(this.FilePathManager.ServiceInterfaceTemplateFilePath));

            // Determine the output directory
            var outputDirectoryBuilder = new StringBuilder();
            var projectDirectoryBuilder = new StringBuilder();
            var relativeFilePathBuilder = new StringBuilder();
            this.DetermineOutputDirectory(mapping,
                this.SettingsFile.Data.ModelProjectNamespace,
                outputDirectoryBuilder,
                projectDirectoryBuilder,
                relativeFilePathBuilder);

            // Determine string replacement values specific to the service interface
            var fileName = $"I{mapping.ProposedTypeName}Service.{this.CodeFactory.CodeFileExtension}";
            var @namespace = this.CodeFactory.DetermineCodeFileNamespace(mapping.ServiceInterfaceCodeFile.Namespace, this.SettingsFile.Data.ModelProjectNamespace);
            mapping.ServiceInterfaceCodeFile.ImportedNamespaces = new[] { "System.Collections.Generic" }.ToList();

            // If this is a POCO mapping, include the namespace.
            if (this.Mappings.HasPocoMappingForProposedTypeName(mapping.ProposedTypeName))
            {
                mapping.ServiceInterfaceCodeFile.ImportedNamespaces.UniqueAdd(mapping.ProposedTypeNamespace);
            }

            var usings = $"{this.CodeFactory.GenerateUsingStatements(mapping.ServiceInterfaceCodeFile.ImportedNamespaces.Where(a => this.SettingsFile.Data.UpdateProjectFiles && this.FindImportedNamespaces(projectDirectoryBuilder.ToString()).All(b => b != a)), @namespace)}";

            // Make replacements
            mapping.ServiceInterfaceCodeFile.Contents.Replace("{endNamespace}", this.CodeFactory.GenerateEndNamespaceStatement(@namespace));
            mapping.ServiceInterfaceCodeFile.Contents.Replace("{fileName}", fileName);
            mapping.ServiceInterfaceCodeFile.Contents.Replace("{startNamespace}", this.CodeFactory.GenerateStartNamespaceStatement(@namespace));
            mapping.ServiceInterfaceCodeFile.Contents.Replace("{usings}", string.IsNullOrWhiteSpace(usings) ? string.Empty : $"\r\n{usings}");

            // Make string replacements in the file from mapping settings
            foreach (var replacement in mapping.CodeFileStringReplacements)
            {
                mapping.ServiceInterfaceCodeFile.Contents.Replace(replacement.Key, replacement.Value);
            }

            outputDirectoryBuilder.AppendWithDelimiter("ServiceInterfaces", "\\");

            // Store the file path in the mapping
            mapping.ServiceInterfaceCodeFile.Path = Path.Combine(outputDirectoryBuilder.ToString(), fileName);

            // If the setting "UpdateProjectFiles" is set to true, update the project files.
            if (this.SettingsFile.Data.UpdateProjectFiles)
            {
                relativeFilePathBuilder.AppendWithDelimiter("ServiceInterfaces", "\\");
                this.SetProjectItemSettings(mapping.ServiceInterfaceCodeFile, projectDirectoryBuilder.ToString(), relativeFilePathBuilder, fileName, ProjectItemType.Compile);
            }

            // Indicate that the service interface code file has been prepared.
            mapping.ServiceInterfaceCodeFile.IsReady = true;
        }

        /// <summary>
        /// Produces a code file containing service unit tests according to some mapping settings.
        /// </summary>
        /// <param name="mapping">A mapping to produce a code file for.</param>
        protected virtual void ProduceServiceUnitTestsCodeFile(IFluentNHibernateMapping mapping)
        {
            // NULL-check the parameter
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            // If a service unit test code file for this mapping has already been prepared, return.
            if (mapping.ServiceTestsCodeFile.IsReady)
            {
                return;
            }

            foreach (var mappingForSameTypeName in this.Mappings.Where(m => m.ProposedTypeFullName == mapping.ProposedTypeFullName))
            {
                mappingForSameTypeName.ServiceTestsCodeFile = mapping.ServiceTestsCodeFile;
            }

            // NULL-check ServiceUnitTestsTemplateFilePath
            if (string.IsNullOrWhiteSpace(this.FilePathManager.ServiceUnitTestsTemplateFilePath))
            {
                throw new ArgumentNullException(nameof(this.FilePathManager.ServiceUnitTestsTemplateFilePath));
            }

            // Ensure the service unit tests template file exists
            if (!File.Exists(this.FilePathManager.ServiceUnitTestsTemplateFilePath))
            {
                throw new FileNotFoundException($"File {this.FilePathManager.ServiceUnitTestsTemplateFilePath} not found.");
            }

            // Read the file
            mapping.ServiceTestsCodeFile.Contents = new StringBuilder(File.ReadAllText(this.FilePathManager.ServiceUnitTestsTemplateFilePath));

            // Determine the output directory
            var outputDirectoryBuilder = new StringBuilder();
            var projectDirectoryBuilder = new StringBuilder();
            var relativeFilePathBuilder = new StringBuilder();
            this.DetermineOutputDirectory(mapping,
                $"{this.SettingsFile.Data.ServiceProjectNamespace}.Tests.Unit",
                outputDirectoryBuilder,
                projectDirectoryBuilder,
                relativeFilePathBuilder);

            // Determine string replacement values specific to the service unit tests
            var fileName = $"{mapping.ProposedTypeName}ServiceTests.{this.CodeFactory.CodeFileExtension}";
            var @namespace = this.CodeFactory.DetermineCodeFileNamespace(
                mapping.ServiceTestsCodeFile.Namespace,
                $"{this.SettingsFile.Data.ServiceProjectNamespace}.Tests.Unit");
            var serviceSubNamespace = mapping.ServiceTestsCodeFile.Namespace;

            mapping.ServiceTestsCodeFile.ImportedNamespaces.UniqueAdd("Microsoft.VisualStudio.TestTools.UnitTesting");
            mapping.ServiceTestsCodeFile.ImportedNamespaces.UniqueAdd("Moq");
            mapping.ServiceTestsCodeFile.ImportedNamespaces.UniqueAdd(mapping.ProposedTypeNamespace);
            mapping.ServiceTestsCodeFile.ImportedNamespaces.UniqueAdd(mapping.RepositoryInterfaceCodeFile.Namespace);
            mapping.ServiceTestsCodeFile.ImportedNamespaces.UniqueAdd(mapping.ServiceCodeFile.Namespace);

            if (serviceSubNamespace != this.SettingsFile.Data.ServiceProjectNamespace)
            {
                mapping.ServiceTestsCodeFile.ImportedNamespaces.UniqueAdd(serviceSubNamespace);
            }

            // Generate the unit tests
            var testsString = this.CodeFactory.GenerateServiceUnitTests(mapping, mapping.ServiceTestsCodeFile.ImportedNamespaces);

            // Only search the project files for imported namespaces *if* the 
            // UpdateProjectFiles settings is set to true
            mapping.ServiceTestsCodeFile.ImportedNamespaces =
                mapping.ServiceTestsCodeFile.ImportedNamespaces.Where(a =>
                a != mapping.ServiceTestsCodeFile.Namespace
                && !mapping.ServiceTestsCodeFile.Namespace.StartsWith($"{a}.")
                && (!this.SettingsFile.Data.UpdateProjectFiles
                    ||
                    (this.SettingsFile.Data.UpdateProjectFiles
                        && this.FindImportedNamespaces(projectDirectoryBuilder.ToString()).All(b => b != a)))).ToList();
            var usings = $"{this.CodeFactory.GenerateUsingStatements(mapping.ServiceTestsCodeFile.ImportedNamespaces, @namespace)}";

            // Make replacements
            mapping.ServiceTestsCodeFile.Contents.Replace("{tests}", testsString.Length == 0 ? string.Empty : $"\r\n{testsString}");
            mapping.ServiceTestsCodeFile.Contents.Replace("{endNamespace}", this.CodeFactory.GenerateEndNamespaceStatement(@namespace));
            mapping.ServiceTestsCodeFile.Contents.Replace("{fileName}", fileName);
            mapping.ServiceTestsCodeFile.Contents.Replace("{startNamespace}", this.CodeFactory.GenerateStartNamespaceStatement(@namespace));
            mapping.ServiceTestsCodeFile.Contents.Replace("{usings}", string.IsNullOrWhiteSpace(usings) ? string.Empty : $"\r\n{usings}");

            // Make string replacements in the file from mapping settings
            foreach (var replacement in mapping.CodeFileStringReplacements)
            {
                mapping.ServiceTestsCodeFile.Contents.Replace(replacement.Key, replacement.Value);
            }

            mapping.ServiceTestsCodeFile.Path = Path.Combine(outputDirectoryBuilder.ToString(), fileName);

            // If the setting "UpdateProjectFiles" is set to true, update the project files.
            if (this.SettingsFile.Data.UpdateProjectFiles)
            {
                this.SetProjectItemSettings(mapping.ServiceTestsCodeFile, projectDirectoryBuilder.ToString(), relativeFilePathBuilder, fileName, ProjectItemType.Compile);
            }

            // Indicate that the service tests code file has been prepared.
            mapping.ServiceTestsCodeFile.IsReady = true;
        }

        /// <summary>
        /// Sets the namespaces of the mapping's code files.
        /// </summary>
        /// <param name="mapping">The mapping to set the namespaces of.</param>
        protected virtual void SetNamespaces(IFluentNHibernateMapping mapping)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            // FluentNHibernateMappingCodeFile
            if (string.IsNullOrWhiteSpace(mapping.FluentNHibernateMappingCodeFile.Namespace))
            {
                var fluentMappingsCodeFileRootNamespace = this.DetermineCorrespondingNamespace(
                    mapping,
                    this.SettingsFile.Data.ModelProjectNamespace,
                    this.SettingsFile.Data.RepositoryProjectNamespace);
                mapping.FluentNHibernateMappingCodeFile.Namespace = $"{fluentMappingsCodeFileRootNamespace}.FluentMappings";
                mapping.FluentNHibernateMappingCodeFile.TypeName = $"{mapping.ProposedTypeName}Map";
            }

            // FluentNHibernateMappingTestsCodeFile
            if (string.IsNullOrWhiteSpace(mapping.FluentNHibernateMappingTestsCodeFile.Namespace))
            {
                var repositoryTestsProjectNamespace = $"{this.SettingsFile.Data.RepositoryProjectNamespace}.Tests.Integration";
                var fluentMappingsTestsCodeFileRootNamespace = this.DetermineCorrespondingNamespace(
                    mapping,
                    this.SettingsFile.Data.ModelProjectNamespace,
                    repositoryTestsProjectNamespace);
                mapping.FluentNHibernateMappingTestsCodeFile.Namespace = $"{fluentMappingsTestsCodeFileRootNamespace}.FluentMappings";
                mapping.FluentNHibernateMappingTestsCodeFile.TypeName = $"{mapping.ProposedTypeName}MapTests";
            }

            // RepositoryCodeFile
            if (string.IsNullOrWhiteSpace(mapping.RepositoryCodeFile.Namespace))
            {
                mapping.RepositoryCodeFile.Namespace = this.DetermineCorrespondingNamespace(
                    mapping,
                    this.SettingsFile.Data.ModelProjectNamespace,
                    this.SettingsFile.Data.RepositoryProjectNamespace);
                mapping.RepositoryCodeFile.TypeName = $"{mapping.ProposedTypeName}Repository";
            }

            // RepositoryInterfaceCodeFile
            if (string.IsNullOrWhiteSpace(mapping.RepositoryInterfaceCodeFile.Namespace))
            {
                var correspondingNamespace = this.DetermineCorrespondingNamespace(
                    mapping,
                    this.SettingsFile.Data.ModelProjectNamespace,
                    this.SettingsFile.Data.ModelProjectNamespace);
                mapping.RepositoryInterfaceCodeFile.Namespace = $"{correspondingNamespace}.RepositoryInterfaces";
                mapping.RepositoryInterfaceCodeFile.TypeName = $"I{mapping.ProposedTypeName}Repository";
            }

            // RepositoryTestsCodeFile
            if (string.IsNullOrWhiteSpace(mapping.RepositoryTestsCodeFile.Namespace))
            {
                var repositoryTestsProjectNamespace = $"{this.SettingsFile.Data.RepositoryProjectNamespace}.Tests.Integration";
                mapping.RepositoryTestsCodeFile.Namespace = this.DetermineCorrespondingNamespace(
                    mapping,
                    this.SettingsFile.Data.ModelProjectNamespace,
                    repositoryTestsProjectNamespace);
                mapping.RepositoryTestsCodeFile.TypeName = $"{mapping.ProposedTypeName}RepositoryTests";
            }

            // ServiceInterfaceCodeFile
            if (!string.IsNullOrWhiteSpace(this.SettingsFile.Data.ServiceProjectNamespace) && 
                string.IsNullOrWhiteSpace(mapping.ServiceInterfaceCodeFile.Namespace))
            {
                var correspondingNamespace = this.DetermineCorrespondingNamespace(
                    mapping,
                    this.SettingsFile.Data.ModelProjectNamespace,
                    this.SettingsFile.Data.ModelProjectNamespace);
                mapping.ServiceInterfaceCodeFile.Namespace = $"{correspondingNamespace}.ServiceInterfaces";
                mapping.ServiceInterfaceCodeFile.TypeName = $"I{mapping.ProposedTypeName}Service";
            }

            // ServiceCodeFile
            if (!string.IsNullOrWhiteSpace(this.SettingsFile.Data.ServiceProjectNamespace) && 
                string.IsNullOrWhiteSpace(mapping.ServiceCodeFile.Namespace))
            {
                mapping.ServiceCodeFile.Namespace = this.DetermineCorrespondingNamespace(
                    mapping,
                    this.SettingsFile.Data.ModelProjectNamespace,
                    this.SettingsFile.Data.ServiceProjectNamespace);
                mapping.ServiceCodeFile.TypeName = $"{mapping.ProposedTypeName}Service";
            }

            // ServiceTestsCodeFile
            if (!string.IsNullOrWhiteSpace(this.SettingsFile.Data.ServiceProjectNamespace) && 
                string.IsNullOrWhiteSpace(mapping.ServiceTestsCodeFile.Namespace))
            {
                var serviceTestsProjectNamespace = $"{this.SettingsFile.Data.ServiceProjectNamespace}.Tests.Unit";
                mapping.ServiceTestsCodeFile.Namespace = this.DetermineCorrespondingNamespace(
                    mapping,
                    this.SettingsFile.Data.ModelProjectNamespace,
                    serviceTestsProjectNamespace);
                mapping.ServiceTestsCodeFile.TypeName = $"{mapping.ProposedTypeName}ServiceTests";
            }
        }
    }
}
