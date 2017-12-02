// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISettingsFile.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core.IO
{
    /// <summary>
    /// Provides an interface for accessing a settings file
    /// </summary>
    public interface ISettingsFile : ISerializedDataFile<SettingsInfo>
    {
    }
}
