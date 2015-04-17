using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace FileFolderPermissions
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        
        public MainWindow() 
        {
            InitializeComponent();
        }

        private void Button_Browse_Click(object sender, RoutedEventArgs e) {
            FolderBrowserDialog FolderView = new FolderBrowserDialog();
            FolderView.SelectedPath = "C:\\";
            DialogResult result = FolderView.ShowDialog();

            if (result.ToString() == "OK") {
                Textbox_Browse_Path.Text = FolderView.SelectedPath;
            }
        }

        private void Button_Browse_Log_Click(object sender, RoutedEventArgs e) {
            FolderBrowserDialog FolderView = new FolderBrowserDialog();
            FolderView.SelectedPath = "C:\\";
            DialogResult result = FolderView.ShowDialog();

            if (result.ToString() == "OK") {
                Textbox_Browse_Log_Path.Text = FolderView.SelectedPath;
            }
        }

        private void Button_Check_Path_Click(object sender, RoutedEventArgs e)
        {
            Textbox_Browse_Path.Text = "W:\\Utmp";
            Textbox_Browse_Log_Path.Text = "C:\\Users\\tuschl\\Desktop";
            CheckFolder CheckDir = new CheckFolder(Textbox_Browse_Path.Text, Textbox_Browse_Log_Path.Text);
            if (CheckDir.directoryInfo())
            {
                RTextBox_Output.Document.Blocks.Clear();
                RTextBox_Output.AppendText("Logfile successfully created." + System.Environment.NewLine 
                                         + "Successfully retrieved rights for all folders found at " 
                                         + Textbox_Browse_Path.Text + System.Environment.NewLine);
            }
            else
            {
                RTextBox_Output.Document.Blocks.Clear();
                RTextBox_Output.AppendText("No write access to selected folder");
            }
        }
    }
}
