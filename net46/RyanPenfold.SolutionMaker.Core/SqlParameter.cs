// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlParameter.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System;
    using System.Data;

    /// <summary>
    /// A stored procedure parameter.
    /// </summary>
    /// <remarks>
    /// Adapter pattern.
    /// </remarks>
    public class SqlParameter : ISqlParameter
    {
        /// <summary>
        /// A SQL stored procedure parameter.
        /// </summary>
        private readonly System.Data.SqlClient.SqlParameter sqlParameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlParameter"/> class.
        /// </summary>
        /// <param name="sqlParameter">A SQL parameter.</param>
        public SqlParameter(System.Data.SqlClient.SqlParameter sqlParameter)
        {
            if (sqlParameter == null)
            {
                throw new ArgumentNullException();
            }

            this.sqlParameter = sqlParameter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlParameter"/> class.
        /// </summary>
        public SqlParameter() : this(new System.Data.SqlClient.SqlParameter())
        {
        }

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        public string Name
        {
            get
            {
                return this.sqlParameter.ParameterName;
            }

            set
            {
                this.sqlParameter.ParameterName = value;
            }
        }

        /// <summary>
        /// Gets or sets the data type of the parameter.
        /// </summary>
        public SqlDbType Type
        {
            get
            {
                return this.sqlParameter.SqlDbType;
            }

            set
            {
                this.sqlParameter.SqlDbType = value;
            }
        }

        /// <summary>
        /// Gets or sets the direction of the parameter - either input or output.
        /// </summary>
        public ParameterDirection Direction
        {
            get
            {
                return this.sqlParameter.Direction;
            }

            set
            {
                this.sqlParameter.Direction = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a NULL value is to be passed.
        /// </summary>
        public bool PassNullValue { get; set; }

        /// <summary>
        /// Gets the size of the parameter.
        /// </summary>
        public int Size => this.sqlParameter.Size;

        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        public object Value
        {
            get
            {
                return this.sqlParameter.Value;
            }

            set
            {
                this.sqlParameter.Value = this.PassNullValue ? null : value;
            }
        }
    }
}
