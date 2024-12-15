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

namespace ArduinoInterface


{
    public partial class Form1 : Form
    {
        int Flag = 1;

        string messageToSend = "";

        int COMFlag = -1;

        private ComPortHandler _COMport;
        public Form1()
        {
           InitializeComponent();

            _COMport = new ComPortHandler("COM240");
            _COMport.BaudRate = 115200;
            _COMport.DataReceived += ComPortHandler_DataR;
            Console.WriteLine("DATAREAD");
            _COMport.ErrorOccurred += ComPortHandler_ErrorOccurred;
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            _COMport.OpenPort(); //Open the COM port when form loads
            Thread.Sleep(250);
            _COMport.DiscardInBuffer();
            _COMport.DiscardOutBuffer();
            _COMport.SendData("PD");

        }
        //Label label1 = new Label();
        
        private void ComPortHandler_DataR(object sender, string e)
        {
            if (COMFlag == -1)
            {
                textBox1.BeginInvoke((MethodInvoker)delegate
                {

                    textBox1.AppendText(e + '\n');

                });
            }
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
        private void button1_Click(object sender, EventArgs e)
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

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = "COM Changed";
            COMFlag = -1*COMFlag;
            while (!(_COMport.B2Read() == 0 && _COMport.B2Write() == 0))
            {
                _COMport.DiscardInBuffer();
                _COMport.DiscardOutBuffer();
            }

            if (COMFlag == 1)
            {          
                //_COMport.ClosePort();
                _COMport.ClosePortAsync();
                textBox2.Text = "Closed Port";
            }
            else if (COMFlag == -1) { 
                
                _COMport.OpenPort();
                _COMport.DiscardInBuffer();
                _COMport.DiscardOutBuffer();
                textBox2.Text = "Opened Port";
                label1.Text = "COM Opened";
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

        private async void button4_Click(object sender, EventArgs e)
        {   
            if (_COMport.B2Read() != 0) {
                
                Console.WriteLine("COM Stop Sent");
            }

            _COMport.SendData("COM");
            await Task.Delay(700);
            Console.WriteLine("COM Stop Sent");

            _COMport.ClosePortAsync();
            await Task.Delay(500); // Wait 200 milliseconds (adjust as needed
            _COMport.BaudRate = 9600;
            _COMport.BaudRate = 9600;
            if (_COMport.OpenPort())
            {
                label1.Text = "Baud rate changed to 9600.";
            }
            else
            {
                MessageBox.Show("Failed to open COM port with new baud rate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private async void button5_Click_1(object sender, EventArgs e)
        {
            Console.WriteLine("115200 Try");
            _COMport.ClosePortAsync();
            await Task.Delay(500); // Wait 200 milliseconds (adjust as needed
            _COMport.BaudRate = 115200;
            //_COMport
            if (_COMport.OpenPort())
            {
                label1.Text = "Baud rate changed to 115200.";
            }
            else
            {
                MessageBox.Show("Failed to open COM port with new baud rate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
