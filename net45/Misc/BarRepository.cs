// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BarRepository.cs" company="Inspire IT Ltd">
//   Copyright © Inspire IT Ltd. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.TestProject.Repository
{
    using Model;
    using Model.RepositoryInterfaces;

    using NHibernateRepository;

    /// <summary>
    /// Provides data access functionality relating to <see cref="Bar" /> objects.
    /// </summary>
    public class BarRepository : BaseRepository<Bar>, IBarRepository
    {
    }
}