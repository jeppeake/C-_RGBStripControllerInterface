using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rgbController
{
    public partial class Form1 : Form
    {
        SerialPort sPort;
        char[] send = { (char)255, (char)255, (char)255};
        int r = 255; int g = 255; int b = 255;
        float intensity = 1.0f;

        Thread loop;

        public Form1(string s)
        {
            InitializeComponent();
            sPort = new SerialPort(s, 9600);
            sPort.Open();
            this.Text = s;
            loop = new Thread(new ThreadStart(loopFunc));
            loop.IsBackground = true;
            loop.Start();
            Bitmap bitmap = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            /*int r, g, b;
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    r = 255 - x;
                    g = 255 - Math.Abs(bitmap.Width / 2 - x);
                    b = 255 - (bitmap.Width - x);
                    bitmap.SetPixel(x, y, Color.FromArgb(Math.Max(Math.Min(r,255),0), Math.Max(Math.Min(g,255),0), Math.Max(Math.Min(b,255),0)));
                }
            }
            pictureBox1.Image = bitmap;*/
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Left)
            {
                Point coordinates = me.Location;
                Bitmap map = (Bitmap)pictureBox1.Image;
                Color c = map.GetPixel(Math.Min(Math.Max(coordinates.X, 0), 254), Math.Min(Math.Max(coordinates.Y, 0), 254));
                panel1.BackColor = c;

                r = c.R; g = c.G; b = c.B;
                sendValues();
                numericUpDown1.Value = r; numericUpDown2.Value = g; numericUpDown3.Value = b;
            }
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            r = (int)numericUpDown1.Value;
            sendValues();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            g = (int)numericUpDown2.Value;
            sendValues();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            b = (int)numericUpDown3.Value;
            sendValues();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Left)
            {
                Point coordinates = me.Location;
                Bitmap map = (Bitmap)pictureBox1.Image;
                Color c = map.GetPixel(Math.Min(Math.Max(coordinates.X,0),254), Math.Min(Math.Max(coordinates.Y, 0), 254));
                panel1.BackColor = c;
                
                r = c.R; g = c.G; b = c.B;
                sendValues();
                numericUpDown1.Value = r; numericUpDown2.Value = g; numericUpDown3.Value = b;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            intensity = trackBar1.Value / 100.0f;
            sendValues();
        }

        void sendValues()
        {
            send[0] = (char)(r * intensity);
            send[1] = (char)(g * intensity);
            send[2] = (char)(b * intensity);
            if (!sPort.IsOpen)
                sPort.Open();
            sPort.Write(send, 0, 3);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        void fadeFrom(string str)
        {
            if(str == "redgreen")
            {
                while(g < 255)
                {
                    g += 1;
                    r -= 1;
                    sendValues();
                    Thread.Sleep(100);
                }
                r = 0;
                g = 255;
            }
            else if (str == "greenblue")
            {
                while (b <= 255)
                {
                    b += 1;
                    g -= 1;
                    sendValues();
                    Thread.Sleep(100);
                }
                g = 0;
                b = 255;
            }
            else if (str == "bluered")
            {
                while (r < 255)
                {
                    b -= 1;
                    r += 1;
                    sendValues();
                    Thread.Sleep(100);
                }
                b = 0;
                r = 255;
            }
        }

        bool intensUp = true;

        void loopFunc()
        {
            while(true)
            {
                if (checkBox1.Checked)
                {
                    r = 255; g = 0; b = 0;
                    fadeFrom("redgreen");
                    fadeFrom("greenblue");
                    fadeFrom("bluered");
                }
                if(checkBox2.Checked)
                {
                    if(intensUp)
                    {
                        intensity += 0.01f;
                        if(intensity >= 0.5f)
                        {
                            intensUp = false;
                            intensity = 0.5f;
                            Thread.Sleep(100);
                        }
                    }
                    else
                    {
                        intensity -= 0.01f;
                        if (intensity <= 0.04f)
                        {
                            intensUp = true;
                            intensity = 0.04f;
                           Thread.Sleep(100);
                        }
                    }
                    sendValues();
                    Thread.Sleep(10);
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            intensUp = true;
            intensity = 0.4f;
        }
    }
}
