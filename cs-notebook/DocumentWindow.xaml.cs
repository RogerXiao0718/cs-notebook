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
using Media = System.Windows.Media;
using Forms = System.Windows.Forms;

namespace cs_notebook
{
    public partial class DocumentWindow : Window
    {
        string currFilePath = ""; //表示這個檔案是否有被存檔，如果是新檔案的話這個欄位會是空字串
        Media.Brush currFontBrush;
        Media.Brush currBackgroundBrush;

        public DocumentWindow()
        {
            InitializeComponent();
            EnumerateSystemFontFamilies();  //將電腦上安裝的FontFamily都列舉出來並放進fontComboBox
            FillFontSizeComboBox();     //將可以選的文字大小都填進fontSizeComboBox
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

        
        private void FillFontSizeComboBox()
        {
            //把fontSizeComboBox的ItemsSource設為List<int>
            //在Xaml檔用DataTemplate配合IValueConverter將ComboBox所要顯示的內容轉成字串
            // ./IntToPxStringConverter.cs內的IntToPxStringConverter class實作IValueConverter
            //ex. 10 -> "10px"
            List<int> size_list = new List<int>();
            for (int i = 8; i <= 80; i+=2)
            {
                size_list.Add(i);
            }
            fontSizeComboBox.ItemsSource = size_list;
        }

        //參考 https://stackoverflow.com/questions/17994674/c-sharp-wpf-line-and-column-number-from-richtextbox
        private void richTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextPointer tp1 = richTextBox.Selection.Start.GetLineStartPosition(0); //指標指的那一列的第一個字元的位置
            TextPointer tp2 = richTextBox.Selection.Start;  //指標所指的位置
            int column = tp1.GetOffsetToPosition(tp2);  //取得tp1和tp2的位移
            int someBigNumber = int.MaxValue;  //取得整數型別的最大值
            int lineMoved, line;
            //GetLineStartPosition的第一個參數如果是-n的話回傳現在所指的位置的前n列的第一個位置
            //把-someBigNumber當第一個參數傳入的用意是保證能夠回傳第一列第一行(但這次不需要這個回傳值)
            //第二個參數可以取得實際所移動的列數
            richTextBox.Selection.Start.GetLineStartPosition(-someBigNumber, out lineMoved);
            line = (-lineMoved) + 1;
            statusLbl.Content = $"第{line}列:第{column}行";
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
            //若currFilePath為null或空字串則提示使用者儲存檔案到本機，否則儲存至currFilePath的檔案路徑
            if (String.IsNullOrEmpty(this.currFilePath))
            {
                SaveFileInSelectedPath();
            }
            else
            {
                SaveFileInCurrentPath();
            }
        }

        //提示使用者儲存檔案到本機
        private void SaveFileInCurrentPath()
        {
            using (FileStream fs = new FileStream(currFilePath, FileMode.Create))
            {
                TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                range.Save(fs, DataFormats.Rtf);
            }
        }

        //將檔案儲存至currFilePath
        private void SaveFileInSelectedPath()
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
        }

        //另存新檔
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
            this.Close(); //關閉這個Window
        }

        //新增一個視窗
        private void NewWindowClicked(object sender, RoutedEventArgs e)
        {
            DocumentWindow doc_window = new DocumentWindow();
            doc_window.Show();

        }

        //開啟新檔
        private void New_Executed(object sender, RoutedEventArgs e)
        {
            //問User是否儲存檔案，如果選擇存檔則做何Save_Executed相同的動作
            if (MessageBox.Show("是否儲存檔案?", "儲存檔案", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (String.IsNullOrEmpty(this.currFilePath))
                {
                    SaveFileInSelectedPath();
                }
                else
                {
                    SaveFileInCurrentPath();
                }
            }
            //將richTextBox的內容清空並且將this.Title重新設為NoteBook並把currFilePath設成空字串表示目前這個檔案還沒被存檔
            var textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            textRange.Text = "";
            this.Title = "NoteBook";
            this.currFilePath = "";
        }

        private void fontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selection = richTextBox.Selection;
            if (!selection.IsEmpty)
            {
                selection.ApplyPropertyValue(FontFamilyProperty, fontComboBox.SelectedItem as Media.FontFamily);         
            }
        }

        private void fontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            double size = Convert.ToDouble(fontSizeComboBox.SelectedItem);
            richTextBox.Selection.ApplyPropertyValue(FontSizeProperty, size);
        }

        private void fontColorBtn_Click(object sender, RoutedEventArgs e)
        {
            Forms.ColorDialog dialog = new Forms.ColorDialog();
            if (dialog.ShowDialog() == Forms.DialogResult.OK)
            {
                var choosedColor = dialog.Color;
                currFontBrush = new Media.SolidColorBrush(Media.Color.FromArgb(choosedColor.A, choosedColor.R, choosedColor.G, choosedColor.B));
                richTextBox.Selection.ApplyPropertyValue(ForegroundProperty, currFontBrush);
                //連richTextBox的指標筆刷都要改變才能夠讓之後打出來的字也能套用所選取的顏色
                richTextBox.CaretBrush = currFontBrush; 
                fontColorBtn.Background = currFontBrush;
            }
        }


        private void backgroundBtn_Click(object sender, RoutedEventArgs e)
        {
            Forms.ColorDialog dialog = new Forms.ColorDialog();
            if (dialog.ShowDialog() == Forms.DialogResult.OK)
            {
                var choosedColor = dialog.Color;
                currBackgroundBrush = new Media.SolidColorBrush(Media.Color.FromArgb(choosedColor.A,
                    choosedColor.R, choosedColor.G, choosedColor.B));
                richTextBox.Background = currBackgroundBrush;
            }
        }

        
    }

}
