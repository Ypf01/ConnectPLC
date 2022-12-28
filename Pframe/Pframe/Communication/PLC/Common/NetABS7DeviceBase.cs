using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Pframe.Base;
using Pframe.DataConvert;
using System.Net;

namespace Pframe.PLC.Common
{
    public class NetABS7DeviceBase
    {
        public Guid Token { get; set; }
        /// <summary>
        /// 接收报文
        /// </summary>
        /// <param name="socket">Socket对象</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="netMessage"> message对象</param>
        /// <param name="reportProgress">进度</param>
        /// <returns>带结果的操作对象</returns>
        public CalResult<byte[]> ReceiveByMessage(Socket socket, int timeOut, IMessage netMessage, Action<long, long> reportProgress = null)
        {
            CalResult<byte[]> result;
            if (netMessage == null)
                result = this.Receive(socket, -1, timeOut, null);
            else
            {
                CalResult<byte[]> xktResult = this.Receive(socket, netMessage.HeadDataLength, timeOut, null);
                if (!xktResult.IsSuccess)
                    result = xktResult;
                else
                {
                    netMessage.HeadData = xktResult.Content;
                    int contentLength = netMessage.GetContentLength();
                    if (contentLength <= 0)
                        result = xktResult;
                    else
                    {
                        CalResult<byte[]> xktResult2 = this.Receive(socket, contentLength, timeOut, reportProgress);
                        if (!xktResult2.IsSuccess)
                            result = xktResult2;
                        else
                        {
                            netMessage.ContentData = xktResult2.Content;
                            result = CalResult.CreateSuccessResult<byte[]>(ByteArrayLib.CombineTwoByteArray(xktResult.Content, xktResult2.Content));
                        }
                    }
                }
            }
            return result;
        }

        private CalResult<byte[]> Receive(Socket socket, int length, int timeout = 60000, Action<long, long> reportProgress = null)
        {
            CalResult<byte[]> result;
            if (length == 0)
                result = CalResult.CreateSuccessResult<byte[]>(new byte[0]);
            else
            {
                try
                {
                    socket.ReceiveTimeout = timeout;
                    if (length > 0)
                    {
                        byte[] value = this.ReadBytesFromSocket(socket, length, reportProgress);
                        result = CalResult.CreateSuccessResult<byte[]>(value);
                    }
                    else
                    {
                        byte[] array = new byte[1024];
                        int length2 = socket.Receive(array);
                        result = CalResult.CreateSuccessResult<byte[]>(ByteArrayLib.GetByteArray(array, 0, length2));
                    }
                }
                catch (Exception ex)
                {
                    result = new CalResult<byte[]>(0, ex.Message);
                }
            }
            return result;
        }

        private byte[] ReadBytesFromSocket(Socket socket, int receive, Action<long, long> reportProgress = null)
        {
            byte[] array = new byte[receive];
            int i = 0;
            while (i < receive)
            {
                int size = Math.Min(receive - i, 8192);
                int num = socket.Receive(array, i, size, SocketFlags.None);
                i += num;
                if (num == 0)
                    throw new Exception();
                if (reportProgress != null)
                    reportProgress((long)i, (long)receive);
            }
            return array;
        }
        /// <summary>
        /// 通信对象
        /// </summary>
        public Socket tcpclient;
    }
}
