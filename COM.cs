using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

public class ComPortHandler
{
    public event EventHandler<string> DataReceived; // Event to signal data received
    public event EventHandler<Exception> ErrorOccurred; //New Event for errors
    public Exception LastError { get; private set; } // Store the last error

    private SerialPort _serialPort;
    private string _portName;
    public int BaudRate { get; set; }
    public string PortName { get; set; }

    //Variable to store last complete message

    public ComPortHandler() //Constructor
    {
        //_portName = portName;
    }

    public bool OpenPort()
    {
        if (_serialPort != null && _serialPort.IsOpen) return true; //Already open

        if (_serialPort != null) _serialPort.Dispose(); //Dispose of existing serialport

        _serialPort = new SerialPort(PortName, BaudRate); // Create SerialPort with current BaudRate and PortName

        try
        {
            _serialPort.Open();
            _serialPort.DataReceived += SerialPort_DataReceived;
            return true; //Success
        }
        catch (Exception ex)
        {
            LastError = ex; // Store the last error
            ErrorOccurred?.Invoke(this, ex);
            return false;
        }
    }

    public void ClosePort()
    {
        if (_serialPort != null && _serialPort.IsOpen)
        {
            try
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;
                _serialPort.Close();
                if (_serialPort != null) _serialPort.Dispose();
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, ex);
            }
        }
    }
    public async void ClosePortAsync()
    {
        if (_serialPort != null && _serialPort.IsOpen)
        {
            try
            {
                await Task.Run(() => ClosePort());
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, ex);
            }
        }
    }

    public int B2Read() {

        int e = 0;
        if (_serialPort.IsOpen)
        {
            e = _serialPort.BytesToRead;
        }
        return e;
    }
    public int B2Write()
    {
        int e = 0;
        if (_serialPort.IsOpen)
        {
            e = _serialPort.BytesToWrite;
        }
        return e;
    }

    public void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
    {
        

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
        if (_serialPort.IsOpen)
        {
            _serialPort.DiscardOutBuffer();
        }
    }

    private void handleReceivedBytes(object state)
    {
        //Do the more interesting handling of the receivedBytes list here.
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            string data = _serialPort.ReadLine(); // Read data from COM port
            object[] myArray = new object[2];
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

    public void Dispose() {

        if (_serialPort.IsOpen)
        {
            _serialPort.Dispose();
        }

    }

}