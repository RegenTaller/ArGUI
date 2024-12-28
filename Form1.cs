using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArduinoInterface


{
    public partial class Form1 : Form
    {
        int Flag = 1;

        string messageToSend = "";

        int COMFlag = -1;

        private ComPortHandler _COMport;

        private Bitmap _buffer;
        private Graphics _graphics;
        public Form1()
        {
           InitializeComponent();

            _COMport = new ComPortHandler();
            _COMport.PortName = "COM240";
            _COMport.BaudRate = 115200;
            _COMport.DataReceived += ComPortHandler_DataR;
            Console.WriteLine("DATAREAD");
            _COMport.ErrorOccurred += ComPortHandler_ErrorOccurred;
            this.comboBox1.SelectedItem = "115200";
            Resize += Form1_Resize;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //SetStyle(ControlStyles.Opaque, true);

            this.BackColor = Color.FromName("GhostWhite");
            //button1.BackColor = Color.FromName("WhiteSmoke");
            //button2.BackColor = Color.FromName("WhiteSmoke");
            //button3.BackColor = Color.FromName("WhiteSmoke");
            button4.BackColor = Color.FromName("WhiteSmoke");

        }

        public double koeff;
        public int FormHeight;
        public int textBoxHeight;

        public int textBox2Height;

        public int button2ToggleY;

        public int label1Y;

        public void Form1_Load(object sender, EventArgs e)
        {
            _COMport.OpenPort(); //Open the COM port when form loads
            Thread.Sleep(250);
            _COMport.DiscardInBuffer();
            _COMport.DiscardOutBuffer();
            //_COMport.SendData("PD");

            koeff = ((double)textBox1.Height / (double)this.Height) + 0.03;
            //koeff = (double)1/(double)3;
            //System.Console.WriteLine($"{this.Height} + VISOTA + {textBox1.Height} + {koeff}");
            //label1.Text = $"{this.Height} + {koeff}";
            FormHeight = this.Height;

            textBoxHeight = textBox1.Height;
            textBox2Height = textBox2.Height;
            
            button2ToggleY = button2.Location.Y;
            label1Y = label1.Location.Y;
            

    }

        //private void Form1_Resize(object sender, EventArgs e)
        //{
        //    textBox1.Text = "";
        //    textBox2.Text = "";
        //    textBox3.Text = "";
        //    textBox4.Text = "";
        //    textBox5.Text = "";
        //}
        private async void Form1_Resize(object sender, System.EventArgs e)
        {
            int newHeight = (this.Height - FormHeight);
            textBox1.Size = new System.Drawing.Size(textBox1.Width, textBoxHeight + newHeight);
            textBox2.Size = new System.Drawing.Size(textBox2.Width, textBox2Height + newHeight);
            
            
            Control control = (Control)sender;
            button2.Location = new System.Drawing.Point(button2.Location.X, button2ToggleY + newHeight);
            this.label1.Location = new System.Drawing.Point(label1.Location.X, label1Y + newHeight);

        }
        //Label label1 = new Label();

        private void ComPortHandler_DataR(object sender, string e)
        {
            textBox1.Invoke((MethodInvoker)delegate {
                if (COMFlag == -1)
                {
                    textBox1.AppendText(e + '\n');
                }
            });
        }

        private void ComPortHandler_ErrorOccurred(object sender, Exception e)
        {
            label1.BeginInvoke((MethodInvoker)delegate {

                label1.Text = $"Error: {e.Message}"; 

            });
        }
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e) //Sending a message
        {
            if (textBox2.Text == "")
            {
                messageToSend = "NaN";
                textBox2.Text = "";
            }
            else { messageToSend = textBox2.Text; }

            _COMport.SendData(messageToSend);
            textBox2.Text = "";
            label1.Text = "Sent: " + messageToSend;
        }

        private async void button2_Click(object sender, EventArgs e)//Toggle COM
        {
            textBox2.Text = $"{_COMport.PortName} toggled";
            COMFlag = -1 * COMFlag;
            while (!(_COMport.B2Read() == 0 && _COMport.B2Write() == 0))
            {
                _COMport.DiscardInBuffer();
                _COMport.DiscardOutBuffer();
            }

            if (COMFlag == 1)
            {
                //_COMport.ClosePort();
                _COMport.ClosePortAsync();
                textBox2.Text = $"Closed Port {_COMport.PortName}";
                await Task.Delay(2000);
                textBox2.Text = "";
            }
            else if (COMFlag == -1)
            {

                _COMport.OpenPort();
                _COMport.DiscardInBuffer();
                _COMport.DiscardOutBuffer();
                textBox2.Text = $"Opened Port {_COMport.PortName}";
                label1.Text = $"{_COMport.PortName} Opened";
                await Task.Delay(2000);
                textBox2.Text = "";

            }

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            _COMport.DiscardInBuffer();
            _COMport.DiscardOutBuffer();
            _COMport.ClosePort();
            Thread.Sleep(250);
            Console.WriteLine("Off");
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e) //Clear received
        {
            this.BeginInvoke((MethodInvoker)delegate { textBox1.ResetText(); });

        }


        async void BaudChange(int NewBaud) {

            Console.WriteLine(NewBaud.ToString() + " Try");
            _COMport.ClosePortAsync();
            await Task.Delay(500); // Wait 500 milliseconds
            _COMport.BaudRate = NewBaud;
            //_COMport
            if (_COMport.OpenPort())
            {
                label1.Text = "Baud rate changed to " + NewBaud.ToString();
            }
            else
            {
                MessageBox.Show("Failed to open COM port with new baud rate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private async Task ChangePORTAsync(string newPort)
        {
            try
            {
                //Send command to Arduino to change baud rate first (if applicable).
                Console.WriteLine($"COM {newPort} try");
                //_COMport.SendData($"COM{newPort}");
                await Task.Delay(700);

                _COMport.ClosePortAsync();
                await Task.Delay(500);

                _COMport.PortName = newPort;

                if (_COMport.OpenPort())
                {
                    //Use Invoke for thread safety
                    this.Invoke((MethodInvoker)delegate { label1.Text = $"Switched to {newPort}."; });
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate {
                        MessageBox.Show($"Failed to open COM port: {_COMport.LastError?.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }
        private async Task ChangeBaudRateAsync(int newBaud)
        {
            try
            {
                //Send command to Arduino to change baud rate first (if applicable).
                Console.WriteLine($"COM {newBaud} try");
                _COMport.SendData($"COM{newBaud}");
                await Task.Delay(700);

                _COMport.ClosePortAsync();
                await Task.Delay(500);

                _COMport.BaudRate = newBaud;

                if (_COMport.OpenPort())
                {
                    //Use Invoke for thread safety
                    this.Invoke((MethodInvoker)delegate { label1.Text = $"Baud rate changed to {newBaud}."; });
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate {
                        MessageBox.Show($"Failed to open COM port with new baud rate: {_COMport.LastError?.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }

        private async void button5_Click_1(object sender, EventArgs e)
        {
            int newBaud = 115200;
            bool available = true;

            if (comboBox1.SelectedItem != null)
            {
                available = int.TryParse(comboBox1.SelectedItem.ToString(), out newBaud);

                if (available)
                {
                    await ChangeBaudRateAsync(newBaud);
                }
                else
                {
                    MessageBox.Show("Invalid baud rate selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else { label1.Text = "Invalid baud choice"; }
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        async void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int NewBaud = 115200;
            string selectedState = comboBox1.SelectedItem.ToString();
            int.TryParse(selectedState, out NewBaud);
            Console.WriteLine(NewBaud);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _COMport.Dispose();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
         
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            string newport = "COM240";
            bool available = true;

            if (textBox3.Text != null)
            {
                
                newport = textBox3.Text.ToString();
                int newportint = 0;
                string newPortString = "COM" + newport;
                available = int.TryParse(newport, out newportint);

                if (available)
                {
                    await ChangePORTAsync(newPortString);
                }
                else
                {
                    MessageBox.Show("Invalid PORT selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else { label1.Text = "Invalid PORT choice"; }

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        //private void Form1_SizeChanged(object sender, EventArgs e)
        //{
        //    int w = (int)this.ClientSize.Width; // ширина окна            
        //    int h = (int)this.ClientSize.Height; // высота окна
        //    _ = new System.Drawing.Size(w / 4, h / 4)
        //}
    }
}
