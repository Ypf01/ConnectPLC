using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using Pframe.Common;
using System.IO;
using System.Threading;

namespace Pframe.Custom
{
    public class Sk
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public Sk()
        {
            this.InteractiveLock = new SimpleHybirdLock();
            this.SendTimeOut = 2000;
            this.ReceiveTimeOut = 2000;
            this.ConnectTimeOut = 2000;
            this.WaitTimes = 10;
            this.SleepTime = 10;
            isStop = false;
        }
        /// <summary>
        /// 通信对象
        /// </summary>
        private Socket socket;
        /// <summary>
        /// 连接超时的时间，单位毫秒
        /// </summary>
        public int ConnectTimeOut { get; set; }
        /// <summary>
        ///  重连时间，单位毫秒
        /// </summary>
        public int ReceiveTimeOut { get; set; }
        /// <summary>
        /// 发送超时时间
        /// </summary>
        public int SendTimeOut { get; set; }
        /// <summary>
        /// 一次正常的交互的互斥锁
        /// </summary>
        public SimpleHybirdLock InteractiveLock { get; private set; }
        /// <summary>
        /// 接受返回报文等待次数，默认为10次
        /// </summary>
        public int WaitTimes { get; private set; }
        /// <summary>
        /// 接受返回报文等待时间，每次为10ms
        /// </summary>
        public int SleepTime { get; private set; }
        /// <summary>
        /// 建立Socket连接
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口号</param>
        /// <returns></returns>
        public bool Connect(string ip, int port)
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, this.ReceiveTimeOut);
            this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, this.SendTimeOut);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
            IAsyncResult asyncResult = this.socket.BeginConnect(remoteEP, null, null);
            bool result;
            if (!(result = asyncResult.AsyncWaitHandle.WaitOne(this.ConnectTimeOut, true)))
                this.socket.Close();
            isStop = false;
            return result;
        }

        public bool isStop;

        /// <summary>
        /// 断开连接
        /// </summary>
        public void DisConnect()
        {
            Socket socket = this.socket;
            isStop = true;
            if (socket != null)
                socket.Close();
        }
        /// <summary>
        /// 发送报文并接收返回值
        /// </summary>
        /// <param name="SendByte">字节数组</param>
        /// <param name="response">返回报文</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public bool SendAndReceive(byte[] SendByte, ref byte[] response, int timeout = 5000)
        {
            if (isStop) return false;
            bool result;
            this.InteractiveLock.Enter();
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                this.socket.ReceiveTimeout = timeout;
                Send(SendByte);
                int num = 0;
                byte[] buffer = new byte[512];
                while (true)
                {
                    Thread.Sleep(this.SleepTime);
                    if (this.socket.Available > 0)
                    {
                        int count = this.socket.Receive(buffer, SocketFlags.None);
                        memoryStream.Write(buffer, 0, count);
                    }
                    else if (isStop)
                        goto IL_B6;
                    else
                    {
                        num++;
                        if (num >= this.WaitTimes)
                            goto IL_B6;
                        if (memoryStream.Length > 0L)
                            break;
                    }
                }
                goto IL_112;
                IL_B6:
                return false;
                IL_112:
                if (response == null || response.Length != memoryStream.Length)
                    response = new byte[memoryStream.Length];
                response = memoryStream.ToArray();
                memoryStream.Dispose();
                result = true;
            }
            catch (Exception)
            {
                response = null;
                result = false;
            }
            finally
            {
                this.InteractiveLock.Leave();
            }
            return result;
        }
        object _slock = new object();
        /// <summary>
        /// 仅发送字节数组
        /// </summary>
        /// <param name="bt"></param>
        /// <returns></returns>
        public bool Send(byte[] bt)
        {
            lock (_slock)
            {
                try
                {
                    this.socket?.Send(bt, bt.Length, SocketFlags.None);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 读取指令结果
        /// </summary>
        /// <param name="code">读取指令</param>
        /// <returns>读取结果</returns>
        public byte[] Read(string code, Encoding ecoding)
        {
            if (code.Trim().Length == 0)
                return null;
            byte[] result = null;
            return SendAndReceive(ecoding.GetBytes(code), ref result) ? result : null;
        }
        /// <summary>
        /// 写入指令
        /// </summary>
        /// <param name="bts">指令</param>
        /// <returns>写入结果</returns>
        public CalResult Write(byte[] bts)
        {
            if (bts == null || bts.Length == 0)
                return new CalResult() { IsSuccess = false, Message = "写入指令不可为空" };
            if (Send(bts))
                return new CalResult() { IsSuccess = false, Message = "写入成功" };
            else
                return new CalResult() { IsSuccess = true, Message = "指令写入异常,具体原因不明确!" };
        }
        int length = 0;
        /// <summary>
        /// 接收事件
        /// </summary>
        /// <param name="bt"></param>
        /// <returns></returns>
        public byte[] Recive(byte[] bt, int minsize = 22)
        {
            length = 0;
            this.InteractiveLock.Enter();
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                while (minsize > length)
                {
                    if (this.socket.Available > 0)
                    {
                        int count = this.socket.Receive(bt, SocketFlags.None);
                        length += count;
                        memoryStream.Write(bt, 0, count);
                    }
                    else if (isStop)
                        return null;
                    Thread.Sleep(10);
                }
                byte[] rtByte = memoryStream.ToArray();
                return rtByte;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                this.InteractiveLock.Leave();
            }
        }
    }
}
