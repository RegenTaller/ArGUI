using System;
using System.IO.Ports;
using System.Threading;

public class ComPortHandler
{
    public event EventHandler<string> DataReceived; // Event to signal data received
    public event EventHandler<Exception> ErrorOccurred; //New Event for errors

    private SerialPort _serialPort;

    public ComPortHandler(string portName, int baudRate)
    {
        _serialPort = new SerialPort(portName, baudRate);
    }

    public void OpenPort()
    {
        if (!_serialPort.IsOpen)
        {
            try
            {
                //_serialPort.DiscardInBuffer();
                _serialPort.Open();
                _serialPort.DiscardInBuffer();
                _serialPort.DataReceived += SerialPort_DataReceived;
                _serialPort.WriteLine("PD");
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately (e.g., log error, display message)
                Console.WriteLine($"Error opening COM port: {ex.Message}");
            }
        }
    }

    public void ClosePort()
    {
        if (_serialPort.IsOpen)
        {   
            //_serialPort.DiscardInBuffer();
            //_serialPort.DiscardOutBuffer();
            ////Thread.Sleep(550);
            
            _serialPort.Close();
        }
    }

    public void DiscardInBuffer()
    {
        if (_serialPort.IsOpen)
        {
            _serialPort.DiscardInBuffer();
        }
    }

    public void DiscardOutBuffer()
    {
        //if (_serialPort.IsOpen)
        //{
            _serialPort.DiscardOutBuffer();
        //}
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            string data = _serialPort.ReadLine(); // Read data from COM port
            DataReceived?.Invoke(this, data); // Raise the event to notify Form1
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., log error, display message)
            Console.WriteLine($"Error reading COM port: {ex.Message}");
        }
    }

    public void SendData(string data)
    {
        try
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.WriteLine(data);
            }
            else
            {
                //Handle the case when port is not open.
                ErrorOccurred?.Invoke(this, new Exception("COM port is closed."));
            }
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, ex);
        }
    }
}