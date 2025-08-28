using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CGMViewerWPF.Services
{
    public interface IDialogService
    {
        bool ShowConfirmation(string message, string title = "Confirmation");
        void ShowError(string message, string title = "Erreur");
        void ShowInfo(string message, string title = "Information");
        string? ShowSaveFileDialog(string filter, string defaultFileName = "");
        string[]? ShowOpenFileDialog(string filter, bool multiSelect = false);
    }

    public class DialogService : IDialogService
    {
        public bool ShowConfirmation(string message, string title = "Confirmation")
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public void ShowError(string message, string title = "Erreur")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowInfo(string message, string title = "Information")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public string? ShowSaveFileDialog(string filter, string defaultFileName = "")
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = filter,
                FileName = defaultFileName
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public string[]? ShowOpenFileDialog(string filter, bool multiSelect = false)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = filter,
                Multiselect = multiSelect
            };

            return dialog.ShowDialog() == true ? dialog.FileNames : null;
        }
    }
}
