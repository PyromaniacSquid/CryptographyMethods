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
using System.Security.Cryptography;
using System.IO;
using Microsoft.Win32;

namespace ESignature
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _userChosen = false;
        private bool _documentSaved = true;
        private string _user = "";
        
        public MainWindow()
        {
            InitializeComponent();
            ChooseUserButton.Focus();
            EnableInteractions(false);
            string key_path = AppDomain.CurrentDomain.BaseDirectory + "\\Keys";
            if (!Directory.Exists(key_path))
                Directory.CreateDirectory(key_path);
        }
        private void EnableInteractions(bool fl)
        {
            this.Title = "Подписанный документ";
            TextEditor.Document.Blocks.Clear();
            TextEditor.IsEnabled = 
            LoadDocumentButton.IsEnabled =
            SaveDocumentButton.IsEnabled =
            menuCreateDocumentButton.IsEnabled = 
            menuLoadDocumentButton.IsEnabled = 
            menuSaveDocumentButton.IsEnabled = 
            menuKeySection.IsEnabled =
            fl;
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
        private void ChooseUserButtonClick(object sender, RoutedEventArgs e)
        {
            EnableInteractions(true);
        }
        private void LoadDocumentButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // bool? может быть null так что явно проверяем
            if (openFileDialog.ShowDialog() == false)
                return;
            SignedDoc doc = new SignedDoc(openFileDialog.FileName);
            if (ValidateSignatures(doc))
            {
                this.Title = "Подписано - " + doc._userName;
                TextRange range = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd);
                range.Text = doc.DocContent;
            }

        }
        private bool ValidateSignatures(SignedDoc doc)
        {
            PublicKey key;
            try
            {
                string keyPath =
                    AppDomain.CurrentDomain.BaseDirectory + "\\Keys\\" + doc._userName + ".pk";
                key = new PublicKey(keyPath);
            }
            catch (Exception)
            {
                MessageBox.Show("Открытый ключ не найден");
                return false;
            }
            if (!CryptoProvider.CheckKeySignature(key.ESignature, key.publicKeyBlob))
            {
                MessageBox.Show("Электронная подпись открытого ключа не подтверждена.");
                return false;
            }
            if (!CryptoProvider.CheckDocSignature(doc.ESignature, Encoding.Default.GetBytes(doc.DocContent)
                , key.publicKeyBlob))
            {
                MessageBox.Show("Электронная подпись документа не подтверждена.");
                return false;
            }
            return true;
        }
        private void SaveDocumentButtonClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == false)
                return;
            SignedDoc doc = new SignedDoc();
            TextRange range = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd);
            doc.DocContent = range.Text;
            doc._userName = UsernameTextBox.Text;
            doc.ESignature = CryptoProvider.GetDocSignature(Encoding.Default.GetBytes(range.Text));
            doc.SaveToFile(saveFileDialog.FileName);
            this.Title = "Подписано - " + doc._userName;
        }
        

        private void UsernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                UsernameTextBox.IsEnabled = false;
                _user = UsernameTextBox.Text;
                if (!CryptoProvider.CspContainerExists(_user))
                {
                    if (MessageBox.Show(this, 
                        "Пара ключей пользователя не найдена. Создать новую пару?", "Ключи не найдены", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        CryptoProvider.CreateCspContainer(_user);
                        CryptoProvider.SetExistingCspContainer(_user);
                    }
                    else
                    {
                        UsernameTextBox.Text = "";
                        EnableInteractions(false);
                        return;
                    }
                }
                else
                {
                    CryptoProvider.SetExistingCspContainer(UsernameTextBox.Text);
                }
                EnableInteractions(true);
                TextEditor.Focus();
            }
        }

        private void menuExportKeyButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == false)
                return;
            PublicKey key = new PublicKey();
            key.publicKeyBlob = CryptoProvider.GetCurrentPublicKey();
            key._ownerName = UsernameTextBox.Text;
            key.ESignature = null;
            key.Save(saveFileDialog.FileName);
        }

        private void menuImportKeyButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == false)
                return;
            PublicKey key = new PublicKey(openFileDialog.FileName);
            key.ESignature = CryptoProvider.GetKeySignature(key.publicKeyBlob);
            string keyPath = AppDomain.CurrentDomain.BaseDirectory + "\\Keys\\" 
                + key._ownerName + ".pk";
            key.Save(keyPath);
        }

        private void menuDeleteKeysButton_Click(object sender, RoutedEventArgs e)
        {
            CryptoProvider.RemoveKeysByName(UsernameTextBox.Text);
            UsernameTextBox.Text = "";
            EnableInteractions(false);
        }

        private void menuChooseKeyButton_Click(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.IsEnabled = true;
            UsernameTextBox.Text = "";
            UsernameTextBox.Focus();
            EnableInteractions(false);
        }

        private void menuCreateDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            Title = "Подписанный документ";
            TextEditor.Document.Blocks.Clear();
        }

        private void AboutBoxButton_Click(object sender, RoutedEventArgs e)
        {
            new About().ShowDialog();
        }
    }

}   
