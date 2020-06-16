using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;
using WindAPIServer;
// namespace MO
// {
//     public class Program
//     {
//         static void Main(string[] args)
//         {
//             DataService.SocketServer s = new DataService.SocketServer();
//             s.StartService();
//         }
//     }
// }
namespace SocketService
{
    public class SocketServer
    {
        ConcurrentDictionary<string, Socket> dSocket = new ConcurrentDictionary<string, Socket>();
        // List<object> ClientProSocketList = new List<object>();
        ConcurrentBag<Socket> ClientProSocketList = new ConcurrentBag<Socket>();
        public delegate string Callback(string args);
        public Callback DataDrive;
        public void StartService(Callback DataDrive)
        {
            //创建socket对象
            //第一个参数：设置网络寻址的协议、第二参数设置数据传输的方式、第三个参数设置通信协议
            Socket oSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //2.绑定IP端口
            string str = "0.0.0.0";
            int ports = 8888;
            IPAddress ip = IPAddress.Parse(str);
            IPEndPoint ipENdpoint = new IPEndPoint(ip, ports);

            oSocket.Bind(ipENdpoint);
            //3.开启监听
            oSocket.Listen(10);
            Console.WriteLine("开始监听···");

            //4.开始接受客户端的链接
            ThreadPool.QueueUserWorkItem(new WaitCallback((o)=>this.StartAcceptClient(o, DataDrive)), oSocket);
            Console.WriteLine("点击输入任意数据回车退出程序。。。");
            Console.ReadKey();
            Console.WriteLine("退出监听，并关闭程序。");
            // this.StartAcceptClient(oSocket);
        }
        public void StartAcceptClient(object state, Callback DataDrive)
        {
            var iSocket = (Socket)state;
            Console.WriteLine("服务器开始接受客户端的链接");
            while (true)
            {
                try
                {
                    Socket prosock = iSocket.Accept();
                    //将远程链接的客户端的IP地址和socket存入集合中
                    dSocket.TryAdd(prosock.RemoteEndPoint.ToString(), prosock);
                    string ipPort = prosock.RemoteEndPoint.ToString();
                    //链接对象的信息
                    string stinfo = prosock.RemoteEndPoint.ToString();
                    Console.WriteLine(string.Format("客户端{0}链接上了", stinfo));

                    ClientProSocketList.Add(prosock);
                    //服务器接收客户端的消息
                    ThreadPool.QueueUserWorkItem(new WaitCallback((o)=>this.ReceiveData(o, DataDrive)), prosock);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    return;
                    //throw;
                }
            }
        }
        public void ReceiveData(object obj, Callback DataDrive)
        {
            var prosock = (Socket)obj;
            byte[] data = new byte[1024 * 1024];
            //方法返回代表实际接受的数据的长度
            while (true)
            {
                int realen = 0;
                try
                {
                    realen = prosock.Receive(data, 0, data.Length, SocketFlags.None);
                }
                catch (Exception ex)
                {
                    //异常退出
                    Console.WriteLine(string.Format("设备 {0} 异常退出", prosock.RemoteEndPoint.ToString()));
                    StopConnecte(prosock);
                    Console.WriteLine(ex.StackTrace);
                    return;
                }
                if (realen <= 0)
                {
                    //对方正常退出
                    Console.WriteLine(string.Format("设备 {0} 正常退出：", prosock.RemoteEndPoint.ToString()));
                    prosock.Shutdown(SocketShutdown.Both);
                    prosock.Close();
                    // ClientProSocketList.Remove(prosock);
                    ClientProSocketList.TryTake(out prosock);
                    return;
                }
                //接受到的数据
                string fromClientMsg = Encoding.Default.GetString(data, 0, realen);
                Console.WriteLine(string.Format("接收到 {0} 的消息是：{1}", prosock.RemoteEndPoint.ToString(), fromClientMsg));
                // string[] args = fromClientMsg.Split('|');
                string args = fromClientMsg;
                string oData = "";
                oData = DataDrive(args);
                prosock.Send(Encoding.Default.GetBytes(string.Format("{0}", oData)));
                // prosock.Send(Encoding.Default.GetBytes("\r\n"));
                // prosock.Send(Encoding.Default.GetBytes(string.Format("接收到 {0} 的消息是：{1}\r\n", prosock.RemoteEndPoint.ToString(), fromClientMsg)));
            }
        }
        private void StopConnecte(Socket prosock)
        {
            try
            {
                if (prosock.Connected)
                {
                    prosock.Shutdown(SocketShutdown.Both);
                    prosock.Close(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}