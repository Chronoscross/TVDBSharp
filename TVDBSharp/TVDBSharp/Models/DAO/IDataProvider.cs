﻿using System.Collections.Generic;
using TVDBSharp.Models.Enums;

namespace TVDBSharp.Models.DAO
{
    /// <summary>
    ///     Defines a Dataprovider API.
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        ///     Retrieves the show with the given id and returns the corresponding XML tree.
        /// </summary>
        /// <param name="showId">ID of the show you wish to lookup.</param>
        /// <returns>Returns an XML tree of the show object.</returns>
        Show GetShow(int showId);

        List<Episode> GetEpisodes(int showId);

        /// <summary>
        ///     Retrieves the episode with the given id and returns the corresponding XML tree.
        /// </summary>
        /// <param name="episodeId">ID of the episode to retrieve</param>
        /// <param name="lang">ISO 639-1 language code of the episode</param>
        /// <returns>XML tree of the episode object</returns>
        Episode GetEpisode(int episodeId, string lang);

        /// <summary>
        ///     Retrieves updates on tvdb (Shows, Episodes and Banners)
        /// </summary>
        /// <param name="interval">The interval for the updates</param>
        /// <returns>XML tree of the Updates object</returns>
        Updates GetUpdates(Interval interval);

        /// <summary>
        ///     Returns an XML tree representing a search query for the given parameter.
        /// </summary>
        /// <param name="query">Query to perform the search with.</param>
        /// <returns>Returns an XML tree of a search result.</returns>
        List<Show> Search(string query);
    }
}