using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mp3TitleEditor
{
    public partial class Mp3Info : Form
    {
        private string path;
        private TagLib.File mp3;

        public Mp3Info()
        {
            InitializeComponent();
        }

        private void Mp3Info_Load(object sender, EventArgs e)
        {

        }

        public void Init(string path)
        {
            this.path = path;
            this.mp3 = TagLib.File.Create(path);

            textBox1.Text = mp3.Name;
            textBox2.Text = mp3.Tag.Title;
            textBox3.Text = mp3.Tag.JoinedArtists;
            textBox4.Text = mp3.Tag.Album;
            numericUpDown1.Value = mp3.Tag.Track;
            numericUpDown2.Value = mp3.Tag.Year;
            richTextBox1.Text = mp3.Tag.Lyrics;
            TagLib.Tag tag = mp3.GetTag(TagLib.TagTypes.AllTags);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mp3.Tag.Title = textBox2.Text;
            mp3.Tag.Album = textBox4.Text;
            mp3.Tag.Track = Convert.ToUInt32(numericUpDown1.Value);
            mp3.Tag.Year = Convert.ToUInt32(numericUpDown2.Value);
            mp3.Tag.Lyrics = richTextBox1.Text;
            mp3.Save();
            this.Close();
        }
    }
}
