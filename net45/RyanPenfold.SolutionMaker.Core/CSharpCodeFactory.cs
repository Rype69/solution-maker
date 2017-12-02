// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSharpCodeFactory.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Utilities.Data.SqlClient;

    using Utilities;
    using Utilities.Collections.Generic;
    using Utilities.Text;

    /// <summary>
    /// Produces C# code
    /// </summary>
    public class CSharpCodeFactory : ICodeFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpCodeFactory"/> class.
        /// </summary>
        public CSharpCodeFactory()
        {
            this.Tab = "    ";
            this.CodeFileExtension = "cs";
            this.Keywords = System.IO.File.ReadAllLines($"{System.IO.Directory.GetCurrentDirectory()}\\Keywords.{this.CodeFileExtension}.txt");
            this.LineContinuationOperator = string.Empty;
            this.NullOperator = "null";
            this.ProjectFileExtension = "csproj";
            this.PropertyDeclarationDelimiter = $"\r\n{this.Tab}{this.Tab}";
            this.StatementDelimiter = ";";
        }

        /// <summary>
        /// Gets the file extension for code files written in the specific language.
        /// </summary>
        public string CodeFileExtension { get; }

        /// <summary>
        /// Gets the keywords of the C# language.
        /// </summary>
        public IEnumerable<string> Keywords { get; }

        /// <summary>
        /// Gets the line continuation operator for the C# language.
        /// </summary>
        public string LineContinuationOperator { get; }

        /// <summary>
        /// Gets the null operator for the C# language.
        /// </summary>
        public string NullOperator { get; }

        /// <summary>
        /// Gets the file extension for projects for code written in the C# language.
        /// </summary>
        public string ProjectFileExtension { get; }

        /// <summary>
        /// Gets the property declaration delimiter.
        /// </summary>
        public string PropertyDeclarationDelimiter { get; }

        /// <summary>
        /// Gets the statement delimiter specific to the language.
        /// </summary>
        public string StatementDelimiter { get; }

        /// <summary>
        /// Gets a tab in the specified language.
        /// </summary>
        public string Tab { get; }

        /// <summary>
        /// Attempts to find an alias for a type name in the C# language.
        /// </summary>
        /// <param name="type">The type to find an alias name for.</param>
        /// <returns>An alias type name.</returns>
        public virtual string AliasTypeName(System.Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var result = type.FullName;

            switch (type.FullName)
            {
                case "System.Boolean":
                    result = "bool";
                    break;
                case "System.Boolean[]":
                    result = "bool[]";
                    break;
                case "System.Byte":
                    result = "byte";
                    break;
                case "System.Byte[]":
                    result = "byte[]";
                    break;
                case "System.Char":
                    result = "char";
                    break;
                case "System.Char[]":
                    result = "char[]";
                    break;
                case "System.Decimal":
                    result = "decimal";
                    break;
                case "System.Decimal[]":
                    result = "decimal[]";
                    break;
                case "System.Double":
                    result = "double";
                    break;
                case "System.Double[]":
                    result = "double[]";
                    break;
                case "System.Int32":
                    result = "int";
                    break;
                case "System.Int32[]":
                    result = "int[]";
                    break;
                case "System.Int16":
                    result = "short";
                    break;
                case "System.Int16[]":
                    result = "short[]";
                    break;
                case "System.Int64":
                    result = "long";
                    break;
                case "System.Int64[]":
                    result = "long[]";
                    break;
                case "System.Object":
                    result = "object";
                    break;
                case "System.Object[]":
                    result = "object[]";
                    break;
                case "System.SByte":
                    result = "sbyte";
                    break;
                case "System.SByte[]":
                    result = "sbyte[]";
                    break;
                case "System.Single":
                    result = "float";
                    break;
                case "System.Single[]":
                    result = "float[]";
                    break;
                case "System.String":
                    result = "string";
                    break;
                case "System.String[]":
                    result = "string[]";
                    break;
                case "System.UInt16":
                    result = "ushort";
                    break;
                case "System.UInt16[]":
                    result = "ushort[]";
                    break;
                case "System.UInt32":
                    result = "uint";
                    break;
                case "System.UInt32[]":
                    result = "uint[]";
                    break;
                case "System.UInt64":
                    result = "ulong";
                    break;
                case "System.UInt64[]":
                    result = "ulong[]";
                    break;
            }

            return result;
        }

        /// <summary>
        /// Generates an "Assert.AreEqual" statement that compares the values of two of the same properties from two 
        /// different instances of the same type, and appends it to a <see cref="System.Text.StringBuilder"/> instance.
        /// </summary>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <param name="assertMethodString">A <see cref="string"/> containing a method containing a set of asserts.</param>
        /// <param name="importedNamespaces">A set of imported namespaces for the code file.</param>
        /// <returns>A <see cref="string"/> containing an "Assert.AreEqual" statement.</returns>
        public virtual void AppendAssertAreEqualStatement(IMappedProperty mappedProperty, System.Text.StringBuilder assertMethodString, IList<string> importedNamespaces)
        {
            if (mappedProperty == null)
            {
                throw new ArgumentNullException(nameof(mappedProperty));
            }

            if (assertMethodString == null)
            {
                throw new ArgumentNullException(nameof(assertMethodString));
            }

            if (importedNamespaces == null)
            {
                importedNamespaces = new List<string>();
            }

            if (assertMethodString.Length == 0)
            {
                assertMethodString.Append("\r\n");
            }

            var propertyAccessor = string.Empty;

            var mappedPropertyClrTypeIsNullable = mappedProperty.ClrType.IsNullable() ||
                (mappedProperty.ClrType.IsValueType && mappedProperty.IsNullable.HasValue && mappedProperty.IsNullable.Value);

            if (mappedPropertyClrTypeIsNullable)
            {
                if (!assertMethodString.ToString().EndsWith("}\r\n\r\n"))
                {
                    assertMethodString.Append("\r\n");
                }
            }

            assertMethodString.Append($"{this.Tab}{this.Tab}{this.Tab}");

            if (mappedProperty.SqlType.ToLower() == "timestamp")
            {
                assertMethodString.Append("/*");
            }

            if (mappedPropertyClrTypeIsNullable)
            {
                assertMethodString.AppendLine($"if (expected.{mappedProperty.PropertyName}.HasValue && actual.{mappedProperty.PropertyName}.HasValue)");
                assertMethodString.AppendLine($"{this.Tab}{this.Tab}{this.Tab}{{");
                assertMethodString.Append($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}");
                propertyAccessor = ".Value";
            }

            var underlyingType = Nullable.GetUnderlyingType(mappedProperty.ClrType);
            switch (underlyingType == null ? mappedProperty.ClrType.FullName : underlyingType.FullName)
            {
                case "System.Byte[]":
                    assertMethodString.Append($"Assert.AreEqual(BitConverter.ToString(expected.{mappedProperty.PropertyName}{propertyAccessor}), BitConverter.ToString(actual.{mappedProperty.PropertyName}{propertyAccessor})){this.StatementDelimiter}");
                    importedNamespaces.UniqueAdd("System");
                    break;
                case "System.DateTime":
                    assertMethodString.Append($"Assert.AreEqual(expected.{mappedProperty.PropertyName}{propertyAccessor}.ToString(\"dd/MM/yyyy HH:mm:ss\"), actual.{mappedProperty.PropertyName}{propertyAccessor}.ToString(\"dd/MM/yyyy HH:mm:ss\")){this.StatementDelimiter}");
                    break;
                case "System.DateTimeOffset":
                    assertMethodString.Append($"Assert.AreEqual(expected.{mappedProperty.PropertyName}{propertyAccessor}.ToString(\"dd/MM/yyyy HH:mm:ss %K\"), actual.{mappedProperty.PropertyName}{propertyAccessor}.ToString(\"dd/MM/yyyy HH:mm:ss %K\")){this.StatementDelimiter}");
                    break;
                case "System.Single":
                case "System.Double":
                    assertMethodString.Append($"Assert.AreEqual(expected.{mappedProperty.PropertyName}{propertyAccessor}.ToString(CultureInfo.InvariantCulture), actual.{mappedProperty.PropertyName}{propertyAccessor}.ToString(CultureInfo.InvariantCulture)){this.StatementDelimiter}");
                    importedNamespaces.UniqueAdd("System.Globalization");
                    break;
                default:
                    assertMethodString.Append($"Assert.AreEqual(expected.{mappedProperty.PropertyName}{propertyAccessor}, actual.{mappedProperty.PropertyName}{propertyAccessor}){this.StatementDelimiter}");
                    break;
            }

            if (mappedPropertyClrTypeIsNullable)
            {
                assertMethodString.Append($"\r\n{this.Tab}{this.Tab}{this.Tab}}}");
            }

            if (mappedProperty.SqlType.ToLower() == "timestamp")
            {
                assertMethodString.Append(" not able to test a timestamp property a specific value can't be inserted, nor can the column be updated. */");
            }

            assertMethodString.Append("\r\n");

            if (mappedPropertyClrTypeIsNullable)
            {
                assertMethodString.Append("\r\n");
            }
        }

        /// <summary>
        /// Determines the code file namespace for a given type namespace.
        /// </summary>
        /// <param name="typeNamespace">A namespace.</param>
        /// <param name="targetProjectRootNamespace">The root namespace of the project this namespace pertains to.</param>
        /// <returns>A <see cref="string"/></returns>
        public virtual string DetermineCodeFileNamespace(string typeNamespace, string targetProjectRootNamespace)
        {
            // NULL-check the typeNamespace parameter
            if (string.IsNullOrWhiteSpace(typeNamespace))
            {
                throw new ArgumentNullException(nameof(typeNamespace));
            }

            // NULL-check the targetProjectRootNamespace parameter
            if (string.IsNullOrWhiteSpace(targetProjectRootNamespace))
            {
                throw new ArgumentNullException(nameof(targetProjectRootNamespace));
            }

            // In the case of C#, the code file namespace is the same as the type namespace.
            return typeNamespace;
        }

        /// <summary>
        /// Generates an end namespace statement in the C# language.
        /// </summary>
        /// <param name="namespace">The namespace to base the statement on.</param>
        /// <returns>A start namespace statement in the C# language.</returns>
        public virtual string GenerateEndNamespaceStatement(string @namespace)
        {
            return "}";
        }

        /// <summary>
        /// Generates a fluent NHibernate composite ID from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public virtual string GenerateFluentNHibernateCompositeIdMappingCode(IMapping mapping, IMappedProperty mappedProperty)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            if (mappedProperty == null)
            {
                throw new ArgumentNullException(nameof(mappedProperty));
            }

            return $"\r\n{this.Tab}{this.Tab}{this.Tab}this.CompositeId()";
        }

        /// <summary>
        /// Generates a fluent NHibernate get named query interface method from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="importedNamespaces">A set of imported namespaces for the code file.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public virtual string GenerateFluentNHibernateGetNamedQueryCode(IMapping mapping, IList<string> importedNamespaces)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            if (mapping.Type != MappingType.DbStoredProcedure && mapping.Type != MappingType.DbTableValuedFunction)
            {
                throw new InvalidOperationException("Mapping type must be stored procedure or table valued function.");
            }

            if (mapping.SqlParameters == null)
            {
                throw new ArgumentNullException(nameof(mapping.SqlParameters));
            }

            if (importedNamespaces == null)
            {
                throw new ArgumentNullException(nameof(importedNamespaces));
            }

            // Add the necessary namespaces
            importedNamespaces.UniqueAdd("System.Collections.Generic");
            importedNamespaces.UniqueAdd(mapping.ProposedTypeNamespace);

            // Stringbuilder for the xmldoc
            var xmldocStringBuilder = new System.Text.StringBuilder();
            xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}/// <summary>");
            xmldocStringBuilder.Append($"{this.Tab}{this.Tab}/// Calls to the ");
            xmldocStringBuilder.Append(mapping.SourceObjectName);
            xmldocStringBuilder.AppendLine(" named query. ");
            xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}/// </summary>");

            // Result string builder
            var methodStringBuilder = new System.Text.StringBuilder();
            methodStringBuilder.Append($"{this.Tab}{this.Tab}public IEnumerable<");
            methodStringBuilder.Append(mapping.ProposedTypeName);
            methodStringBuilder.Append("> ");
            methodStringBuilder.Append(this.ToPropertyName(mapping.SourceObjectName));
            methodStringBuilder.Append("(");

            // Call parameter string builder
            var callParameterStringBuilder = new System.Text.StringBuilder();

            // Set parameter string builder
            var setParameterStringBuilder = new System.Text.StringBuilder();

            // Append the parameters
            var isFirst = true;
            foreach (var parameter in mapping.SqlParameters.Items)
            {
                var sqlParameterName = parameter.Name.Replace("@", string.Empty);

                if (sqlParameterName == "RETURN_VALUE" || sqlParameterName == "TABLE_RETURN_VALUE")
                {
                    continue;
                }

                var parameterClrType = parameter.Type.ToClrType();
                methodStringBuilder.AppendWithDelimiter(this.AliasTypeName(parameterClrType), isFirst ? string.Empty : ", ");

                if (parameterClrType.IsValueType)
                {
                    methodStringBuilder.Append("?");
                }

                var methodParameterName = this.SanitiseKeyword(sqlParameterName.ToLowerFirstLetter());
                methodStringBuilder.AppendWithDelimiter(methodParameterName);

                methodStringBuilder.Append(" = null");

                callParameterStringBuilder.AppendWithDelimiter($":{sqlParameterName}", ", ");
                setParameterStringBuilder.AppendLine($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}{this.Tab}.SetParameter(\"{sqlParameterName}\", {methodParameterName})");

                xmldocStringBuilder.Append($"{this.Tab}{this.Tab}/// <param name=\"");
                xmldocStringBuilder.Append(methodParameterName);
                xmldocStringBuilder.AppendLine("\">");
                xmldocStringBuilder.Append($"{this.Tab}{this.Tab}/// The ");
                xmldocStringBuilder.Append(methodParameterName.FromCamelCase().ToLower());
                xmldocStringBuilder.AppendLine(".");
                xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}/// </param>");

                isFirst = false;
            }

            xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}/// <returns>");
            xmldocStringBuilder.Append($"{this.Tab}{this.Tab}/// An <see cref=\"IEnumerable{{");
            xmldocStringBuilder.Append(mapping.ProposedTypeName);
            xmldocStringBuilder.AppendLine("}\" />.");
            xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}/// </returns>");
            methodStringBuilder.AppendLine(")");
            methodStringBuilder.AppendLine($"{this.Tab}{this.Tab}{{");
            methodStringBuilder.AppendLine($"{this.Tab}{this.Tab}{this.Tab}// Start a new session");
            methodStringBuilder.AppendLine($"{this.Tab}{this.Tab}{this.Tab}using (var session = this.SessionFactory.GetNewSession())");
            methodStringBuilder.AppendLine($"{this.Tab}{this.Tab}{this.Tab}{{");
            methodStringBuilder.Append($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}return session.CreateSQLQuery(\"");
            methodStringBuilder.Append(mapping.Type == MappingType.DbStoredProcedure ? "EXEC [" : "SELECT * FROM [");
            methodStringBuilder.Append(mapping.SourceObjectSchema);
            methodStringBuilder.Append("].[");
            methodStringBuilder.Append(mapping.SourceObjectName);
            methodStringBuilder.Append("] ");

            if (mapping.Type == MappingType.DbTableValuedFunction)
            {
                methodStringBuilder.Append("(");
            }

            methodStringBuilder.Append(callParameterStringBuilder);

            if (mapping.Type == MappingType.DbTableValuedFunction)
            {
                methodStringBuilder.Append(")");
            }

            methodStringBuilder.AppendLine("\")");
            methodStringBuilder.Append($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}{this.Tab}.AddEntity(typeof(");
            methodStringBuilder.Append(mapping.ProposedTypeName);
            methodStringBuilder.AppendLine("))");
            methodStringBuilder.Append(setParameterStringBuilder);
            methodStringBuilder.Append($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}{this.Tab}.List<");
            methodStringBuilder.Append(mapping.ProposedTypeName);
            methodStringBuilder.AppendLine(">();");
            methodStringBuilder.AppendLine($"{this.Tab}{this.Tab}{this.Tab}}}");
            methodStringBuilder.Append($"{this.Tab}{this.Tab}}}");

            // Return result
            return $"{xmldocStringBuilder}{methodStringBuilder}";
        }

        /// <summary>
        /// Generates a fluent NHibernate get named query interface method from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="importedNamespaces">A set of imported namespaces for the code file.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public virtual string GenerateFluentNHibernateGetNamedQueryInterfaceCode(IMapping mapping, IList<string> importedNamespaces)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            if (mapping.Type != MappingType.DbStoredProcedure && mapping.Type != MappingType.DbTableValuedFunction)
            {
                throw new InvalidOperationException("Mapping type must be stored procedure or table valued function.");
            }

            if (mapping.SqlParameters == null)
            {
                throw new ArgumentNullException(nameof(mapping.SqlParameters));
            }

            if (importedNamespaces == null)
            {
                throw new ArgumentNullException(nameof(importedNamespaces));
            }

            // Add the necessary namespaces
            importedNamespaces.UniqueAdd("System.Collections.Generic");
            importedNamespaces.UniqueAdd(mapping.ProposedTypeNamespace);

            // Stringbuilder for the xmldoc
            var xmldocStringBuilder = new System.Text.StringBuilder();
            xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}/// <summary>");
            xmldocStringBuilder.Append($"{this.Tab}{this.Tab}/// Calls to the ");
            xmldocStringBuilder.Append(mapping.SourceObjectName);
            xmldocStringBuilder.AppendLine(" named query. ");
            xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}/// </summary>");

            // Result string builder
            var methodStringBuilder = new System.Text.StringBuilder();
            methodStringBuilder.Append($"{this.Tab}{this.Tab}IEnumerable<");
            methodStringBuilder.Append(mapping.ProposedTypeName);
            methodStringBuilder.Append("> ");
            methodStringBuilder.Append(this.ToPropertyName(mapping.SourceObjectName));
            methodStringBuilder.Append("(");

            // Append the parameters
            var isFirst = true;
            foreach (var parameter in mapping.SqlParameters.Items)
            {
                var parameterName = parameter.Name.Replace("@", string.Empty);

                if (parameterName == "RETURN_VALUE" || parameterName == "TABLE_RETURN_VALUE")
                {
                    continue;
                }

                var parameterClrType = parameter.Type.ToClrType();
                methodStringBuilder.AppendWithDelimiter(this.AliasTypeName(parameterClrType), isFirst ? string.Empty : ", ");

                if (parameterClrType.IsValueType)
                {
                    methodStringBuilder.Append("?");
                }

                parameterName = this.SanitiseKeyword(parameterName.ToLowerFirstLetter());
                methodStringBuilder.AppendWithDelimiter(parameterName);

                methodStringBuilder.Append(" = null");

                xmldocStringBuilder.Append($"{this.Tab}{this.Tab}/// <param name=\"");
                xmldocStringBuilder.Append(parameterName);
                xmldocStringBuilder.AppendLine("\">");
                xmldocStringBuilder.Append($"{this.Tab}{this.Tab}/// The ");
                xmldocStringBuilder.Append(parameterName.FromCamelCase().ToLower());
                xmldocStringBuilder.AppendLine(".");
                xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}/// </param>");

                isFirst = false;
            }

            xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}/// <returns>");
            xmldocStringBuilder.Append($"{this.Tab}{this.Tab}/// An <see cref=\"IEnumerable{{");
            xmldocStringBuilder.Append(mapping.ProposedTypeName);
            xmldocStringBuilder.AppendLine("}\" />.");
            xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}/// </returns>");
            methodStringBuilder.Append(");");

            // Return result
            return $"{xmldocStringBuilder}{methodStringBuilder}";
        }

        /// <summary>
        /// Generates a unit test for a get named query method from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="importedNamespaces">A set of imported namespaces for the code file.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public string GenerateFluentNHibernateGetNamedQueryTestsCode(IMapping mapping, IList<string> importedNamespaces)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            if (importedNamespaces == null)
            {
                importedNamespaces = new List<string>();
            }

            var result = new System.Text.StringBuilder();

            result.AppendWithDelimiter($"\r\n{this.Tab}{this.Tab}/// <summary>", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}/// Tests the <see cref=\"{{className}}Repository.{this.SanitiseKeyword(mapping.SourceObjectName)}\" /> method.", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}/// </summary>", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}[TestMethod]", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}public void {this.SanitiseKeyword(mapping.SourceObjectName)}_TestConditions_ExpectedResult()", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{{", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}// TODO: Implement", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}throw new NotImplementedException(){this.StatementDelimiter}", "\r\n");
            importedNamespaces.UniqueAdd("System");
            result.AppendLine();
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}/*try", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}{{", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}// Arrange", "\r\n");
            result.AppendLine();
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}// Drop the \"{{className}}\" table if it exists already", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(this.GetType().Assembly, $\"{{this.GetType().Assembly.GetName().Name}}.SQL.DROP TABLE [{{sourceObjectSchema}}].[{{className}}].sql\", ConnectionStringName){this.StatementDelimiter}", "\r\n");
            result.AppendLine();
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}// Create the \"{{className}}\" table", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(this.GetType().Assembly, $\"{{this.GetType().Assembly.GetName().Name}}.SQL.CREATE TABLE [{{sourceObjectSchema}}].[{{className}}].sql\", ConnectionStringName){this.StatementDelimiter}", "\r\n");
            result.AppendLine();
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}// Create new repository", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}var repository = new {{className}}Repository(){this.StatementDelimiter}", "\r\n");

            // Append the parameters
            var isFirst = true;
            var parametersBuilder = new System.Text.StringBuilder();
            foreach (var parameter in mapping.SqlParameters.Items)
            {
                var sqlParameterName = parameter.Name.Replace("@", string.Empty);

                if (sqlParameterName == "RETURN_VALUE" || sqlParameterName == "TABLE_RETURN_VALUE")
                {
                    continue;
                }

                if (isFirst)
                {
                    result.AppendLine();
                    isFirst = false;
                }

                var variableName = this.SanitiseKeyword(sqlParameterName.ToLowerFirstLetter());
                var variableDeclaration = string.Empty;

                var defaultLengthForBytes =
                    (long)(typeof(Utilities.Random).GetMethod("NextBytes", new[] { typeof(int) })
                        .GetParameters()
                        .First(p => p.Name == "length").RawDefaultValue);

                var defaultLengthForString =
                    (int)(typeof(Utilities.Random).GetMethod("NextString", new[] { typeof(int) })
                        .GetParameters()
                        .First(p => p.Name == "length").RawDefaultValue);

                result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}", "\r\n");

                switch (parameter.Type.ToString().ToLower())
                {
                    case "bigint":
                        result.Append($"var {variableName} = Utilities.Random.NextInt64(){this.StatementDelimiter}");
                        break;
                    case "binary":
                        result.Append(parameter.Size == defaultLengthForBytes ? $"var {variableName} = Utilities.Random.NextBytes(){this.StatementDelimiter}" : $"var {variableName} = Utilities.Random.NextBytes({parameter.Size}){this.StatementDelimiter}");
                        break;
                    case "bit":
                        result.Append($"var {variableName} = Utilities.Random.NextBoolean(){this.StatementDelimiter}");
                        break;
                    case "char":
                    case "nchar":
                        result.Append(parameter.Size == defaultLengthForString ? $"var {variableName} = Utilities.Random.NextString().Replace(\"'\", \";\"){this.StatementDelimiter}" : $"var {variableName} = Utilities.Random.NextString({parameter.Size}).Replace(\"'\", \";\"){this.StatementDelimiter}");
                        break;
                    case "date":
                        result.Append($"var {variableName} = Utilities.Random.NextDateTime(false){this.StatementDelimiter}");
                        break;
                    case "datetime":
                        result.Append($"var {variableName} = Utilities.Random.NextDateTime(new DateTime(1753, 1, 1), DateTime.MaxValue){this.StatementDelimiter}");
                        importedNamespaces.UniqueAdd("System");
                        break;
                    case "datetime2":
                        result.Append($"var {variableName} = Utilities.Random.NextDateTime(){this.StatementDelimiter}");
                        break;
                    case "datetimeoffset":
                        result.Append($"var {variableName} = Utilities.Random.NextDateTimeOffset(){this.StatementDelimiter}");
                        break;
                    case "decimal":
                        result.Append($"var {variableName} = Utilities.Random.NextInt16(-1, 1){this.StatementDelimiter}");
                        break;
                    case "float":
                        result.Append($"var {variableName} = Utilities.Random.NextDouble(){this.StatementDelimiter}");
                        break;
                    case "image":
                        result.Append(16 == defaultLengthForBytes ? $"var {variableName} = Utilities.Random.NextBytes(){this.StatementDelimiter}" : $"var {variableName} = Utilities.Random.NextBytes(16){this.StatementDelimiter}");
                        break;
                    case "int":
                        result.Append($"var {variableName} = Utilities.Random.NextInt32(){this.StatementDelimiter}");
                        break;
                    case "money":
                        result.Append($"var {variableName} = (decimal)Utilities.Random.NextDouble(-922337203685477.62, 922337203685477.62){this.StatementDelimiter}");
                        break;
                    case "ntext":
                    case "text":
                    case "nvarchar":
                    case "varchar":
                    case "variant":
                        result.Append($"var {variableName} = Utilities.Random.NextString(1).Replace(\"'\", \";\"){this.StatementDelimiter}");
                        break;
                    case "real":
                        result.Append($"var {variableName} = Utilities.Random.NextSingle(-0.9F, 0.9F){this.StatementDelimiter}");
                        break;
                    case "smalldatetime":
                        result.Append($"var {variableName} = Utilities.Random.NextDateTime(new DateTime(1900, 1, 1), new DateTime(2079, 6, 6), false){this.StatementDelimiter}");
                        importedNamespaces.UniqueAdd("System");
                        break;
                    case "smallint":
                        result.Append($"var {variableName} = Utilities.Random.NextInt16(){this.StatementDelimiter}");
                        break;
                    case "smallmoney":
                        result.Append($"var {variableName} = (decimal)Utilities.Random.NextSingle(-214748.359F, 214748.359F){this.StatementDelimiter}");
                        break;
                    case "time":
                        result.Append($"var {variableName} = Utilities.Random.NextTimeSpan(){this.StatementDelimiter}");
                        break;
                    case "timestamp":
                        result.Append(8 == defaultLengthForBytes ? $"var {variableName} = Utilities.Random.NextBytes(){this.StatementDelimiter}" : $"var {variableName} = Utilities.Random.NextBytes(8){this.StatementDelimiter}");
                        return string.Empty;
                    case "tinyint":
                        result.Append($"var {variableName} = Utilities.Random.NextByte(){this.StatementDelimiter}");
                        break;
                    case "udt":
                        result.Append($"object {variableName} = {this.NullOperator}{this.StatementDelimiter}");
                        break;
                    case "uniqueidentifier":
                        result.Append($"var {variableName} = Guid.NewGuid(){this.StatementDelimiter}");
                        importedNamespaces.UniqueAdd("System");
                        break;
                    case "varbinary":
                        result.Append($"var {variableName} = Utilities.Random.NextBytes(1){this.StatementDelimiter}");
                        break;
                    case "xml":
                        result.Append($"var {variableName} = new SqlXml(XmlReader.Create(\"<{mapping.ProposedTypeName} />\")){this.StatementDelimiter}");
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown sql data type \"{parameter.Type}\"");
                }

                result.Append(variableDeclaration);
                parametersBuilder.AppendWithDelimiter(variableName, ", ");
            }

            result.AppendLine();
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}// Act", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}var result = repository.{this.SanitiseKeyword(mapping.SourceObjectName)}(", "\r\n");
            result.Append(parametersBuilder);
            result.AppendLine($"){this.StatementDelimiter}");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}// Assert", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}Assert.AreEqual(1, result.Count()){this.StatementDelimiter}", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}}}", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}catch", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}{{", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}// Drop the \"{{className}}\" table if it still exists", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(this.GetType().Assembly, $\"{{this.GetType().Assembly.GetName().Name}}.SQL.DROP TABLE [{{sourceObjectSchema}}].[{{className}}].sql\", ConnectionStringName){this.StatementDelimiter}", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}}}*/", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}}}", "\r\n");

            return result.ToString();
        }

        /// <summary>
        /// Generates a fluent NHibernate key property mapping from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public virtual string GenerateFluentNHibernateKeyPropertyMappingCode(IMapping mapping, IMappedProperty mappedProperty)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            if (mappedProperty == null)
            {
                throw new ArgumentNullException(nameof(mappedProperty));
            }

            var builder = new System.Text.StringBuilder();

            builder.Append($"\r\n{this.Tab}{this.Tab}{this.Tab}{this.Tab}");
            builder.Append(".KeyProperty(x => x.");
            builder.Append(mappedProperty.PropertyName);
            builder.Append(")");
            builder.Append(this.GenerateFluentNHibernatePropertyMappingCustomTypes(mappedProperty));
            builder.Append(this.StatementDelimiter);

            return builder.ToString();
        }

        /// <summary>
        /// Generates a fluent NHibernate primary key mapping from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public virtual string GenerateFluentNHibernatePrimaryKeyMappingCode(IMapping mapping, IMappedProperty mappedProperty)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            if (mappedProperty == null)
            {
                throw new ArgumentNullException(nameof(mappedProperty));
            }

            var builder = new System.Text.StringBuilder();

            builder.Append($"\r\n{this.Tab}{this.Tab}{this.Tab}");
            builder.Append("this.Id(x => x.");
            builder.Append(mappedProperty.PropertyName);
            builder.Append(").GeneratedBy.Assigned()");
            builder.Append(this.GenerateFluentNHibernatePropertyMappingCustomTypes(mappedProperty));
            builder.Append(this.StatementDelimiter);

            builder.Append($"\r\n{this.Tab}{this.Tab}{this.Tab}");
            builder.Append("this.PrimaryKeyColumnNames.UniqueAdd(\"");
            builder.Append(mappedProperty.PropertyName);
            builder.Append("\")");
            builder.Append(this.StatementDelimiter);

            return builder.ToString();
        }

        /// <summary>
        /// Generates a fluent NHibernate mapping from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public virtual string GenerateFluentNHibernatePropertyMappingCode(IMapping mapping, IMappedProperty mappedProperty)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            if (mappedProperty == null)
            {
                throw new ArgumentNullException(nameof(mappedProperty));
            }

            var builder = new System.Text.StringBuilder();

            builder.Append($"{this.Tab}{this.Tab}{this.Tab}");
            builder.Append("this.Map(x => x.");
            builder.Append(mappedProperty.PropertyName);
            builder.Append(")");
            builder.Append(this.GenerateFluentNHibernatePropertyMappingCustomTypes(mappedProperty));
            builder.Append(this.StatementDelimiter);

            return builder.ToString();
        }

        /// <summary>
        /// Generates a fluent NHibernate key property mapping from some mapping settings.
        /// </summary>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <returns>A <see cref="string"/>.</returns>
        protected virtual string GenerateFluentNHibernatePropertyMappingCustomTypes(IMappedProperty mappedProperty)
        {
            if (mappedProperty == null)
            {
                throw new ArgumentNullException(nameof(mappedProperty));
            }

            var builder = new System.Text.StringBuilder();

            if (string.Equals(mappedProperty.SqlType, "Date", StringComparison.InvariantCultureIgnoreCase))
            {
                builder.Append(".CustomType(\"date\")");
            }

            if (string.Equals(mappedProperty.SqlType, "DateTime2", StringComparison.InvariantCultureIgnoreCase))
            {
                builder.Append(".CustomType(\"datetime2\")");
            }

            var underlyingType = Nullable.GetUnderlyingType(mappedProperty.ClrType);
            if (mappedProperty.ClrType.FullName == "System.TimeSpan" ||
               (underlyingType != null && underlyingType.FullName == "System.TimeSpan"))
            {
                builder.Append(".CustomType(\"TimeAsTimeSpan\")");
            }

            if (string.Equals(mappedProperty.SqlType, "timestamp", StringComparison.InvariantCultureIgnoreCase))
            {
                builder.Append(".ReadOnly()");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Generates a fluent NHibernate mapping integration test from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <param name="imports">A set of import / using namespaces.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public virtual string GenerateFluentNHibernatePropertyMappingTestsCode(IMapping mapping, IMappedProperty mappedProperty, IList<string> imports)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            if (mappedProperty == null)
            {
                throw new ArgumentNullException(nameof(mappedProperty));
            }

            if (imports == null)
            {
                imports = new List<string>();
            }

            var result = new System.Text.StringBuilder();

            string testValue;

            var defaultLengthForBytes =
                (long)(typeof(Utilities.Random).GetMethod("NextBytes", new[] { typeof(int) })
                    .GetParameters()
                    .First(p => p.Name == "length").RawDefaultValue);

            var defaultLengthForString =
                (int)(typeof(Utilities.Random).GetMethod("NextString", new[] { typeof(int) })
                    .GetParameters()
                    .First(p => p.Name == "length").RawDefaultValue);

            switch (mappedProperty.SqlType.ToLower())
            {
                case "bigint":
                    testValue = "Utilities.Random.NextInt64()";
                    break;
                case "binary":
                    testValue = mappedProperty.Length == defaultLengthForBytes ? "Utilities.Random.NextBytes()" : $"Utilities.Random.NextBytes({mappedProperty.Length})";
                    break;
                case "bit":
                    testValue = "Utilities.Random.NextBoolean()";
                    break;
                case "char":
                case "nchar":
                    testValue = mappedProperty.Length == defaultLengthForString ? "Utilities.Random.NextString().Replace(\"'\", \";\")" : $"Utilities.Random.NextString({mappedProperty.Length}).Replace(\"'\", \";\")";
                    break;
                case "date":
                    testValue = this.NullOperator;
                    break;
                case "datetime":
                    imports.UniqueAdd("RyanPenfold.Utilities.Collections");
                    testValue = "Utilities.Random.NextDateTime(new DateTime(1753, 1, 1), DateTime.MaxValue), new DateTimeEqualityComparer(new TimeSpan(0, 0, 1, 0))";
                    break;
                case "datetime2":
                    testValue = "Utilities.Random.NextDateTime()";
                    break;
                case "datetimeoffset":
                    testValue = "Utilities.Random.NextDateTimeOffset()";
                    break;
                case "decimal":
                case "numeric":
                    testValue = "(decimal)Utilities.Random.NextInt16(-1, 1)";
                    break;
                case "float":
                    testValue = "Utilities.Random.NextDouble()";
                    break;
                case "geography":
                case "test.sys.geography":
                case "geometry":
                case "test.sys.geometry":
                case "hierarchyid":
                case "test.sys.hierarchyid":
                    testValue = this.NullOperator;
                    break;
                case "image":
                    testValue = 16 == defaultLengthForBytes ? "Utilities.Random.NextBytes()" : "Utilities.Random.NextBytes(16)";
                    break;
                case "int":
                    testValue = "Utilities.Random.NextInt32()";
                    break;
                case "money":
                    testValue = "(decimal)Utilities.Random.NextDouble(-922337203685477.5808, 922337203685477.62)";
                    break;
                case "ntext":
                case "nvarchar":
                case "sql_variant":
                case "text":
                case "varchar":
                    testValue = "Utilities.Random.NextString(1)";
                    break;
                case "real":
                    testValue = "Utilities.Random.NextSingle(-0.9F, 0.9F)";
                    break;
                case "smalldatetime":
                    imports.UniqueAdd("RyanPenfold.Utilities.Collections");
                    testValue = "Utilities.Random.NextDateTime(new DateTime(1900, 1, 1), new DateTime(2079, 6, 6)), new DateTimeEqualityComparer(new TimeSpan(0, 0, 1, 0))";
                    break;
                case "smallint":
                    testValue = "Utilities.Random.NextInt16()";
                    break;
                case "smallmoney":
                    testValue = "(decimal)Utilities.Random.NextInt16(-1, 1)";
                    break;
                case "time":
                    imports.UniqueAdd("RyanPenfold.Utilities.Collections");
                    testValue = "Utilities.Random.NextTimeSpan(), new TimeSpanEqualityComparer(new TimeSpan(0, 0, 1, 0))";
                    break;
                case "timestamp":
                    // not able to test a timestamp property since a specific value can't be inserted, nor can the column be updated.
                    // testValue = 8 == defaultLengthForBytes ? "Utilities.Random.NextBytes()" : "Utilities.Random.NextBytes(8)";
                    return string.Empty;
                case "tinyint":
                    testValue = "Utilities.Random.NextByte()";
                    break;
                case "uniqueidentifier":
                    testValue = "Guid.NewGuid()";
                    imports.UniqueAdd("System");
                    break;
                case "varbinary":
                    testValue = "Utilities.Random.NextBytes(1)";
                    break;
                case "xml":
                    testValue = $"\"<{mapping.ProposedTypeName} />\"";
                    break;
                default:
                    throw new InvalidOperationException($"Unknown sql data type \"{mappedProperty.SqlType}\"");
            }

            for (var tabCount = 0; tabCount < 7; tabCount++)
            {
                result.Append(this.Tab);
            }

            result.Append($".CheckProperty(x => x.{mappedProperty.PropertyName}, {testValue})");

            return result.ToString();
        }

        /// <summary>
        /// Generates a set of fluent NHibernate mappings from some mapping settings.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public virtual string GenerateFluentNHibernateTypeMappingCode(IMapping mapping)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            var builder = new System.Text.StringBuilder();

            builder.Append($"\r\n{this.Tab}{this.Tab}{this.Tab}");
            builder.Append($"this.TableName = \"{mapping.SourceObjectName}\";");
            builder.Append($"\r\n{this.Tab}{this.Tab}{this.Tab}");
            builder.Append($"this.SchemaName = \"{mapping.SourceObjectSchema}\";");
            builder.Append($"\r\n{this.Tab}{this.Tab}{this.Tab}");
            builder.Append("this.Table(this.TableName);");
            builder.Append($"\r\n{this.Tab}{this.Tab}{this.Tab}");
            builder.Append("this.Schema(this.SchemaName);");
            builder.Append($"\r\n{this.Tab}{this.Tab}{this.Tab}");
            builder.Append($"this.EnsureTableExists(typeof({mapping.ProposedTypeName}), this.SchemaName, this.TableName);");
            
            return builder.ToString();
        }

        /// <summary>
        /// Generates an initialize property with a random value statement.
        /// </summary>
        /// <param name="mapping">Mapping information.</param>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <param name="imports">Some imported namespaces.</param>
        /// <returns>A <see cref="string"/> containing an initialize property statement.</returns>
        public virtual string GenerateInitialisePropertyStatement(IMapping mapping, IMappedProperty mappedProperty, IList<string> imports)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            if (mappedProperty == null)
            {
                throw new ArgumentNullException(nameof(mappedProperty));
            }

            var defaultLengthForBytes =
                (long)(typeof(Utilities.Random).GetMethod("NextBytes", new[] { typeof(int) })
                    .GetParameters()
                    .First(p => p.Name == "length").RawDefaultValue);

            var defaultLengthForString =
                (int)(typeof(Utilities.Random).GetMethod("NextString", new[] { typeof(int) })
                    .GetParameters()
                    .First(p => p.Name == "length").RawDefaultValue);

            string value;

            switch (mappedProperty.SqlType.ToLower())
            {
                case "bigint":
                    value = "Utilities.Random.NextInt64()";
                    break;
                case "binary":
                    value = mappedProperty.Length == defaultLengthForBytes ? "Utilities.Random.NextBytes()" : $"Utilities.Random.NextBytes({mappedProperty.Length})";
                    break;
                case "bit":
                    value = "Utilities.Random.NextBoolean()";
                    break;
                case "char":
                case "nchar":
                    value = mappedProperty.Length == defaultLengthForString ? "Utilities.Random.NextString().Replace(\"'\", \";\")" : $"Utilities.Random.NextString({mappedProperty.Length}).Replace(\"'\", \";\")";
                    break;
                case "date":
                    value = "Utilities.Random.NextDateTime(false)";
                    break;
                case "datetime":
                    value = "Utilities.Random.NextDateTime(new DateTime(1753, 1, 1), DateTime.MaxValue)";
                    imports.UniqueAdd("System");
                    break;
                case "datetime2":
                    value = "Utilities.Random.NextDateTime()";
                    break;
                case "datetimeoffset":
                    value = "Utilities.Random.NextDateTimeOffset()";
                    break;
                case "decimal":
                    value = "Utilities.Random.NextInt16(-1, 1)";
                    break;
                case "float":
                    value = "Utilities.Random.NextDouble()";
                    break;
                case "geography":
                case "test.sys.geography":
                case "geometry":
                case "test.sys.geometry":
                case "hierarchyid":
                case "test.sys.hierarchyid":
                    value = this.NullOperator;
                    break;
                case "image":
                    value = 16 == defaultLengthForBytes ? "Utilities.Random.NextBytes()" : "Utilities.Random.NextBytes(16)";
                    break;
                case "int":
                    value = "Utilities.Random.NextInt32()";
                    break;
                case "money":
                    value = "(decimal)Utilities.Random.NextDouble(-922337203685477.62, 922337203685477.62)";
                    break;
                case "ntext":
                case "sql_variant":
                case "text":
                case "nvarchar":
                case "varchar":
                    value = "Utilities.Random.NextString(1).Replace(\"'\", \";\")";
                    break;
                case "numeric":
                    value = "Utilities.Random.NextInt16(-1, 1)";
                    break;
                case "real":
                    value = "Utilities.Random.NextSingle(-0.9F, 0.9F)";
                    break;
                case "smalldatetime":
                    value = "Utilities.Random.NextDateTime(new DateTime(1900, 1, 1), new DateTime(2079, 6, 6), false)";
                    imports.UniqueAdd("System");
                    break;
                case "smallint":
                    value = "Utilities.Random.NextInt16()";
                    break;
                case "smallmoney":
                    value = "(decimal)Utilities.Random.NextSingle(-214748.359F, 214748.359F)";
                    break;
                case "time":
                    value = "Utilities.Random.NextTimeSpan()";
                    break;
                case "timestamp":
                    // not able to test a timestamp property since a specific value can't be inserted, nor can the column be updated.
                    // testValue = 8 == defaultLengthForBytes ? "Utilities.Random.NextBytes()" : "Utilities.Random.NextBytes(8)";
                    return string.Empty;
                case "tinyint":
                    value = "Utilities.Random.NextByte()";
                    break;
                case "uniqueidentifier":
                    value = "Guid.NewGuid()";
                    imports.UniqueAdd("System");
                    break;
                case "varbinary":
                    value = "Utilities.Random.NextBytes(1)";
                    break;
                case "xml":
                    value = $"\"<{mapping.ProposedTypeName} />\"";
                    break;
                default:
                    throw new InvalidOperationException($"Unknown sql data type \"{mappedProperty.SqlType}\"");
            }

            return $"{this.Tab}{this.Tab}{this.Tab}instance.{mappedProperty.PropertyName} = {value}{this.StatementDelimiter}";
        }

        /// <summary>
        /// Generates a line of code that forms part of an insert statement that contains the column name.
        /// </summary>
        /// <param name="mapping">Some mapping settings.</param>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <param name="requiresComma">Indicates whether a comma is required in the line.</param>
        /// <returns>A <see cref="string"/> containing part of an insert statement.</returns>
        public virtual string GenerateInsertStatementColumnPortion(IMapping mapping, IMappedProperty mappedProperty, bool requiresComma = true)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            if (mappedProperty == null)
            {
                throw new ArgumentNullException(nameof(mappedProperty));
            }

            var comma = requiresComma ? "," : "";
            return $"{this.Tab}{this.Tab}{this.Tab}sqlcommandBuilder.AppendWithDelimiter(\"   {comma}[{mapping.SourceObjectSchema}].[{mapping.SourceObjectName}].[{mappedProperty.ColumnName}]\", \"\\r\\n\");";
        }

        /// <summary>
        /// Generates a line of code that forms part of an insert statement that contains a datum.
        /// </summary>
        /// <param name="mappedProperty">A property mapping settings.</param>
        /// <param name="requiresComma">Indicates whether a comma is required in the line.</param>
        /// <returns>A <see cref="string"/> containing part of an insert statement.</returns>
        public virtual string GenerateInsertStatementValuePortion(IMappedProperty mappedProperty, bool requiresComma = true)
        {
            if (mappedProperty == null)
            {
                throw new ArgumentNullException(nameof(mappedProperty));
            }

            var comma = requiresComma ? "," : " ";
            var valueBuilder = new System.Text.StringBuilder();
            var propertyAccessor = string.Empty;
            var mappedPropertyClrTypeIsNullable = mappedProperty.ClrType.IsNullable() ||
               (mappedProperty.ClrType.IsValueType && mappedProperty.IsNullable.HasValue && mappedProperty.IsNullable.Value);

            if (mappedPropertyClrTypeIsNullable)
            {
                propertyAccessor = ".Value";
                valueBuilder.Append($"entity.{mappedProperty.PropertyName}.HasValue ? ");
            }

            switch (mappedProperty.SqlType.ToLower())
            {
                case "bigint":
                case "decimal":
                case "float":
                case "int":
                case "money":
                case "numeric":
                case "real":
                case "smallint":
                case "smallmoney":
                case "tinyint":
                    valueBuilder.Append($"$\"{comma}{{entity.{mappedProperty.PropertyName}{propertyAccessor}}}\"");
                    break;
                case "binary":
                case "image":
                case "varbinary":
                    valueBuilder.Append($"$\"{comma}0x{{string.Concat(entity.{mappedProperty.PropertyName}{propertyAccessor}.Select(b => b.ToString(\"X2\")).ToArray())}}\"");
                    break;
                case "bit":
                    valueBuilder.Append($"$\"{comma}{{Utilities.Boolean.ToByte(entity.{mappedProperty.PropertyName}{propertyAccessor})}}\"");
                    break;
                case "date":
                    valueBuilder.Append($"$\"{comma}'{{entity.{mappedProperty.PropertyName}{propertyAccessor}.ToString(\"yyyy-MM-dd\")}}'\"");
                    break;
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    valueBuilder.Append($"$\"{comma}'{{entity.{mappedProperty.PropertyName}{propertyAccessor}.ToString(\"yyyy-MM-dd HH:mm:ss\")}}'\"");
                    break;
                case "datetimeoffset":
                    valueBuilder.Append($"$\"{comma}'{{entity.{mappedProperty.PropertyName}{propertyAccessor}.ToString(\"yyyy-MM-dd HH:mm:ss %K\")}}'\"");
                    break;
                case "geography":
                case "test.sys.geography":
                case "geometry":
                case "test.sys.geometry":
                case "hierarchyid":
                case "test.sys.hierarchyid":
                    valueBuilder.Append($"\"{comma}NULL\"");
                    break;
                case "char":
                case "nchar":
                case "ntext":
                case "nvarchar":
                case "sql_variant":
                case "text":
                case "time":
                case "uniqueidentifier":
                case "varchar":
                case "xml":
                    valueBuilder.Append($"$\"{comma}'{{entity.{mappedProperty.PropertyName}{propertyAccessor}}}'\"");
                    break;
                case "timestamp":
                    valueBuilder.Append($"\"{comma}DEFAULT\"");
                    break;
                default:
                    throw new InvalidOperationException($"Unknown sql data type \"{mappedProperty.SqlType}\"");
            }

            if (mappedPropertyClrTypeIsNullable)
            {
                if (valueBuilder.ToString().EndsWith($"? \"{comma}NULL\""))
                {
                    valueBuilder = new System.Text.StringBuilder($"\"{comma}NULL\"");
                }
                else
                {
                    valueBuilder.Append($" : \"{comma}NULL\"");
                }
            }

            return $"{this.Tab}{this.Tab}{this.Tab}sqlcommandBuilder.AppendWithDelimiter({this.Tab}{valueBuilder}, \"\\r\\n\");";
        }

        /// <summary>
        /// Generates a property string for a code file.
        /// </summary>
        /// <param name="mappedProperty">A property mapping.</param>
        /// <returns>A property declaration string.</returns>
        public virtual string GeneratePropertyDeclarationString(IMappedProperty mappedProperty)
        {
            // NULL-check the parameter
            if (mappedProperty == null)
            {
                throw new ArgumentNullException(nameof(mappedProperty));
            }

            var documentationComment = mappedProperty.ClrType.FullName == "System.Boolean"
                ? $"Gets or sets a value indicating whether {mappedProperty.ColumnName.FromCamelCase().ToLower()}."
                : $"Gets or sets the {mappedProperty.ColumnName.FromCamelCase().ToLower()}.";

            var aliasedTypeName = this.AliasTypeName(mappedProperty.ClrType);

            // HACK: Use string instead of object
            if (aliasedTypeName == "object")
            {
                aliasedTypeName = "string";
            }

            var mappedPropertyClrTypeIsNullable = mappedProperty.ClrType.IsNullable() ||
                (mappedProperty.ClrType.IsValueType && mappedProperty.IsNullable.HasValue && mappedProperty.IsNullable.Value);

            if (mappedPropertyClrTypeIsNullable)
            {
                aliasedTypeName = $"{aliasedTypeName}?";
            }

            var propertyDeclarationStringBuilder = new System.Text.StringBuilder();
            propertyDeclarationStringBuilder.AppendWithDelimiter("/// <summary>", this.PropertyDeclarationDelimiter);
            propertyDeclarationStringBuilder.AppendWithDelimiter($"/// {documentationComment}", this.PropertyDeclarationDelimiter);
            propertyDeclarationStringBuilder.AppendWithDelimiter("/// </summary>", this.PropertyDeclarationDelimiter);
            propertyDeclarationStringBuilder.AppendWithDelimiter($"public virtual {aliasedTypeName} {this.ToPropertyName(mappedProperty.ColumnName)} {{ get; set; }}", this.PropertyDeclarationDelimiter);
            return propertyDeclarationStringBuilder.ToString();
        }

        /// <summary>
        /// Generates a start namespace statement in the C# language.
        /// </summary>
        /// <param name="namespace">The namespace to base the statement on.</param>
        /// <returns>A start namespace statement in the C# language.</returns>
        public virtual string GenerateStartNamespaceStatement(string @namespace)
        {
            return $"namespace {@namespace}\r\n{{";
        }

        /// <summary>
        /// Generates a unit test method for a property in the specific language.
        /// </summary>
        /// <param name="mapping">The type mapping.</param>
        /// <param name="mappedProperty">The mapped property.</param>
        /// <param name="imports">A set of import / using namespaces.</param>
        /// <returns>A <see cref="string"/> containing a unit test.</returns>
        public virtual string GeneratePropertyUnitTestMethod(IMapping mapping, IMappedProperty mappedProperty, IList<string> imports = null)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            if (mappedProperty == null)
            {
                throw new ArgumentNullException(nameof(mappedProperty));
            }

            if (imports == null)
            {
                imports = new List<string>();
            }

            imports.UniqueAdd(mapping.ProposedTypeNamespace);
            imports.UniqueAdd("Microsoft.VisualStudio.TestTools.UnitTesting");
            imports.UniqueAdd("System");

            var indent = $"{this.Tab}{this.Tab}";
            var expectedValue = this.GenerateRandomValueMethodCallDeclaration(mappedProperty.ClrType, imports);

            var builder = new System.Text.StringBuilder();
            builder.AppendWithDelimiter($"{indent}/// <summary>", "\r\n");
            builder.AppendWithDelimiter($"{indent}/// Tests the <see cref=\"{mapping.ProposedTypeName}.{mappedProperty.PropertyName}\" /> property.", "\r\n");
            builder.AppendWithDelimiter($"{indent}/// </summary>", "\r\n");
            builder.AppendWithDelimiter($"{indent}[TestMethod]", "\r\n");
            builder.AppendWithDelimiter($"{indent}public void {mappedProperty.PropertyName}_SetItToValue_ReturnsValue()", "\r\n");
            builder.AppendWithDelimiter($"{indent}{{", "\r\n");
            builder.AppendWithDelimiter($"{indent}{this.Tab}// Arrange", "\r\n");
            builder.AppendWithDelimiter($"{indent}{this.Tab}var subject = new {mapping.ProposedTypeName}();", "\r\n");
            builder.AppendWithDelimiter($"{indent}{this.Tab}var expectedValue = {expectedValue};", "\r\n");
            builder.AppendWithDelimiter($"{indent}{this.Tab}subject.{mappedProperty.PropertyName} = expectedValue;\r\n", "\r\n");
            builder.AppendWithDelimiter($"{indent}{this.Tab}// Act", "\r\n");
            builder.AppendWithDelimiter($"{indent}{this.Tab}var result = subject.{mappedProperty.PropertyName};\r\n", "\r\n");
            builder.AppendWithDelimiter($"{indent}{this.Tab}// Assert", "\r\n");
            builder.AppendWithDelimiter($"{indent}{this.Tab}Assert.AreEqual(expectedValue, result);", "\r\n");
            builder.AppendWithDelimiter($"{indent}}}", "\r\n");

            return builder.ToString();
        }

        /// <summary>
        /// Generates a method call or contstant value declaration string.
        /// </summary>
        /// <param name="type">The type to generate a value for.</param>
        /// <param name="imports">Any imported namespaces.</param>
        /// <returns>A <see cref="string"/>.</returns>
        protected virtual string GenerateRandomValueMethodCallDeclaration(System.Type type, IList<string> imports)
        {
            // NULL-check the type
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // NULL-check the imports
            if (imports == null)
            {
                imports = new List<string>();
            }

            var result = $"new {type.FullName}()";

            switch (type.FullName)
            {
                case "Microsoft.SqlServer.Types.SqlGeography":
                    result = "new SqlGeography { STSrid = new SqlInt32(4326) }";
                    imports.UniqueAdd("Microsoft.SqlServer.Types");
                    imports.UniqueAdd("System.Data.SqlTypes");
                    break;
                case "Microsoft.SqlServer.Types.SqlGeometry":
                    result = "new SqlGeometry { STSrid = new SqlInt32(4326) }";
                    imports.UniqueAdd("Microsoft.SqlServer.Types");
                    imports.UniqueAdd("System.Data.SqlTypes");
                    break;
                case "Microsoft.SqlServer.Types.SqlHierarchyId":
                    result = "new SqlHierarchyId()";
                    imports.UniqueAdd("Microsoft.SqlServer.Types");
                    break;
                case "System.Boolean":
                    result = "Utilities.Random.NextBoolean()";
                    break;
                case "System.Byte":
                    result = "Utilities.Random.NextByte()";
                    break;
                case "System.Byte[]":
                    result = "Utilities.Random.NextBytes()";
                    break;
                case "System.DateTime":
                    result = "Utilities.Random.NextDateTime(new DateTime(1753, 1, 1), DateTime.MaxValue)";
                    break;
                case "System.DateTimeOffset":
                    result = "Utilities.Random.NextDateTimeOffset(new DateTimeOffset(new DateTime(1753, 1, 1)), new DateTimeOffset(DateTime.MaxValue))";
                    break;
                case "System.Decimal":
                    result = "Utilities.Random.NextDecimal()";
                    break;
                case "System.Double":
                    result = "Utilities.Random.NextDouble()";
                    break;
                case "System.Guid":
                    result = "Guid.NewGuid()";
                    imports.UniqueAdd("System");
                    break;
                case "System.Int16":
                    result = "Utilities.Random.NextInt16()";
                    break;
                case "System.Int32":
                    result = "Utilities.Random.NextInt32()";
                    break;
                case "System.Int64":
                    result = "Utilities.Random.NextInt64()";
                    break;
                case "System.Object":
                    result = "Utilities.Random.NextString()";
                    break;
                case "System.Single":
                    result = "Utilities.Random.NextSingle()";
                    break;
                case "System.String":
                    result = "Utilities.Random.NextString()";
                    break;
                case "System.TimeSpan":
                    result = "Utilities.Random.NextTimeSpan()";
                    break;
            }

            return result;
        }

        /// <summary>
        /// Generates service unit tests code in the C# language.
        /// </summary>
        /// <param name="mapping">A mapping to generate the code for.</param>
        /// <param name="imports">A set of import / using namespaces.</param>
        /// <returns>A <see cref="string"/> containing the code.</returns>
        public virtual string GenerateServiceUnitTests(IMapping mapping, IList<string> imports)
        {
            // NULL-check the mapping parameter
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            var randomId = "new object()";
            var firstPrimaryKey = mapping.MappedProperties.FirstOrDefault(p => p.IsPrimaryKey);
            if (firstPrimaryKey != null)
            {
                randomId = this.GenerateRandomValueMethodCallDeclaration(firstPrimaryKey.ClrType, imports);
            }

            var builder = new System.Text.StringBuilder();
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}/// <summary>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}/// Tests the <see cref=\"{mapping.ProposedTypeName}Service.FindAll\" /> method.", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}/// </summary>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}[TestMethod]", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}public void FindAll_NoParameters_RepositoryFindAllCalled()", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{{", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}// Arrange", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}var repository = new Mock<I{mapping.ProposedTypeName}Repository>();", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}var service = new {mapping.ProposedTypeName}Service(repository.Object);", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}// Act", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}service.FindAll();", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}// Assert", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}repository.Verify(x => x.FindAll());", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}}}", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}/// <summary>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}/// Tests the <see cref=\"{mapping.ProposedTypeName}Service.Save\" /> method.", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}/// </summary>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}[TestMethod]", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}public void Save_NoParameters_RepositorySaveCalled()", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{{", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}// Arrange", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}var repository = new Mock<I{mapping.ProposedTypeName}Repository>();", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}repository.Setup(r => r.NewId(\"{firstPrimaryKey?.PropertyName ?? "Id"}\")).Returns(System.Guid.NewGuid().ToString());", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}var service = new {mapping.ProposedTypeName}Service(repository.Object);", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}var subject = new {mapping.ProposedTypeName}();", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}// Act", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}service.Save(subject);", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}// Assert", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}repository.Verify(x => x.Save(subject));", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}}}", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}/// <summary>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}/// Tests the <see cref=\"{mapping.ProposedTypeName}Service.FindById\" /> method.", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}/// </summary>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}[TestMethod]", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}public void FindById_NoParameters_RepositoryFindByIdCalled()", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{{", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}// Arrange", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}var id = {randomId};", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}var repository = new Mock<I{mapping.ProposedTypeName}Repository>();", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}var service = new {mapping.ProposedTypeName}Service(repository.Object);", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}// Act", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}service.FindById(id);", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}// Assert", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}repository.Verify(x => x.FindById(id));", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}}}", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}/// <summary>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}/// Tests the <see cref=\"{mapping.ProposedTypeName}Service.Remove\" /> method.", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}/// </summary>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}[TestMethod]", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}public void Remove_NoParameters_RepositoryRemoveCalled()", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{{", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}// Arrange", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}var repository = new Mock<I{mapping.ProposedTypeName}Repository>();", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}var service = new {mapping.ProposedTypeName}Service(repository.Object);", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}var subject = new {mapping.ProposedTypeName}();", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}// Act", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}service.Remove(subject);", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}// Assert", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}repository.Verify(x => x.Remove(subject));", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}}}", "\r\n");
            return builder.ToString();
        }

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
        public virtual string GenerateUsingStatements(IEnumerable<string> namespaces, string scopeNamespace = null)
        {
            if (namespaces == null)
            {
                throw new ArgumentNullException();
            }

            var collection = new CSharpUsingDirectiveCollection();

            foreach (var @namespace in namespaces.Where(n => n != null))
            {
                var namespaceToAdd = @namespace;
                if (!string.IsNullOrWhiteSpace(scopeNamespace))
                {
                    if (namespaceToAdd == scopeNamespace || scopeNamespace.StartsWith($"{namespaceToAdd}."))
                    {
                        continue;
                    }

                    var scopeNamespacePortions = scopeNamespace.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    for (var i = scopeNamespacePortions.Length - 1; i >= 0; i--)
                    {
                        var partialScopeNamespaceBuilder = new System.Text.StringBuilder();
                        for (var j = 0; j <= i; j++)
                        {
                            partialScopeNamespaceBuilder.AppendWithDelimiter(scopeNamespacePortions[j], ".");
                        }

                        if (namespaceToAdd.StartsWith($"{partialScopeNamespaceBuilder}."))
                        {
                            namespaceToAdd = @namespace.Substring($"{partialScopeNamespaceBuilder}.".Length);

                            // if scopeNamespace portions contains namespaceToAdd then
                            // append previous partial namespace

                            // if (scopeNamespacePortions.Any(s => s == namespaceToAdd && partialScopeNamespaceBuilder.Length > 1))
                            if (scopeNamespace != namespaceToAdd && scopeNamespace.Contains(namespaceToAdd) && !scopeNamespace.StartsWith(@namespace))
                            {
                                namespaceToAdd = $"{partialScopeNamespaceBuilder.ToString().Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Last()}.{namespaceToAdd}";
                            }

                            break;
                        }
                    }
                }

                collection.Add(namespaceToAdd);
            }

            return collection.ToString(string.IsNullOrWhiteSpace(scopeNamespace) ? string.Empty : this.Tab);
        }

        /// <summary>
        /// Determines whether a specified string is a keyword of the C# language.
        /// If it is, a <see cref="string"/> containing the value preceded with an "@" is returned.
        /// If it isn't, the original value is returned.
        /// </summary>
        /// <param name="value">The string to evaluate.</param>
        /// <returns>
        /// If the specified string is a keyword of the C# language, a <see cref="string"/> containing the value preceded with an "@" is returned.
        /// If it isn't, the original value is returned.
        /// </returns>
        public virtual string SanitiseKeyword(string value)
        {
            return string.IsNullOrEmpty(value) ? value : this.Keywords.Any(k => k == value) ? $"@{value}" : value;
        }

        /// <summary>
        /// Converts a database column name into an appropriate format 
        /// for a property name in the specific CLR language.
        /// </summary>
        /// <param name="columnName">The name of a database column to parse.</param>
        /// <returns>A <see cref="string"/></returns>
        public virtual string ToPropertyName(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            return this.SanitiseKeyword(columnName.ToUpperFirstLetter());
        }
    }
}
