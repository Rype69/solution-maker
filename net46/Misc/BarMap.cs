// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BarMap.cs" company="Inspire IT Ltd">
//   Copyright © Inspire IT Ltd. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.TestProject.Repository.FluentMappings
{
    using FluentNHibernate.Mapping;

    using Model;

    /// <summary>
    /// Defines a mapping for type <see cref="Bar"/>.
    /// </summary>
    public class BarMap : ClassMap<Bar>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarMap"/> class.
        /// </summary>
        public BarMap()
        {
            this.Id(x => x.Id).GeneratedBy.Assigned();
            this.Map(x => x.BigIntColumn);
            this.Map(x => x.BinaryColumn);
            this.Map(x => x.BitColumn);
            this.Map(x => x.CharColumn);
            this.Map(x => x.DateColumn).CustomType("date");
            this.Map(x => x.DateTimeColumn);
            this.Map(x => x.DateTime2Column).CustomType("datetime2");
            this.Map(x => x.DateTimeOffsetColumn);
            this.Map(x => x.DecimalColumn);
            this.Map(x => x.FloatColumn);
            this.Map(x => x.GeographyColumn);
            this.Map(x => x.GeometryColumn);
            this.Map(x => x.HierarchyIdColumn);
            this.Map(x => x.ImageColumn);
            this.Map(x => x.IntColumn);
            this.Map(x => x.MoneyColumn);
            this.Map(x => x.NCharColumn);
            this.Map(x => x.NTextColumn);
            this.Map(x => x.NumericColumn);
            this.Map(x => x.NVarCharColumn);
            this.Map(x => x.RealColumn);
            this.Map(x => x.SmallDateTimeColumn);
            this.Map(x => x.SmallIntColumn);
            this.Map(x => x.SmallMoneyColumn);
            this.Map(x => x.SqlVariantColumn);
            this.Map(x => x.TextColumn);
            this.Map(x => x.TimeColumn).CustomType("TimeAsTimeSpan");
            this.Map(x => x.TimestampColumn).ReadOnly();
            this.Map(x => x.TinyIntColumn);
            this.Map(x => x.UniqueIdentifierColumn);
            this.Map(x => x.VarBinaryColumn);
            this.Map(x => x.VarCharColumn);
            this.Map(x => x.XmlColumn);
            this.Table("Bar");
            this.Schema("dbo");
        }
    }
}