// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BarMapTests.cs" company="Inspire IT Ltd">
//     Copyright © Inspire IT Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.TestProject.Repository.Tests.Integration.FluentMappings
{
    using System;

    using FluentNHibernate.Testing;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Model;

    using NHibernateRepository;

    using Utilities.Collections;

    /// <summary>
    /// Provides unit tests for the <see cref="Repository.FluentMappings.BarMap" /> type
    /// </summary>
    [TestClass]
    public class BarMapTests
    {
        /// <summary>
        /// The name of the connection string
        /// </summary>
        private const string ConnectionStringName = "IntegrationTests";

        /// <summary>
        /// Tests the constructor of the <see cref="Repository.FluentMappings.BarMap" /> type
        /// </summary>
        [TestMethod]
        public void New_NoParameters_FieldsAreCorrectlyMapped()
        {
            try
            {
                // Drop the "Bar" table if it exists already
                Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(this.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.SQL.DROP TABLE [dbo].[Bar].sql", ConnectionStringName);

                // Create the "Bar" table
                Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(this.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.SQL.CREATE TABLE [dbo].[Bar].sql", ConnectionStringName);

                // Arrange
                using (var session = new SessionFactory().GetNewSession())
                {
                    // Act & Assert
                    var result = new PersistenceSpecification<Bar>(session)
                        .CheckProperty(x => x.Id, Guid.NewGuid()) 
                        .CheckProperty(x => x.BigIntColumn, Utilities.Random.NextInt64())
                        .CheckProperty(x => x.BinaryColumn, Utilities.Random.NextBytes(50)) /* Get the length from the mapped property */
                        .CheckProperty(x => x.BitColumn, Utilities.Random.NextBoolean())
                        .CheckProperty(x => x.CharColumn, Utilities.Random.NextString()) /* Get the length from the mapped property - 10 is the default */
                        .CheckProperty(x => x.DateColumn, null)
                        .CheckProperty(x => x.DateTimeColumn, Utilities.Random.NextDateTime(new DateTime(1753, 1, 1), DateTime.MaxValue), new DateTimeEqualityComparer(new TimeSpan(0, 0, 1, 0)))
                        .CheckProperty(x => x.DateTime2Column, Utilities.Random.NextDateTime())
                        .CheckProperty(x => x.DateTimeOffsetColumn, Utilities.Random.NextDateTimeOffset())
                        .CheckProperty(x => x.DecimalColumn, (decimal)Utilities.Random.NextInt16(-1, 1))
                        .CheckProperty(x => x.FloatColumn, Utilities.Random.NextDouble())
                        .CheckProperty(x => x.GeographyColumn, null)
                        .CheckProperty(x => x.GeometryColumn, null)
                        .CheckProperty(x => x.HierarchyIdColumn, null)
                        .CheckProperty(x => x.ImageColumn, Utilities.Random.NextBytes()) /* Get the length from the mapped property - 16 is the default */
                        .CheckProperty(x => x.IntColumn, Utilities.Random.NextInt32())
                        .CheckProperty(x => x.MoneyColumn, (decimal)Utilities.Random.NextDouble(-922337203685477.5808, 922337203685477.5807))
                        .CheckProperty(x => x.NCharColumn, Utilities.Random.NextString())  /* Get the length from the mapped property - 10 is the default */
                        .CheckProperty(x => x.NTextColumn, Utilities.Random.NextString(1))
                        .CheckProperty(x => x.NumericColumn, (decimal)Utilities.Random.NextInt16(-1, 1))
                        .CheckProperty(x => x.NVarCharColumn, Utilities.Random.NextString(1))
                        .CheckProperty(x => x.RealColumn, Utilities.Random.NextSingle(-0.9f, 0.9f))
                        .CheckProperty(x => x.SmallDateTimeColumn, Utilities.Random.NextDateTime(new DateTime(1900, 1, 1), new DateTime(2079, 6, 6)), new DateTimeEqualityComparer(new TimeSpan(0, 0, 1, 0)))
                        .CheckProperty(x => x.SmallIntColumn, Utilities.Random.NextInt16())
                        .CheckProperty(x => x.SmallMoneyColumn, (decimal)Utilities.Random.NextInt16(-1, 1))
                        .CheckProperty(x => x.SqlVariantColumn, Utilities.Random.NextString(1))
                        .CheckProperty(x => x.TextColumn, Utilities.Random.NextString(1))
                        .CheckProperty(x => x.TimeColumn, Utilities.Random.NextTimeSpan(), new TimeSpanEqualityComparer(new TimeSpan(0, 0, 1, 0)))
                        /*.CheckProperty(x => x.TimestampColumn, Utilities.Random.NextBytes(8)) not able to test a timestamp property a specific value can't be inserted, nor can the column be updated. */
                        .CheckProperty(x => x.TinyIntColumn, Utilities.Random.NextByte())
                        .CheckProperty(x => x.UniqueIdentifierColumn, Guid.NewGuid())
                        .CheckProperty(x => x.VarBinaryColumn, Utilities.Random.NextBytes(1))
                        .CheckProperty(x => x.VarCharColumn, Utilities.Random.NextString(1))
                        .CheckProperty(x => x.XmlColumn, "<Bar />")
                        .VerifyTheMappings();

                    // Assert
                    Assert.IsNotNull(result);
                }
            }
            finally
            {
                // Drop the "Bar" table if it still exists
                Utilities.Data.SqlClient.SqlCommand.RunExecuteScalarFromManifestResource(this.GetType().Assembly, "RyanPenfold.TestProject.Repository.Tests.Integration.SQL.DROP TABLE [dbo].[Bar].sql", ConnectionStringName);
            }
        }
    }
}
