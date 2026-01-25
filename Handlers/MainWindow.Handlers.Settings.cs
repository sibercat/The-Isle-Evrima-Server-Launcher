using IsleServerLauncher.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace IsleServerLauncher
{
    public partial class ServerSettingsView : UserControl
    {
        private void txtDinoSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = txtDinoSearch.Text.ToLower();
            if (lstDinos.ItemsSource is List<DinoOption> dinos)
            {
                var view = CollectionViewSource.GetDefaultView(dinos);
                if (string.IsNullOrWhiteSpace(filter))
                    view.Filter = null;
                else
                    view.Filter = obj => (obj as DinoOption)?.Name.ToLower().Contains(filter) ?? false;
            }
        }

        private void btnDinoSelectAll_Click(object sender, RoutedEventArgs e)
        {
            // Use the ListBox's native selection to ensure internal state is updated
            lstDinos.SelectAll();
            
            // Manually sync properties for the data model since it lacks INotifyPropertyChanged
            foreach (var item in lstDinos.Items)
            {
                if (item is DinoOption dino)
                    dino.IsEnabled = true;
            }
            
            lstDinos.Items.Refresh();
        }

        private void btnDinoSelectNone_Click(object sender, RoutedEventArgs e)
        {
            lstDinos.UnselectAll();
            
            foreach (var item in lstDinos.Items)
            {
                if (item is DinoOption dino)
                    dino.IsEnabled = false;
            }
            
            lstDinos.Items.Refresh();
        }

        private void txtAiSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = txtAiSearch.Text.ToLower();
            if (lstDisallowedAI.ItemsSource is List<AiOption> aiOptions)
            {
                var view = CollectionViewSource.GetDefaultView(aiOptions);
                if (string.IsNullOrWhiteSpace(filter))
                    view.Filter = null;
                else
                    view.Filter = obj => (obj as AiOption)?.Name.ToLower().Contains(filter) ?? false;
            }
        }

        private void btnAiSelectAll_Click(object sender, RoutedEventArgs e)
        {
            lstDisallowedAI.SelectAll();
            
            foreach (var item in lstDisallowedAI.Items)
            {
                if (item is AiOption ai)
                    ai.IsEnabled = true;
            }
            
            lstDisallowedAI.Items.Refresh();
        }

        private void btnAiSelectNone_Click(object sender, RoutedEventArgs e)
        {
            lstDisallowedAI.UnselectAll();
            
            foreach (var item in lstDisallowedAI.Items)
            {
                if (item is AiOption ai)
                    ai.IsEnabled = false;
            }
            
            lstDisallowedAI.Items.Refresh();
        }
    }
}
