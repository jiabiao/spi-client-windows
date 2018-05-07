using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace SPIClient
{
    /// <summary>
    /// This class is meant to be used as a replacement of the Connection class
    /// when you want to talk to the PinPad server via bitbucket.org/simplepayments/spi-http-adapter/
    /// instead of directly. This is discouraged and only useful if for some reason your technology of 
    /// choice cannot use websockets but can make http requests.
    /// 
    /// To make AcmePos use this, just instantiate it instead of the standard Connection class.
    /// </summary>
    public class ConnectionViaHttpAdapter
    {
        public ConnectionState State { get; private set; }
        public bool Connected { get; private set; }

        private EventHandler<MessageEventArgs> _messageReceived;

        public event EventHandler<MessageEventArgs> MessageReceived
        {
            add { _messageReceived = _messageReceived + value; }
            remove { _messageReceived = _messageReceived - value; }
        }

        private EventHandler<ConnectionStateEventArgs> _connectionStatusChanged;

        public event EventHandler<ConnectionStateEventArgs> ConnectionStatusChanged
        {
            add { _connectionStatusChanged = _connectionStatusChanged + value; }
            remove { _connectionStatusChanged = _connectionStatusChanged - value; }
        }

        public string Address { get; set; }

        private const string AdapterAddr = "http://127.0.0.1:56561/";

        public ConnectionViaHttpAdapter()
        {
            State = ConnectionState.Disconnected;
        }

        public void Connect()
        {
            if (State == ConnectionState.Connected || State == ConnectionState.Connecting)
            {
                // already connected or connecting. disconnect first.
                return;
            }

            // This one is non-blocking
            State = ConnectionState.Connecting;
            _connectionStatusChanged(this, new ConnectionStateEventArgs{ConnectionState = ConnectionState.Connecting}); 

            var webRequest = (HttpWebRequest) WebRequest.Create($"{AdapterAddr}connect/" + Address.Substring(5));
            webRequest.Timeout = 5000;
            try
            {
                var webResp = (HttpWebResponse) webRequest.GetResponse();
                if (webResp.StatusCode == HttpStatusCode.OK)
                {
                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        _onOpened();
                    }).Start();
                }
            }
            catch (WebException we)
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            var webRequest = (HttpWebRequest) WebRequest.Create($"{AdapterAddr}disconnect");
            try
            {
                webRequest.GetResponse();
            }
            catch (WebException we)
            {
                Console.WriteLine("Couldn't even call disconnect..");
            }
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                _onClosed();
            }).Start();
        }

        public void Send(string message)
        {
            var mBytes = Encoding.UTF8.GetBytes(message);

            var request = (HttpWebRequest) WebRequest.Create($"{AdapterAddr}send");
            request.Method = "POST";
            request.ContentType = "text/plain";
            request.ContentLength = mBytes.Length;
            try
            {
                var stream = request.GetRequestStream();
                stream.Write(mBytes, 0, mBytes.Length);
                request.GetResponse();
            }
            catch (WebException we)
            {
                Console.WriteLine("Problem calling send.");
                var resp = we.Response as HttpWebResponse;
                if (resp == null)
                {
                    Console.WriteLine("Could not even get a response. {0}", we.Message);
                }
                else
                {
                    Console.WriteLine(resp.StatusCode.ToString() + ":" + resp.ToString());
                }
            }
        }

        private void _onOpened()
        {
            State = ConnectionState.Connected;
            Connected = true;
            _connectionStatusChanged(this, new ConnectionStateEventArgs{ConnectionState = ConnectionState.Connected});

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                while (Connected)
                {
                    var request = (HttpWebRequest) WebRequest.Create($"{AdapterAddr}receive");
                    HttpWebResponse response;
                    try
                    {
                        response = (HttpWebResponse) request.GetResponse();
                    }
                    catch (WebException we)
                    {
                        Console.WriteLine("Problem calling receive.");
                        var resp = we.Response as HttpWebResponse;
                        if (resp != null) Console.WriteLine(resp.StatusCode.ToString() + ":" + resp.ToString());
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Thread.Sleep(500);
                        continue;
                    }

                    using (var responseStream = response.GetResponseStream())
                    using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                        _onMessageReceived(reader.ReadToEnd());
                }
            }).Start();
        }

        private void _onClosed()
        {
            Connected = false;
            State = ConnectionState.Disconnected;
            _connectionStatusChanged(this, new ConnectionStateEventArgs{ConnectionState = ConnectionState.Disconnected});
        }

        private void _onMessageReceived(string msg)
        {
            _messageReceived(this, new MessageEventArgs{Message = msg});
        }
    }
}