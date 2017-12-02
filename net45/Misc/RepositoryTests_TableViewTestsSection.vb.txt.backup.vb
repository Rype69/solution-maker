﻿    ''' <summary>
    ''' The name of the connection string
    ''' </summary>
    Private Const ConnectionStringName = "IntegrationTests"

    ''' <summary>
    ''' Tests the <see cref="{className}Repository.FindAll" /> method.
    ''' </summary>
    <TestMethod>
    Public Sub FindAll_NoParameters_FindsAllObjects()

        Try

            ' Drop the "{className}" table if it exists already
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [{sourceObjectSchema}].[{sourceObjectName}].sql", ConnectionStringName)

            ' Create the "{className}" table
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.CREATE TABLE [{sourceObjectSchema}].[{sourceObjectName}].sql", ConnectionStringName)

            ' Create new repository
            Dim repository = New {className}Repository()

            ' Get list of results
            Dim results = repository.FindAll()

            ' Assert that the results is instantiated
            Assert.IsNotNull(results)

            ' Assert that the results contains zero objects
            Assert.AreEqual(0, results.Count)

            ' List of entities generated by the insert method
            Dim insertedEntities = New List(Of {className})()

            ' Determine the amount of records 
            Dim amountOfRecords = Utilities.Random.NextInt32(1, 10)

            ' Insert into the table
            For i = 0 To amountOfRecords - 1
                Dim instance = Initialise{className}()
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

            ' Drop the "{className}" table if it still exists
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [{sourceObjectSchema}].[{sourceObjectName}].sql", ConnectionStringName)

        End Try

    End Sub

    ''' <summary>
    ''' Tests the <see cref="{className}Repository.FindById" /> method.
    ''' </summary>
    <TestMethod>
    Public Sub FindById_GivenID_ObjectFound()

        Try

            ' Drop the "{className}" table if it exists already
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [{sourceObjectSchema}].[{sourceObjectName}].sql", ConnectionStringName)

            ' Create the "{className}" table
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.CREATE TABLE [{sourceObjectSchema}].[{sourceObjectName}].sql", ConnectionStringName)

            ' List of entities generated by the insert method
            Dim insertedEntities = New List(Of {className})()

            ' Determine the amount of records 
            Dim amountOfRecords = Utilities.Random.NextInt32(1, 10)

            ' Insert into the table
            For i = 0 To amountOfRecords - 1

                Dim instance = Initialise{className}()
                insertedEntities.Add(instance)
                InsertRecord(instance)

            Next

            ' Create new repository
            Dim repository = New {className}Repository()

            ' Attempt to find the objects
            For Each insertedEntity In insertedEntities

                Dim retrievedEntity = repository.FindById(insertedEntity.Id)
                Assert.IsNotNull(retrievedEntity)
                AssertEqualPropertyValues(insertedEntity, retrievedEntity)

            Next

        Finally

            ' Drop the "{className}" table if it still exists
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [{sourceObjectSchema}].[{sourceObjectName}].sql", ConnectionStringName)

        End Try

    End Sub

    ''' <summary>
    ''' Tests the <see cref="{className}Repository.Remove" /> method.
    ''' </summary>
    <TestMethod>
    Public Sub Remove_ObjectToRemove_Removed()

        Try

            ' Drop the "{className}" table if it exists already
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [{sourceObjectSchema}].[{sourceObjectName}].sql", ConnectionStringName)

            ' Create the "{className}" table
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.CREATE TABLE [{sourceObjectSchema}].[{sourceObjectName}].sql", ConnectionStringName)

            ' List of entities generated by the insert method
            Dim insertedEntities = New List(Of {className})()

            ' Determine the amount of records 
            Dim amountOfRecords = Utilities.Random.NextInt32(1, 10)

            ' Insert into the table
            For i = 0 To amountOfRecords - 1

                Dim instance = Initialise{className}()
                insertedEntities.Add(instance)
                InsertRecord(instance)

            Next

            ' Select count from table, it should be equal to amountOfRecords
            Dim count = Utilities.Data.SqlClient.SqlCommand.RunExecuteScalar("SELECT COUNT(*) FROM [{sourceObjectSchema}].[{sourceObjectName}];", ConnectionStringName)
            Assert.AreEqual(amountOfRecords, count)

            ' Create new repository
            Dim repository = New {className}Repository()

            ' Attempt to remove
            Dim removeCount = 0
            For Each insertedEntity In insertedEntities

                removeCount = removeCount + 1
                repository.Remove(insertedEntity)

                ' Select count from table, it should be equal to amountOfRecords
                Dim newCount = Utilities.Data.SqlClient.SqlCommand.RunExecuteScalar("SELECT COUNT(*) FROM [{className}];", ConnectionStringName)
                Assert.AreEqual(amountOfRecords - removeCount, newCount)

            Next

        Finally

            ' Drop the "{className}" table if it still exists
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [{sourceObjectSchema}].[{sourceObjectName}].sql", ConnectionStringName)

        End Try

    End Sub

    ''' <summary>
    ''' Tests the <see cref="{className}Repository.Save" /> method.
    ''' </summary>
    <TestMethod>
    Public Sub Save_ObjectToSave_Saved()

        Try

            ' Drop the "{className}" table if it exists already
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [{sourceObjectSchema}].[{sourceObjectName}].sql", ConnectionStringName)

            ' Create the "{className}" table
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.CREATE TABLE [{sourceObjectSchema}].[{sourceObjectName}].sql", ConnectionStringName)

            ' Create a new instance
            Dim instance = Initialise{className}()

            ' Create new repository
            Dim repository = New {className}Repository()

            ' Attempt to insert the instance
            repository.Save(instance)

            ' Select count from table, it should be one
            Dim countAfterInsert = Utilities.Data.SqlClient.SqlCommand.RunExecuteScalar("SELECT COUNT(*) FROM [{sourceObjectSchema}].[{sourceObjectName}];", ConnectionStringName)
            Assert.AreEqual(1, countAfterInsert)

            ' Re-initialise the instance
            Initialise{className}(instance)

            ' Attempt to insert the instance
            repository.Save(instance)

            ' Select count from table, it should still be one
            Dim countAfterUpdate = Utilities.Data.SqlClient.SqlCommand.RunExecuteScalar("SELECT COUNT(*) FROM [{sourceObjectSchema}].[{sourceObjectName}];", ConnectionStringName)
            Assert.AreEqual(1, countAfterUpdate)

        Finally

            ' Drop the "{className}" table if it still exists
            Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(Me.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.DROP TABLE [{sourceObjectSchema}].[{sourceObjectName}].sql", ConnectionStringName)

        End Try

    End Sub

    ''' <summary>
    ''' Asserts each of the property values on each of the instances are equal.
    ''' </summary>
    ''' <param name="expected">Instance containing expected property values.</param>
    ''' <param name="actual">Instance containing actual property values.</param>
    Private Sub AssertEqualPropertyValues(expected As {className}, actual As {className})

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
    ''' Creates a <see cref="{className}"/> with random property values.
    ''' </summary>
    ''' <param name="instance">An instance to initialise.</param>
    ''' <returns>A <see cref="{className}"/>.</returns>
    Private Function Initialise{className}(Optional instance As {className} = Nothing) As {className}

        If instance Is Nothing Then

            instance = New {className} With {.Id = Guid.NewGuid()}

        End If

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
        instance.XmlColumn = "<{className} />"

        Return instance
    End Function

    ''' <summary>
    ''' Inserts a record into the mapped table
    ''' </summary>
    Private Sub InsertRecord(entity As {className})

        ' NULL-check the entity
        If entity Is Nothing Then

            Throw New ArgumentNullException("entity")

        End If

        Dim sqlcommandBuilder = New Text.StringBuilder()

        sqlcommandBuilder.AppendWithDelimiter("INSERT INTO [{sourceObjectSchema}].[{sourceObjectName}]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("(", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("    [{sourceObjectSchema}].[{sourceObjectName}].[id]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[BigIntColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[BinaryColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[BitColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[CharColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[DateColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[DateTimeColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[DateTime2Column]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[DateTimeOffsetColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[DecimalColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[FloatColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[GeographyColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[GeometryColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[HierarchyIdColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[ImageColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[IntColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[MoneyColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[NCharColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[NTextColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[NumericColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[NVarCharColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[RealColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[SmallDateTimeColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[SmallIntColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[SmallMoneyColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[SqlVariantColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[TextColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[TimeColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[TimestampColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[TinyIntColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[UniqueIdentifierColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[VarBinaryColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[VarCharColumn]", vbCrLf)
        sqlcommandBuilder.AppendWithDelimiter("   ,[{sourceObjectSchema}].[{sourceObjectName}].[XmlColumn]", vbCrLf)
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