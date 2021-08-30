using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace DuplicateFileSearcher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void select_dir_button_Click(Object sender, RoutedEventArgs e)
        {
            ChooseForlderWindow wind = new ChooseForlderWindow();
            wind.ShowDialog();
            current_dir_textbox.Text = Common.searchingDir;
        }

        private void find_duplicates_button_Click(Object sender, RoutedEventArgs e)
        {
            DuplicateSearcher dp = new DuplicateSearcher(current_dir_textbox.Text, (Boolean)is_sibdir_checkbox.IsChecked);

            Stopwatch st = new Stopwatch();
            st.Start();
            List<FileGroup> DuplicateGroupList = dp.StartSearch();
            st.Stop();
            var t = st.ElapsedTicks;
            var ms = st.ElapsedMilliseconds;

            var groupList = new FileGroupListViewModel();
            for (Int32 i = 0; i < DuplicateGroupList.Count; i++)
                groupList.FileGroupOptions.Add(new FileGroupOption(i + 1, DuplicateGroupList[i].FileSize, DuplicateGroupList[i].FileCount, DuplicateGroupList[i].TotalSize, DuplicateGroupList[i].Files));

            DataContext = groupList;

            search_result_textblock.Text = $"Всего групп: {dp.searchResult.totalGroup} | Всего файлов: {dp.searchResult.totalFiles} | Общий размер: {Common.getFormattedSize(dp.searchResult.totalSize)}";
        }

        private void openFileExplorer(Object sender, RoutedEventArgs e)
        {
            var fileInfo = (sender as Button).DataContext as FileListOption;

            Process PrFolder = new Process();
            ProcessStartInfo psi = new ProcessStartInfo
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Normal,
                FileName = "explorer",
                Arguments = @"/n, /select, " + fileInfo.Path
            };
            PrFolder.StartInfo = psi;
            PrFolder.Start();
            PrFolder.Dispose();
        }
    }

    public class FileGroupListViewModel
    {
        public ObservableCollection<FileGroupOption> FileGroupOptions { get; } = new ObservableCollection<FileGroupOption>();
    }

    public class FileListOption
    {
        public String Name { get; set; }
        public String Path { get; set; }
        public String CreatDateTime { get; set; }
        public String ModifDateTime { get; set; }

        public FileListOption(String path, DateTime creatDateTime, DateTime modifDateTime)
        {
            Path = path;
            Name = Path.Substring(Path.LastIndexOf('\\') + 1);
            CreatDateTime = creatDateTime.ToString("G");
            ModifDateTime = modifDateTime.ToString("G");
        }
    }


    public class FileGroupOption
    {
        public String GroupNum { get; set; }
        public String FileSize { get; set; }
        public String FileCount { get; set; }
        public String TotalSize { get; set; }
        public List<FileListOption> FileInfoList;
        public ObservableCollection<FileListOption> FileInfoOptions { get; }
        public ICommand Command { get; set; }

        public FileGroupOption(Int32 groupNum, Int64 fileSize, Int32 fileCount, Int64 totalSize, List<FileInfo> files)
        {
            GroupNum = Convert.ToString(groupNum);
            FileSize = Common.getFormattedSize(fileSize);

            FileCount = Convert.ToString(fileCount);
            TotalSize = Common.getFormattedSize(totalSize); //Convert.ToString(totalSize);

            FileInfoOptions = new ObservableCollection<FileListOption>();
            for (Int32 i = 0; i < files.Count; i++) 
                FileInfoOptions.Add(new FileListOption(files[i].Path, files[i].CreatDateTime, files[i].ModifDateTime));
        }
    }
}
