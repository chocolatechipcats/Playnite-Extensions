﻿using KNARZhelper;
using LinkUtilities.BaseClasses;
using LinkUtilities.Helper;
using LinkUtilities.Models;
using LinkUtilities.Models.Steam;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkUtilities.Linker
{
    /// <summary>
    ///     Adds a link to Steam.
    /// </summary>
    internal class LibraryLinkSteam : LibraryLink
    {
        private readonly string _steamAppPrefix = "steam://openurl/";
        private readonly string _urlAchievements = "https://steamcommunity.com/stats/{0}/achievements";
        private readonly string _urlCommunity = "https://steamcommunity.com/app/{0}";
        private readonly string _urlDiscussion = "https://steamcommunity.com/app/{0}/discussions/";
        private readonly string _urlGuides = "https://steamcommunity.com/app/{0}/guides/";
        private readonly string _urlNews = "https://store.steampowered.com/news/?appids={0}";
        private readonly string _urlStorePage = "https://store.steampowered.com/app/{0}";

        /// <summary>
        ///     ID of the game library to identify it in Playnite.
        /// </summary>
        public override Guid Id { get; } = Guid.Parse("cb91dfc9-b977-43bf-8e70-55f46e410fab");

        public override string LinkName => "Steam";
        public override LinkAddTypes AddType => LinkAddTypes.SingleSearchResult;
        public override string SearchUrl => "https://steamcommunity.com/actions/SearchApps/";

        public bool UseAppLinks { get; set; } = false;
        public bool AddAchievementLink { get; set; } = false;
        public bool AddCommunityLink { get; set; } = false;
        public bool AddDiscussionLink { get; set; } = false;
        public bool AddGuidesLink { get; set; } = false;
        public bool AddNewsLink { get; set; } = false;
        public bool AddStorePageLink { get; set; } = true;

        public string NameAchievementLink { get; set; } = "Achievements";
        public string NameCommunityLink { get; set; } = "Community Hub";
        public string NameDiscussionLink { get; set; } = "Discussion";
        public string NameGuidesLink { get; set; } = "Guides";
        public string NameNewsLink { get; set; } = "News";
        public string NameStorePageLink { get; set; } = "Store Page";

        private void AddLink(Game game, List<Link> links, bool canAdd, string gameId, string url, string name)
        {
            if (LinkHelper.LinkExists(game, name))
            {
                return;
            }

            if (canAdd)
            {
                links.Add(new Link(name, $"{GetPrefix()}{string.Format(url, gameId)}"));
            }
        }

        private bool AddLinks(Game game, out List<Link> links, string gameId)
        {
            links = new List<Link>();

            AddLink(game, links, AddAchievementLink, gameId, _urlAchievements, NameAchievementLink);
            AddLink(game, links, AddCommunityLink, gameId, _urlCommunity, NameCommunityLink);
            AddLink(game, links, AddDiscussionLink, gameId, _urlDiscussion, NameDiscussionLink);
            AddLink(game, links, AddGuidesLink, gameId, _urlGuides, NameGuidesLink);
            AddLink(game, links, AddNewsLink, gameId, _urlNews, NameNewsLink);
            AddLink(game, links, AddStorePageLink, gameId, _urlStorePage, NameStorePageLink);

            if (links.Any())
            {
                return true;
            }

            AddLink(game, links, true, gameId, _urlStorePage, LinkName);

            return links.Any();
        }

        public override bool FindLibraryLink(Game game, out List<Link> links) => AddLinks(game, out links, game.GameId);

        public override List<GenericItemOption> GetSearchResults(string searchTerm)
        {
            List<SteamSearchResult> games = ParseHelper.GetJsonFromApi<List<SteamSearchResult>>($"{SearchUrl}{searchTerm.UrlEncode()}", LinkName);

            return games?.Any() ?? false
                ? new List<GenericItemOption>(games.Select(g => new SearchResult
                {
                    Name = g.Name,
                    Url = g.Appid,
                    Description = g.Appid
                }))
                : base.GetSearchResults(searchTerm);
        }

        public override bool AddLinkFromSearch(Game game, SearchResult result, bool cleanUpAfterAdding = true)
            => AddLinks(game, out List<Link> links, result.Url) && LinkHelper.AddLinks(game, links, cleanUpAfterAdding);

        private string GetPrefix() => UseAppLinks ? _steamAppPrefix : string.Empty;


        public override bool FindLinks(Game game, out List<Link> links)
        {
            if (game.PluginId == Id)
            {
                return FindLibraryLink(game, out links);
            }


            LinkUrl = string.Empty;
            links = new List<Link>();

            LinkUrl = GetGamePath(game);

            return LinkUrl.Any() && AddLinks(game, out links, LinkUrl);
        }
    }
}