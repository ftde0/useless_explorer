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
using System.Diagnostics;

namespace UselessExplorer
{
    public partial class Form1 : Form
    {

        Random rng = new Random();
        string c_dir = "C:\\Users\\" + Environment.UserName + "\\";
        string file_open = "";

        public Form1()
        {
            InitializeComponent();
        }

        public string random_series() {

            string res = "";


            char[] a = "qwertyuiopasdfghjklzxcvbnm".ToArray();
            while (res.Length != 10) {
                res += a[rng.Next(a.Length)];
            }

            return res;
        }

        void render_dir(string dir) {

            // remove previous labels
            previous_remove();

            try
            {
                // get a list of folders and files
                foreach (string directory in Directory.GetDirectories(dir))
                {
                    string char_code = random_series();

                    Label a = new Label();
                    a.Text = "[" + char_code + "] " + Path.GetFileName(directory);
                    a.Location = new Point(rng.Next(600), rng.Next(500));
                    a.ForeColor = Color.FromArgb(rng.Next(255), rng.Next(255), rng.Next(255));
                    a.AutoSize = true;
                    a.Tag = "d"; //directory
                    a.AccessibleDescription = directory; //accessible dirname
                    Controls.Add(a);
                }

                foreach (string file in Directory.GetFiles(dir))
                {
                    string char_code = random_series();

                    Label a = new Label();
                    a.Text = "[" + char_code + "] " + Path.GetFileName(file);
                    a.Location = new Point(rng.Next(600), rng.Next(500));
                    a.ForeColor = Color.FromArgb(rng.Next(255), rng.Next(255), rng.Next(255));
                    a.AutoSize = true;
                    a.Tag = "f"; //file
                    a.AccessibleDescription = file; //accessible filename
                    Controls.Add(a);
                }
            }
            catch (UnauthorizedAccessException) {
                MessageBox.Show("No permission. Try restarting as Admin?");
                Application.Exit();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Tick += timer1_Tick;
            textBox1.TextChanged += TextBox1_Input;
            render_dir(c_dir);
        }


        public void TextBox1_Input(object sender, EventArgs e) {
            // remove the labels that don't start with the textbox text
            foreach (Control c in Controls) {
                if (c.Name != "textBox1" && c.Text.Substring(0, textBox1.Text.Length + 1) != "[" + textBox1.Text && c.Tag.ToString() != "x") {
                    c.Hide();
                    c.Parent.Controls.RemoveByKey(c.Name);
                }

                if(c.Text.StartsWith("[" + textBox1.Text + "]")) {
                    // exact code
                    // check the tag
                    // "d" == navigate folders
                    // "f" == bring up an action screen
                    // "o" == start the file
                    // "x" == remove

                    switch (c.Tag.ToString()) { 
                        case "d": {
                            c_dir = c.AccessibleDescription;
                            render_dir(c_dir);
                            break;
                        }
                        case "f": {
                            file_open = c.AccessibleDescription;
                            action(c.AccessibleDescription);
                            break;
                        }
                        case "o": {
                            timer1.Stop();
                            Process.Start(c.AccessibleDescription);
                            render_dir(c_dir);
                            break;
                        }
                        case "x": {
                            File.Delete(c.AccessibleDescription);
                            MessageBox.Show("File removed");
                            break;
                        }
                    }
                }
            }
        }

        // action screen
        public void action(string file) {
            previous_remove();

            string[] texts = { "Select what to do with " + Path.GetFileName(file), String.Concat("[", random_series(), "] Open"), String.Concat("[", random_series(), "] Remove"), "Auto-defaulting to Remove in 10 seconds..." };
            string[] tags = { "", "o", "x", "" };

            int index = 0; //c# doesn't have indexOf? i'm probably missing something though, more of a webdev guy

            foreach(string text in texts) {
                Label a = new Label();
                a.Text = text;
                a.Location = new Point(30, 30 * (index + 1));
                a.ForeColor = Color.FromArgb(rng.Next(255), rng.Next(255), rng.Next(255));
                a.AutoSize = true;
                a.Tag = tags[index];
                a.AccessibleDescription = file; //accessible dirname
                Controls.Add(a);
                index += 1;
            }

            // timer start
            timer1.Start();
        }

        // remove elements
        public void previous_remove() {
            textBox1.Text = "";

            foreach (Control c in Controls) {
                if (c.Name != "textBox1") {
                    c.Hide();
                    c.Parent.Controls.RemoveByKey(c.Name);
                }
            }
        }

        // remove current file
        public void timer1_Tick(object sender, EventArgs e) {
            timer1.Stop();

            File.Delete(file_open);
            MessageBox.Show("File removed");

            render_dir(c_dir);
        }
    }
}
