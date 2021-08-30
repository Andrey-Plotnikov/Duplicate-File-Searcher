using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DuplicateFileSearcher
{
    public partial class ChooseForlderWindow : Window
    {
        private String currentDir = String.Empty;
        private Int32 currentDirLevel = 0;
        public ChooseForlderWindow()
        {
            InitializeComponent();

            String[] drives = GetLogicalDrives();

            directory_listbox.ItemsSource = drives;
        }

        private String[] GetLogicalDrives()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            String[] driveNames = new String[drives.Length];
            for (Int32 i = 0; i < drives.Length; i++) driveNames[i] = drives[i].Name;

            return driveNames;
        }

        private void directory_listbox_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            Int32 selected = directory_listbox.SelectedIndex;
            String selectedDir = currentDir;

            if (currentDirLevel > 1) selectedDir = selectedDir + "\\" + directory_listbox.Items[selected];
            else selectedDir = selectedDir + directory_listbox.Items[selected];

            String[] folders = Directory.GetDirectories(selectedDir);
            for (Int32 i = 0; i < folders.Length; i++) folders[i] = folders[i].Substring(folders[i].LastIndexOf('\\') + 1);
            directory_listbox.ItemsSource = folders;
            current_path_textbox.Text = selectedDir;

            currentDir = selectedDir;
            currentDirLevel++;
        }

        private void back_dir_button_Click(Object sender, RoutedEventArgs e)
        {
            if (currentDirLevel > 0)
            {
                Int32 selected = directory_listbox.SelectedIndex;
                String selectedDir = currentDir;

                if (currentDirLevel > 1)
                {
                    if (currentDirLevel > 2) selectedDir = selectedDir.Substring(0, selectedDir.LastIndexOf('\\'));
                    else if (currentDirLevel == 2) selectedDir = selectedDir.Substring(0, selectedDir.IndexOf('\\') + 1);

                    String[] folders = Directory.GetDirectories(selectedDir);
                    for (Int32 i = 0; i < folders.Length; i++) folders[i] = folders[i].Substring(folders[i].LastIndexOf('\\') + 1);
                    directory_listbox.ItemsSource = folders;
                    current_path_textbox.Text = selectedDir;
                }
                else
                {
                    selectedDir = String.Empty;
                    current_path_textbox.Text = "Выберите логический том";
                    directory_listbox.ItemsSource = GetLogicalDrives();
                }

                currentDir = selectedDir;
                currentDirLevel--;
            }
        }

        private void choose_button_Click(Object sender, RoutedEventArgs e)
        {
            Common.searchingDir = currentDir;
            this.Close();
        }

        private void cancel_button_Click(Object sender, RoutedEventArgs e)
        {
            Common.searchingDir = String.Empty;
            this.Close();
        }
    }
}
