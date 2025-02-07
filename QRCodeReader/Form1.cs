using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using IronQr;
using IronSoftware.Drawing;
using SixLabors.Fonts.Unicode;

namespace QRCodeReader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.TopMost = true; // Ensures the form stays on top
            this.BackColor = System.Drawing.Color.LimeGreen; // Set a color that won't be visible
            this.TransparencyKey = System.Drawing.Color.LimeGreen; // Make that color transparent
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Get the form's position and size in screen coordinates
            // Had to manually adjust position
            int startX = (int)(this.Left + this.Bounds.Left * 1.3);
            int startY = (int)(this.Top * 2.53);
            int width = (int)(this.Width * 2 * 1.1);
            int height = (int)(this.Height * 2 * 1.045);

            // Create a new bitmap with the form's dimensions
            using (Bitmap bm = new Bitmap(width, height, PixelFormat.Format32bppRgb))
            {
                using (Graphics g = Graphics.FromImage(bm))
                {
                    // Capture the screen area where the form is positioned
                    g.CopyFromScreen(new System.Drawing.Point(startX, startY), System.Drawing.Point.Empty, new System.Drawing.Size(width, height));
                }

                // Dispose of old background image before assigning a new one
                if (this.BackgroundImage != null)
                {
                    this.BackgroundImage.Dispose();
                }

                // Set the new captured image
                this.BackgroundImage = (Bitmap)bm.Clone();

                // Change backcolor to visible color and show exit button
                this.BackColor = System.Drawing.Color.LightGray;
                button2.Visible = true;

                // Save the image
                string filePath = "MyImage.bmp";
                bm.Save(filePath, ImageFormat.Bmp);

                //Read QR Code

                var inputBmp = AnyBitmap.FromFile("MyImage.bmp");

                QrImageInput impageInput = new QrImageInput(inputBmp);

                QrReader reader = new QrReader();

                IEnumerable<QrResult> results = reader.Read(impageInput);

                richTextBox1.Visible = true;
                richTextBox1.Text = "";

                foreach (var result in results)
                {
                    string value = result.Value;
                    int start = richTextBox1.TextLength; // To get the current position in the text box
                    richTextBox1.AppendText(value + Environment.NewLine);
                    richTextBox1.Select(start, value.Length);
                    richTextBox1.SelectionColor = System.Drawing.Color.Blue;
                    richTextBox1.SelectionFont = new System.Drawing.Font(richTextBox1.Font, System.Drawing.FontStyle.Underline);
                    richTextBox1.DeselectAll();
                }
            }
        }
        // X button
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.BackgroundImage == null)
            {
                return;
            }
            else
            {
                // Change to transparent and remove image
                this.BackColor = System.Drawing.Color.LimeGreen;
                this.BackgroundImage = null;

                // Delete MyImage if it exists
                if (System.IO.File.Exists("MyImage.bmp"))
                {
                    System.IO.File.Delete("MyImage.bmp");
                }

                // Reset form
                button2.Visible = false;
                richTextBox1.Text = "";
                richTextBox1.Visible = false;
            }
        }

        // Allows hyperlinks
        private void richTextBox1_Clicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }
    }
}
