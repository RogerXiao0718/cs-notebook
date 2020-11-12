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
        }
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog filedialog = new OpenFileDialog()
            {
                Filter = "文字檔(*.txt)|*.txt|所有檔案|*.*",
            };
            if (filedialog.ShowDialog() == true)
            {
                this.currFilePath = filedialog.FileName;
                this.Title = filedialog.SafeFileName;
                textBox.Text = File.ReadAllText(filedialog.FileName, Encoding.Default);
            }

        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.currFilePath))
            {
                SaveFileDialog filedialog = new SaveFileDialog()
                {
                    Filter = "文字檔(*.txt)|*.txt|所有檔案|*.*"
                };
                if (filedialog.ShowDialog() == true)
                {
                    using (StreamWriter sw = new StreamWriter(filedialog.FileName, false, Encoding.Default))
                    {
                        sw.WriteLine(textBox.Text);
                    }
                }
            } else
            {
                using (StreamWriter sw = new StreamWriter(currFilePath, false, Encoding.Default))
                {
                    sw.WriteLine(textBox.Text);
                }
            }
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog filedialog = new SaveFileDialog()
            {
                Filter = "文字檔(*.txt)|*.txt|所有檔案|*.*"
            };
            if (filedialog.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(filedialog.FileName, false, Encoding.Default))
                {
                    sw.WriteLine(textBox.Text);
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
                string text = textBox.Text;
                
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
    }
}
