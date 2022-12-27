﻿using LinkUtilities.LinkActions;
using LinkUtilities.Models;
using Playnite.SDK;
using Playnite.SDK.Models;
using System.Collections.Generic;
using System.Linq;

namespace LinkUtilities.Linker
{
    /// <summary>
    /// Base class for a website link 
    /// </summary>
    public abstract class Link : ILink, ILinkAction
    {
        public abstract string LinkName { get; }
        public virtual string BaseUrl { get; } = string.Empty;
        public virtual string SearchUrl { get; } = string.Empty;
        public virtual string LinkUrl { get; set; } = string.Empty;
        public virtual bool CanBeAdded { get { return !string.IsNullOrWhiteSpace(BaseUrl); } }
        public virtual bool CanBeSearched { get { return !string.IsNullOrWhiteSpace(SearchUrl); } }
        public LinkSourceSetting Settings { get; set; }
        public virtual bool AllowRedirects { get; set; } = true;
        public string ProgressMessage { get; } = "LOCLinkUtilitiesProgressLink";
        public string ResultMessage { get; } = "LOCLinkUtilitiesDialogAddedMessage";

        private readonly LinkUtilities plugin;
        public LinkUtilities Plugin { get { return plugin; } }
        public List<SearchResult> SearchResults { get; set; } = new List<SearchResult>();

        public virtual bool AddSearchedLink(Game game)
        {
            GenericItemOption result = API.Instance.Dialogs.ChooseItemWithSearch(
                new List<GenericItemOption>(),
                (a) => SearchLink(a),
                game.Name,
                $"{ResourceProvider.GetString("LOCLinkUtilitiesDialogSearchGame")} ({LinkName})");

            if (result != null)
            {
                return LinkHelper.AddLink(game, LinkName, SearchResults.Find(x => x.Name == result.Name).Url, plugin, false);
            }
            else
            {
                return false;
            }
        }

        public virtual List<GenericItemOption> SearchLink(string searchTerm)
        {
            return new List<GenericItemOption>(SearchResults.Select(x => new GenericItemOption(x.Name, x.Description)));
        }

        public virtual bool AddLink(Game game)
        {
            LinkUrl = string.Empty;
            bool result = false;

            if (!LinkHelper.LinkExists(game, LinkName))
            {
                string gameName = GetGamePath(game);

                if (!string.IsNullOrEmpty(gameName))
                {
                    LinkUrl = $"{BaseUrl}{GetGamePath(game)}";

                    if (CheckLink(LinkUrl))
                    {
                        result = LinkHelper.AddLink(game, LinkName, LinkUrl, plugin);
                    }
                }
            }

            return result;
        }

        public virtual bool CheckLink(string link)
        {
            return LinkHelper.CheckUrl(link, AllowRedirects);
        }

        public virtual string GetGamePath(Game game)
        {
            return game.Name;
        }

        public virtual bool Execute(Game game, ActionModifierTypes actionModifier = ActionModifierTypes.None, bool isBulkAction = true)
        {
            switch (actionModifier)
            {
                case ActionModifierTypes.Add:
                    return AddLink(game);
                case ActionModifierTypes.Search:
                    return AddSearchedLink(game);
                default:
                    return false;
            }
        }

        public Link(LinkUtilities plugin)
        {
            this.plugin = plugin;
            Settings = new LinkSourceSetting()
            {
                LinkName = LinkName,
                IsAddable = CanBeAdded ? true : (bool?)null,
                IsSearchable = CanBeSearched ? true : (bool?)null,
                ShowInMenus = true,
                ApiKey = null,
                NeedsApiKey = false
            };
        }
    }
}
