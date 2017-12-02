// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualBasicCodeFactory.cs" company="Inspire IT Ltd">
//     Copyright © Ryan Penfold. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Utilities;
    using Utilities.Collections.Generic;
    using Utilities.Data.SqlClient;
    using Utilities.Text;

    public class VisualBasicCodeFactory : ICodeFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualBasicCodeFactory"/> class.
        /// </summary>
        public VisualBasicCodeFactory()
        {
            // Set the field values
            this.Tab = "\t";
            this.CodeFileExtension = "vb";
            this.Keywords = System.IO.File.ReadAllLines($"{System.IO.Directory.GetCurrentDirectory()}\\Keywords.{this.CodeFileExtension}.txt");
            this.LineContinuationOperator = " _";
            this.NullOperator = "Nothing";
            this.ProjectFileExtension = "vbproj";
            this.PropertyDeclarationDelimiter = $"\r\n{this.Tab}";
            this.StatementDelimiter = string.Empty;
        }

        /// <summary>
        /// Gets the file extension for code files written in the specific language.
        /// </summary>
        public string CodeFileExtension { get; }

        /// <summary>
        /// Gets the keywords of the Visual Basic language.
        /// </summary>
        public IEnumerable<string> Keywords { get; }

        /// <summary>
        /// Gets the line continuation operator for the Visual Basic language.
        /// </summary>
        public string LineContinuationOperator { get; }

        /// <summary>
        /// Gets the null operator for the Visual Basic language.
        /// </summary>
        public string NullOperator { get; }

        /// <summary>
        /// Gets the file extension for projects for code written in the Visual Basic language.
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
        //// Gets a tab in the specified language.
        /// </summary>
        public string Tab { get; }

        /// <summary>
        /// Attempts to find an alias for a type name in the Visual Basic language.
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
                    result = "Boolean";
                    break;
                case "System.Boolean[]":
                    result = "Boolean()";
                    break;
                case "System.Byte":
                    result = "Byte";
                    break;
                case "System.Byte[]":
                    result = "Byte()";
                    break;
                case "System.Char":
                    result = "Char";
                    break;
                case "System.Char[]":
                    result = "Char()";
                    break;
                case "System.DateTime":
                    result = "Date";
                    break;
                case "System.DateTime[]":
                    result = "Date()";
                    break;
                case "System.Decimal":
                    result = "Decimal";
                    break;
                case "System.Decimal[]":
                    result = "Decimal()";
                    break;
                case "System.Double":
                    result = "Double";
                    break;
                case "System.Double[]":
                    result = "Double()";
                    break;
                case "System.Int16":
                    result = "Short";
                    break;
                case "System.Int16[]":
                    result = "Short()";
                    break;
                case "System.Int32":
                    result = "Integer";
                    break;
                case "System.Int32[]":
                    result = "Integer()";
                    break;
                case "System.Int64":
                    result = "Long";
                    break;
                case "System.Int64[]":
                    result = "Long()";
                    break;
                case "System.Object":
                    result = "Object";
                    break;
                case "System.Object[]":
                    result = "Object()";
                    break;
                case "System.SByte":
                    result = "Sbyte";
                    break;
                case "System.SByte[]":
                    result = "Sbyte()";
                    break;
                case "System.Single":
                    result = "Single";
                    break;
                case "System.Single[]":
                    result = "Single()";
                    break;
                case "System.String":
                    result = "String";
                    break;
                case "System.String[]":
                    result = "String()";
                    break;
                case "System.UInt32":
                    result = "UInteger";
                    break;
                case "System.UInt32[]":
                    result = "UInteger()";
                    break;
                case "System.UInt64":
                    result = "ULong";
                    break;
                case "System.UInt64[]":
                    result = "ULong()";
                    break;
                case "System.UInt16":
                    result = "UShort";
                    break;
                case "System.UInt16[]":
                    result = "UShort()";
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
                if (!assertMethodString.ToString().EndsWith("End If\r\n\r\n"))
                {
                    assertMethodString.Append("\r\n");
                }
            }

            assertMethodString.Append($"{this.Tab}{this.Tab}");

            if (mappedProperty.SqlType.ToLower() == "timestamp")
            {
                assertMethodString.Append("'");
            }

            if (mappedPropertyClrTypeIsNullable)
            {
                assertMethodString.AppendLine($"If expected.{mappedProperty.PropertyName}.HasValue AndAlso actual.{mappedProperty.PropertyName}.HasValue Then");
                assertMethodString.Append($"\r\n{this.Tab}{this.Tab}{this.Tab}");
                propertyAccessor = ".Value";
            }

            var underlyingType = Nullable.GetUnderlyingType(mappedProperty.ClrType);
            switch (underlyingType == null ? mappedProperty.ClrType.FullName : underlyingType.FullName)
            {
                case "System.Byte[]":
                    assertMethodString.Append($"Assert.AreEqual(BitConverter.ToString(expected.{mappedProperty.PropertyName}{propertyAccessor}), BitConverter.ToString(actual.{mappedProperty.PropertyName}{propertyAccessor}))");
                    importedNamespaces.UniqueAdd("System");
                    break;
                case "System.DateTime":
                    assertMethodString.Append($"Assert.AreEqual(expected.{mappedProperty.PropertyName}{propertyAccessor}.ToString(\"dd/MM/yyyy HH:mm:ss\"), actual.{mappedProperty.PropertyName}{propertyAccessor}.ToString(\"dd/MM/yyyy HH:mm:ss\"))");
                    break;
                case "System.DateTimeOffset":
                    assertMethodString.Append($"Assert.AreEqual(expected.{mappedProperty.PropertyName}{propertyAccessor}.ToString(\"dd/MM/yyyy HH:mm:ss %K\"), actual.{mappedProperty.PropertyName}{propertyAccessor}.ToString(\"dd/MM/yyyy HH:mm:ss %K\"))");
                    break;
                case "System.Single":
                case "System.Double":
                    assertMethodString.Append($"Assert.AreEqual(expected.{mappedProperty.PropertyName}{propertyAccessor}.ToString(CultureInfo.InvariantCulture), actual.{mappedProperty.PropertyName}{propertyAccessor}.ToString(CultureInfo.InvariantCulture))");
                    importedNamespaces.UniqueAdd("System.Globalization");
                    break;
                default:
                    assertMethodString.Append($"Assert.AreEqual(expected.{mappedProperty.PropertyName}{propertyAccessor}, actual.{mappedProperty.PropertyName}{propertyAccessor})");
                    break;
            }

            if (mappedPropertyClrTypeIsNullable)
            {
                assertMethodString.Append($"\r\n\r\n{this.Tab}{this.Tab}End If");
            }

            if (mappedProperty.SqlType.ToLower() == "timestamp")
            {
                assertMethodString.Append(" not able to test a timestamp property a specific value can't be inserted, nor can the column be updated.");
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
        /// <param name="typeNamespace">A namespace for a given type.</param>
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

            // In the case of VB.NET, the code file namespace only includes any sub namespaces 
            // from the project root. Otherwise it is null.

            // If the model type name is equal to the model project namespace
            if (typeNamespace == targetProjectRootNamespace)
            {
                return string.Empty;
            }

            // If the model type name starts with the model project namespace
            if (typeNamespace.StartsWith($"{targetProjectRootNamespace}."))
            {
                return $"{typeNamespace.Substring($"{targetProjectRootNamespace}.".Length)}";
            }

            return typeNamespace;
        }

        /// <summary>
        /// Generates an end namespace statement in the Visual Basic language.
        /// </summary>
        /// <param name="namespace">The namespace to base the statement on.</param>
        /// <returns>A start namespace statement in the Visual Basic language.</returns>
        public virtual string GenerateEndNamespaceStatement(string @namespace)
        {
            return string.IsNullOrWhiteSpace(@namespace) ? string.Empty : "\r\nEnd Namespace";
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

            return $"\r\n{this.Tab}{this.Tab}CompositeId()";
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
            xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}''' <summary>");
            xmldocStringBuilder.Append($"{this.Tab}{this.Tab}''' Calls to the ");
            xmldocStringBuilder.Append(mapping.SourceObjectName);
            xmldocStringBuilder.AppendLine("] named query. ");
            xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}''' </summary>");

            // Result string builder
            var methodStringBuilder = new System.Text.StringBuilder();
            methodStringBuilder.Append($"{this.Tab}{this.Tab}Public Function ");
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

                methodStringBuilder.AppendWithDelimiter("Optional ", isFirst ? string.Empty : ", ");

                var methodParameterName = this.SanitiseKeyword(sqlParameterName.ToLowerFirstLetter());
                methodStringBuilder.Append(methodParameterName);

                var parameterClrType = parameter.Type.ToClrType();
                var aliasedTypeName = this.AliasTypeName(parameterClrType);
                foreach (var importedNamespace in importedNamespaces.OrderByDescending(i => i))
                {
                    if (aliasedTypeName.StartsWith($"{importedNamespace}."))
                    {
                        aliasedTypeName = aliasedTypeName.Substring(importedNamespace.Length + 1);
                    }
                }

                methodStringBuilder.Append(" As ");
                methodStringBuilder.Append(aliasedTypeName);

                if (parameterClrType.IsValueType)
                {
                    methodStringBuilder.Append("?");
                }

                methodStringBuilder.Append(" = Nothing");

                callParameterStringBuilder.AppendWithDelimiter($":{sqlParameterName}", ", ");
                setParameterStringBuilder.AppendLine($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}{this.Tab}.SetParameter(\"{sqlParameterName}\", {methodParameterName}) _ ");

                xmldocStringBuilder.Append($"{this.Tab}{this.Tab}''' <param name=\"");
                xmldocStringBuilder.Append(methodParameterName);
                xmldocStringBuilder.AppendLine("\">");
                xmldocStringBuilder.Append($"{this.Tab}{this.Tab}''' The ");
                xmldocStringBuilder.Append(methodParameterName.FromCamelCase().ToLower());
                xmldocStringBuilder.AppendLine(".");
                xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}''' </param>");

                isFirst = false;
            }

            methodStringBuilder.Append(") As IEnumerable(Of ");
            methodStringBuilder.Append(mapping.ProposedTypeName);
            methodStringBuilder.AppendLine($")  Implements I{mapping.ProposedTypeName}Repository.{this.ToPropertyName(mapping.SourceObjectName)}");

            xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}''' <returns>");
            xmldocStringBuilder.Append($"{this.Tab}{this.Tab}''' An <see cref=\"IEnumerable(Of ");
            xmldocStringBuilder.Append(mapping.ProposedTypeName);
            xmldocStringBuilder.AppendLine(")\" />.");
            xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}''' </returns>");
            methodStringBuilder.AppendLine($"\r\n{this.Tab}{this.Tab}{this.Tab}' Start a new session");
            methodStringBuilder.AppendLine($"{this.Tab}{this.Tab}{this.Tab}Using session = SessionFactory.GetNewSession()");
            methodStringBuilder.Append($"\r\n{this.Tab}{this.Tab}{this.Tab}{this.Tab}Return session.CreateSQLQuery(\"");
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

            methodStringBuilder.Append(mapping.SourceObjectSchema);
            methodStringBuilder.Append("].[");
            methodStringBuilder.Append(mapping.SourceObjectName);
            methodStringBuilder.Append("] ");
            methodStringBuilder.Append(callParameterStringBuilder);
            methodStringBuilder.AppendLine("\") _ ");
            methodStringBuilder.Append($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}{this.Tab}.AddEntity(GetType(");
            methodStringBuilder.Append(mapping.ProposedTypeName);
            methodStringBuilder.AppendLine(")) _ ");
            methodStringBuilder.Append(setParameterStringBuilder);
            methodStringBuilder.Append($"{this.Tab}{this.Tab}{this.Tab}{this.Tab}{this.Tab}.List(Of ");
            methodStringBuilder.Append(mapping.ProposedTypeName);
            methodStringBuilder.AppendLine(")()\r\n");
            methodStringBuilder.AppendLine($"{this.Tab}{this.Tab}{this.Tab}End Using\r\n");
            methodStringBuilder.Append($"{this.Tab}{this.Tab}End Function");

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
            xmldocStringBuilder.AppendLine($"\r\n{this.Tab}{this.Tab}''' <summary>");
            xmldocStringBuilder.Append($"{this.Tab}{this.Tab}''' Calls to the ");
            xmldocStringBuilder.Append(mapping.SourceObjectName);
            xmldocStringBuilder.AppendLine(" named query. ");
            xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}''' </summary>");

            // Result string builder
            var methodStringBuilder = new System.Text.StringBuilder();
            methodStringBuilder.Append($"{this.Tab}{this.Tab}Function ");
            methodStringBuilder.Append(this.ToPropertyName(mapping.SourceObjectName));
            methodStringBuilder.Append("(");

            // Append the parameters
            var isFirst = true;
            foreach (var parameter in mapping.SqlParameters.Items)
            {
                var sqlParameterName = parameter.Name.Replace("@", string.Empty);

                if (sqlParameterName == "RETURN_VALUE" || sqlParameterName == "TABLE_RETURN_VALUE")
                {
                    continue;
                }

                methodStringBuilder.AppendWithDelimiter("Optional ", isFirst ? string.Empty : ", ");

                var methodParameterName = this.SanitiseKeyword(sqlParameterName.ToLowerFirstLetter());
                methodStringBuilder.Append(methodParameterName);

                var parameterClrType = parameter.Type.ToClrType();
                var aliasedTypeName = this.AliasTypeName(parameterClrType);
                foreach (var importedNamespace in importedNamespaces.OrderByDescending(i => i))
                {
                    if (aliasedTypeName.StartsWith($"{importedNamespace}."))
                    {
                        aliasedTypeName = aliasedTypeName.Substring(importedNamespace.Length + 1);
                    }
                }

                methodStringBuilder.Append(" As ");
                methodStringBuilder.Append(aliasedTypeName);

                if (parameterClrType.IsValueType)
                {
                    methodStringBuilder.Append("?");
                }

                methodStringBuilder.Append(" = Nothing");

                xmldocStringBuilder.Append($"{this.Tab}{this.Tab}''' <param name=\"");
                xmldocStringBuilder.Append(sqlParameterName);
                xmldocStringBuilder.AppendLine("\">");
                xmldocStringBuilder.Append($"{this.Tab}{this.Tab}''' The ");
                xmldocStringBuilder.Append(sqlParameterName.FromCamelCase().ToLower());
                xmldocStringBuilder.AppendLine(".");
                xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}''' </param>");

                isFirst = false;
            }

            methodStringBuilder.Append(") As IEnumerable(Of ");
            methodStringBuilder.Append(mapping.ProposedTypeName);
            methodStringBuilder.Append(")");

            xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}''' <returns>");
            xmldocStringBuilder.Append($"{this.Tab}{this.Tab}''' An <see cref=\"IEnumerable(Of ");
            xmldocStringBuilder.Append(mapping.ProposedTypeName);
            xmldocStringBuilder.AppendLine(")\" />.");
            xmldocStringBuilder.AppendLine($"{this.Tab}{this.Tab}''' </returns>");

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

            result.AppendWithDelimiter($"\r\n{this.Tab}''' <summary>", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}''' Tests the <see cref=\"{{className}}Repository.{this.SanitiseKeyword(mapping.SourceObjectName)}\" /> method.", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}''' </summary>", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}<TestMethod>{this.LineContinuationOperator}", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}Public Sub {this.SanitiseKeyword(mapping.SourceObjectName)}_TestConditions_ExpectedResult()", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}' TODO: Implement", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}Throw New NotImplementedException()", "\r\n");
            importedNamespaces.UniqueAdd("System");
            result.AppendLine();
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}'Try", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}'' Arrange", "\r\n\r\n");
            result.AppendLine();
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}'' Drop the \"{{className}}\" table if it exists already", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}'Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, String.Format(\"{{0}}.SQL.DROP TABLE [{{sourceObjectSchema}}].[{{className}}].sql\", Me.GetType().Assembly.GetName().Name), ConnectionStringName)", "\r\n");
            result.AppendLine();
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}'' Create the \"{{className}}\" table", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}'Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, String.Format(\"{{0}}.SQL.CREATE TABLE [{{sourceObjectSchema}}].[{{className}}].sql\", Me.GetType().Assembly.GetName().Name), ConnectionStringName)", "\r\n");
            result.AppendLine();
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}'' Create new repository", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}'Dim repository As New {{className}}Repository()", "\r\n");

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

                result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}", "\r\n");

                switch (parameter.Type.ToString().ToLower())
                {
                    case "bigint":
                        result.Append($"'Dim {variableName} = Utilities.Random.NextInt64()");
                        break;
                    case "binary":
                        result.Append(parameter.Size == defaultLengthForBytes ? $"'Dim {variableName} = Utilities.Random.NextBytes()" : $"'Dim {variableName} = Utilities.Random.NextBytes({parameter.Size})");
                        break;
                    case "bit":
                        result.Append($"'Dim {variableName} = Utilities.Random.NextBoolean()");
                        break;
                    case "char":
                    case "nchar":
                        result.Append(parameter.Size == defaultLengthForString ? $"'Dim {variableName} = Utilities.Random.NextString().Replace(\"'\", \";\")" : $"'Dim {variableName} = Utilities.Random.NextString({parameter.Size}).Replace(\"'\", \";\")");
                        break;
                    case "date":
                        result.Append($"'Dim {variableName} = Utilities.Random.NextDateTime(false)");
                        break;
                    case "datetime":
                        result.Append($"'Dim {variableName} = Utilities.Random.NextDateTime(New DateTime(1753, 1, 1), DateTime.MaxValue)");
                        importedNamespaces.UniqueAdd("System");
                        break;
                    case "datetime2":
                        result.Append($"'Dim {variableName} = Utilities.Random.NextDateTime()");
                        break;
                    case "datetimeoffset":
                        result.Append($"'Dim {variableName} = Utilities.Random.NextDateTimeOffset()");
                        break;
                    case "decimal":
                        result.Append($"'Dim {variableName} = Utilities.Random.NextInt16(-1, 1)");
                        break;
                    case "float":
                        result.Append($"'Dim {variableName} = Utilities.Random.NextDouble()");
                        break;
                    case "image":
                        result.Append(16 == defaultLengthForBytes ? $"'Dim {variableName} = Utilities.Random.NextBytes()" : $"'Dim {variableName} = Utilities.Random.NextBytes(16)");
                        break;
                    case "int":
                        result.Append($"'Dim {variableName} = Utilities.Random.NextInt32()");
                        break;
                    case "money":
                        result.Append($"'Dim {variableName} = CType(Utilities.Random.NextDouble(-922337203685477.62, 922337203685477.62), Decimal)");
                        break;
                    case "ntext":
                    case "text":
                    case "nvarchar":
                    case "varchar":
                    case "variant":
                        result.Append($"'Dim {variableName} = Utilities.Random.NextString(1).Replace(\"'\", \";\")");
                        break;
                    case "real":
                        result.Append($"'Dim {variableName} = Utilities.Random.NextSingle(-0.9F, 0.9F)");
                        break;
                    case "smalldatetime":
                        result.Append($"'Dim {variableName} = Utilities.Random.NextDateTime(New DateTime(1900, 1, 1), New DateTime(2079, 6, 6), false)");
                        importedNamespaces.UniqueAdd("System");
                        break;
                    case "smallint":
                        result.Append($"'Dim {variableName} = Utilities.Random.NextInt16()");
                        break;
                    case "smallmoney":
                        result.Append($"'Dim {variableName} = CType(Utilities.Random.NextSingle(-214748.359F, 214748.359F), Decimal)");
                        break;
                    case "time":
                        result.Append($"'Dim {variableName} = Utilities.Random.NextTimeSpan()");
                        break;
                    case "timestamp":
                        result.Append(8 == defaultLengthForBytes ? $"'Dim {variableName} = Utilities.Random.NextBytes()" : $"'Dim {variableName} = Utilities.Random.NextBytes(8)");
                        break;
                    case "tinyint":
                        result.Append($"'Dim {variableName} = Utilities.Random.NextByte()");
                        break;
                    case "udt":
                        result.Append($"'Dim {variableName} As Object = {this.NullOperator}");
                        break;
                    case "uniqueidentifier":
                        result.Append($"'Dim {variableName} = Guid.NewGuid()");
                        importedNamespaces.UniqueAdd("System");
                        break;
                    case "varbinary":
                        result.Append($"'Dim {variableName} = Utilities.Random.NextBytes(1)");
                        break;
                    case "xml":
                        result.Append($"'Dim {variableName} = New SqlXml(XmlReader.Create(\"<{mapping.ProposedTypeName} />\"))");
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown sql data type \"{parameter.Type}\"");
                }

                result.Append(variableDeclaration);
                parametersBuilder.AppendWithDelimiter(variableName, ", ");
            }

            result.AppendLine();
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}'' Act", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}'Dim result = repository.{this.SanitiseKeyword(mapping.SourceObjectName)}(", "\r\n");
            result.Append(parametersBuilder);
            result.AppendLine(")");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}'' Assert", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}'Assert.AreEqual(1, result.Count())", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}'Catch", "\r\n\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}'' Drop the \"{{className}}\" table if it still exists", "\r\n\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}{this.Tab}'Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, String.Format(\"{{0}}.SQL.DROP TABLE [{{sourceObjectSchema}}].[{{className}}].sql\", Me.GetType().Assembly.GetName().Name), ConnectionStringName)", "\r\n");
            result.AppendWithDelimiter($"{this.Tab}{this.Tab}'End Try", "\r\n\r\n");
            result.AppendWithDelimiter($"{this.Tab}End Sub\r\n", "\r\n\r\n");

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
            var builder = new System.Text.StringBuilder();

            builder.Append($"{this.Tab}{this.Tab}");
            builder.Append(".KeyProperty(Function(x) x.");
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

            builder.Append($"\r\n{this.Tab}{this.Tab}");
            builder.Append("Id(Function(x) x.");
            builder.Append(mappedProperty.PropertyName);
            builder.Append(").GeneratedBy.Assigned()");
            builder.Append(this.GenerateFluentNHibernatePropertyMappingCustomTypes(mappedProperty));
            builder.Append(this.StatementDelimiter);

            builder.Append($"\r\n{this.Tab}{this.Tab}");
            builder.Append("PrimaryKeyColumnNames.UniqueAdd(\"");
            builder.Append(mappedProperty.PropertyName);
            builder.Append("\")");
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

            builder.Append($"{this.Tab}{this.Tab}");
            builder.Append("Map(Function(x) x.");
            builder.Append(mappedProperty.PropertyName);
            builder.Append(")");
            builder.Append(this.GenerateFluentNHibernatePropertyMappingCustomTypes(mappedProperty));

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
                    testValue = "CType(Utilities.Random.NextInt16(-1, 1), Decimal)";
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
                    testValue = "CType(Utilities.Random.NextDouble(-922337203685477.5808, 922337203685477.62), Decimal)";
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
                    testValue = "CType(Utilities.Random.NextInt16(-1, 1), Decimal)";
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

            for (var tabCount = 0; tabCount < 6; tabCount++)
            {
                result.Append(this.Tab);
            }

            result.Append($".CheckProperty(Function(x) x.{mappedProperty.PropertyName}, {testValue}){this.LineContinuationOperator}");

            return result.ToString();
        }

        /// <summary>
        /// Generates a set of fluent nhibernate mappings from some mapping settings.
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

            builder.Append($"\r\n{this.Tab}{this.Tab}");
            builder.Append($"TableName = \"{mapping.SourceObjectName}\"");
            builder.Append($"\r\n{this.Tab}{this.Tab}");
            builder.Append($"SchemaName = \"{mapping.SourceObjectSchema}\"");
            builder.Append($"\r\n{this.Tab}{this.Tab}");
            builder.Append($"Table(TableName)");
            builder.Append($"\r\n{this.Tab}{this.Tab}");
            builder.Append($"Schema(SchemaName)");
            builder.Append($"\r\n{this.Tab}{this.Tab}");
            builder.Append($"EnsureTableExists(GetType({mapping.ProposedTypeName}), SchemaName, TableName)");
            
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
                    value = "Utilities.Random.NextDateTime(New Date(1753, 1, 1), DateTime.MaxValue)";
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
                    value = "CType(Utilities.Random.NextDouble(-922337203685477.62, 922337203685477.62), Decimal)";
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
                    value = "Utilities.Random.NextDateTime(New Date(1900, 1, 1), New Date(2079, 6, 6), false)";
                    break;
                case "smallint":
                    value = "Utilities.Random.NextInt16()";
                    break;
                case "smallmoney":
                    value = "CType(Utilities.Random.NextSingle(-214748.359F, 214748.359F), Decimal)";
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

            return $"{this.Tab}{this.Tab}instance.{mappedProperty.PropertyName} = {value}";
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
            return $"{this.Tab}{this.Tab}sqlcommandBuilder.AppendWithDelimiter(\"   {comma}[{mapping.SourceObjectSchema}].[{mapping.SourceObjectName}].[{mappedProperty.ColumnName}]\", VbCrLf)";
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
                valueBuilder.Append($"If(entity.{mappedProperty.PropertyName}.HasValue, ");
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
                    valueBuilder.Append($"String.Format(\"{comma}{{0}}\", entity.{mappedProperty.PropertyName}{propertyAccessor})");
                    break;
                case "binary":
                case "image":
                case "varbinary":
                    valueBuilder.Append($"String.Format(\"{comma}0x{{0}}\", String.Concat(entity.{mappedProperty.PropertyName}{propertyAccessor}.Select(Function(b) b.ToString(\"X2\")).ToArray()))");
                    break;
                case "bit":
                    valueBuilder.Append($"String.Format(\"{comma}{{0}}\", Utilities.Boolean.ToByte(entity.{mappedProperty.PropertyName}{propertyAccessor}))");
                    break;
                case "date":
                    valueBuilder.Append($"String.Format(\"{comma}'{{0}}'\", entity.{mappedProperty.PropertyName}{propertyAccessor}.ToString(\"yyyy-MM-dd\"))");
                    break;
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    valueBuilder.Append($"String.Format(\"{comma}'{{0}}'\", entity.{mappedProperty.PropertyName}{propertyAccessor}.ToString(\"yyyy-MM-dd HH:mm:ss\"))");
                    break;
                case "datetimeoffset":
                    valueBuilder.Append($"String.Format(\"{comma}'{{0}}'\", entity.{mappedProperty.PropertyName}{propertyAccessor}.ToString(\"yyyy-MM-dd HH:mm:ss %K\"))");
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
                    valueBuilder.Append($"String.Format(\"{comma}'{{0}}'\", entity.{mappedProperty.PropertyName}{propertyAccessor})");
                    break;
                case "timestamp":
                    valueBuilder.Append($"\"{comma}DEFAULT\"");
                    break;
                default:
                    throw new InvalidOperationException($"Unknown sql data type \"{mappedProperty.SqlType}\"");
            }

            if (mappedPropertyClrTypeIsNullable)
            {
                if (valueBuilder.ToString().EndsWith($", \"{comma}NULL\""))
                {
                    valueBuilder = new System.Text.StringBuilder($"\"{comma}NULL\"");
                }
                else
                {
                    valueBuilder.Append($" , \"{comma}NULL\")");
                }
            }

            return $"{this.Tab}{this.Tab}sqlcommandBuilder.AppendWithDelimiter({valueBuilder}, VbCrLf)";
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

            if (aliasedTypeName.StartsWith("System."))
            {
                aliasedTypeName = aliasedTypeName.Substring("System.".Length);
            }

            // HACK: Use string instead of object
            if (aliasedTypeName == "Object")
            {
                aliasedTypeName = "String";
            }

            var mappedPropertyClrTypeIsNullable = mappedProperty.ClrType.IsNullable() ||
                (mappedProperty.ClrType.IsValueType && mappedProperty.IsNullable.HasValue && mappedProperty.IsNullable.Value);

            if (mappedPropertyClrTypeIsNullable)
            {
                aliasedTypeName = $"{aliasedTypeName}?";
            }

            var propertyDeclarationStringBuilder = new System.Text.StringBuilder();
            propertyDeclarationStringBuilder.AppendWithDelimiter("''' <summary>", this.PropertyDeclarationDelimiter);
            propertyDeclarationStringBuilder.AppendWithDelimiter($"''' {documentationComment}", this.PropertyDeclarationDelimiter);
            propertyDeclarationStringBuilder.AppendWithDelimiter("''' </summary>", this.PropertyDeclarationDelimiter);
            propertyDeclarationStringBuilder.AppendWithDelimiter($"Public Overridable Property {this.ToPropertyName(mappedProperty.ColumnName)} As {aliasedTypeName}", this.PropertyDeclarationDelimiter);
            return propertyDeclarationStringBuilder.ToString();
        }

        /// <summary>
        /// Generates a unit test method for a property in C#.
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

            var codeFileNamespace = this.DetermineCodeFileNamespace(mapping.ProposedTypeNamespace, mapping.ProjectRootNamespace);
            var indent = string.IsNullOrWhiteSpace(codeFileNamespace) ? $"{this.Tab}" : $"{this.Tab}{this.Tab}";
            var expectedValue = this.GenerateRandomValueMethodCallDeclaration(mappedProperty.ClrType, imports);

            var builder = new System.Text.StringBuilder();
            builder.AppendWithDelimiter($"{indent}''' <summary>", "\r\n");
            builder.AppendWithDelimiter($"{indent}''' Tests the <see cref=\"{mapping.ProposedTypeName}.{mappedProperty.PropertyName}\" /> property.", "\r\n");
            builder.AppendWithDelimiter($"{indent}''' </summary>", "\r\n");
            builder.AppendWithDelimiter($"{indent}<TestMethod> _", "\r\n");
            builder.AppendWithDelimiter($"{indent}Public Sub {mappedProperty.PropertyName}_SetItToValue_ReturnsValue()", "\r\n");
            builder.AppendWithDelimiter($"{indent}{this.Tab}' Arrange", "\r\n\r\n");
            builder.AppendWithDelimiter($"{indent}{this.Tab}Dim subject As New {mapping.ProposedTypeName}()", "\r\n");
            builder.AppendWithDelimiter($"{indent}{this.Tab}Dim expectedValue = {expectedValue}", "\r\n");
            builder.AppendWithDelimiter($"{indent}{this.Tab}subject.{mappedProperty.PropertyName} = expectedValue\r\n", "\r\n");
            builder.AppendWithDelimiter($"{indent}{this.Tab}' Act", "\r\n");
            builder.AppendWithDelimiter($"{indent}{this.Tab}Dim result = subject.{mappedProperty.PropertyName}\r\n", "\r\n");
            builder.AppendWithDelimiter($"{indent}{this.Tab}' Assert", "\r\n");
            builder.AppendWithDelimiter($"{indent}{this.Tab}Assert.AreEqual(expectedValue, result)", "\r\n");
            builder.AppendWithDelimiter($"{indent}End Sub", "\r\n\r\n");

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
                    result = "New SqlGeography With { .STSrid = New SqlInt32(4326) }";
                    imports.UniqueAdd("Microsoft.SqlServer.Types");
                    imports.UniqueAdd("System.Data.SqlTypes");
                    break;
                case "Microsoft.SqlServer.Types.SqlGeometry":
                    result = "New SqlGeometry With { .STSrid = New SqlInt32(4326) }";
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
                    result = "Utilities.Random.NextDateTime(new Date(1900, 1, 1), new Date(2079, 06, 06))";
                    break;
                case "System.DateTimeOffset":
                    result = "Utilities.Random.NextDateTimeOffset(New DateTimeOffset(New Date(1753, 1, 1)), New DateTimeOffset(Date.MaxValue))";
                    break;
                case "System.Decimal":
                    result = "Utilities.Random.NextDecimal(1, 9)";
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
                    imports.UniqueAdd("System");
                    break;
                case "System.Single":
                    result = "Utilities.Random.NextSingle()";
                    break;
                case "System.String":
                    result = "Utilities.Random.NextString(1)";
                    break;
                case "System.TimeSpan":
                    result = "Utilities.Random.NextTimeSpan()";
                    break;
            }

            return result;
        }

        /// <summary>
        /// Generates service unit tests code in the Visual Basic language.
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

            builder.AppendWithDelimiter($"{this.Tab}''' <summary>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}''' Tests the <see cref=\"{mapping.ProposedTypeName}Service.FindAll\" /> method.", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}''' </summary>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}<TestMethod>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}Public Sub FindAll_NoParameters_RepositoryFindAllCalled()", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}'Arrange", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}Dim repository As New Mock(Of I{mapping.ProposedTypeName}Repository)()", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}Dim service As New {mapping.ProposedTypeName}Service(repository.Object)", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}'Act", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}service.FindAll()", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}'Assert", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}repository.Verify(Function(x) x.FindAll())", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}End Sub", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}''' <summary>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}''' Tests the <see cref=\"{mapping.ProposedTypeName}Service.Save\" /> method.", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}''' </summary>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}<TestMethod>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}Public Sub Save_NoParameters_RepositorySaveCalled()", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}'Arrange", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}Dim repository As New Mock(Of I{mapping.ProposedTypeName}Repository)()", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}Dim service As New {mapping.ProposedTypeName}Service(repository.Object)", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}Dim subject As New {mapping.ProposedTypeName}", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}'Act", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}service.Save(subject)", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}'Assert", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}repository.Verify(Sub(x) x.Save(subject))", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}End Sub", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}''' <summary>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}''' Tests the <see cref=\"{mapping.ProposedTypeName}Service.FindById\" /> method.", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}''' </summary>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}<TestMethod>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}Public Sub FindById_NoParameters_RepositoryFindByIdCalled()", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}'Arrange", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}Dim id As Object = {randomId}", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}Dim repository As New Mock(Of I{mapping.ProposedTypeName}Repository)()", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}repository.Setup(Function(r) r.NewId(\"{firstPrimaryKey?.PropertyName ?? "Id"}\")).Returns(System.Guid.NewGuid().ToString())", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}Dim service As New {mapping.ProposedTypeName}Service(repository.Object)", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}'Act", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}service.FindById(id)", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}'Assert", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}repository.Verify(Function(x) x.FindById(id))", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}End Sub", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}''' <summary>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}''' Tests the <see cref=\"{mapping.ProposedTypeName}Service.Remove\" /> method.", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}''' </summary>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}<TestMethod>", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}Public Sub Remove_NoParameters_RepositoryRemoveCalled()", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}'Arrange", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}Dim repository As New Mock(Of I{mapping.ProposedTypeName}Repository)()", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}Dim service As New {mapping.ProposedTypeName}Service(repository.Object)", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}Dim subject As New {mapping.ProposedTypeName}", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}'Act", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}service.Remove(subject)", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}'Assert", "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}{this.Tab}repository.Verify(Sub(x) x.Remove(subject))", "\r\n");
            builder.AppendWithDelimiter(string.Empty, "\r\n");
            builder.AppendWithDelimiter($"{this.Tab}End Sub", "\r\n");

            return builder.ToString();
        }

        /// <summary>
        /// Generates a start namespace statement in the Visual Basic language.
        /// </summary>
        /// <param name="namespace">The namespace to base the statement on.</param>
        /// <returns>A start namespace statement in the Visual Basic language.</returns>
        public virtual string GenerateStartNamespaceStatement(string @namespace)
        {
            return string.IsNullOrWhiteSpace(@namespace) ? string.Empty : $"\r\nNamespace {@namespace}\r\n";
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

            return new VisualBasicImportsStatementCollection(namespaces.Where(n => n != null)).ToString();
        }

        /// <summary>
        /// Determines whether a specified string is a keyword of the Visual Basic language.
        /// If it is, a <see cref="string"/> containing the value encased in square brackets is returned.
        /// If it isn't, the original value is returned.
        /// </summary>
        /// <param name="value">The string to evaluate.</param>
        /// <returns>
        /// If the specified string is a keyword of the Visual Basic language, a <see cref="string"/> containing the value encased in square brackets is returned.
        /// If it isn't, the original value is returned.
        /// </returns>
        public virtual string SanitiseKeyword(string value)
        {
            return string.IsNullOrEmpty(value) ? value : this.Keywords.Any(k => string.Equals(k, value, StringComparison.InvariantCultureIgnoreCase)) ? $"[{value}]" : value;
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
