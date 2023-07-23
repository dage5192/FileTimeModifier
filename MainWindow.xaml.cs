using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FileTimeChanger
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            dateTimePicker.SelectedDateTime = DateTime.Now;
        }

        private void ButtonSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                TextBoxFilePath.Text = openFileDialog.FileName;
            }
        }

        private void ButtonSetTime_Click(object sender, RoutedEventArgs e)
        {
            DateTime selectedDateTime = (DateTime)dateTimePicker.SelectedDateTime;
            string filePath = TextBoxFilePath.Text;
            //filePath = "D:\\dage\\Desktop\\test.txt";

            if (!IsFilePathValid(filePath)) return;

            if (cbCreationTime.IsChecked == true)
            {
                File.SetCreationTime(filePath, selectedDateTime);
            }

            if (cbModifiedTime.IsChecked == true)
            {
                File.SetLastWriteTime(filePath, selectedDateTime);
            }

            if (cbAccessTime.IsChecked == true)
            {
                File.SetLastAccessTime(filePath, selectedDateTime);
            }

            // 查看文件属性
            //var process = new Process();
            //process.StartInfo.FileName = @"pwsh.exe";
            //process.StartInfo.Arguments = "-noexit \n Get-ChildItem "+filePath;
            //process.Start();
        }
        private bool IsFilePathValid(string filePath)
        {
            // 1. 非空验证
            if (string.IsNullOrEmpty(filePath))
            {
                ShowLog("请选择一个文件");
                return false;
            }

            // 2. 文件存在性验证
            if (!File.Exists(filePath))
            {
                ShowLog("选择的文件不存在");
                return false;
            }

            // 3. 文件类型验证，暂时不必
            string ext = Path.GetExtension(filePath);
            if (false)
            {
                ShowLog("不支持该类型文件");
                return false;
            }

            string FileNameRegex = @"^[a-zA-Z]:\\(?:[^\\/:*""<>|\r\n]+\\)*[^\\/:*""<>|\r\n]*$";
            // 4. 路径格式验证
            if (!Regex.IsMatch(filePath, FileNameRegex))
            {
                ShowLog("文件名包含非法字符");
                return false;
            }

            // 5. 权限验证
            if (!CheckFileAccess(filePath))
            {
                ShowLog("没有访问权限");
                return false;
            }

            return true;
        }

        private bool CheckFileAccess(string file)
        {
            try
            {
                FileStream fs = File.Open(file, FileMode.Open);
                fs.Close();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private void ShowLog(string message)
        {
            LogTextBox.Text += "> " + DateTime.Now +" : "+ message + "\n";
            LogTextBox.ScrollToEnd();
        }


        public void ShowFileDetails(string filePath)
        {
            // 使用 "/select" 参数打开文件所在目录，并选中该文件
            Process.Start("explorer.exe", $"/select,\"{filePath}\"");

            // 使用 "Properties" 参数打开文件属性窗口
            Process.Start("explorer.exe", $"/Properties \"{filePath}\"");

        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            // 拖拽进入时,检查是否为文件类型
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            // 拖拽释放时,获取文件路径
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            TextBoxFilePath.Text = files[0];
        }
    }
}
