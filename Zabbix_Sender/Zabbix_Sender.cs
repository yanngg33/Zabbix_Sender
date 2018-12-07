using System;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Zabbix_Sender
{
    public class ZS_Data
    {
        public string Host { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public ZS_Data(string Zbxhost, string Zbxkey, string Zbxval)
        {
            Host = Zbxhost;
            Key = Zbxkey;
            Value = Zbxval;
        }
    }

    public class ZS_Response
    {
        public string Response { get; set; }
        public string Info { get; set; }

    }

    public class ZS_Request
    {
        public string Request { get; set; }
        public ZS_Data[] TblData { get; set; }
        public ZS_Request(string ZbxHost, string ZbxKey, string ZbxVal)
        {
            Request = "sender data";
            TblData = new ZS_Data[] { new ZS_Data(ZbxHost, ZbxKey, ZbxVal) };
        }


        public ZS_Response Send(string ZbxServer, int ZbxPort = 10051, int ZbxTimeOut = 500)
        {
            string jr = JsonConvert.SerializeObject(new ZS_Request(TblData[0].Host, TblData[0].Key, TblData[0].Value));
            using (TcpClient lTCPc = new TcpClient(ZbxServer, ZbxPort))
            using (NetworkStream lStream = lTCPc.GetStream())
            {
                byte[] Header = Encoding.ASCII.GetBytes("ZBXD\x01");
                byte[] DataLen = BitConverter.GetBytes((long)jr.Length);
                byte[] Content = Encoding.ASCII.GetBytes(jr);
                byte[] Message = new byte[Header.Length + DataLen.Length + Content.Length];
                Buffer.BlockCopy(Header, 0, Message, 0, Header.Length);
                Buffer.BlockCopy(DataLen, 0, Message, Header.Length, DataLen.Length);
                Buffer.BlockCopy(Content, 0, Message, Header.Length + DataLen.Length, Content.Length);

                lStream.Write(Message, 0, Message.Length);
                lStream.Flush();
                int counter = 0;
                while (!lStream.DataAvailable)
                {
                    if (counter < ZbxTimeOut / 50)
                    {
                        counter++;
                        Task.Delay(50);
                    }
                    else
                    {
                        throw new TimeoutException();
                    }
                }

                byte[] resbytes = new Byte[1024];
                lStream.Read(resbytes, 0, resbytes.Length);
                string s = Encoding.UTF8.GetString(resbytes);
                string jsonRes = s.Substring(s.IndexOf('{'));
                return JsonConvert.DeserializeObject<ZS_Response>(jsonRes);
            }
        }
    }
}
