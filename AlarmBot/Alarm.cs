using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlarmBot
{
    class Alarm
    {
        string pilotName;
        Point startPos;
        Point takeControlPosition;
        Point beginView;
        Point endView;

        Thread t;
        bool work = true;
        Random rand = new Random();

        public void Start()
        {
            DiscordMessager.IsLaunchPilotMode = true;
            DiscordMessager.IsInviterMode = false;
            DiscordMessager.s = Stop;
            UpdateSettings();
            //LaunchPilot();
            if (FindWindow(null, String.Format("EVE - {0}", pilotName)) == IntPtr.Zero)
            {
                DiscordMessager.IsLaunchPilotMode = false;
                DiscordMessager.IsInviterMode = false;
                DiscordMessager.IsWriteToChatMode = false;
                DiscordMessager.SendToAll("Окно не открылось!");
                return;
            }
            DiscordMessager.IsLaunchPilotMode = false;
            DiscordMessager.IsInviterMode = true;
            DiscordMessager.IsWriteToChatMode = false;
            t = new Thread(run);
            if (t.ThreadState == ThreadState.Stopped || t.ThreadState == ThreadState.Unstarted)
            {
                work = true;
                t.Start();
            }
        }

        public void Stop()
        {
            work = false;
            if (t.ThreadState == ThreadState.Stopped || t.ThreadState == ThreadState.Unstarted)
            {
                if (DiscordMessager.IsLaunchPilotMode || DiscordMessager.IsInviterMode)
                {
                    DiscordMessager.SendToAll("Пилот закрыт");
                    DiscordMessager.IsLaunchPilotMode = false;
                    DiscordMessager.IsInviterMode = false;
                }
            }
            Thread.Sleep(2000);
        }

        private void LaunchPilot()
        {
            LeftMouseClick(startPos.X, startPos.Y);
            Thread.Sleep(80000);
            LeftMouseClick(238, 326);
            Thread.Sleep(60000);
            LeftMouseClick(takeControlPosition.X, takeControlPosition.Y);
            Thread.Sleep(20000);
        }

        private void ExitPilot()
        {
            PushEscape();
            Thread.Sleep(rand.Next(1500, 2000));
            LeftMouseClick(929, 655);
        }

        private void CheckOK()
        {
            Bitmap bOut = new Bitmap(26, 8);
            if (compareBitmap(bOut, Screen.getScreen(499, 472, 26, 8)))
            {
                DiscordMessager.SendToAll("Выкинуло из игры");
                LeftMouseClick(504, 476);               //out
                work = false;
                fastClose = true;
            }
        }

        private void run()
        {
            work = true;
            DiscordMessager.SendToAll("Вроде начал смотреть");
            int PosX = beginView.X, PosY = beginView.Y;
            int step = 19;
            int countCheck = Math.Abs(beginView.Y - endView.Y) / step - 1;
            int countEnemyes = 0, oldCountEnemyes;
            Bitmap endCheck = Screen.getScreen(endView.X, endView.Y, 8, 8);
            while (work)
            {
                PushV();
                PosY = beginView.Y;
                oldCountEnemyes = countEnemyes;
                countEnemyes = 0;
                for (int i = 0; i < countCheck; i++)
                {
                    if (!compareBitmap(Screen.getScreen(PosX, endView.Y - step, 8, 8), Screen.getScreen(PosX, PosY, 8, 8)))
                    {
                        countEnemyes++;
                    }
                    else
                    {
                        break;
                    }
                    PosY += step;
                }                
                if (oldCountEnemyes < countEnemyes)
                {
                    oldCountEnemyes = countEnemyes;
                    DiscordMessager.SendToAll(countEnemyes + " врага!");
                    DiscordMessager.SendPhotoAsync();
                    continue;
                }
                if (oldCountEnemyes > countEnemyes)
                {
                    oldCountEnemyes = countEnemyes;
                    DiscordMessager.SendToAll("Ушли с гайки");
                    DiscordMessager.SendPhotoAsync();
                    continue;
                }
                oldCountEnemyes = countEnemyes;
            }
            CheckOK();
            DiscordMessager.SendToAll("Выхожу из бота");
            DiscordMessager.IsLaunchPilotMode = true;
            DiscordMessager.IsInviterMode = false;
            /*while (FindWindow(null, String.Format("EVE - {0}", pilotName)) != IntPtr.Zero)
            {
                ExitPilot();
                Thread.Sleep(10000);
            }*/
            DiscordMessager.SendToAll("Пилот закрыт");
            DiscordMessager.IsLaunchPilotMode = false;
            DiscordMessager.IsInviterMode = false;
            fastClose = false;
            return;
        }

        private bool compareBitmap(Bitmap a, Bitmap b)
        {
            if (a.Size != b.Size)
            {
                return false;
            }
            for (int i = 0; i < a.Size.Width; i++)
            {
                for (int j = 0; j < a.Size.Height; j++)
                {
                    if (a.GetPixel(i, j) != b.GetPixel(i, j))
                    {
                        if (compareColors(a.GetPixel(i, j), b.GetPixel(i, j)))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool compareColors(Color a, Color b)
        {
            int AverageA = (a.R + a.G + a.B) / 3;
            int AverageB = (b.R + b.G + b.B) / 3;
            if (Math.Abs(AverageA - AverageB) > 20)
                return true;
            return false;
        }

        private void UpdateSettings()
        {
            pilotName = Properties.Settings.Default.PilotName;
            startPos = Properties.Settings.Default.StartPosition;
            takeControlPosition = Properties.Settings.Default.TakeControlPosition;
            beginView = Properties.Settings.Default.BeginView;
            endView = Properties.Settings.Default.EndView;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string _ClassName, string _WindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        public const int MOUSE_LEFTDOWN = 0x02;
        public const int MOUSE_LEFTUP = 0x04;
        public const int MOUSE_RIGHTDOWN = 0x08;
        public const int MOUSE_RIGHTUP = 0x10;
        private bool fastClose = false;

        public void LeftMouseClick(int x, int y)
        {
            Thread.Sleep(rand.Next(500, 800));
            SetCursorPos(x, y);
            Thread.Sleep(rand.Next(500, 800));
            mouse_event(MOUSE_LEFTDOWN, x, y, 0, 0);
            Thread.Sleep(rand.Next(70, 90));
            mouse_event(MOUSE_LEFTUP, x, y, 0, 0);
            Thread.Sleep(rand.Next(500, 800));
        }

        public void LeftMouseDoubleClick(int x, int y)
        {
            Thread.Sleep(rand.Next(500, 800));
            SetCursorPos(x, y);
            Thread.Sleep(rand.Next(200, 300));
            mouse_event(MOUSE_LEFTDOWN, x, y, 0, 0);
            Thread.Sleep(rand.Next(30, 40));
            mouse_event(MOUSE_LEFTUP, x, y, 0, 0);
            Thread.Sleep(rand.Next(40, 60));
            mouse_event(MOUSE_LEFTDOWN, x, y, 0, 0);
            Thread.Sleep(rand.Next(30, 40));
            mouse_event(MOUSE_LEFTUP, x, y, 0, 0);
            Thread.Sleep(rand.Next(500, 500));
        }

        public void RightMouseClick(int x, int y)
        {
            Thread.Sleep(rand.Next(500, 800));
            SetCursorPos(x, y);
            Thread.Sleep(rand.Next(200, 300));
            mouse_event(MOUSE_RIGHTDOWN, x, y, 0, 0);
            Thread.Sleep(rand.Next(70, 90));
            mouse_event(MOUSE_RIGHTUP, x, y, 0, 0);
            Thread.Sleep(rand.Next(500, 800));
        }

        public void PushEscape()
        {
            IntPtr nul = (IntPtr)0;
            IntPtr handle = FindWindow(null, String.Format("EVE - {0}", pilotName));
            //IntPtr handle = FindWindow(null, "EVE");
            SendMessage(handle, 0x0100, (IntPtr)0x1B, nul);
            SendMessage(handle, 0x0101, (IntPtr)0x1B, nul);
            /*Point _XY = new Point(958, 650);
            int x = _XY.X;
            int y = _XY.Y;
            Thread.Sleep(2000);
            IntPtr handle2 = GetForegroundWindow();
            SendMessage(handle, WM_NCHITTEST, IntPtr.Zero, new IntPtr(y * 0x10000 + x));
            SendMessage(handle, WM_SETCURSOR, handle2, new IntPtr(WM_MOUSEMOVE * 0x10000 + 1));
            SendMessage(handle, WM_MOUSEMOVE, IntPtr.Zero, new IntPtr(y * 0x10000 + x));*/
            //SendMessage(handle, WM_SETCURSOR, handle, new IntPtr(WM_LBUTTONDOWN * 0x10000 + 1));
            //SendMessage(handle, WM_LBUTTONDOWN, new IntPtr(0x0001), new IntPtr(y * 0x10000 + x));
            //SendMessage(handle, WM_LBUTTONUP, IntPtr.Zero, new IntPtr(y * 0x10000 + x));
        }

        public void PushEnterInCenter()
        {
            LeftMouseClick(1024 / 2, 768 / 2);
            IntPtr nul = (IntPtr)0;
            IntPtr handle = FindWindow(null, String.Format("EVE - {0}", pilotName));
            //IntPtr handle = FindWindow(null, "EVE");
            SendMessage(handle, 0x0100, (IntPtr)0x0D, nul);
            SendMessage(handle, 0x0101, (IntPtr)0x0D, nul);
            /*Point _XY = new Point(958, 650);
            int x = _XY.X;
            int y = _XY.Y;
            Thread.Sleep(2000);
            IntPtr handle2 = GetForegroundWindow();
            SendMessage(handle, WM_NCHITTEST, IntPtr.Zero, new IntPtr(y * 0x10000 + x));
            SendMessage(handle, WM_SETCURSOR, handle2, new IntPtr(WM_MOUSEMOVE * 0x10000 + 1));
            SendMessage(handle, WM_MOUSEMOVE, IntPtr.Zero, new IntPtr(y * 0x10000 + x));*/
            //SendMessage(handle, WM_SETCURSOR, handle, new IntPtr(WM_LBUTTONDOWN * 0x10000 + 1));
            //SendMessage(handle, WM_LBUTTONDOWN, new IntPtr(0x0001), new IntPtr(y * 0x10000 + x));
            //SendMessage(handle, WM_LBUTTONUP, IntPtr.Zero, new IntPtr(y * 0x10000 + x));
        }

        public void PushV()
        {
            IntPtr nul = (IntPtr)0;
            IntPtr handle = FindWindow(null, String.Format("EVE - {0}", pilotName));
            //IntPtr handle = FindWindow(null, "EVE");
            SendMessage(handle, 0x0100, (IntPtr)0x56, nul);
            SendMessage(handle, 0x0101, (IntPtr)0x56, nul);
            /*Point _XY = new Point(958, 650);
            int x = _XY.X;
            int y = _XY.Y;
            Thread.Sleep(2000);
            IntPtr handle2 = GetForegroundWindow();
            SendMessage(handle, WM_NCHITTEST, IntPtr.Zero, new IntPtr(y * 0x10000 + x));
            SendMessage(handle, WM_SETCURSOR, handle2, new IntPtr(WM_MOUSEMOVE * 0x10000 + 1));
            SendMessage(handle, WM_MOUSEMOVE, IntPtr.Zero, new IntPtr(y * 0x10000 + x));*/
            //SendMessage(handle, WM_SETCURSOR, handle, new IntPtr(WM_LBUTTONDOWN * 0x10000 + 1));
            //SendMessage(handle, WM_LBUTTONDOWN, new IntPtr(0x0001), new IntPtr(y * 0x10000 + x));
            //SendMessage(handle, WM_LBUTTONUP, IntPtr.Zero, new IntPtr(y * 0x10000 + x));
        }
    }
}
