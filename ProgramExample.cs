using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MobileGameController;

namespace MobileGameControllerTest
{
    class ProgramExample
    {
        static void Main(string[] args)
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Parse( "192.168.68.68"),567);
            tcpListener.Start();
            while (true)
            {
                MyGameController gameController = new MyGameController(tcpListener);
                while (gameController.GetInfo().Equals("")) { }
                Console.WriteLine("accepted " + gameController.GetInfo().Split(':')[1]);

            }
        }
    }
    class MyGameController : GameController
    {
        public MyGameController(TcpListener tcpListener) : base(tcpListener)
        {
        }
        System.Media.SoundPlayer Leftplayer = new System.Media.SoundPlayer(@"C:\Users\User\Downloads\316913__jaz_the_man_2__si.wav");
        System.Media.SoundPlayer Upplayer = new System.Media.SoundPlayer(@"C:\Users\User\Downloads\316909__jaz_the_man_2__re-stretched.wav");
        System.Media.SoundPlayer DownPlayer = new System.Media.SoundPlayer(@"C:\Users\User\Downloads\316912__jaz_the_man_2__sol.wav");
        System.Media.SoundPlayer RightPlayer = new System.Media.SoundPlayer(@"C:\Users\User\Downloads\316907__jaz_the_man_2__mi-stretched.wav");



        protected override void OnCloseEvent()
        {
            Console.WriteLine("done");
        }
        protected override void OnJoystickMoveEvent()
        {
            Console.WriteLine(this.GetX()+","+this.GetY());
        }
        protected override void OnUpButtonPressEvent()
        {
            Console.WriteLine("up");
            Upplayer.Stop();
            Upplayer.Play();
        } 
        protected override void OnDownButtonPressEvent()
        {
            Console.WriteLine("down");
            DownPlayer.Stop();
            DownPlayer.Play();
        }
        protected override void OnLeftButtonPressEvent()
        {
            Console.WriteLine("left");
            Leftplayer.Stop();
            Leftplayer.Play();
        }
        protected override void OnRightButtonPressEvent()
        {
            Console.WriteLine("right");
            RightPlayer.Stop();
            RightPlayer.Play();
        }
    }
}
