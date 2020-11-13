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
using Drawing = System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Shapes;
using Microsoft.Win32;
using Spire.Pdf;
using Spire.Pdf.Exporting.XPS.Schema;
using Media = System.Windows.Media;
using Forms = System.Windows.Forms;

namespace cs_notebook
{
    /// <summary>
    /// Interaction logic for DocumentWindow.xaml
    /// </summary>
    public partial class DocumentWindow : Window
    {
        string currFilePath = "";
        bool isBold = false;
        bool isItalic = false;
        

        public DocumentWindow()
        {
            InitializeComponent();
            EnumerateSystemFontFamilies();
        }

        private void EnumerateSystemFontFamilies()
        {
            List<Media.FontFamily> fontFamilies = new List<Media.FontFamily>();
            foreach (var fontfamily in Media.Fonts.SystemFontFamilies)
            {
                fontFamilies.Add(fontfamily);
            }
            fontComboBox.ItemsSource = fontFamilies;

        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog filedialog = new OpenFileDialog()
            {
                Filter = "rtf檔案(*.rtf)|*.rtf|所有檔案|*.*",
            };
            if (filedialog.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(filedialog.FileName, FileMode.Open))
                {
                    TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                    range.Load(fs, DataFormats.Rtf);
                }
                this.Title = filedialog.SafeFileName;
                this.currFilePath = filedialog.FileName;
               
            }

        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.currFilePath))
            {
                SaveFileDialog filedialog = new SaveFileDialog()
                {
                    Filter = "rtf檔案(*.rtf)|*.rtf|所有檔案|*.*"
                };
                if (filedialog.ShowDialog() == true)
                {
                    currFilePath = filedialog.FileName;
                    using (FileStream fs = new FileStream(filedialog.FileName, FileMode.Create))
                    {

                        TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);

                        range.Save(fs, DataFormats.Rtf);

                    }
                    this.currFilePath = filedialog.FileName;
                }
            } else
            {
                using (FileStream fs = new FileStream(currFilePath, FileMode.Create))
                {
                    TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                    range.Save(fs, DataFormats.Rtf);
                }
            }
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog filedialog = new SaveFileDialog()
            {
                Filter = "rtf檔案(*.rtf)|*.rtf|所有檔案|*.*"
            };
            if (filedialog.ShowDialog() == true)
            {
                currFilePath = filedialog.FileName;
                using (FileStream fs = new FileStream(filedialog.FileName, FileMode.Create))
                {
                    TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                    range.Save(fs, DataFormats.Rtf);

                }
            }
        }

        private void Close_Executed(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void New_Executed(object sender, RoutedEventArgs e)
        {
            DocumentWindow doc_window = new DocumentWindow();
            doc_window.Show();

        }

        private void Print_Executed(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                //string text = richTextBox.Text;
                
            }
        }

        private void fontSelectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Forms.FontDialog dialog = new Forms.FontDialog()
            {
                ShowColor = true,
                ShowHelp = true,
                MinSize = 10,
                MaxSize = 44,
            };
            if (dialog.ShowDialog() == Forms.DialogResult.OK)
            {
                Drawing.Font selectedFont = dialog.Font;
                Media.FontFamily fontFamily = new Media.FontFamily(selectedFont.Name);
                double fontSize = selectedFont.SizeInPoints;
                var selectedColor = dialog.Color;
                var color = Media.Color.FromArgb(selectedColor.R, selectedColor.G, selectedColor.B, selectedColor.A);
                if (selectedFont.Italic == true) isItalic = !isItalic; //FontStyle fontstyle = Fontstyles.Italic;
                if (selectedFont.Bold == true) isBold = !isBold; // FontWeight fontweight = FontWeights.Bold;
            }
        }

        private void fontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTextValue = richTextBox.Selection;
            if (!selectedTextValue.IsEmpty)
            {
                selectedTextValue.ApplyPropertyValue(FontFamilyProperty, fontComboBox.SelectedItem as Media.FontFamily);
            }
        }
    }
}
