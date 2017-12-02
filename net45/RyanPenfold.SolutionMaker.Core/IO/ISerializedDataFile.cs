// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializedDataFile.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core.IO
{
    /// <summary>
    /// Provides an interface for accessing a file containing serialized data.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object to serialize.
    /// </typeparam>
    public interface ISerializedDataFile<T>
    {
        /// <summary>
        /// Gets or sets the content of the file.
        /// </summary>
        T Data { get; set; }

        /// <summary>
        /// Gets the location of the file
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Reads the contents of a file
        /// </summary>
        void Load();

        /// <summary>
        /// Writes data to a file
        /// </summary>
        void Save();
    }
}
