using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

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
                while (!(_serialPort.BytesToRead == 0 && _serialPort.BytesToWrite == 0))
                {
                    _serialPort.DiscardInBuffer();
                    Console.WriteLine("CLEANED");
                    _serialPort.DiscardOutBuffer();
                }
                _serialPort.DiscardInBuffer();
                _serialPort.DataReceived += SerialPort_DataReceived;
                //_serialPort.WriteLine("PD");
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately (e.g., log error, display message)
                Console.WriteLine($"Error opening COM port: {ex.Message}");
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
    public void ClosePort()
    {
        if (_serialPort.IsOpen)
        {
            while (!(_serialPort.BytesToRead == 0 && _serialPort.BytesToWrite == 0))
            {
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();
            }
            if (_serialPort.IsOpen)
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;
                _serialPort.ErrorReceived -= SerialPort_ErrorReceived;
                _serialPort.BaseStream.Close();
                _serialPort.Close();
            }
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
        if (_serialPort.IsOpen)
        {
            _serialPort.DiscardOutBuffer();
        }
    }

    private void handleReceivedBytes(object state)
    {
        //Do the more interesting handling of the receivedBytes list here.
    }

    public async void ClosePortAsync()
    {
        if (_serialPort.IsOpen)
        {
            try
            {
                await Task.Run(() => _serialPort.Close()); //Асинхронное закрытие
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, ex);
            }
        }
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
}