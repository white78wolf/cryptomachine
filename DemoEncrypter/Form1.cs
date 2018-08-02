using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
//using Microsoft.Office.Interop.Word;
using Code7248.word_reader;

namespace DemoEncrypter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        // Кнопка "Выход"
        private void tsmiExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // Кнопка "Очистить поля"
        private void tsmiClear_Click(object sender, EventArgs e)
        {
            rtbIn.Text = String.Empty;
            tbOut.Text = String.Empty;
            tbKey.Text = "12";
        }
        // Справка -> О программе
        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Автор: Андерс Хейлсберг, \nДата релиза: январь 2002 г.",
                "Enigma v 1.0",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
                );
        }
        // Кнопка "Преобразовать"
        private void btnAction_Click(object sender, EventArgs e)
        {
            int key;

            Int32.TryParse(tbKey.Text, out key);
            tbOut.Text = Encryption(rtbIn.Text, key);            
        }
        // Метод зашифровки/дешифровки по ключу на основе побитовых операций
        private string Encryption(string p1, int p2)
        {
            string temp = String.Empty;

            foreach (char c in p1)
            {
                temp += Convert.ToString((char)(((int)(c) ^ p2))); // Магия строгой дизьюнкции (XOR)
            }

            return temp;
        }
        // Заморочился с возможностью открывать текстовые файлы для зашифровки содержимого (без сохранения обратно)
        private void tsmiOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            // Фильтруем расширения для отображения текстовых файлов
            openFile.Filter = "Все текстовые файлы|*.txt;*rtf;*.doc;*docx;|Файлы .txt|*.txt|Файлы .rtf|*.rtf|Файлы .doc|*.doc|Файлы .docx|*.docx";
            
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string type = Path.GetExtension(openFile.FileName); // Извлекаем расширение
                switch (type)
                {
                    case ".txt":
                        try
                        {
                            rtbIn.Text = File.ReadAllText(openFile.FileName, Encoding.Default);                            
                        }
                        catch
                        {
                            MessageBox.Show("Ошибка загрузки");
                        }
                        break;

                    case ".rtf":
                        try
                        {
                            rtbIn.LoadFile(openFile.FileName, RichTextBoxStreamType.RichText);
                        }
                        catch
                        {
                            MessageBox.Show("Ошибка загрузки");
                        }
                        break;

                    case ".doc" : // Работа с Interop.Word заставила задуматься, но вот чудо...
                    case ".docx": // ...случайно нашёл в сети библиотеку Code7248.word_reader.dll
                        try
                        {
                            TextExtractor extractor = new TextExtractor(openFile.FileName);
                            string contents = extractor.ExtractText();
                            rtbIn.Text = contents;
                        }
                        catch
                        {
                            MessageBox.Show("Ошибка загрузки");
                        }                        
                        break;
                }
            }            
        }
        // Сохранение результата в буфер
        private void tsmiClipboard_Click(object sender, EventArgs e)
        {            
            if (tbOut.Text == "")
                Clipboard.Clear();
            Clipboard.SetText(tbOut.Text);
        }
        // Проблему с фокусом на текстовой форме ввода решил в визуальном конструкторе 
        // заданием номера в свойстве TabIndex элемента формы
    }
}
