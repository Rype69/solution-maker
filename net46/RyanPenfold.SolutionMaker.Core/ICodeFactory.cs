// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICodeFactory.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface for producing program code
    /// </summary>
    public interface ICodeFactory
    {
        /// <summary>
        /// Gets the file extension for code files written in the specific language.
        /// </summary>
        string CodeFileExtension { get; }

        /// <summary>
        /// Gets the keywords of the specific language.
        /// </summary>
        IEnumerable<string> Keywords { get; }

        /// <summary>
        /// Gets the line continuation operator for the specific language.
        /// </summary>
        string LineContinuationOperator { get; }

        /// <summary>
        /// Gets the null operator for the specific language.
        /// </summary>
        string NullOperator { get; }

        /// <summary>
        /// Gets the file extension for projects for code written in the specific language.
        /// </summary>
        string ProjectFileExtension { get; }

        /// <summary>
        /// Gets the property declaration delimiter for the specific language.
        /// </summary>
        string PropertyDeclarationDelimiter { get; }

        /// <summary>
        /// Gets the statement delimiter specific to the language.
        /// </summary>
        string StatementDelimiter { get; }

        /// <summary>
        /// Attempts to find an alias for a type name in the specific language.
        /// </summary>
        /// <param name="type">The type to find an alias name for.</param>
        /// <returns>An alias type name.</returns>
        string AliasTypeName(System.Type type);

        /// <summary>
        /// Generates an "Assert.AreEqual" statement that compares the values of two of the same properties from two 
        /// different instances of the same type, and appends it to a <see cref="System.Text.StringBuilder"/> instance.
        /// </summary>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <param name="assertMethodString">A <see cref="string"/> containing a method containing a set of asserts.</param>
        /// <param name="importedNamespaces">A set of imported namespaces for the code file.</param>
        void AppendAssertAreEqualStatement(IMappedProperty mappedProperty, System.Text.StringBuilder assertMethodString, IList<string> importedNamespaces);

        /// <summary>
        /// Determines the code file namespace for a given type namespace.
        /// </summary>
        /// <param name="typeNamespace">A namespace for a given type.</param>
        /// <param name="targetProjectRootNamespace">The root namespace of the project this namespace pertains to.</param>
        /// <returns>A <see cref="string"/></returns>
        string DetermineCodeFileNamespace(string typeNamespace, string targetProjectRootNamespace);

        /// <summary>
        /// Generates an end namespace statement in the specific language.
        /// </summary>
        /// <param name="namespace">The namespace to base the statement on.</param>
        /// <returns>A start namespace statement in the specific language.</returns>
        string GenerateEndNamespaceStatement(string @namespace);

        /// <summary>
        /// Generates a fluent NHibernate composite ID from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <returns>A <see cref="string"/>.</returns>
        string GenerateFluentNHibernateCompositeIdMappingCode(IMapping mapping, IMappedProperty mappedProperty);

        /// <summary>
        /// Generates a fluent NHibernate get named query interface method from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="importedNamespaces">A set of imported namespaces for the code file.</param>
        /// <returns>A <see cref="string"/>.</returns>
        string GenerateFluentNHibernateGetNamedQueryCode(IMapping mapping, IList<string> importedNamespaces);

        /// <summary>
        /// Generates a fluent NHibernate get named query interface method from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="importedNamespaces">A set of imported namespaces for the code file.</param>
        /// <returns>A <see cref="string"/>.</returns>
        string GenerateFluentNHibernateGetNamedQueryInterfaceCode(IMapping mapping, IList<string> importedNamespaces);

        /// <summary>
        /// Generates a unit test for a get named query method from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="importedNamespaces">A set of imported namespaces for the code file.</param>
        /// <returns>A <see cref="string"/>.</returns>
        string GenerateFluentNHibernateGetNamedQueryTestsCode(IMapping mapping, IList<string> importedNamespaces);

        /// <summary>
        /// Generates a fluent NHibernate key property mapping from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <returns>A <see cref="string"/>.</returns>
        string GenerateFluentNHibernateKeyPropertyMappingCode(IMapping mapping, IMappedProperty mappedProperty);

        /// <summary>
        /// Generates a fluent NHibernate primary key mapping from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <returns>A <see cref="string"/>.</returns>
        string GenerateFluentNHibernatePrimaryKeyMappingCode(IMapping mapping, IMappedProperty mappedProperty);

        /// <summary>
        /// Generates a fluent NHibernate mapping from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <returns>A <see cref="string"/>.</returns>
        string GenerateFluentNHibernatePropertyMappingCode(IMapping mapping, IMappedProperty mappedProperty);

        /// <summary>
        /// Generates a fluent NHibernate mapping integration test from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <param name="imports">A set of import / using namespaces.</param>
        /// <returns>A <see cref="string"/>.</returns>
        string GenerateFluentNHibernatePropertyMappingTestsCode(IMapping mapping, IMappedProperty mappedProperty, IList<string> imports);

        /// <summary>
        /// Generates a set of fluent NHibernate mappings from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <returns>A <see cref="string"/>.</returns>
        string GenerateFluentNHibernateTypeMappingCode(IMapping mapping);

        /// <summary>
        /// Generates an initialize property with a random value statement.
        /// </summary>
        /// <param name="mapping">Mapping information.</param>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <param name="imports">Some imported namespaces.</param>
        /// <returns>A <see cref="string"/> containing an initialize property statement.</returns>
        string GenerateInitialisePropertyStatement(IMapping mapping, IMappedProperty mappedProperty, IList<string> imports);

        /// <summary>
        /// Generates a line of code that forms part of an insert statement that contains the column name.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <param name="requiresComma">Indicates whether a comma is required in the line.</param>
        /// <returns>A <see cref="string"/> containing part of an insert statement.</returns>
        string GenerateInsertStatementColumnPortion(IMapping mapping, IMappedProperty mappedProperty, bool requiresComma = true);

        /// <summary>
        /// Generates a line of code that forms part of an insert statement that contains a datum.
        /// </summary>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <param name="requiresComma">Indicates whether a comma is required in the line.</param>
        /// <returns>A <see cref="string"/> containing part of an insert statement.</returns>
        string GenerateInsertStatementValuePortion(IMappedProperty mappedProperty, bool requiresComma = true);

        /// <summary>
        /// Generates a property string for a code file.
        /// </summary>
        /// <param name="mappedProperty">A property mapping.</param>
        /// <returns>A property declaration string.</returns>
        string GeneratePropertyDeclarationString(IMappedProperty mappedProperty);

        /// <summary>
        /// Generates a unit test method for a property in the specific language.
        /// </summary>
        /// <param name="mapping">The type mapping.</param>
        /// <param name="mappedProperty">The mapped property.</param>
        /// <param name="imports">A set of import / using namespaces.</param>
        /// <returns>A <see cref="string"/> containing a unit test.</returns>
        string GeneratePropertyUnitTestMethod(IMapping mapping, IMappedProperty mappedProperty, IList<string> imports);

        /// <summary>
        /// Generates service unit tests code in the specific language.
        /// </summary>
        /// <param name="mapping">A mapping to generate the code for.</param>
        /// <param name="imports">A set of import / using namespaces.</param>
        /// <returns>A <see cref="string"/> containing the code.</returns>
        string GenerateServiceUnitTests(IMapping mapping, IList<string> imports);

        /// <summary>
        /// Generates a start namespace statement in the specific language.
        /// </summary>
        /// <param name="namespace">The namespace to base the statement on.</param>
        /// <returns>A start namespace statement in the specific language.</returns>
        string GenerateStartNamespaceStatement(string @namespace);

        /// <summary>
        /// Generates a using / imports statement.
        /// </summary>
        /// <param name="namespaces">
        /// The set of namespaces to import / use.
        /// </param>
        /// <param name="scopeNamespace">
        /// When specified, the generated statement nests the namespaces within the parent namespace.
        /// </param>
        /// <returns>
        /// A using / imports statement as a <see cref="string"/>.
        /// </returns>
        string GenerateUsingStatements(IEnumerable<string> namespaces, string scopeNamespace = null);

        /// <summary>
        /// Determines whether a specified string is a keyword of a specific CLR language.
        /// If it is, a <see cref="string"/> containing the value, appropriately formatted, is returned.
        /// If it isn't, the original value is returned.
        /// </summary>
        /// <param name="value">The string to evaluate.</param>
        /// <returns>
        /// If the specified string is a keyword of a specific CLR language, a <see cref="string"/> containing the value, appropriately formatted, is returned.
        /// If it isn't, the original value is returned.
        /// </returns>
        string SanitiseKeyword(string value);

        /// <summary>
        /// Converts a database column name into an appropriate format 
        /// for a property name in the specific CLR language.
        /// </summary>
        /// <param name="columnName">The name of a database column to parse.</param>
        /// <returns>A <see cref="string"/></returns>
        string ToPropertyName(string columnName);
    }
}
