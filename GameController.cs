using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//credit-itaythepro1234
namespace MobileGameController
{
    class GameController
    {
        protected TcpClient tcpClient;
        protected Stream stream;
        private float x;
        private float y;
        private bool run;
        private string info;


        public GameController(TcpListener tcpListener) {
            this.tcpClient = tcpListener.AcceptTcpClient();
            this.x = 0;
            this.y=0;
            this.run = true;
            this.info = "";
            this.stream = tcpClient.GetStream();

            new Thread(()=> { StartDataReading(); }).Start();
            new Thread(() => { MoveDetector(); }).Start();
        }

        public float GetX() { return this.x; }
        public float GetY() { return this.y; }

        public string GetInfo() { return this.info; }

        public void Close() {
            this.run = false;
            tcpClient.Close();
            OnCloseEvent();
        }

        protected virtual void OnCloseEvent(){ }

        byte[] data;
        public void StartDataReading()
        {
            data = new byte[tcpClient.ReceiveBufferSize];
            stream.BeginRead(data,
            0,
                                              System.Convert.ToInt32(tcpClient.ReceiveBufferSize),
                                              ReciveData,
                                              null);
        }
        public void ReciveData(IAsyncResult ar)
        {
            int bytesRead;
            try
            {
                lock (stream)
                {
                    bytesRead = stream.EndRead(ar);
                }

                //disconnect
                if (bytesRead < 1) {
                    run = false;
                    tcpClient.Close();
                    OnCloseEvent();
                }
                else
                {   
                    string input= Encoding.ASCII.GetString(data,0,bytesRead);
                    HandleInput(input);
                }

                lock (stream)
                {
                    if (run)
                    {
                        stream.BeginRead(data, 0, System.Convert.ToInt32(tcpClient.ReceiveBufferSize), ReciveData, null);
                    }
                }

            }
            catch (Exception ex) { }
        }

        private void MoveDetector() {
            float locx = x;
            float locy = y;
            while (true) {
                if (locx != x) { OnJoystickMoveEvent(); }
                else if (locy != y) { OnJoystickMoveEvent(); }
                locx = x;
                locy = y;
                Thread.Sleep(25);
            }
        }

        private void HandleInput(string input) {
            string[] messages = input.Split(';');
            foreach (string message in messages) {
                string[] nums= message.Split(',');
                if (nums.Length == 2) {
                    this.x = float.Parse(nums[0]);
                    this.y = float.Parse(nums[1]);
                } else if(!message.Equals("")){
                    switch (message.Substring(0,1)) { 
                        case "u":
                            OnUpButtonPressEvent();
                            break;
                        case "d":
                            OnDownButtonPressEvent();
                            break;
                        case "l":
                            OnLeftButtonPressEvent();
                            break;
                        case "r":
                            OnRightButtonPressEvent();  
                            break;
                        case "i":
                            this.info = message.Substring(1);
                            OnInfoSentEvent();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        protected virtual void OnInfoSentEvent() { }

        protected virtual void OnJoystickMoveEvent() { }

        protected virtual void OnUpButtonPressEvent() {}
        protected virtual void OnDownButtonPressEvent() { }
        protected virtual void OnLeftButtonPressEvent() { } 
        
        protected virtual void OnRightButtonPressEvent() {}

    }
}
