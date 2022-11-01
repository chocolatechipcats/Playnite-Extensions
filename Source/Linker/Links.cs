﻿using System.Collections.Generic;

namespace LinkUtilities.Linker
{
    /// <summary>
    /// List of all website Links that can be added.
    /// </summary>
    public class Links : List<Link>
    {
        public Links(LinkUtilities plugin)
        {
            Add(new LinkEpic(plugin));
            Add(new LibraryLinkGog(plugin));
            Add(new LinkHG101(plugin));
            if (!string.IsNullOrWhiteSpace(plugin.Settings.Settings.ItchApiKey))
            {
                Add(new LibraryLinkItch(plugin));
            }
            Add(new LinkMetacritic(plugin));
            Add(new LinkMobyGames(plugin));
            Add(new LinkPCGamingWiki(plugin));
            Add(new LibraryLinkSteam(plugin));
            Add(new LinkWikipedia(plugin));
        }
    }
}
