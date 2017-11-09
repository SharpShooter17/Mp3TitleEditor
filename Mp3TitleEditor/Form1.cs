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
using System.Media;

namespace Mp3TitleEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if ( checkBox2.Checked == true )
            {
                textBox1.Enabled = true;
            }
            else if ( checkBox2.Checked == false )
            {
                textBox1.Enabled = false;
            }
        }

        private List<string> GetFiles()
        {
            List<string> plikiMp3;
            List<string> sciezki;
            if (checkBox9.Checked == true)
            {
                plikiMp3 = new List<string>();
                sciezki = new List<string>(Directory.GetDirectories(folderBrowserDialog1.SelectedPath, "*", SearchOption.AllDirectories));
                ///Dodawanie plików z głównego katalogu
                /// 
                string[] k = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.mp3");
                foreach (string str in k)
                {
                    plikiMp3.Add(str);
                }

                ///Dodawanie plików z podkatalogów
                /// 
                for (int i = 0; i < sciezki.Count; ++i)
                {
                    string[] s = Directory.GetFiles(sciezki[i], "*.mp3");
                    foreach (string str in s)
                    {
                        plikiMp3.Add(str);
                    }
                }
            }
            else
            {
               plikiMp3 = new List<string>(Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.mp3"));
            }

            return plikiMp3;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            label2.Text = folderBrowserDialog1.SelectedPath;

            listBox1.Items.Clear();

            if (Directory.Exists(folderBrowserDialog1.SelectedPath))
            {
                List<string> pliki = new List<string>();
                pliki = GetFiles();

                listBox1.BeginUpdate();
                for (int i = 0; i < pliki.Count; ++i)
                {
                    listBox1.Items.Add(pliki[i]);
                }
                listBox1.EndUpdate();
            }
        }

        private void addToConsola( string str )
        {
            konsola.Text += "\n" + str;
            konsola.SelectionStart = konsola.Text.Length;
            konsola.ScrollToCaret();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ///Tylko pliki mp3          
            List<string> filePaths = new List<string>(GetFiles());

            progressBar1.Maximum = filePaths.Count;

            ///Start!
            /// 
            konsola.Text = "|________________|"
                          +"|\t Start!     \t|"
                          +"|________________|";


            for ( int i = 0; i < filePaths.Count; ++i)
            {
                progressBar1.PerformStep();
                progressBar1.Update();
                addToConsola("----ZACZYNAM PRZETWARZAĆ ŚCIEŻKĘ----");
                addToConsola(filePaths[i]);
                int p = Convert.ToInt32(((Convert.ToDouble(i) + 1.0) / Convert.ToDouble(filePaths.Count)) * 100.00);
                label5.Text = p.ToString() + "%";
                label7.Text = "[" + (i + 1) + "/" + filePaths.Count + "]";
                groupBox3.Refresh();
                string FileName = Path.GetFileNameWithoutExtension(filePaths[i]);
                
                ///Usuń podaną frazę
                if (checkBox2.Checked == true && textBox1.Text != String.Empty)
                {
                    if (FileName.Contains(textBox1.Text))
                    {
                        FileName = FileName.ToLower().Replace(textBox1.Text.ToLower(), String.Empty);
                        if (checkBox10.Checked == true)
                        {
                            FileName += " " + textBox1.Text;
                        }
                    }
                }

                ///Usuwa liczby
                if (checkBox1.Checked == true)
                {
                    for (int j = 0; j < 10; ++j)
                    {
                        FileName = FileName.Replace(j.ToString(), String.Empty);
                    }
                }

                ///Usuwa znaki interpunkcyjne
                if (checkBox3.Checked == true)
                {
                    string[] znaki = { ".", ",", "/", "?", "-", "_",
                                       "!", "@", "#", "$", "%", "<",
                                       ">", "+", "|", "\\", "[", "]",
                                       "{", "}", ":", ";", "\"", "'",
                                       "=", "*", "~", "`"};

                    for (int j = 0; j < znaki.Count(); ++j)
                    {
                        FileName = FileName.Replace(znaki[j], " ");
                    }
                }

                ///Dodaje PreFix
                /// 

                if ( textBox2.Text != String.Empty  )
                {
                    FileName = textBox2.Text + " " + FileName;
                }

                ///Dodaje Surfix
                /// 

                if (textBox3.Text != String.Empty)
                {
                    FileName += " " + textBox3.Text;
                }

                TagLib.File mp3 = TagLib.File.Create(filePaths[i]);

                if (mp3.Tag.IsEmpty == false)
                {
                    ///Usuwa tag numer
                    if (checkBox7.Checked)
                    {
                        try
                        {
                            mp3.Tag.Track = 0;
                            mp3.Save();
                        }
                        catch
                        {
                            addToConsola("[Error] Błąd usunięcia numeracji z pliku");
                        }
                    }

                    ///Ustawia nazwę pliku jako tytuł utworu - tylko w przypadku gdy plik nie ma tytułu
                    /// 
                    if ( checkBox11.Checked && FileName.ToLower() != mp3.Tag.Title.ToLower())
                    {
                        try
                        {
                            mp3.Tag.Title = FileName;
                            mp3.Save();
                        }
                        catch
                        {
                            addToConsola("[Error] Nie można ustawić nazwy pliku jako tytułu");
                        }
                    }

                    ///Ustawia wykonawce utworu podany w textbox4
                    /// 
                    if ( checkBox14.Checked )
                    {
                        mp3.Tag.Artists = textBox4.Text.Split(new char[] { ' ' });
                        mp3.Save();
                    }


                    if (groupBox2.Enabled == true)
                    {
                        ///Jeżeli plik posiada tytuł w tagu to ustaw tytuł jako nazwę pliku

                        try
                        {
                            if (checkBox6.Checked == true && mp3.Tag.Title.Trim() != String.Empty)
                            {
                                File.Move(filePaths[i], Path.GetDirectoryName(filePaths[i]) + "\\" + mp3.Tag.Title + ".mp3");
                                continue;
                            }
                        }
                        catch
                        {
                            addToConsola("[Error] Błąd zmiany tytułu pliku na tytuł z tag'u");
                        }

                        ///Usuwa wykonawce utworu z tytułu
                        try
                        {
                            if (checkBox4.Checked == true && mp3.Tag.JoinedArtists.ToString().Trim() != String.Empty)
                            {
                                FileName = FileName.ToLower().Replace(mp3.Tag.JoinedArtists.ToString().Trim().ToLower(), String.Empty);
                            }
                        }
                        catch
                        {
                            addToConsola("[Error] Nie mogę usunąć nazwy artysty");
                        }

                        ///Usuwa nazwę albumu z utworu
                        try
                        {
                            if (checkBox5.Checked == true && mp3.Tag.Album.Trim() != "")
                            {
                                FileName = FileName.ToLower().Replace(mp3.Tag.Album.ToLower().Trim(), String.Empty);
                            }
                        }
                        catch
                        {
                            addToConsola("[Error] Nie mogę usunąć nazwy albumu");
                        }
                    }
                    else if (groupBox4.Enabled == true)
                    {
                        ///Jeżeli tag nie posiada odpowiednich informacji to opuść daną ścieżkę
                        /// 
                        try
                        {
                           // if (mp3.Tag.Title.Length < 3 || mp3.Tag.Album.Trim().Length < 3 || mp3.Tag.JoinedArtists.Trim().Length < 3)
                           // {
                          //      addToConsola("[BRAK INFORMACJI] Brak informacj o Tytule lub Albumie lub Wykonawcy w ścieżce");
                           // }
                          //  else
                            {
                                List<string> nazwa = new List<string>();
                                nazwa.Add(comboBox1.Text);
                                nazwa.Add(comboBox2.Text);
                                nazwa.Add(comboBox3.Text);
                                for (int k = 0; k < 3; ++k)
                                {
                                    switch (nazwa[k])
                                    {
                                        case "Tytuł":
                                            {
                                                if (mp3.Tag.Title != String.Empty)
                                                    nazwa[k] = mp3.Tag.Title.Trim();
                                                else
                                                    nazwa[k] = FileName;
                                            }
                                            break;
                                        case "Wykonawca":
                                            {
                                                if (mp3.Tag.JoinedArtists.ToString().Trim() != String.Empty)
                                                    nazwa[k] = mp3.Tag.JoinedArtists.ToString().Trim();
                                            }
                                            break;
                                        case "Album":
                                            {
                                                if (mp3.Tag.Album.Trim() != String.Empty)
                                                    nazwa[k] = mp3.Tag.Album.Trim();
                                            }
                                            break;
                                        case "#########":
                                            {
                                                nazwa[k] = "";
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                FileName = "";
                                for (int f = 0; f < 3; f++)
                                {
                                    if (nazwa[f] != "")
                                    {
                                        FileName += nazwa[f] + " ";
                                    }
                                }
                            }
                        }
                        catch
                        {
                            addToConsola("[ERROR!!!] Plik nie nadaje się do edycji!");
                        }
                    }
                }
                else
                {
                    addToConsola("[Error] Plik nie posiada Tagów");
                }

                ///Usun białe znaki
                FileName = FileName.Trim();

                ///Zmiana nazwy pliku ;)
                try
                {
                    File.Move(filePaths[i], Path.GetDirectoryName(filePaths[i]) + "\\" + FileName + ".mp3");
                }
                catch
                {
                    addToConsola("[Error] Nie mogę zmienić nazwy pliku na: " + FileName);
                }

                ///Sortowanie
                /// 
                if ( checkBox12.Checked == true)
                {
                    string path = Path.GetDirectoryName(filePaths[i]);
                    if ( mp3.Tag.JoinedArtists.ToString().Trim() != "" )
                    {                        
                        if ( !Directory.Exists(path + mp3.Tag.JoinedArtists.ToString().Trim()) )
                        {
                            Directory.CreateDirectory(path + "\\" + mp3.Tag.JoinedArtists.ToString().Trim());
                        }
                        File.Move(Path.GetDirectoryName(filePaths[i]) + "\\" + FileName + ".mp3", path + "\\" + mp3.Tag.JoinedArtists.ToString().Trim() + "\\" + FileName + ".mp3");
                    }
                    else
                    {
                        if ( !Directory.Exists(path + "Nieznany wykonawca") )
                        {
                            Directory.CreateDirectory(path + "\\Nieznany wykonawca");
                        }
                        File.Move(Path.GetDirectoryName(filePaths[i]) + "\\" + FileName + ".mp3", path + "\\Nieznany wykonawca" + "\\" + FileName + ".mp3");
                    }
                }

                ///Nie sortowanie - wypakowywanie plików do jednego folderu
                /// 
                if ( checkBox13.Checked == true )
                {
                    File.Move(Path.GetDirectoryName(filePaths[i]) + "\\" + FileName + ".mp3", folderBrowserDialog1.SelectedPath + "\\" + FileName + ".mp3");
                }

                addToConsola("------------------------------------------------------------------");
                continue;
            }
            textBox1.Text = String.Empty;
            addToConsola("KONIEC!");
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if ( checkBox8.Checked == true )
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                groupBox4.Enabled = true;
            }
            else if (checkBox8.Checked == false )
            {
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                groupBox4.Enabled = false;
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.Items.Count != 0)
            {
                Mp3Info dlg = new Mp3Info();
                dlg.Init(listBox1.SelectedItem.ToString());
                dlg.ShowDialog();
            }
        }
    }
}
