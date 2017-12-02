' --------------------------------------------------------------------------------------------------------------------
' <copyright file="BarMap.vb" company="Inspire IT Ltd">
'   Copyright © Inspire IT Ltd. All Rights Reserved.
' </copyright>
' --------------------------------------------------------------------------------------------------------------------

Imports FluentNHibernate.Mapping

Imports RyanPenfold.TestProject.Model

Namespace FluentMappings

''' <summary>
''' Defines a mapping for type <see cref="Bar"/>.
''' </summary>
Public Class BarMap
	Inherits ClassMap(Of Bar)

	''' <summary>
	''' Initializes a new instance of the <see cref="BarMap"/> class.
	''' </summary>
	Public Sub New()
		Id(Function(x) x.Id).GeneratedBy.Assigned()
		Map(Function(x) x.BigIntColumn)
		Map(Function(x) x.BinaryColumn)
		Map(Function(x) x.BitColumn)
		Map(Function(x) x.CharColumn)
		Map(Function(x) x.DateColumn).CustomType("date")
		Map(Function(x) x.DateTimeColumn)
		Map(Function(x) x.DateTime2Column).CustomType("datetime2")
		Map(Function(x) x.DateTimeOffsetColumn)
		Map(Function(x) x.DecimalColumn)
		Map(Function(x) x.FloatColumn)
		Map(Function(x) x.GeographyColumn)
		Map(Function(x) x.GeometryColumn)
		Map(Function(x) x.HierarchyIdColumn)
		Map(Function(x) x.ImageColumn)
		Map(Function(x) x.IntColumn)
		Map(Function(x) x.MoneyColumn)
		Map(Function(x) x.NCharColumn)
		Map(Function(x) x.NTextColumn)
		Map(Function(x) x.NumericColumn)
		Map(Function(x) x.NVarCharColumn)
		Map(Function(x) x.RealColumn)
		Map(Function(x) x.SmallDateTimeColumn)
		Map(Function(x) x.SmallIntColumn)
		Map(Function(x) x.SmallMoneyColumn)
		Map(Function(x) x.SqlVariantColumn)
		Map(Function(x) x.TextColumn)
		Map(Function(x) x.TimeColumn).CustomType("TimeAsTimeSpan")
		Map(Function(x) x.TimestampColumn).ReadOnly()
		Map(Function(x) x.TinyIntColumn)
		Map(Function(x) x.UniqueIdentifierColumn)
		Map(Function(x) x.VarBinaryColumn)
		Map(Function(x) x.VarCharColumn)
		Map(Function(x) x.XmlColumn)
		Table("Bar")
		Schema("dbo")
	End Sub

End Class

End Namespace