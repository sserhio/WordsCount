﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FontAwesome.WPF;
using WordsCount.Services;
using WordsCount.ViewModels;

namespace WordsCount
{
    /// <summary>
    /// Interaction logic for Requests.xaml
    /// </summary>
    public partial class TextRequestsWindow
    {
        private ImageAwesome _loader;

        public TextRequestsWindow()
        {
            WindowStyle = WindowStyle.None;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            InitializeComponent();
            UserNameLabel.Content = StationManager.CurrentUser.UserName;

            var textRequestsViewModel = new TextRequestsViewModel();
            textRequestsViewModel.RequestClose += Close;
            textRequestsViewModel.RequestFillPath += FillPath;
            textRequestsViewModel.RequestFillText += FillText;
            textRequestsViewModel.RequestShowResults += ShowResultsLabels;
            textRequestsViewModel.RequestProgressBar += UpdateProgressBar;

            DataContext = textRequestsViewModel;
        }

        private void UpdateProgressBar(bool isShow, int step = 0)
        {
            if(isShow)
            {
                if (_loader == null)
                {
                    _loader = new ImageAwesome();
                    TextAnalyzerGrid.Children.Add(_loader);
                    _loader.Width = 400;
                    _loader.Height = 75;
                }

                switch (step)
                {
                    case 1:
                        _loader.Icon = FontAwesomeIcon.Battery1;
                        break;
                    case 2:
                        _loader.Icon = FontAwesomeIcon.Battery2;
                        break;
                    case 3:
                        _loader.Icon = FontAwesomeIcon.Battery3;
                        break;
                    default:
                        _loader.Icon = FontAwesomeIcon.BatteryEmpty;
                        break;
                }
                
                IsEnabled = false;
            }
            else
            {
                TextAnalyzerGrid.Children.Remove(_loader);
                _loader = null;
                IsEnabled = true;
            }
        }

        private void Close(bool isQuitApp)
        {
            if (!isQuitApp)
            {
                Close();
            }
            else
            {
                // Serialize user before exit, to update it's fields at next start of program
                SerializeManager.RemoveFile(StationManager.UserFilePath);
                SerializeManager.Serialize(StationManager.CurrentUser);
                Logger.Log($"User {StationManager.CurrentUser.UserName} closed program wuthout log out");

                MessageBox.Show("Salut!");
                Environment.Exit(0);
            }
        }

        private void FillPath(string path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                PathLabel.Content = path;
            }
        }

        private void FillText(string text)
        {
            FileText.Text = !String.IsNullOrEmpty(text) ? text : "Text is empty";
        }

        // Method to show results label when they are available
        private void ShowResultsLabels(bool visible = true)
        {
            SymbolsAmount.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
            SymbolsAmountValue.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
            WordsAmount.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
            WordsAmountValue.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
            LinesAmount.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
            LinesAmountValue.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }

        // Allow to move form
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}
