using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;

namespace TinifyClient.FileUtilities
{
    public class NetTraffic
    {
        private List<NetworkInterface> _ifs;
        private long[] _bytesSend;
        private long[] _bytesRcv;
        //private long[] _speedbytesSend;
        //private long[] _speedbytesRcv;
        private readonly Timer _timer1 = new Timer();

        public NetTraffic()
        {
            _timer1.Enabled = false;
            InitializeNetwork();
        }

        public List<NetworkInterface> GetAllNetworkInterfaces
        {
            get
            {
                //filter card/lan/network here
                return (from n in NetworkInterface.GetAllNetworkInterfaces()
                        where (n.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                            n.NetworkInterfaceType == NetworkInterfaceType.Ppp ||
                              n.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                              n.NetworkInterfaceType.ToString().Contains("Wireless"))
                          && n.OperationalStatus == OperationalStatus.Up
                          && !n.Description.Contains("VMware")
                          && !n.Name.Contains("VMware")
                          && !n.Name.Contains("Emulator")
                        select n).ToList();
            }
        }
        private void InitializeNetwork()
        {
            _ifs = GetAllNetworkInterfaces;
            _bytesSend = new long[_ifs.Count];
            _bytesRcv = new long[_ifs.Count];
            for (int i = 0; i < _ifs.Count; i++)
            {
                _bytesSend[i] = _ifs[i].GetIPv4Statistics().BytesSent;
                _bytesRcv[i] = _ifs[i].GetIPv4Statistics().BytesReceived;
            }
            //_stp.Start();
            if (_timer1.Enabled == false)
            {
                _timer1.Interval = 1000;
                _timer1.Tick += timer1_Tick;
                _timer1.Enabled = true;
            }
        }

        public void RescanNetwork()
        {
            InitializeNetwork();
        }

        public delegate void DataReceveDelegate(object sender, List<NetTrafficEventArg> e);

        public event DataReceveDelegate DataReceiving;

        protected virtual void OnDataReceiving(List<NetTrafficEventArg> e)
        {
            DataReceveDelegate handler = DataReceiving;
            if (handler != null) handler(this, e);
        }
        // private Stopwatch _stp=new Stopwatch();
        private void timer1_Tick(object sender, EventArgs e)
        {

            try
            {
                List<NetTrafficEventArg> netTrafficEventArgList = new List<NetTrafficEventArg>();
                for (int i = 0; i < _ifs.Count; i++)
                {
                    var snd = _ifs[i].GetIPv4Statistics().BytesSent;
                    var rcv = _ifs[i].GetIPv4Statistics().BytesReceived;


                    var speedbytesSend = snd - _bytesSend[i];
                    var speedbytesRcv = rcv - _bytesRcv[i];

                    _bytesSend[i] = snd;
                    _bytesRcv[i] = rcv;


                    NetTrafficEventArg trafficEventArg = new NetTrafficEventArg
                    {
                        Interval = 1000,//_stp.ElapsedMilliseconds,
                        NetworkInterface = _ifs[i],
                        DownloadSpeed = speedbytesRcv,
                        UploadSpeed = speedbytesSend,
                        TotalReceived = _bytesRcv[i],
                        TotalSent = _bytesSend[i]
                    };

                    netTrafficEventArgList.Add(trafficEventArg);
                }
                //_stp.Reset();
                OnDataReceiving(netTrafficEventArgList);
            }
            catch (Exception ex)
            {


            }

        }

        public List<NetworkInterface> GetActiveNetworkInterfaces
        {
            get { return _ifs; }
        }

        internal void Resume()
        {
            _timer1.Enabled = true;
        }

        internal void Pause()
        {
            _timer1.Enabled = false;
        }
    }

    public class NetTrafficEventArg : EventArgs
    {
        public NetworkInterface NetworkInterface { get; set; }
        public long UploadSpeed { get; set; }
        public long DownloadSpeed { get; set; }
        public long TotalSent { get; set; }
        public long TotalReceived { get; set; }
        public long Interval { get; set; }
        //public NetworkInterface NetworkInterface
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string DownSpeed()//string unit
        {
            return FileSizeFormatter.FormatSize(DownloadSpeed ) + "/S";
        }
        public string UpSpeed()//string unit
        {
            return FileSizeFormatter.FormatSize(UploadSpeed) + "/S";
        }
        //public string Speed()
        //{
        //    return "DownSpeed:" + DownSpeed() + " \r\nUpSpeed:" + UpSpeed();
        //}


    }
}
