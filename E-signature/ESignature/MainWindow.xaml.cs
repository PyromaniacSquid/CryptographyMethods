using System;
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

namespace ESignature
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _userChosen = false;
        private bool _documentSaved = true;
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool AssertUserChosen()
        {
            if (!_userChosen) MessageBox.Show("Необходимо выбрать пользователя.");
            return _userChosen;
        }
        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void CreateDocument()
        {
            TextEditor.Document.Blocks.Clear();
            TextEditor.IsEnabled = true;
            this.Title = "Подписанный документ";
        }

        private void ChooseUserButtonClick(object sender, RoutedEventArgs e)
        {
            _userChosen = true;
        }
        private void LoadDocumentButtonClick(object sender, RoutedEventArgs e)
        {
            
        }
        private void SaveDocumentButtonClick(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
