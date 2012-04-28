using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;

using Bespoke.Common.Osc;

namespace Tuio
{
    /// <summary>
    /// Simple, still uncomplete implementation of a TUIO server in C#.
    /// 
    /// Current shortcomings:
    /// Object support missing.
    /// Does not implement frame times.
    /// Only supports external TUIO cursors.
    /// Allways commits all cursors.
    /// 
    /// (c) 2010 by Dominik Schmidt (schmidtd@comp.lancs.ac.uk)
    /// </summary>
    public class TuioServer
    {
        #region constants
        
        private const string _cursorAddressPattern = "/tuio/2Dcur";

        private const string _objectAddressPattern = "/tuio/2Dobj"; 

        #endregion

        #region fields

        private IPEndPoint _ipEndPoint;

        private Dictionary<int, TuioCursor> _cursors;

        private int _currentFrame;

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new server with and endpoint at localhost, port 3333.
        /// </summary>
        public TuioServer() : this("127.0.0.1", 3333) { }

        /// <summary>
        /// Creates a new server.
        /// </summary>
        /// <param name="host">Endpoint host</param>
        /// <param name="port">Endpoint port</param>
        public TuioServer(string host, int port)
        {
            _cursors = new Dictionary<int, TuioCursor>();
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
            _currentFrame = 0;
        }

        #endregion

        #region frame related methods

        /// <summary>
        /// Initialized a new frame and increases the frame counter.
        /// </summary>
        public void InitFrame()
        {
            _currentFrame++;
        }

        /// <summary>
        /// Commits the current frame.
        /// </summary>
        public void CommitFrame()
        {
            GetFrameBundle().Send(_ipEndPoint);
        }

        #endregion

        #region cursor related methods
        
        /// <summary>
        /// Adds a TUIO cursor. A new id, not used before, must be provided.
        /// </summary>
        /// <param name="id">New id</param>
        /// <param name="location">Location</param>
        public void AddTuioCursor(int id, PointF location)
        {
            lock(_cursors)
                if(!_cursors.ContainsKey(id))
                    _cursors.Add(id, new TuioCursor(id, location));
        }

        /// <summary>
        /// Updates a TUIO cursor. An id of an existing cursor must be provided.
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="location">Location</param>
        public void UpdateTuioCursor(int id, PointF location)
        {
            TuioCursor cursor;
            if(_cursors.TryGetValue(id, out cursor))
                cursor.Location = location;
        }

        /// <summary>
        /// Deletes a TUIO cursor. An id of an existing cursor must be provided.
        /// </summary>
        /// <param name="id">Id</param>
        public void DeleteTuioCursor(int id)
        {
            lock (_cursors)
                _cursors.Remove(id);
        }

        #endregion

        #region osc message assembly

        private OscBundle GetFrameBundle()
        {
            OscBundle bundle = new OscBundle(_ipEndPoint);

            bundle.Append(GetAliveMessage());
            foreach (OscMessage msg in GetCursorMessages())
                bundle.Append(msg);
            bundle.Append(GetSequenceMessage());

            return bundle;
        }

        private OscMessage GetAliveMessage()
        {
            OscMessage msg = new OscMessage(_ipEndPoint, _cursorAddressPattern);

            msg.Append("alive");
            lock (_cursors)
                foreach (TuioCursor cursor in _cursors.Values)
                    msg.Append((Int32)cursor.Id);

            return msg;
        }

        private OscMessage GetSequenceMessage()
        {
            OscMessage msg = new OscMessage(_ipEndPoint, _cursorAddressPattern);

            msg.Append("fseq");
            msg.Append((Int32)_currentFrame);

            return msg;
        }

        private OscMessage GetCursorMessage(TuioCursor cursor)
        {
            OscMessage msg = new OscMessage(_ipEndPoint, _cursorAddressPattern);

            msg.Append("set");
            msg.Append((Int32)cursor.Id);
            msg.Append(cursor.Location.X);
            msg.Append(cursor.Location.Y);
            msg.Append(cursor.Speed.X);
            msg.Append(cursor.Speed.Y);
            msg.Append(cursor.MotionAcceleration);

            return msg;
        }

        private IEnumerable<OscMessage> GetCursorMessages()
        {
            List<OscMessage> msgs = new List<OscMessage>();

            lock (_cursors)
                foreach (TuioCursor cursor in _cursors.Values)
                    msgs.Add(GetCursorMessage(cursor));

            return msgs.AsEnumerable();
        }

        #endregion
    }
}
