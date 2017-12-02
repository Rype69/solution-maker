' --------------------------------------------------------------------------------------------------------------------
' <copyright file="BarRepository.vb" company="Inspire IT Ltd">
'   Copyright © Inspire IT Ltd. All Rights Reserved.
' </copyright>
' --------------------------------------------------------------------------------------------------------------------

Imports RyanPenfold.NHibernateRepository
Imports RyanPenfold.TestProject.Model
Imports RyanPenfold.TestProject.Model.RepositoryInterfaces

''' <summary>
''' Provides data access functionality relating to <see cref="Bar" /> objects.
''' </summary>
Public Class BarRepository
	Inherits BaseRepository(Of Bar)
	Implements IBarRepository

End Class
