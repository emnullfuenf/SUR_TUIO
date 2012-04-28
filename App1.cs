using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Surface;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Tuio;

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
    public class App1 : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager graphics;
        private TouchTarget touchTarget;
        private bool applicationLoadCompleteSignalled;
        private const int millisecondsToDisappear = 3000;
        private TuioServer tuioserver;
        private HashSet<int> tuio_ids;
        private int size_x;
        private int size_y;

        public App1()
        {
            graphics = new GraphicsDeviceManager(this);
            tuioserver = new TuioServer();
            tuio_ids = new HashSet<int>();

            size_x = InteractiveSurface.PrimarySurfaceDevice.Width;
            size_y = InteractiveSurface.PrimarySurfaceDevice.Height;
        }

        private int process_touches(ReadOnlyTouchPointCollection touches)
        {
            int count = 0;
            foreach (TouchPoint touch in touches)
            {
                if (touch.IsFingerRecognized || InteractiveSurface.PrimarySurfaceDevice.IsFingerRecognitionSupported == false)
                {
                    float x = touch.X / size_x;
                    float y = touch.Y / size_y;
                    //touch.X, touch.Y),touch.Orientation

                    if (tuio_ids.Contains(touch.Id))
                    {
                        tuioserver.UpdateTuioCursor(touch.Id, new System.Drawing.PointF(x, y));
                    }
                    else
                    {
                        tuioserver.AddTuioCursor(touch.Id, new System.Drawing.PointF(x, y));
                        tuio_ids.Add(touch.Id);
                    }

                }
                tuioserver.InitFrame();
                tuioserver.CommitFrame();
            }
            return count;
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            IsFixedTimeStep = false;
            SetWindowOnSurface();
            InitializeSurfaceInput();

            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;

            base.Initialize();
        }

        private void SetWindowOnSurface()
        {
            System.Diagnostics.Debug.Assert(Window != null && Window.Handle != IntPtr.Zero,
                "Window initialization must be complete before SetWindowOnSurface is called");
            if (Window == null || Window.Handle == IntPtr.Zero)
                return;

            Program.InitializeWindow(Window);
            graphics.PreferredBackBufferWidth = Program.WindowSize.Width;
            graphics.PreferredBackBufferHeight = Program.WindowSize.Height;
            graphics.ApplyChanges();
            Program.PositionWindow();
        }

        private void InitializeSurfaceInput()
        {
            System.Diagnostics.Debug.Assert(Window != null && Window.Handle != IntPtr.Zero,
                "Window initialization must be complete before InitializeSurfaceInput is called");
            if (Window == null || Window.Handle == IntPtr.Zero)
                return;
            System.Diagnostics.Debug.Assert(touchTarget == null,
                "Surface input already initialized");
            if (touchTarget != null)
                return;

            touchTarget = new TouchTarget(IntPtr.Zero, EventThreadChoice.OnBackgroundThread);
            touchTarget.EnableInput();
        }
        
        protected override void Update(GameTime gameTime)
        {
            ReadOnlyTouchPointCollection t = touchTarget.GetState();
            process_touches(t);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            if (!applicationLoadCompleteSignalled)
            {
                ApplicationServices.SignalApplicationLoadComplete();
                applicationLoadCompleteSignalled = true;
            }

            graphics.GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }

        private void OnWindowInteractive(object sender, EventArgs e)
        {

        }

        private void OnWindowNoninteractive(object sender, EventArgs e)
        {

        }

        private void OnWindowUnavailable(object sender, EventArgs e)
        {

        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                touchTarget.Dispose();

                IDisposable graphicsDispose = graphics as IDisposable;
                if (graphicsDispose != null)
                {
                    graphicsDispose.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion       
    }

}
