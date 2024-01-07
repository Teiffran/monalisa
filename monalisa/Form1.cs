using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace monalisa
{
    public partial class Form1 : Form
    {
        bool working = false;
        int sizepic = 480;
        int pixels;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int xpos = 0;
            int ypos = 0;
            //Random random = new Random();
            for (int i = 0; i != 12; i++)
            {
                for (int j = 0; j != 12; j++)
                {
                    PictureBox pb = new PictureBox();
                    pb.Name = i.ToString();
                    pb.Size = new Size(40, 40);
                    pb.Location = new Point(xpos, ypos);
                    //Color randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                    pb.BackColor = Color.Black;
                    panel1.Controls.Add(pb);
                    xpos += 40;
                }
                xpos= 0;
                ypos += 40;
            }
        }

        private int currentX = 10;
        private int currentY = 10;
        private const int padding = 10;

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            working = true;
            Random random = new Random();
            uint attempts = (uint)numericUpDown1.Value;
            int sizepic = (int)numSize.Value;
            int pixels = (int)numPixels.Value;
            int sp = sizepic / pixels;

            foreach (PictureBox pb in panel1.Controls.OfType<PictureBox>().ToList())//delete all pixels
            {
                panel1.Controls.Remove(pb);
                pb.Dispose();
            }
            int xpos = 0;
            int ypos = 0;
            for (int i = 0; i != pixels; i++)//create pixels
            {
                for (int j = 0; j != pixels; j++)
                {
                    PictureBox pb = new PictureBox();
                    pb.Name = i.ToString();
                    pb.Size = new Size(sp, sp);
                    pb.Location = new Point(xpos, ypos);
                    //Color randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                    pb.BackColor = Color.Black;
                    panel1.Controls.Add(pb);
                    xpos += sp;
                }
                xpos = 0;
                ypos += sp;
            }

            for (uint i = 0; i != attempts; i++)
            {
                Bitmap miniImage = new Bitmap(120, 120);
                Bitmap maxImage = new Bitmap(sizepic, sizepic);
                foreach (PictureBox pb in panel1.Controls)
                {
                    Color randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                    pb.BackColor = randomColor;
                }

                using (Graphics g = Graphics.FromImage(miniImage))
                {
                    int x = 0;
                    int y = 0;

                    foreach (PictureBox pb in panel1.Controls)
                    {
                        Color color = pb.BackColor;
                        int sizep = 120 / pixels;

                        g.FillRectangle(new SolidBrush(color), x, y, sizep, sizep);

                        x += sizep;

                        if (x >= 120)
                        {
                            x = 0;
                            y += sizep;
                        }
                    }
                }

                using (Graphics g = Graphics.FromImage(maxImage))
                {
                    int x = 0;
                    int y = 0;

                    foreach (PictureBox pb in panel1.Controls)
                    {
                        Color color = pb.BackColor;

                        g.FillRectangle(new SolidBrush(color), x, y, sp, sp);

                        x += sp;

                        if (x >= sizepic)
                        {
                            x = 0;
                            y += sp;
                        }
                    }
                }

                PictureBox miniPictureBox = new PictureBox
                {
                    Size = new Size(120, 120),
                    Image = miniImage,
                };

                miniPictureBox.Click += (s, ev) => ShowImageInNewWindow(maxImage);

                miniPictureBox.Location = new Point(currentX, currentY);

                currentX += miniPictureBox.Width + padding;

                if (currentX + miniPictureBox.Width > panel2.Width)
                {
                    currentX = padding;
                    currentY += miniPictureBox.Height + padding;
                }

                panel2.Controls.Add(miniPictureBox);

                await Task.Delay(500);
                if (working == false)
                {
                    break;
                }
            }
            button1.Enabled = true;
            working = false;
        }

        private void ShowImageInNewWindow(Bitmap image)
        {
            Form newForm = new Form
            {
                Text = "Picture",
                Size = new Size(sizepic, sizepic)
            };

            PictureBox pictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Fill,
                Image = image
            };

            Button btnSave = new Button
            {
                Text = "Save",
                Size = new Size(75, 23),
                Dock = DockStyle.Bottom,
                DialogResult = DialogResult.OK
            };
            btnSave.Click += (sender, args) => SaveImage(image);

            newForm.Controls.Add(pictureBox);
            newForm.Controls.Add(btnSave);

            newForm.ShowDialog();
        }

        private void SaveImage(Bitmap image)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Image PNG (*.png)|*.png|Image JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg";
                saveDialog.FilterIndex = 1;

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    System.Drawing.Imaging.ImageFormat imageFormat;

                    switch (saveDialog.FilterIndex)
                    {
                        case 1:
                            imageFormat = System.Drawing.Imaging.ImageFormat.Png;
                            break;
                        case 2:
                            imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                            break;
                        default:
                            MessageBox.Show("Invalid format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                    }

                    image.Save(saveDialog.FileName, imageFormat);
                    MessageBox.Show("Saved", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (working == true)
            {
                button2.Enabled = false;
                working = false;
                button2.Enabled = true;
            }
        }

        private void numSize_ValueChanged(object sender, EventArgs e)
        {
            textBox1.Text = numSize.Value.ToString();
            numPixels.Maximum = (int)numSize.Value;
        }

        private void numPixels_ValueChanged(object sender, EventArgs e)
        {
            
        }
    }
}
