using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            for (uint i = 0; i != attempts; i++)
            {
                Bitmap miniImage = new Bitmap(120, 120);
                Bitmap maxImage = new Bitmap(480, 480);
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

                        g.FillRectangle(new SolidBrush(color), x, y, 10, 10);

                        x += 10;

                        if (x >= 120)
                        {
                            x = 0;
                            y += 10;
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

                        g.FillRectangle(new SolidBrush(color), x, y, 40, 40);

                        x += 40;

                        if (x >= 480)
                        {
                            x = 0;
                            y += 40;
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
                Size = new Size(480, 480)
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
                saveDialog.Filter = "Image PNG (*.png)|*.png|Image JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|All files (*.*)|*.*";
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
                            throw new ArgumentOutOfRangeException();//
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
    }
}
