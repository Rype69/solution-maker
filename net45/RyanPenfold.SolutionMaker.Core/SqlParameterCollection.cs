// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlParameterCollection.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// Provides access to a set of <see cref="ISqlParameter"/>s.
    /// </summary>
    public class SqlParameterCollection : ISqlParameterCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlParameterCollection"/> class. 
        /// </summary>
        public SqlParameterCollection()
        {
            this.Items = new List<ISqlParameter>();
        }

        /// <summary>
        /// Gets the set of stored procedure parameters.
        /// </summary>
        public IList<ISqlParameter> Items { get; }

        /// <summary>
        /// Adds a <see cref="ISqlParameter"/> to the collection.
        /// </summary>
        /// <param name="parameter">The <see cref="ISqlParameter"/> to add.</param>
        public virtual void Add(ISqlParameter parameter)
        {
            this.Items.Add(parameter);
        }

        /// <summary>
        /// Clears the contents of the <see cref="MappingCollection"/>
        /// </summary>
        public virtual void Clear()
        {
            this.Items.Clear();
        }

        /// <summary>
        /// Produces a string.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this.Items);
        }
    }
}
