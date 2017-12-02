' --------------------------------------------------------------------------------------------------------------------
' <copyright file="BarMapTests.cs" company="Inspire IT Ltd">
'     Copyright © Inspire IT Ltd. All rights reserved.
' </copyright>
' --------------------------------------------------------------------------------------------------------------------

Imports FluentNHibernate.Testing
Imports NHibernate
Imports RyanPenfold.NHibernateRepository
Imports RyanPenfold.TestProject.Model
Imports RyanPenfold.Utilities.Collections

Namespace FluentMappings

    ''' <summary>
    ''' Provides unit tests for the <see cref="Repository.FluentMappings.BarMap" /> type
    ''' </summary>
    <TestClass>
    Public Class BarMapTests

        ''' <summary>
        ''' The name of the connection string
        ''' </summary>
        Private Const ConnectionStringName = "IntegrationTests"

        ''' <summary>
        ''' Tests the constructor of the <see cref="Repository.FluentMappings.BarMap" /> type
        ''' </summary>
        <TestMethod>
        Public Sub New_NoParameters_FieldsAreCorrectlyMapped()

            Try

                ' Drop the "Bar" table if it exists already
                Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [dbo].[Bar].sql", ConnectionStringName)

                ' Create the "Bar" table
                Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.CREATE TABLE [dbo].[Bar].sql", ConnectionStringName)

                ' Arrange
                Using session As ISession = New SessionFactory().GetNewSession()

                    ' Get the length from the mapped property 
                    '.CheckProperty(x => x.TimestampColumn, Utilities.Random.NextBytes(8)) not able to test a timestamp property a specific value can't be inserted, nor can the column be updated. 

                    ' Act & Assert
                    Dim result = New PersistenceSpecification(Of Bar)(session) _
                            .CheckProperty(Function(x) x.Id, Guid.NewGuid()) _
                            .CheckProperty(Function(x) x.BigIntColumn, Utilities.Random.NextInt64())  _
                            .CheckProperty(Function(x) x.BinaryColumn, Utilities.Random.NextBytes(50))  _ 
                            .CheckProperty(Function(x) x.BitColumn, Utilities.Random.NextBoolean()) _
                            .CheckProperty(Function(x) x.CharColumn, Utilities.Random.NextString()) _
                            .CheckProperty(Function(x) x.DateColumn, Nothing) _
                            .CheckProperty(Function(x) x.DateTimeColumn, Utilities.Random.NextDateTime(New DateTime(1753, 1, 1), DateTime.MaxValue), New DateTimeEqualityComparer(New TimeSpan(0, 0, 1, 0))) _
                            .CheckProperty(Function(x) x.DateTime2Column, Utilities.Random.NextDateTime()) _
                            .CheckProperty(Function(x) x.DateTimeOffsetColumn, Utilities.Random.NextDateTimeOffset()) _
                            .CheckProperty(Function(x) x.DecimalColumn, CType(Utilities.Random.NextInt16(-1, 1), Decimal)) _
                            .CheckProperty(Function(x) x.FloatColumn, Utilities.Random.NextDouble()) _
                            .CheckProperty(Function(x) x.GeographyColumn, Nothing) _
                            .CheckProperty(Function(x) x.GeometryColumn, Nothing) _
                            .CheckProperty(Function(x) x.HierarchyIdColumn, Nothing) _
                            .CheckProperty(Function(x) x.ImageColumn, Utilities.Random.NextBytes())  _
                            .CheckProperty(Function(x) x.IntColumn, Utilities.Random.NextInt32())  _
                            .CheckProperty(Function(x) x.MoneyColumn, CType(Utilities.Random.NextDouble(-922337203685477.5808, 922337203685477.62), Decimal))  _
                            .CheckProperty(Function(x) x.NCharColumn, Utilities.Random.NextString())  _
                            .CheckProperty(Function(x) x.NTextColumn, Utilities.Random.NextString(1))  _
                            .CheckProperty(Function(x) x.NumericColumn, CType(Utilities.Random.NextInt16(-1, 1), Decimal))  _
                            .CheckProperty(Function(x) x.NVarCharColumn, Utilities.Random.NextString(1))  _
                            .CheckProperty(Function(x) x.RealColumn, Utilities.Random.NextSingle(-0.9F, 0.9F))  _
                            .CheckProperty(Function(x) x.SmallDateTimeColumn, Utilities.Random.NextDateTime(New DateTime(1900, 1, 1), New DateTime(2079, 6, 6)), New DateTimeEqualityComparer(New TimeSpan(0, 0, 1, 0)))  _
                            .CheckProperty(Function(x) x.SmallIntColumn, Utilities.Random.NextInt16())  _
                            .CheckProperty(Function(x) x.SmallMoneyColumn, CType(Utilities.Random.NextInt16(-1, 1), Decimal))  _
                            .CheckProperty(Function(x) x.SqlVariantColumn, Utilities.Random.NextString(1))  _
                            .CheckProperty(Function(x) x.TextColumn, Utilities.Random.NextString(1)) _
                            .CheckProperty(Function(x) x.TimeColumn, Utilities.Random.NextTimeSpan(), New TimeSpanEqualityComparer(New TimeSpan(0, 0, 1, 0))) _
                            .CheckProperty(Function(x) x.TinyIntColumn, Utilities.Random.NextByte()) _
                            .CheckProperty(Function(x) x.UniqueIdentifierColumn, Guid.NewGuid()) _
                            .CheckProperty(Function(x) x.VarBinaryColumn, Utilities.Random.NextBytes(1)) _
                            .CheckProperty(Function(x) x.VarCharColumn, Utilities.Random.NextString(1)) _
                            .CheckProperty(Function(x) x.XmlColumn, "<Bar />") _
                            .VerifyTheMappings()

                    ' Assert
                    Assert.IsNotNull(result)

                End Using

            Finally

                ' Drop the "Bar" table if it still exists
                Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [dbo].[Bar].sql", ConnectionStringName)

            End Try

        End Sub

    End Class

End NameSpace