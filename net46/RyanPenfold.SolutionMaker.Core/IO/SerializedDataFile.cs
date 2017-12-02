// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializedDataFile.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core.IO
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides functionality for accessing a file containing serialized data.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object to serialize.
    /// </typeparam>
    public class SerializedDataFile<T> : ISerializedDataFile<T> where T : new()
    {
        /// <summary>
        /// Gets or sets the content of the file.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets the location of the file
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Reads the contents of a settings file
        /// </summary>
        public virtual void Load()
        {
            // The default contents of the file is a fresh one
            this.Data = new T();

            // Only attempt to read the file if it exists
            if (!System.IO.File.Exists(this.Path))
            {
                return;
            }

            // Read the contents of the file as a string
            var serialised = System.IO.File.ReadAllText(this.Path);

            // If the contents is null, empty, or whitespace, set the settings to default
            if (string.IsNullOrWhiteSpace(serialised))
            {
                return;
            }

            // Attempt to deserialise the settings instance
            try
            {
                this.Data = JsonConvert.DeserializeObject<T>(serialised);
            }
            catch
            {
                this.Data = new T();
            }
        }

        /// <summary>
        /// Writes data to a settings file
        /// </summary>
        public virtual void Save()
        {
            // If the file if it exists, delete it
            if (System.IO.File.Exists(this.Path))
            {
                System.IO.File.Delete(this.Path);
            }

            // Write the data
            System.IO.File.WriteAllText(this.Path, JsonConvert.SerializeObject(this.Data));
        }
    }
}
