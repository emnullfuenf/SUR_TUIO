using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Surface;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework;

namespace SUR_TUIO
{
    /// <summary>
    /// SUR TUIO 0.1
    /// 
    /// A simple TUIO server for SUR40
    /// 
    /// It sends all touches as TUIO pointers at the moment.
    /// Finger's directions and objects will follow.
    /// 
    /// Based on Dominik Schmidt's .NET/C# TUIO server.
    /// http://www.dominikschmidt.net/2010/11/net-c-tuio-server/
    /// 
    /// Copyright (c) 2012 Michael Zoellner
    /// http://i.document.m05.de
    /// 
    /// Licensed under GPLv3 license.
    /// http://www.gnu.org/licenses/gpl.html
    /// </summary>
    static class Program
    {
        static GameWindow Window;

        [STAThread]
        static void Main(string[] args)
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

            GlobalizationSettings.ApplyToCurrentThread();

            using (App1 app = new App1())
            {
                app.Run();
            }
        }

        internal static Size WindowSize
        {
            get
            {
                return ((Form)Form.FromHandle(Window.Handle)).DesktopBounds.Size;
            }
        }

        internal static void InitializeWindow(GameWindow window)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            Window = window;

            Form form = (Form)Form.FromHandle(Window.Handle);
            form.LocationChanged += OnFormLocationChanged;

            SetWindowStyle();
            SetWindowSize();
        }

        private static void OnFormLocationChanged(object sender, EventArgs e)
        {
            if (SurfaceEnvironment.IsSurfaceEnvironmentAvailable)
            {
                Form form = (Form)Form.FromHandle(Window.Handle);
                form.LocationChanged -= OnFormLocationChanged;
                PositionWindow();
                form.LocationChanged += OnFormLocationChanged;
            }
        }

        internal static void PositionWindow()
        {
            int left = (InteractiveSurface.PrimarySurfaceDevice != null)
                            ? InteractiveSurface.PrimarySurfaceDevice.WorkingAreaLeft
                            : Screen.PrimaryScreen.WorkingArea.Left;
            int top = (InteractiveSurface.PrimarySurfaceDevice != null)
                            ? InteractiveSurface.PrimarySurfaceDevice.WorkingAreaTop
                            : Screen.PrimaryScreen.WorkingArea.Top;

            Form form = (Form)Form.FromHandle(Window.Handle);
            FormWindowState windowState = form.WindowState;
            form.WindowState = FormWindowState.Normal;
            form.Location = new System.Drawing.Point(left, top);
            form.WindowState = windowState;
        }

        private static void SetWindowStyle()
        {
            Window.AllowUserResizing = true;
            Form form = (Form)Form.FromHandle(Window.Handle);
            form.FormBorderStyle = (SurfaceEnvironment.IsSurfaceEnvironmentAvailable)
                                    ? FormBorderStyle.None
                                    : FormBorderStyle.Sizable;
        }


        private static void SetWindowSize()
        {
            int width = 200; /* (InteractiveSurface.PrimarySurfaceDevice != null)
                             ? InteractiveSurface.PrimarySurfaceDevice.WorkingAreaWidth
                             : Screen.PrimaryScreen.WorkingArea.Width;*/
            int height = 200; /* (InteractiveSurface.PrimarySurfaceDevice != null)
                             ? InteractiveSurface.PrimarySurfaceDevice.WorkingAreaHeight
                             : Screen.PrimaryScreen.WorkingArea.Height;*/

            Form form = (Form)Form.FromHandle(Window.Handle);
            form.ClientSize = new Size(width, height);
            form.WindowState = (SurfaceEnvironment.IsSurfaceEnvironmentAvailable)
                            ? FormWindowState.Normal
                            : FormWindowState.Normal;
        }
    }
}

