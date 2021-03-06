﻿using GalaSoft.MvvmLight.Messaging;
using Microsoft.Toolkit.Uwp.Helpers;
using SCEELibs.Editor.Notifications;
using SerrisModulesServer.Manager;
using SerrisModulesServer.Type.ProgrammingLanguage;
using SerrisTabsServer.Items;
using SerrisTabsServer.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace SerrisCodeEditor.Xaml.Components
{
    public sealed partial class Tab : UserControl
    {
        InfosTab current_tab = new InfosTab(); int current_list; bool infos_opened = false, enable_selection = false;
        TabID CurrentIDs;
        ApplicationDataContainer AppSettings = ApplicationData.Current.LocalSettings;

        public Tab()
        {
            this.InitializeComponent();

            DataContextChanged += Tab_DataContextChanged;
        }

        private void Tab_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (DataContext != null)
            {
                TabID ids = (TabID)DataContext;

                if(CurrentIDs.ID_Tab != ids.ID_Tab && CurrentIDs.ID_TabsList != ids.ID_TabsList)
                {
                    if (current_tab == null)
                        current_tab = new InfosTab();

                    if (AppSettings.Values.ContainsKey("ui_leftpanelength"))
                    {
                        GridInfoLeft.Width = (int)AppSettings.Values["ui_leftpanelength"];
                        StackInfos.Margin = new Thickness((int)AppSettings.Values["ui_leftpanelength"], 0, 0, 0);
                    }

                    current_tab.ID = ids.ID_Tab; current_list = ids.ID_TabsList;
                    UpdateTabInformations();
                }

            }
        }

        private void TabComponent_Loaded(object sender, RoutedEventArgs e)
        {
            SetMessenger();
            //UpdateTabInformations();
        }

        private void TabComponent_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(current_tab.PathContent) && current_tab.TabContentType == ContentType.File)
                ShowPath.Begin();
        }

        private void TabComponent_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(current_tab.PathContent) && current_tab.TabContentType == ContentType.File)
                ShowName.Begin();
        }



        /*
         * =============
         * = FUNCTIONS =
         * =============
         */



        private void SetMessenger()
        {
            Messenger.Default.Register<STSNotification>(this, async (nm) =>
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    try
                    {

                        if (nm.ID.ID_Tab == current_tab.ID && nm.ID.ID_TabsList == current_list)
                        {
                            switch (nm.Type)
                            {
                                case TypeUpdateTab.TabUpdated:
                                    UpdateTabInformations();
                                    break;
                            }
                        }

                    }
                    catch { }
                });

            });

            Messenger.Default.Register<TabSelectedNotification>(this, async (nm) =>
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                {
                    try
                    {

                        if (nm.tabID == current_tab.ID && nm.tabsListID == current_list)
                        {
                            switch (nm.contactType)
                            {
                                case ContactTypeSCEE.GetCodeForTab:
                                    await TabsWriteManager.PushTabContentViaIDAsync(new TabID { ID_Tab = current_tab.ID, ID_TabsList = current_list }, current_tab.TabContentTemporary, true);
                                    break;
                            }
                        }

                    }
                    catch { }

                });

            });
        }

        int TempTabID = 0;
        private async void UpdateTabInformations()
        {
            //Set temp tab + tabs list ID
            try
            {
                current_tab = TabsAccessManager.GetTabViaID(new TabID { ID_Tab = current_tab.ID, ID_TabsList = current_list });

                name_tab.Text = current_tab.TabName;

                switch (current_tab.TabContentType)
                {
                    case ContentType.File:
                        int ModuleIDIcon = LanguagesHelper.GetModuleIDOfLangageType(current_tab.TabType);
                        TabIcon.Source = await ModulesAccessManager.GetModuleIconViaIDAsync(ModuleIDIcon, ModulesAccessManager.GetModuleViaID(ModuleIDIcon).ModuleSystem);

                        if (!string.IsNullOrEmpty(current_tab.PathContent))
                        {
                            path_tab.Text = current_tab.PathContent;
                            encoding_file.Text = Encoding.GetEncoding(current_tab.TabEncoding).EncodingName;
                            More_Tab.Visibility = Visibility.Visible;
                        }
                        else
                            More_Tab.Visibility = Visibility.Collapsed;

                        TabsListGrid.Visibility = Visibility.Collapsed;
                        TabIcon.Visibility = Visibility.Visible;
                        FolderIcon.Visibility = Visibility.Collapsed;
                        StackInfos.Visibility = Visibility.Visible;
                        break;

                    case ContentType.Folder:
                        More_Tab.Visibility = Visibility.Visible;
                        TabsListGrid.Visibility = Visibility.Visible;
                        StackInfos.Visibility = Visibility.Collapsed;

                        TabIcon.Visibility = Visibility.Collapsed;
                        FolderIcon.Visibility = Visibility.Visible;

                        if(TempTabID != current_tab.ID && TabsList.ListID != current_list)
                        {
                            ShowInfos.Begin();

                            TabsList.ListTabs.Items.Clear();
                            TempTabID = current_tab.ID;
                            TabsList.ListID = current_list;
                            foreach (int ID in current_tab.FolderContent)
                            {
                                try
                                {
                                    if (TabsAccessManager.GetTabViaID(new TabID { ID_Tab = ID, ID_TabsList = current_list }) != null)
                                    {
                                        TabsList.ListTabs.Items.Add(new TabID { ID_Tab = ID, ID_TabsList = current_list });
                                        if ((int)AppSettings.Values["Tabs_tab-selected-index"] == ID && (int)AppSettings.Values["Tabs_list-selected-index"] == current_list)
                                        {
                                            TabsList.ListTabs.SelectedIndex = TabsList.ListTabs.Items.Count - 1;
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                        
                        break;
                }


            }
            catch { }
        }

        private async void TabsList_ListTabDeleted(object sender, TabID e)
        {
            if(current_tab != null)
            {
                current_tab.FolderContent.Remove(e.ID_Tab);
                await TabsWriteManager.PushUpdateTabAsync(current_tab, current_list);
            }
        }

        private void Close_Tab_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Messenger.Default.Send(new STSNotification { ID = new TabID { ID_Tab = current_tab.ID, ID_TabsList = current_list }, Type = TypeUpdateTab.TabDeleted });
            }
            catch { }
        }

        private void list_types_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void More_Tab_Click(object sender, RoutedEventArgs e)
        {
            if (infos_opened)
            {
                RemoveInfos.Begin(); infos_opened = false;
            }
            else
            {
                enable_selection = false;
                try
                {
                    StorageFile file = await StorageFile.GetFileFromPathAsync(current_tab.PathContent);
                    BasicProperties properties = await file.GetBasicPropertiesAsync();

                    if (properties.Size != 0)
                    {

                        if (properties.Size > 1024f) //Ko
                        {
                            size_file.Text = String.Format("{0:0.00}", (properties.Size / 1024f)) + " Ko";

                            if ((properties.Size / 1024f) > 1024f) //Mo
                            {
                                size_file.Text = String.Format("{0:0.00}", ((properties.Size / 1024f) / 1024f)) + " Mo";
                            }
                        }
                        else //Octect
                        {
                            size_file.Text = properties.Size + " Octect(s)";
                        }

                    }

                    modified_file.Text = properties.DateModified.ToString();
                    created_file.Text = file.DateCreated.ToString();
                }
                catch { }

                ShowInfos.Begin(); infos_opened = true; enable_selection = true;
            }

        }
    }
}
