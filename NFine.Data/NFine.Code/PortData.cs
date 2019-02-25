using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Code
{
    public class PortData
    {
        public event PortDataReceivedEventHandle Received;
        public event SerialErrorReceivedEventHandler Error;
        public SerialPort Port;
        public bool ReceiveEventFlag = false;  //接收事件是否有效 false表示有效

        public PortData(string sPortName, int baudrate, Parity parity)
        {
            Port = new SerialPort(sPortName, baudrate, parity, 8, StopBits.One)
            {
                RtsEnable = true,
                ReadTimeout = 3000
            };
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            Port.ErrorReceived += new SerialErrorReceivedEventHandler(ErrorEvent);
        }
        ~PortData()
        {
            Close();
        }
        public void Open()
        {
            if (!Port.IsOpen)
            {
                Port.Open();
            }
        }

        public static string[] GetPorts()
        {
            return SerialPort.GetPortNames();
        }

        public void Close()
        {
            if (Port.IsOpen)
            {
                Port.Close();
            }
        }
        //数据发送
        public void SendData(byte[] data)
        {
            if (Port.IsOpen)
            {
                Port.Write(data, 0, data.Length);
            }
        }
        public void SendData(byte[] data, int offset, int count)
        {
            if (Port.IsOpen)
            {
                Port.Write(data, offset, count);
            }
        }
        //发送命令
        public byte[] SendCommand(byte[] sendData, ref int receiveData, int overtime)
        {
            if (!Port.IsOpen) return null;
            ReceiveEventFlag = true;        //关闭接收事件
            Port.DiscardInBuffer();         //清空接收缓冲区                 
            Port.Write(sendData, 0, sendData.Length);
            var num = 0;
            while (num++ < overtime)
            {
                if (Port.BytesToRead >= receiveData) break;
                System.Threading.Thread.Sleep(1);
            }
            var data = new byte[Port.BytesToRead];
            receiveData = Port.Read(data, 0, data.Length);
            ReceiveEventFlag = false;       //打开事件
            return data;
        }
        public void ErrorEvent(object sender, SerialErrorReceivedEventArgs e)
        {
            if (Error != null) Error(sender, e);
        }
        //数据接收
        public void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //禁止接收事件时直接退出
            if (ReceiveEventFlag) return;
            byte[] data = new byte[Port.BytesToRead];
            Port.Read(data, 0, data.Length);
            if (Received != null) Received(sender, new PortDataReciveEventArgs(data));
        }
        public bool IsOpen()
        {
            return Port.IsOpen;
        }
    }
    public delegate void PortDataReceivedEventHandle(object sender, PortDataReciveEventArgs e);
    public class PortDataReciveEventArgs : EventArgs
    {
        public PortDataReciveEventArgs()
        {
            this.Data = null;
        }

        public PortDataReciveEventArgs(byte[] data)
        {
            this.Data = data;
        }

        public byte[] Data { get; set; }
    }
}
