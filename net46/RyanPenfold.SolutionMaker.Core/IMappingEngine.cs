// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMappingEngine.cs" company="Inspire IT Ltd">
//   Copyright © Ryan Penfold. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RyanPenfold.SolutionMaker.Core
{
    using System.ComponentModel;

    /// <summary>
    /// Provides an interface for types that 
    /// provide the core functionality for producing mapping code.
    /// </summary>
    public interface IMappingEngine
    {
        /// <summary>
        /// Produces mapping code.
        /// </summary>
        void Process();

        /// <summary>
        /// Produces mapping code.
        /// </summary>
        /// <param name="backgroundWorker">
        /// A background worker to report progress on.
        /// </param>
        void Process(BackgroundWorker backgroundWorker);
    }
}
