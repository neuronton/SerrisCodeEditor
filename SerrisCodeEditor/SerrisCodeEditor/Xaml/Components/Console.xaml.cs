﻿using GalaSoft.MvvmLight.Messaging;
using SCEELibs.Editor.Notifications;
using SerrisModulesServer.Type.Addon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SerrisCodeEditor.Xaml.Components
{
    public class ConsoleNotificationContent
    {
        public string notifIcon { get; set; }
        public string notifDate { get { return notifContent.date.ToString("HH:mm:ss"); } }
        public ConsoleNotification notifContent { get; set; }
    }

    public sealed partial class Console : UserControl
    {
        public Console()
        {
            this.InitializeComponent();
        }

        private void ConsoleUI_Loaded(object sender, RoutedEventArgs e)
        { SetMessenger(); }

        private void ConsoleUI_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!isFlyoutOpened) { OpenConsole(); }
        }

        private void ConsoleUI_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (isFlyoutOpened) { CloseConsole(); }
        }



        /* =============
         * = FUNCTIONS =
         * =============
         */



        private void SetMessenger()
        {
            Messenger.Default.Register<ConsoleNotification>(this, (notification) =>
            {
                try
                {
                    SetLastNotificationInfos(notification);

                    switch (notification.typeNotification)
                    {
                        case ConsoleTypeNotification.Error:
                            errors_list.Add(new ConsoleNotificationContent { notifContent = notification, notifIcon = "" });

                            if (ShowErrors)
                                CurrentListNotifications.Items.Insert(0, new ConsoleNotificationContent { notifContent = notification, notifIcon = "" });
                            break;

                        case ConsoleTypeNotification.Information:
                            informations_list.Add(new ConsoleNotificationContent { notifContent = notification, notifIcon = "" });

                            if (ShowInformations)
                                CurrentListNotifications.Items.Insert(0, new ConsoleNotificationContent { notifContent = notification, notifIcon = "" });

                            break;

                        case ConsoleTypeNotification.Result:
                            results_list.Add(new ConsoleNotificationContent { notifContent = notification, notifIcon = "" });

                            if (ShowResults)
                                CurrentListNotifications.Items.Insert(0, new ConsoleNotificationContent { notifContent = notification, notifIcon = "" });

                            break;

                        case ConsoleTypeNotification.Warning:
                            warnings_list.Add(new ConsoleNotificationContent { notifContent = notification, notifIcon = "" });

                            if (ShowWarnings)
                                CurrentListNotifications.Items.Insert(0, new ConsoleNotificationContent { notifContent = notification, notifIcon = "" });

                            break;
                    }

                    UpdateNotifsNumber();
                }
                catch { }
            });

        }

        private void SetLastNotificationInfos(ConsoleNotification notif)
        {
            switch (notif.typeNotification)
            {
                case ConsoleTypeNotification.Error:
                    LastNotifInfos_Icon.Text = "";
                    break;

                case ConsoleTypeNotification.Information:
                    LastNotifInfos_Icon.Text = "";
                    break;

                case ConsoleTypeNotification.Result:
                    LastNotifInfos_Icon.Text = "";
                    break;

                case ConsoleTypeNotification.Warning:
                    LastNotifInfos_Icon.Text = "";
                    break;
            }

            LastNotifInfos_Text.Text = "[" + notif.date.ToString("HH:mm:ss") + "] " + notif.content;
        }

        private void Command_box_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch(e.Key)
            {
                case VirtualKey.Enter:
                    try
                    {
                        ChakraSMS executor = new ChakraSMS(); executor.Chakra.ProjectObjectToGlobal(new SCEELibs.Editor.ConsoleManager(), "Console");
                        executor.Chakra.RunScript(Command_box.Text);
                    }
                    catch { }

                    commands_list.Add(Command_box.Text);
                    commands_list_index = -1; Command_box.Text = "";
                    break;

                case VirtualKey.Down:
                    if(commands_list_index < 0)
                    {
                        commands_list_index = commands_list.Count;
                    }
                    commands_list_index--;

                    if (commands_list_index >= 0)
                        Command_box.Text = commands_list[commands_list_index];

                    break;

                case VirtualKey.Up:
                    if (commands_list_index + 1 <= commands_list.Count - 1)
                    {
                        commands_list_index++;

                        if (commands_list_index >= 0)
                            Command_box.Text = commands_list[commands_list_index];
                    }
                    else
                    {
                        Command_box.Text = ""; commands_list_index = -1;
                    }
                    break;
            }
        }

        private void InformationsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowInformations = !ShowInformations;

            if (ShowInformations)
            {
                InformationsButton.Background = new SolidColorBrush(Color.FromArgb(255, 62, 33, 89));
                InformationsButton.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                InformationsButton.Background = new SolidColorBrush(Colors.Transparent);
                InformationsButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 62, 33, 89));
            }

            RefreshNotificationsList();
        }

        private void ResultsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowResults = !ShowResults;

            if (ShowResults)
            {
                ResultsButton.Background = new SolidColorBrush(Color.FromArgb(255, 62, 33, 89));
                ResultsButton.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                ResultsButton.Background = new SolidColorBrush(Colors.Transparent);
                ResultsButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 62, 33, 89));
            }

            RefreshNotificationsList();
        }

        private void WarningsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowWarnings = !ShowWarnings;

            if (ShowWarnings)
            {
                WarningsButton.Background = new SolidColorBrush(Color.FromArgb(255, 62, 33, 89));
                WarningsButton.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                WarningsButton.Background = new SolidColorBrush(Colors.Transparent);
                WarningsButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 62, 33, 89));
            }

            RefreshNotificationsList();
        }

        private void ErrorsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowErrors = !ShowErrors;

            if (ShowErrors)
            {
                ErrorsButton.Background = new SolidColorBrush(Color.FromArgb(255, 62, 33, 89));
                ErrorsButton.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                ErrorsButton.Background = new SolidColorBrush(Colors.Transparent);
                ErrorsButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 62, 33, 89));
            }

            RefreshNotificationsList();
        }

        private void CloseConsole()
        {
            LastNotifInfos.Visibility = Visibility.Visible; SymbolOpened.Text = "";
            ConsoleMoreInfosViewer.Visibility = Visibility.Collapsed; Command_box.Visibility = Visibility.Collapsed;
            isFlyoutOpened = false;
        }

        private void OpenConsole()
        {
            LastNotifInfos.Visibility = Visibility.Collapsed; SymbolOpened.Text = "";
            ConsoleMoreInfosViewer.Visibility = Visibility.Visible; Command_box.Visibility = Visibility.Visible;
            isFlyoutOpened = true;

            UpdateNotifsNumber();
        }

        private void UpdateNotifsNumber()
        {
            ErrorsNumber.Text = "" + errors_list.Count;
            InformationsNumber.Text = "" + informations_list.Count;
            ResultsNumber.Text = "" + results_list.Count;
            WarningsNumber.Text = "" + warnings_list.Count;
        }

        private void RefreshNotificationsList()
        {
            CurrentListNotifications.Items.Clear();
            List<ConsoleNotificationContent> temp_list = new List<ConsoleNotificationContent>();
            
            if(ShowErrors)
                foreach(var element in errors_list) { temp_list.Add(element); }

            if (ShowInformations)
                foreach (var element in informations_list) { temp_list.Add(element); }

            if (ShowResults)
                foreach (var element in results_list) { temp_list.Add(element); }

            if (ShowWarnings)
                foreach (var element in warnings_list) { temp_list.Add(element); }

            temp_list.OrderBy(o => o.notifContent.date).ToList();
            temp_list.Reverse();
            foreach (var element in temp_list) { CurrentListNotifications.Items.Add(element); }
        }



        /* =============
         * = VARIABLES =
         * =============
         */



        List<ConsoleNotificationContent> errors_list = new List<ConsoleNotificationContent>(), informations_list = new List<ConsoleNotificationContent>(), results_list = new List<ConsoleNotificationContent>(), warnings_list = new List<ConsoleNotificationContent>();
        List<string> commands_list = new List<string>(); int commands_list_index = -1;
        bool isFlyoutOpened = false;
        bool ShowErrors = true, ShowInformations = true, ShowResults = true, ShowWarnings = true;
    }
}
