using System.Collections.Generic;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Defines the importation result
    /// </summary>
    public class ImportResult
    {

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        public IList<ImportFileResult> Results { get; set; } = new List<ImportFileResult>();
    }

    /// <summary>
    /// Import file result
    /// </summary>
    public class ImportFileResult
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the updated list.
        /// </summary>
        /// <value>
        /// The updated.
        /// </value>
        public IList<string> Updated { get; set; } = new List<string>();


        /// <summary>
        /// Gets or sets the created list.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        public IList<string> Created { get; set; } = new List<string>();
    }
}
