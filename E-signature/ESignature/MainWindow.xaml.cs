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
using System.Security.Cryptography.X509Certificates;

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
        private X509Certificate2 _certificate;
        public MainWindow()
        {
            InitializeComponent();
            ChooseUserButton.Focus();
            EnableInteractions(false);
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
            //menuKeySection.IsEnabled =
            fl;
        }

        private bool AssertUserChosen()
        {
            if (!_userChosen) MessageBox.Show("Необходимо выбрать сертификат.");
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
            try
            {
                SignedDoc doc = new SignedDoc(openFileDialog.FileName);
                if (ValidateSignatures(doc))
                {
                    var name = new X509Certificate2(doc.cert).GetNameInfo(X509NameType.SimpleName, false);
                        this.Title = "Подписано - " + name;
                    TextRange range = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd);
                    range.Text = doc.DocContent;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Файл повреждён.");
                return;
            }
        }
        private bool ValidateSignatures(SignedDoc doc)
        {
            if (!CryptoProvider.ValidateCertificate(new X509Certificate2(doc.cert)))
            {
                MessageBox.Show("Подлинность сертификата не подтверждена.");
                return false;
            }
            CryptoProvider.SetPublicKey(new X509Certificate2(doc.cert));
            if (!CryptoProvider.CheckDocSignature(doc.ESignature, Encoding.Default.GetBytes(doc.DocContent)))
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
            doc.cert = _certificate.GetRawCertData();
            doc.ESignature = CryptoProvider.GetDocSignature(Encoding.Default.GetBytes(range.Text));
            doc.SaveToFile(saveFileDialog.FileName);
            this.Title = "Подписано - " + doc._userName;
        }
        


        private void menuDeleteKeysButton_Click(object sender, RoutedEventArgs e)
        {
            //CryptoProvider.RemoveKeysByName(UsernameTextBox.Text);
            CryptoProvider.RemoveCert(_certificate);
            UsernameTextBox.Text = "";
            EnableInteractions(false);
        }

        private void menuChooseKeyButton_Click(object sender, RoutedEventArgs e)
        {
            var collection = X509Certificate2UI.SelectFromCollection(CryptoProvider.GetX509Certificate2Collection(),
                "Выберите сертификат", "", X509SelectionFlag.SingleSelection);
            if (collection == null || collection.Count == 0) return;
            _certificate = collection[0];
            UsernameTextBox.Text = _certificate.GetNameInfo(X509NameType.SimpleName, false);
            CryptoProvider.SetPrivateKey(_certificate);
            EnableInteractions(true);
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

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

}   
