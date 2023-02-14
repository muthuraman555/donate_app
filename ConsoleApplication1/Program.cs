using System;
using System.IO.Ports;
using MPOST;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using AutoUpdaterDotNET;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp
{
    public class ByteDetails
    {
        public byte[] bytes { get; set; }
        public int length { get; set; }

    }


   static class Program
    {
        static private CalibrateFinishEventHandler CalibrateFinishDelegate;
        static private CalibrateProgressEventHandler CalibrateProgressDelegate;
        static private CalibrateStartEventHandler CalibrateStartDelegate;
        static private CashBoxCleanlinessEventHandler CashBoxCleanlinessDelegate;
        static private CashBoxAttachedEventHandler CashBoxAttachedDelegate;
        static private CashBoxRemovedEventHandler CashBoxRemovedDelegate;
        static private CheatedEventHandler CheatedDelegate;
        static private ClearAuditEventHandler ClearAuditDelegate;
        static private ConnectedEventHandler ConnectedDelegate;
        static private DisconnectedEventHandler DisconnectedDelegate;
        static private DownloadFinishEventHandler DownloadFinishDelegate;
        static private DownloadProgressEventHandler DownloadProgressDelegate;
        static private DownloadRestartEventHandler DownloadRestartDelegate;
        static private DownloadStartEventHandler DownloadStartDelegate;
        static private ErrorOnSendMessageEventHandler ErrorOnSendMessageDelegate;
        static private EscrowEventHandler EscrowedDelegate;
        static private FailureClearedEventHandler FailureClearedDelegate;
        static private FailureDetectedEventHandler FailureDetectedDelegate;
        static private InvalidCommandEventHandler InvalidCommandDelegate;
        static private JamClearedEventHandler JamClearedDelegate;
        static private JamDetectedEventHandler JamDetectedDelegate;
        static private NoteRetrievedEventHandler NoteRetrievedDelegate;
        static private PauseClearedEventHandler PauseClearedDelegate;
        static private PauseDetectedEventHandler PauseDetectedDelegate;
        static private PowerUpCompleteEventHandler PowerUpCompleteDelegate;
        static private PowerUpEventHandler PowerUpDelegate;
        static private PUPEscrowEventHandler PUPEscrowDelegate;
        static private RejectedEventHandler RejectedDelegate;
        static private ReturnedEventHandler ReturnedDelegate;
        //static private StackedEventHandler StackedDelegate;
        // A new stacked event with document information has been added. Recommanded to be used.
        static private StackedWithDocInfoEventHandler StackedWithDocInfoDelegate;
        static private StackerFullClearedEventHandler StackerFullClearedDelegate;
        static private StackerFullEventHandler StackerFullDelegate;
        static private StallClearedEventHandler StallClearedDelegate;
        static private StallDetectedEventHandler StallDetectedDelegate;
        //static private HandleConnectEventHandler HandleConnectEventDelegate;
        //static private HandleClientConnecdEventHandler HandleClientConnecdEvent;
        static MPOST.Acceptor BillAcceptor = new MPOST.Acceptor();
        static public decimal depositAmount;
        static public decimal maxAmount;
        static decimal balance;
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);
        //private static readonly string receivedMessage;

        public static byte[] buffer { get; private set; }
        public static object AutoUpdaterOnCheckForUpdateEvent { get; private set; }
        public static int AutoUpdaterOnDownloadUpdateProgressChangedEvent { get; private set; }

        static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        static private string guid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        static public Socket userClient = null;


        public static void Main()
        {
            AutoUpdater.Start("https://github.com/muthuraman555/donate_app/blob/main/update.xml");
            AutoUpdater.RunUpdateAsAdmin = false;
            AutoUpdater.Mandatory = true;
            AutoUpdater.UpdateMode = Mode.Forced;
            AutoUpdater.ShowRemindLaterButton = false;
            AutoUpdater.Synchronous = true;
            AutoUpdater.ShowSkipButton = false;
            AutoUpdater.ClearAppDirectory = true;
            AutoUpdater.LetUserSelectRemindLater = false;
            AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Days;
            AutoUpdater.RemindLaterAt = 2;

            Console.WriteLine(GetLocalIPAddress());
            connectMachine();
            Console.CancelKeyPress += (sender, eArgs) => {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };


            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 8080));
            serverSocket.Listen(1); //just one socket
            serverSocket.BeginAccept(null, 0, OnAccept, null);
            Console.Read();

            // kick off asynchronous stuff 

            _quitEvent.WaitOne();
        }


        static private void HandleConnectedEvent(object sender, EventArgs e)
        {
            //BillAcceptor.EscrowReturn();
            Console.WriteLine("Event: Connected");
            Console.WriteLine("Enter total max allowed amount:");
            enableCashAccepting();
            //BillAcceptor.AutoStack = false;

        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }


        public static void connectMachine()

        {

            EscrowedDelegate = new EscrowEventHandler(HandleEscrowedEvent);
            ReturnedDelegate = new ReturnedEventHandler(HandleReturnedEvent);
            ConnectedDelegate = new ConnectedEventHandler(HandleConnectedEvent);
            //StackedDelegate = new StackedEventHandler(HandleEscrowedEvent);

            // Connect to the events.
            BillAcceptor.OnCalibrateFinish += CalibrateFinishDelegate;
            BillAcceptor.OnCalibrateProgress += CalibrateProgressDelegate;
            BillAcceptor.OnCalibrateStart += CalibrateStartDelegate;
            BillAcceptor.OnCashBoxCleanlinessDetected += CashBoxCleanlinessDelegate;
            BillAcceptor.OnCashBoxAttached += CashBoxAttachedDelegate;
            BillAcceptor.OnCashBoxRemoved += CashBoxRemovedDelegate;
            BillAcceptor.OnCheated += CheatedDelegate;
            BillAcceptor.OnClearAuditComplete += ClearAuditDelegate;
            BillAcceptor.OnConnected += ConnectedDelegate;
            BillAcceptor.OnDisconnected += DisconnectedDelegate;
            BillAcceptor.OnDownloadFinish += DownloadFinishDelegate;
            BillAcceptor.OnDownloadProgress += DownloadProgressDelegate;
            BillAcceptor.OnDownloadRestart += DownloadRestartDelegate;
            BillAcceptor.OnDownloadStart += DownloadStartDelegate;
            BillAcceptor.OnSendMessageFailure += ErrorOnSendMessageDelegate;
            BillAcceptor.OnEscrow += EscrowedDelegate;
            BillAcceptor.OnFailureCleared += FailureClearedDelegate;
            BillAcceptor.OnFailureDetected += FailureDetectedDelegate;
            BillAcceptor.OnInvalidCommand += InvalidCommandDelegate;
            BillAcceptor.OnJamCleared += JamClearedDelegate;
            BillAcceptor.OnJamDetected += JamDetectedDelegate;
            BillAcceptor.OnNoteRetrieved += NoteRetrievedDelegate;
            BillAcceptor.OnPauseCleared += PauseClearedDelegate;
            BillAcceptor.OnPauseDetected += PauseDetectedDelegate;
            BillAcceptor.OnPowerUpComplete += PowerUpCompleteDelegate;
            BillAcceptor.OnPowerUp += PowerUpDelegate;
            BillAcceptor.OnPUPEscrow += PUPEscrowDelegate;
            BillAcceptor.OnRejected += RejectedDelegate;
            BillAcceptor.OnReturned += ReturnedDelegate;
            //BillAcceptor.OnStacked += StackedDelegate;
            //A new STACKED event with document information has been added. Recommended to be used.
            BillAcceptor.OnStackedWithDocInfo += StackedWithDocInfoDelegate;
            BillAcceptor.OnStackerFullCleared += StackerFullClearedDelegate;
            BillAcceptor.OnStackerFull += StackerFullDelegate;
            BillAcceptor.OnStallCleared += StallClearedDelegate;
            BillAcceptor.OnStallDetected += StallDetectedDelegate;

            string[] ports = SerialPort.GetPortNames();

            Console.WriteLine("The following serial ports were found:");
            // Display each port name to the console.
            foreach (string port in ports)
            {
                Console.WriteLine(port);
                try
                {
                   
                    
                     BillAcceptor.Open(port, PowerUp.A);
                       
                 
                    //if (port == "COM3")
                    //{
                    //    BillAcceptor.Open("COM3", PowerUp.A);
                    //}
                    //if (port == "COM4")
                    //{
                    //    BillAcceptor.Open("COM4", PowerUp.A);
                    //}
                    //if (port == "COM5")
                    //{
                    //    BillAcceptor.Open("COM5", PowerUp.A);k
                    //}
                    //if (port == "COM6")
                    //{
                    //    BillAcceptor.Open("COM6", PowerUp.A);
                    //}
                }
                catch (Exception err)
                {
                    Console.WriteLine("Unable to open the bill acceptor on com port < COM4 > " + err.Message,
                                    "Open Bill Acceptor Error");
                }

            }

            //Console.ReadLine();


            //try
            //{
            //    BillAcceptor.Open("COM4", PowerUp.A);

            //}
            //catch (Exception err)
            //{
            //    Console.WriteLine("Unable to open the bill acceptor on com port < COM4 > " + err.Message,
            //                    "Open Bill Acceptor Error");
            //}
        }

        private static void OnAccept(IAsyncResult result)
        {
            //Console.WriteLine("Connection Accepted");

            byte[] buffer = new byte[1024 * 10];

            try
            {

                string headerResponse = "";
                if (serverSocket != null && serverSocket.IsBound)
                {
                    userClient = serverSocket.EndAccept(result);
                    var i = userClient.Receive(buffer);

                    headerResponse = (System.Text.Encoding.UTF8.GetString(buffer)).Substring(0, i);
                    // write received data to the console
                    //Console.WriteLine(headerResponse);
                    //Console.WriteLine("=====================");
                }
                if (userClient != null)
                {
                    /* Handshaking and managing ClientSocket */
                    var key = headerResponse.Replace("ey:", "`")
                              .Split('`')[1]                     // dGhlIHNhbXBsZSBub25jZQ== \r\n .......
                              .Replace("\r", "").Split('\n')[0]  // dGhlIHNhbXBsZSBub25jZQ==
                              .Trim();

                    // key should now equal dGhlIHNhbXBsZSBub25jZQ==
                    var test1 = AcceptKey(ref key);


                    var newLine = "\r\n";

                    var response = "HTTP/1.1 101 Switching Protocols" + newLine
                         + "Upgrade: websocket" + newLine
                         + "Connection: Upgrade" + newLine
                         + "Sec-WebSocket-Accept: " + test1 + newLine + newLine
                         //+ "Sec-WebSocket-Protocol: chat, superchat" + newLine
                         //+ "Sec-WebSocket-Version: 13" + newLine
                         ;

                    userClient.Send(System.Text.Encoding.UTF8.GetBytes(response));
                    Thread.Sleep(2000);
                    var byteArray = ReceiveAll(userClient);
                    //var i = userClient.Receive(buffer); // wait for client to send a message
                    //Console.WriteLine(Convert.ToBase64String(buffer).Substring(0, i));
                    //string strData = Encoding.Default.GetString(buffer);
                    //Console.WriteLine("=====================");
                    string browserSent = GetDecodedData(byteArray.bytes, byteArray.length);
                    maxAmount = decimal.Parse(browserSent);
                    Console.WriteLine(maxAmount);

                }
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.ConnectionAborted)
                {
                    Console.WriteLine("Connection aborted by host machine.");
                }
                else
                {
                    Console.WriteLine($"Socket error: {ex.SocketErrorCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
            //catch (SocketException exception)
            //{
            //    throw exception;
            //}

            finally
            {
                if (serverSocket != null && serverSocket.IsBound)
                {
                    serverSocket.BeginAccept(null, 0, OnAccept, null);
                }
            }
        }
        
        public static ByteDetails ReceiveAll(this Socket socket)
        {
            int size = 0;
            var buffer = new List<byte>();

            while (socket.Available > 0)
            {
                var currByte = new Byte[1];
                var byteCounter = socket.Receive(currByte, currByte.Length, SocketFlags.None);
                size += byteCounter;

                if (byteCounter.Equals(1))
                {
                    buffer.Add(currByte[0]);
                }
            }
            ByteDetails byteDetails = new ByteDetails();
            byteDetails.bytes = buffer.ToArray();
            byteDetails.length = size;
            return byteDetails;
        }

        public static T[] SubArray<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        private static string AcceptKey(ref string key)
        {
            string longKey = key + guid;
            byte[] hashBytes = ComputeHash(longKey);
            return Convert.ToBase64String(hashBytes);
        }

        static SHA1 sha1 = SHA1CryptoServiceProvider.Create();
        private static string browserSent;
        private static object client;
        private static NetworkCredential myAssembly;

        private static byte[] ComputeHash(string str)
        {
            return sha1.ComputeHash(System.Text.Encoding.ASCII.GetBytes(str));
        }

        //Needed to decode frame
        public static string GetDecodedData(byte[] buffer, int length)
        {
            byte b = buffer[1];
            int dataLength = 0;
            int totalLength = 0;
            int keyIndex = 0;

            if (b - 128 <= 125)
            {
                dataLength = b - 128;
                keyIndex = 2;
                totalLength = dataLength + 6;
            }

            if (b - 128 == 126)
            {
                dataLength = BitConverter.ToInt16(new byte[] { buffer[3], buffer[2] }, 0);
                keyIndex = 4;
                totalLength = dataLength + 8;
            }

            if (b - 128 == 127)
            {
                dataLength = (int)BitConverter.ToInt64(new byte[] { buffer[9], buffer[8], buffer[7], buffer[6], buffer[5], buffer[4], buffer[3], buffer[2] }, 0);
                keyIndex = 10;
                totalLength = dataLength + 14;
            }


            if (totalLength > length)
                throw new Exception("The buffer length is small than the data length");

            byte[] key = new byte[] { buffer[keyIndex], buffer[keyIndex + 1], buffer[keyIndex + 2], buffer[keyIndex + 3] };

            int dataIndex = keyIndex + 4;
            int count = 0;
            for (int i = dataIndex; i < totalLength; i++)
            {
                buffer[i] = (byte)(buffer[i] ^ key[count % 4]);
                count++;
            }

            return Encoding.ASCII.GetString(buffer, dataIndex, dataLength);
        }

        //function to create  frames to send to client 
        /// <summary>
        /// Enum for opcode types
        /// </summary>
        public enum EOpcodeType
        {
            /* Denotes a continuation code */
            Fragment = 0,

            /* Denotes a text code */
            Text = 1,

            /* Denotes a binary code */
            Binary = 2,

            /* Denotes a closed connection */
            ClosedConnection = 8,

            /* Denotes a ping*/
            Ping = 9,

            /* Denotes a pong */
            Pong = 10
        }

        /// <summary>Gets an encoded websocket frame to send to a client from a string</summary>
        /// <param name="Message">The message to encode into the frame</param>
        /// <param name="Opcode">The opcode of the frame</param>
        /// <returns>Byte array in form of a websocket frame</returns>
        public static byte[] GetFrameFromString(string Message, EOpcodeType Opcode = EOpcodeType.Text)
        {
            byte[] response;
            byte[] bytesRaw = Encoding.Default.GetBytes(Message);
            byte[] frame = new byte[10];

            long indexStartRawData = -1;
            long length = (long)bytesRaw.Length;

            frame[0] = (byte)(128 + (int)Opcode);
            if (length <= 125)
            {
                frame[1] = (byte)length;
                indexStartRawData = 2;
            }
            else if (length >= 126 && length <= 65535)
            {
                frame[1] = (byte)126;
                frame[2] = (byte)((length >> 8) & 255);
                frame[3] = (byte)(length & 2048);
                indexStartRawData = 4;
            }
            else
            {
                frame[1] = (byte)127;
                frame[2] = (byte)((length >> 56) & 255);
                frame[3] = (byte)((length >> 48) & 255);
                frame[4] = (byte)((length >> 40) & 255);
                frame[5] = (byte)((length >> 32) & 255);
                frame[6] = (byte)((length >> 24) & 255);
                frame[7] = (byte)((length >> 16) & 255);
                frame[8] = (byte)((length >> 8) & 255);
                frame[9] = (byte)(length & 255);

                indexStartRawData = 10;
            }

            response = new byte[indexStartRawData + length];

            long i, reponseIdx = 0;

            //Add the frame bytes to the reponse
            for (i = 0; i < indexStartRawData; i++)
            {
                response[reponseIdx] = frame[i];
                reponseIdx++;
            }

            //Add the data bytes to the response
            for (i = 0; i < length; i++)
            {
                response[reponseIdx] = bytesRaw[i];
                reponseIdx++;
            }

            return response;
        }

        public static void enableCashAccepting()
        {
            BillAcceptor.EnableAcceptance = true;
        }
        public static void disableCashAccepting()
        {
            BillAcceptor.EnableAcceptance = false;
        }

        static private String DocInfoToString(DocumentType docType, IDocument doc)
        {
            if (docType == DocumentType.None)
                return "Doc Type: None";
            else if (docType == DocumentType.NoValue)
                return "Doc Type: No Value";
            else if (docType == DocumentType.Bill)
            {
                if (doc == null)
                    return "Doc Type Bill = null";
                else if (!BillAcceptor.CapOrientationExt)
                    return "Doc Type Bill = " + doc.ToString();
                else
                    return "Doc Type Bill = " + doc.ToString() +
                           " (" + BillAcceptor.EscrowOrientation.ToString() + ")";
            }
            else if (docType == DocumentType.Barcode)
            {
                if (doc == null)
                    return "Doc Type Bar Code = null";
                else
                    return "Doc Type Bar Code = " + doc.ToString();
            }
            else if (docType == DocumentType.Coupon)
            {
                if (doc == null)
                    return "Doc Type Coupon = null";
                else
                    return "Doc Type Coupon = " + doc.ToString();
            }
            else
                return "Unknown Doc Type Error";
        }

        static private void HandleEscrowedEvent(object sender, EventArgs e)

        {
            Console.WriteLine(BillAcceptor.DeviceState);
            //Console.WriteLine("Transaction successful. Deposit amount: " + depositAmount);
            // get the amount value
            Console.WriteLine("Event: Escrowed: " + DocInfoToString(BillAcceptor.DocType, BillAcceptor.getDocument()));
            //if(BillAcceptor.DeviceState==State.Escrow)
            //{
            //    BillAcceptor.EscrowReturn();
            //}

            try
            {

                depositAmount += (decimal)BillAcceptor.Bill.Value;


            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
                BillAcceptor.EscrowReturn();

                depositAmount = 0;
            }

            Console.WriteLine(BillAcceptor.DeviceState.ToString());


            balance = depositAmount - maxAmount;

            if (depositAmount <= maxAmount)
            {

                BillAcceptor.EscrowStack();
                userClient.Send(GetFrameFromString("This is message from server to client loading ......."));

                if (depositAmount == maxAmount)
                {
                    userClient.Send(GetFrameFromString("This is message from server to client successfully"));
                    BillAcceptor.AutoStack = true;
                    disableCashAccepting();
                }
      
                else
                {
                    userClient.Send(GetFrameFromString("This is message from server to client successfully but the amount is less than the maximum allowed. Please add more money."));
                }
            }
            else
            {  
            
                userClient.Send(GetFrameFromString("This is message from server to unsuccessful You put correct amount " + balance));
                BillAcceptor.EscrowReturn();

            }


        }

        static private void HandleReturnedEvent(object sender, EventArgs e)
        {
            Console.WriteLine("Command: BillAccepter.EscrowReturn()");
            BillAcceptor.EscrowReturn();
           
        }
    }



    internal class stream
    {
        internal static int Read(byte[] buffer, int v, int length)
        {
            throw new NotImplementedException();
        }
    }

    internal class socket
    {
        internal static int Receive(byte[] buffer, int bytesRead, int v, SocketFlags none)
        {
            throw new NotImplementedException();
        }
    }

    internal class WebSocketState
    {
        public static object Open { get; internal set; }
    }

    internal class HandleClientConnecdEventHandler
    {
        private Action<object, EventArgs> handleClientConnecdEvent;

        public HandleClientConnecdEventHandler(Action<object, EventArgs> handleClientConnecdEvent)
        {
            this.handleClientConnecdEvent = handleClientConnecdEvent;
        }
    }

    internal class ProcessRequestEventHandler
    {
        private Func<object, object> processRequestAsync;
        private Func<HttpListenerContext, Task> processRequestAsync1;

        public ProcessRequestEventHandler(Func<HttpListenerContext, Task> processRequestAsync1)
        {
            this.processRequestAsync1 = processRequestAsync1;
        }

        public ProcessRequestEventHandler(Func<object, object> processRequestAsync)
        {
            this.processRequestAsync = processRequestAsync;
        }
    }

    internal class WebSocketMessageType
    {
        internal static object Text;
    }

    internal class HttpListener
    {
        public object Prefixes { get; internal set; }

        internal object GetContext()
        {
            throw new NotImplementedException();
        }

        internal Task<HttpListenerContext> GetContextAsync()
        {
            throw new NotImplementedException();
        }

        internal void Start()
        {
            throw new NotImplementedException();
        }
    }

    internal class WebSocketServer
    {
        public WebSocketServer()
        {
        }
    }

    internal class WebSocketListener
    {
        public WebSocketListener()
        {
        }
    }
}
