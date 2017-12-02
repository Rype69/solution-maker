// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingType.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    /// <summary>
    /// Denotes the type of a mapping.
    /// </summary>
    public enum MappingType
    {
        /// <summary>
        /// Invalid, default value.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// Denotes that the mapping is from a database stored procedure.
        /// </summary>
        DbStoredProcedure = 1,

        /// <summary>
        /// Denotes that the mapping is from a database table.
        /// </summary>
        DbTable = 2,

        /// <summary>
        /// Denotes that the mapping is from a database table-valued function.
        /// </summary>
        DbTableValuedFunction = 3,

        /// <summary>
        /// Denotes that the mapping is from a database view.
        /// </summary>
        DbView = 4,

        /// <summary>
        /// Denotes that the mapping is from a CLR assembly POCO.
        /// </summary>
        Poco = 5
    }
}
