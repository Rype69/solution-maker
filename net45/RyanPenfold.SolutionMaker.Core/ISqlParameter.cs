// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqlParameter.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System.Data;

    /// <summary>
    /// A stored procedure parameter.
    /// </summary>
    public interface ISqlParameter
    {
        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        string Name { get; set; } // Starts with an "@"

        /// <summary>
        /// Gets or sets the data type of the parameter.
        /// </summary>
        SqlDbType Type { get; set; }

        /// <summary>
        /// Gets or sets the direction of the parameter - either input or output.
        /// </summary>
        ParameterDirection Direction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a NULL value is to be passed.
        /// </summary>
        bool PassNullValue { get; set; }

        /// <summary>
        /// Gets the size of the parameter.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        object Value { get; set; }
    }
}
