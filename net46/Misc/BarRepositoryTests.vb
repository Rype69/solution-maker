﻿' --------------------------------------------------------------------------------------------------------------------
' <copyright file="BarRepositoryTests.cs" company="Inspire IT Ltd">
'   Copyright © Inspire IT Ltd. All Rights Reserved.
' </copyright>
' --------------------------------------------------------------------------------------------------------------------

' TODO: What to do if the type doesn't have any properties other than the identifier

Imports System.Globalization

Imports RyanPenfold.TestProject.Model

Imports RyanPenfold.Utilities.Text

''' <summary>
''' Provides integration tests for the <see cref="BarRepository" /> class
''' </summary>
<TestClass>
Public Class BarRepositoryTests

    ' NB: C# manifest resource strings have the "SQL" sub namespace but VB.

    ''' <summary>
    ''' The name of the connection string
    ''' </summary>
    Private Const ConnectionStringName = "IntegrationTests"

    ''' <summary>
    ''' Tests the <see cref="BarRepository.FindAll" /> method.
    ''' </summary>
    <TestMethod>
    Public Sub FindAll_NoParameters_FindsAllObjects()

        Try

            ' Drop the "Bar" table if it exists already
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [dbo].[Bar].sql", ConnectionStringName)

            ' Create the "Bar" table
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.CREATE TABLE [dbo].[Bar].sql", ConnectionStringName)

            ' Create new repository
            Dim repository = New BarRepository()

            ' Get list of results
            Dim results = repository.FindAll()

            ' Assert that the results is instantiated
            Assert.IsNotNull(results)

            ' Assert that the results contains zero objects
            Assert.AreEqual(0, results.Count)

            ' List of entities generated by the insert method
            Dim insertedEntities = New List(Of Bar)()

            ' Determine the amount of records 
            Dim amountOfRecords = Utilities.Random.NextInt32(1, 10)

            ' Insert into the table
            For i = 0 To amountOfRecords - 1
                Dim instance = InitialiseBar()
                insertedEntities.Add(instance)
                InsertRecord(instance)
            Next

            ' Re-select all the data
            results = repository.FindAll()

            ' Assert that the results is instantiated
            Assert.IsNotNull(results)

            ' Assert that the results contains the amount objects that were inserted
            Assert.AreEqual(amountOfRecords, results.Count)

            ' Attempt to find the objects and assert that they are as expected
            For Each result In results

                AssertEqualPropertyValues(insertedEntities.First(Function(r) r.Id = result.Id), result)

            Next

        Finally

            ' Drop the "Bar" table if it still exists
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [dbo].[Bar].sql", ConnectionStringName)

        End Try

    End Sub

    ''' <summary>
    ''' Tests the <see cref="BarRepository.FindById" /> method.
    ''' </summary>
    <TestMethod>
    Public Sub FindById_GivenID_ObjectFound()

        Try

            ' Drop the "Bar" table if it exists already
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [dbo].[Bar].sql", ConnectionStringName)

            ' Create the "Bar" table
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.CREATE TABLE [dbo].[Bar].sql", ConnectionStringName)

            ' List of entities generated by the insert method
            Dim insertedEntities = New List(Of Bar)()

            ' Determine the amount of records 
            Dim amountOfRecords = Utilities.Random.NextInt32(1, 10)

            ' Insert into the table
            For i = 0 To amountOfRecords - 1

                Dim instance = InitialiseBar()
                insertedEntities.Add(instance)
                InsertRecord(instance)

            Next

            ' Create new repository
            Dim repository = New BarRepository()

            ' Attempt to find the objects
            For Each insertedEntity In insertedEntities

                Dim retrievedEntity = repository.FindById(insertedEntity.Id)
                Assert.IsNotNull(retrievedEntity)
                AssertEqualPropertyValues(insertedEntity, retrievedEntity)

            Next

        Finally

            ' Drop the "Bar" table if it still exists
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [dbo].[Bar].sql", ConnectionStringName)

        End Try

    End Sub

    ''' <summary>
    ''' Tests the <see cref="BarRepository.Remove" /> method.
    ''' </summary>
    <TestMethod>
    Public Sub Remove_ObjectToRemove_Removed()

        Try

            ' Drop the "Bar" table if it exists already
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [dbo].[Bar].sql", ConnectionStringName)

            ' Create the "Bar" table
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.CREATE TABLE [dbo].[Bar].sql", ConnectionStringName)

            ' List of entities generated by the insert method
            Dim insertedEntities = New List(Of Bar)()

            ' Determine the amount of records 
            Dim amountOfRecords = Utilities.Random.NextInt32(1, 10)

            ' Insert into the table
            For i = 0 To amountOfRecords - 1

                Dim instance = InitialiseBar()
                insertedEntities.Add(instance)
                InsertRecord(instance)

            Next

            ' Select count from table, it should be equal to amountOfRecords
            Dim count = Utilities.Data.SqlClient.SqlCommand.RunExecuteScalar("SELECT COUNT(*) FROM [Bar];", ConnectionStringName)
            Assert.AreEqual(amountOfRecords, count)

            ' Create new repository
            Dim repository = New BarRepository()

            ' Attempt to remove
            Dim removeCount = 0
            For Each insertedEntity In insertedEntities

                removeCount = removeCount + 1
                repository.Remove(insertedEntity)

                ' Select count from table, it should be equal to amountOfRecords
                Dim newCount = Utilities.Data.SqlClient.SqlCommand.RunExecuteScalar("SELECT COUNT(*) FROM [Bar];", ConnectionStringName)
                Assert.AreEqual(amountOfRecords - removeCount, newCount)

            Next

        Finally

            ' Drop the "Bar" table if it still exists
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [dbo].[Bar].sql", ConnectionStringName)

        End Try

    End Sub

    ''' <summary>
    ''' Tests the <see cref="BarRepository.Save" /> method.
    ''' </summary>
    <TestMethod>
    Public Sub Save_ObjectToSave_Saved()

        Try

            ' Drop the "Bar" table if it exists already
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [dbo].[Bar].sql", ConnectionStringName)

            ' Create the "Bar" table
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.CREATE TABLE [dbo].[Bar].sql", ConnectionStringName)

            ' Create a new instance
            Dim instance = InitialiseBar()

            ' Create new repository
            Dim repository = New BarRepository()

            ' Attempt to insert the instance
            repository.Save(instance)

            ' Select count from table, it should be one
            Dim countAfterInsert = Utilities.Data.SqlClient.SqlCommand.RunExecuteScalar("SELECT COUNT(*) FROM [Bar];", ConnectionStringName)
            Assert.AreEqual(1, countAfterInsert)

            ' Re-initialise the instance
            InitialiseBar(instance)

            ' Attempt to insert the instance
            repository.Save(instance)

            ' Select count from table, it should still be one
            Dim countAfterUpdate = Utilities.Data.SqlClient.SqlCommand.RunExecuteScalar("SELECT COUNT(*) FROM [Bar];", ConnectionStringName)
            Assert.AreEqual(1, countAfterUpdate)

        Finally

            ' Drop the "Bar" table if it still exists
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [dbo].[Bar].sql", ConnectionStringName)

        End Try

    End Sub

    ''' <summary>
    ''' Asserts each of the property values on each of the instances are equal.
    ''' </summary>
    ''' <param name="expected">Instance containing expected property values.</param>
    ''' <param name="actual">Instance containing actual property values.</param>
    Private Sub AssertEqualPropertyValues(expected As Bar, actual As Bar)

        If expected Is Nothing Then

            Throw New ArgumentNullException("expected")

        End If

        If actual Is Nothing Then

            Throw New ArgumentNullException("actual")

        End If

        Assert.AreEqual(expected.Id, actual.Id)
        Assert.AreEqual(expected.BigIntColumn, actual.BigIntColumn)
        Assert.AreEqual(BitConverter.ToString(expected.BinaryColumn), BitConverter.ToString(actual.BinaryColumn))
        Assert.AreEqual(expected.BitColumn, actual.BitColumn)
        Assert.AreEqual(expected.CharColumn, actual.CharColumn)
        Assert.AreEqual(expected.DateColumn, actual.DateColumn)

        If expected.DateTimeColumn.HasValue AndAlso actual.DateTimeColumn.HasValue Then

            Assert.AreEqual(expected.DateTimeColumn.Value.ToString("dd/MM/yyyy HH:mm:ss"), actual.DateTimeColumn.Value.ToString("dd/MM/yyyy HH:mm:ss"))

        End If

        If expected.DateTime2Column.HasValue AndAlso actual.DateTime2Column.HasValue Then

            Assert.AreEqual(expected.DateTime2Column.Value.ToString("dd/MM/yyyy HH:mm:ss"), actual.DateTime2Column.Value.ToString("dd/MM/yyyy HH:mm:ss"))

        End If

        If expected.DateTimeOffsetColumn.HasValue AndAlso actual.DateTimeOffsetColumn.HasValue Then

            Assert.AreEqual(expected.DateTimeOffsetColumn.Value.ToString("yyyy-MM-dd HH:mm:ss %K"), actual.DateTimeOffsetColumn.Value.ToString("yyyy-MM-dd HH:mm:ss %K"))

        End If

        Assert.AreEqual(expected.DecimalColumn, actual.DecimalColumn)

        If expected.FloatColumn.HasValue AndAlso actual.FloatColumn.HasValue Then

            Assert.AreEqual(expected.FloatColumn.Value.ToString(CultureInfo.InvariantCulture), actual.FloatColumn.Value.ToString(CultureInfo.InvariantCulture))

        End If

        Assert.AreEqual(expected.GeographyColumn, actual.GeographyColumn)
        Assert.AreEqual(expected.GeometryColumn, actual.GeometryColumn)
        Assert.AreEqual(expected.HierarchyIdColumn, actual.HierarchyIdColumn)
        Assert.AreEqual(BitConverter.ToString(expected.ImageColumn), BitConverter.ToString(actual.ImageColumn))
        Assert.AreEqual(expected.IntColumn, actual.IntColumn)
        Assert.AreEqual(expected.MoneyColumn, actual.MoneyColumn)
        Assert.AreEqual(expected.NCharColumn, actual.NCharColumn)
        Assert.AreEqual(expected.NTextColumn, actual.NTextColumn)
        Assert.AreEqual(expected.NumericColumn, actual.NumericColumn)
        Assert.AreEqual(expected.NVarCharColumn, actual.NVarCharColumn)

        If expected.RealColumn.HasValue AndAlso actual.RealColumn.HasValue Then

            Assert.AreEqual(expected.RealColumn.Value.ToString(CultureInfo.InvariantCulture), actual.RealColumn.Value.ToString(CultureInfo.InvariantCulture))
        End If

        Assert.AreEqual(expected.SmallDateTimeColumn, actual.SmallDateTimeColumn)
        Assert.AreEqual(expected.SmallIntColumn, actual.SmallIntColumn)
        Assert.AreEqual(expected.SmallMoneyColumn, actual.SmallMoneyColumn)
        Assert.AreEqual(expected.SqlVariantColumn, actual.SqlVariantColumn)
        Assert.AreEqual(expected.TextColumn, actual.TextColumn)
        Assert.AreEqual(expected.TimeColumn, actual.TimeColumn)
        'Assert.AreEqual(BitConverter.ToString(expected.TimestampColumn), BitConverter.ToString(actual.TimestampColumn)) not able to test a timestamp property a specific value can't be inserted, nor can the column be updated. 
        Assert.AreEqual(expected.TinyIntColumn, actual.TinyIntColumn)
        Assert.AreEqual(expected.UniqueIdentifierColumn, actual.UniqueIdentifierColumn)
        Assert.AreEqual(BitConverter.ToString(expected.VarBinaryColumn), BitConverter.ToString(actual.VarBinaryColumn))
        Assert.AreEqual(expected.VarCharColumn, actual.VarCharColumn)
        Assert.AreEqual(expected.XmlColumn, actual.XmlColumn)

    End Sub

    ''' <summary>
    ''' Creates a <see cref="Bar"/> with random property values.
    ''' </summary>
    ''' <param name="instance">An instance to initialise.</param>
    ''' <returns>A <see cref="Bar"/>.</returns>
    Private Function InitialiseBar(Optional instance As Bar = Nothing) As Bar

        If instance Is Nothing Then

            instance = New Bar With {.Id = Guid.NewGuid()}

        End If

        ' TODO: In solution maker, determine the value for any optional parameters of the random methods
        instance.BigIntColumn = Utilities.Random.NextInt64()
        instance.BinaryColumn = Utilities.Random.NextBytes(50) ' Get the length from the property mapping, the default is 16 
        instance.BitColumn = Utilities.Random.NextBoolean()
        instance.CharColumn = Utilities.Random.NextString().Replace("'", ";") ' Get the length from the property mapping, the default is 10 
        instance.DateColumn = Utilities.Random.NextDateTime(False)
        instance.DateTimeColumn = Utilities.Random.NextDateTime(New DateTime(1753, 1, 1), DateTime.MaxValue)
        instance.DateTime2Column = Utilities.Random.NextDateTime()
        instance.DateTimeOffsetColumn = Utilities.Random.NextDateTimeOffset()
        instance.DecimalColumn = Utilities.Random.NextInt16(-1, 1)
        instance.FloatColumn = Utilities.Random.NextDouble()
        instance.GeographyColumn = Nothing
        instance.GeometryColumn = Nothing
        instance.HierarchyIdColumn = Nothing
        instance.ImageColumn = Utilities.Random.NextBytes()
        instance.IntColumn = Utilities.Random.NextInt32()
        instance.MoneyColumn = CType(Utilities.Random.NextDouble(-922337203685477.62, 922337203685477.62), Decimal)
        instance.NCharColumn = Utilities.Random.NextString().Replace("'", ";")
        instance.NTextColumn = Utilities.Random.NextString(1).Replace("'", ";")
        instance.NumericColumn = Utilities.Random.NextInt16(-1, 1)
        instance.NVarCharColumn = Utilities.Random.NextString(1).Replace("'", ";")
        instance.RealColumn = Utilities.Random.NextSingle(-0.9F, 0.9F)
        instance.SmallDateTimeColumn = Utilities.Random.NextDateTime(New DateTime(1900, 1, 1), New DateTime(2079, 6, 6), False)
        instance.SmallIntColumn = Utilities.Random.NextInt16()
        instance.SmallMoneyColumn = CType(Utilities.Random.NextSingle(-214748.359F, 214748.359F), Decimal)
        instance.SqlVariantColumn = Utilities.Random.NextString(1).Replace("'", ";")
        instance.TextColumn = Utilities.Random.NextString(1).Replace("'", ";")
        instance.TimeColumn = Utilities.Random.NextTimeSpan()
        ' instance.TimestampColumn = Utilities.Random.NextBytes(8) not able to test a timestamp property a specific value can't be inserted, nor can the column be updated. 
        instance.TinyIntColumn = Utilities.Random.NextByte()
        instance.UniqueIdentifierColumn = Guid.NewGuid()
        instance.VarBinaryColumn = Utilities.Random.NextBytes(1)
        instance.VarCharColumn = Utilities.Random.NextString(1).Replace("'", ";")
        instance.XmlColumn = "<Bar />"

        Return instance
    End Function

    ''' <summary>
    ''' Inserts a record into the mapped table
    ''' </summary>
    Private Sub InsertRecord(entity As Bar)

        ' NULL-check the entity
        If entity Is Nothing Then

            Throw New ArgumentNullException("entity")

        End If

        Dim sqlcommandBuilder = New Text.StringBuilder()

        sqlcommandBuilder.AppendWithDelimiter("INSERT INTO [dbo].[Bar]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("(", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("    [dbo].[Bar].[id]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[BigIntColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[BinaryColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[BitColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[CharColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[DateColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[DateTimeColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[DateTime2Column]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[DateTimeOffsetColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[DecimalColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[FloatColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[GeographyColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[GeometryColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[HierarchyIdColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[ImageColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[IntColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[MoneyColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[NCharColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[NTextColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[NumericColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[NVarCharColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[RealColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[SmallDateTimeColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[SmallIntColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[SmallMoneyColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[SqlVariantColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[TextColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[TimeColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[TimestampColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[TinyIntColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[UniqueIdentifierColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[VarBinaryColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[VarCharColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[dbo].[Bar].[XmlColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(")", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("VALUES", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("(", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("   '{0}'", {entity.Id}), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,{0}", entity.BigIntColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,0x{0}", String.Concat(entity.BinaryColumn.Select(Function(b) b.ToString("X2")).ToArray())), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(If(entity.BitColumn.HasValue, String.Format("  , {0}", Utilities.Boolean.ToByte(entity.BitColumn.Value)), ", NULL"), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,'{0}'", entity.CharColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(If(entity.DateColumn.HasValue, String.Format("  ,'{0}'", entity.DateColumn.Value.ToString("yyyy-MM-dd")), ", NULL"), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(If(entity.DateTimeColumn.HasValue, String.Format("  ,'{0}'", entity.DateTimeColumn.Value.ToString("yyyy-MM-dd HH:mm:ss")), ", NULL"), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(If(entity.DateTime2Column.HasValue, String.Format("  ,'{0}'", entity.DateTime2Column.Value.ToString("yyyy-MM-dd HH:mm:ss")), ", NULL"), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(If(entity.DateTimeOffsetColumn.HasValue, String.Format("  ,'{0}'", entity.DateTimeOffsetColumn.Value.ToString("yyyy-MM-dd HH:mm:ss %K")), ", NULL"), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,{0}", entity.DecimalColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,{0}", entity.FloatColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("  ,NULL", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("  ,NULL", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("  ,NULL", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,0x{0}", String.Concat(entity.ImageColumn.Select(Function(b) b.ToString("X2")).ToArray())), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,{0}", entity.IntColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,{0}", entity.MoneyColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,'{0}'", entity.NCharColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,'{0}'", entity.NTextColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,{0}", entity.NumericColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,'{0}'", entity.NVarCharColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,{0}", entity.RealColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(If(entity.SmallDateTimeColumn.HasValue, String.Format("  ,'{0}'", entity.SmallDateTimeColumn.Value.ToString("yyyy-MM-dd HH:mm:ss")), ", NULL"), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,{0}", entity.SmallIntColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,{0}", entity.SmallMoneyColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,'{0}'", entity.SqlVariantColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,'{0}'", entity.TextColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,'{0}'", entity.TimeColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("  ,DEFAULT", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,'{0}'", entity.TinyIntColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,'{0}'", entity.UniqueIdentifierColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,0x{0}", String.Concat(entity.VarBinaryColumn.Select(Function(b) b.ToString("X2")).ToArray())), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,'{0}'", entity.VarCharColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(String.Format("  ,'{0}'", entity.XmlColumn), vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter(")", vbCrLf)

        Utilities.Data.SqlClient.SqlCommand.RunExecuteScalar(
            sqlcommandBuilder.ToString(),
            ConnectionStringName)

    End Sub

End Class
