// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsFile.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core.IO
{
    using System;

    /// <summary>
    /// Provides functionality for accessing a settings file
    /// </summary>
    public class SettingsFile : SerializedDataFile<SettingsInfo>, ISettingsFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsFile"/> class
        /// </summary>
        public SettingsFile() : this(IocContainer.Resolver.Resolve<ISettingsInfo>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsFile"/> class
        /// </summary>
        /// <param name="settings">Settings data</param>
        public SettingsFile(ISettingsInfo settings)
        {
            // NULL-check the settings parameter
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            // Set the settings property
            this.Data = settings as SettingsInfo;

            // Set the default path value
            this.Path = $"{System.IO.Directory.GetCurrentDirectory()}\\Settings.dat";
        }
    }
}
