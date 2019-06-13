using System;
using System.Collections.Generic;
using System.Text;

namespace UnizenBot.Meta
{
    /// <summary>
    /// Meta settings.
    /// </summary>
    public class MetaSettings
    {
        /// <summary>
        /// The path to a Git installation.
        /// </summary>
        public string GitPath { get; set; }

        /// <summary>
        /// Repository settings.
        /// </summary>
        public List<RepositorySettings> Repositories { get; set; }

        /// <summary>
        /// List of allowable file type settings.
        /// </summary>
        public List<MetaFileSettings> Files { get; set; }
    }

    /// <summary>
    /// Repository settings.
    /// </summary>
    public class RepositorySettings
    {
        /// <summary>
        /// Whether this repository is public.
        /// </summary>
        public bool Public { get; set; }

        /// <summary>
        /// Authentication username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Authentication access token.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// The repository URL to pull meta from.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The checkout arguments to use.
        /// </summary>
        public string Checkout { get; set; }
    }

    /// <summary>
    /// Allowable file type settings.
    /// </summary>
    public class MetaFileSettings
    {
        /// <summary>
        /// The file extension.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// The comment delimiter.
        /// </summary>
        public string Delimiter { get; set; }
    }
}
