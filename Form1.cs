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

            _COMport = new ComPortHandler("COM240", 115200);
            _COMport.DataReceived += ComPortHandler_DataR;
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
            textBox1.Invoke((MethodInvoker)delegate {
                //textBox1.Text = textBox1.Text + e;
                textBox1.AppendText(e+'\n');
                //textBox1.AppendText();
            });
        }
        private void ComPortHandler_ErrorOccurred(object sender, Exception e)
        {
            label1.Invoke((MethodInvoker)delegate {

                label1.Text = $"Error: {e.Message}"; 

            });

        }
        
        //private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    _COMport.SendData("PD");
        //    Thread.Sleep(250);
        //    _COMport.DiscardInBuffer();
        //    _COMport.DiscardOutBuffer();
        //    _COMport.ClosePort(); //Close COM port when form closes
        //    _COMport.DiscardInBuffer();
        //    _COMport.DiscardOutBuffer();
        //    Thread.Sleep(250);
        //    Console.WriteLine("Closed Form");
        //}

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

            //_COMport.DiscardInBuffer();
            //_COMport.DiscardOutBuffer();
            _COMport.SendData(messageToSend);
            textBox2.Text = "";
            label1.Text = "Sent: " + messageToSend;



            //_COMport.WriteLine();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = "COM Changed";
            COMFlag = -1*COMFlag;
            
            //textBox2.Text = COMFlag.ToString();
            
            if (COMFlag == 1)
            {
                //_COMport.SendData("PD");
                //_COMport.DiscardOutBuffer();
                //_COMport.DiscardInBuffer();
                //Thread.Sleep(250);
                //_COMport.DiscardInBuffer();
                //_COMport.DiscardOutBuffer();
                //textBox2.Text = "COM Closed";
                _COMport.ClosePort();
                //_COMport = new ComPortHandler("COM240", 115200);
                
                textBox2.Text = "Closed Port";

            }
            else if (COMFlag == -1) { 
                
                _COMport.OpenPort();
                //_COMport.SendData("PD");
                textBox2.Text = "Opened Port";
                label1.Text = "COM Opened";
                //_COMport = new ComPortHandler("COM240", 115200);
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
    }
}
