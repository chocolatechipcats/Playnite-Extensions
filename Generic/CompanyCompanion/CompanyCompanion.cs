﻿using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CompanyCompanion
{
    public class CompanyCompanion : GenericPlugin
    {
        public CompanyCompanionSettingsViewModel Settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("b76edaec-1aa8-48ef-83a4-2a49ab031029");

        public CompanyCompanion(IPlayniteAPI api) : base(api)
        {
            Settings = new CompanyCompanionSettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
        }

        public void ShowMergeView()
        {
            try
            {
                MergeCompaniesView mergeView = new MergeCompaniesView(this);
                Window window = PlayniteApi.Dialogs.CreateWindow(new WindowCreationOptions
                {
                    ShowMinimizeButton = false,
                });

                window.Height = 800;
                window.Width = 800;
                window.Title = ResourceProvider.GetString("LOCCompanyCompanionMergeWindowName");
                window.Content = mergeView;
                window.Owner = PlayniteApi.Dialogs.GetCurrentAppWindow();
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.ShowDialog();
            }
            catch (Exception E)
            {
                Log.Error(E, "Error during initializing MergeCompaniesView", true);
            }
        }


        public override IEnumerable<MainMenuItem> GetMainMenuItems(GetMainMenuItemsArgs args)
        {
            string menuSection = ResourceProvider.GetString("LOCCompanyCompanionName");

            List<MainMenuItem> menuItems = new List<MainMenuItem>
            {
                // Adds the "clean up" item to the main menu.
                new MainMenuItem
                {
                    Description = ResourceProvider.GetString("LOCCompanyCompanionMenuShowMerger"),
                    MenuSection = $"@{menuSection}",
                    Action = a =>
                    {
                        ShowMergeView();
                    }
                },
                new MainMenuItem
                {
                    Description = ResourceProvider.GetString("LOCCompanyCompanionMenuMergeDuplicates"),
                    MenuSection = $"@{menuSection}",
                    Action = a =>
                    {
                        MergeCompanies.MergeDuplicates(this);
                    }
                },
                new MainMenuItem
                {
                    Description = ResourceProvider.GetString("LOCCompanyCompanionMenuRemoveDescriptors"),
                    MenuSection = $"@{menuSection}",
                    Action = a =>
                    {
                        MergeCompanies.RemoveBusinessEntityDescriptors(this);
                    }
                }
            };

            return menuItems;
        }

        public override void OnLibraryUpdated(OnLibraryUpdatedEventArgs args)
        {
            // Add code to be executed when library is updated.
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return Settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new CompanyCompanionSettingsView();
        }
    }
}