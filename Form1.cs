using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArduinoInterface


{
    public partial class Form1 : Form
    {
        int Flag = 1;

        string messageToSend = "";

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
        }
        //Label label1 = new Label();
        private void ComPortHandler_DataR(object sender, string e)
        {
            //label1.Invoke((MethodInvoker)delegate
            //{
            //    // Running on the UI thread
            //    label1.Text = e;
            //    //label1.Text = e;
            //});
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
        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _COMport.ClosePort(); //Close COM port when form closes
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {

            messageToSend = textBox2.Text;
            _COMport.SendData(messageToSend);
            textBox2.Text = "";
            //_COMport.WriteLine();

        }
    }
}
