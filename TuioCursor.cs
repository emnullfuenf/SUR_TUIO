using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Tuio
{
    /// <summary>
    /// TUIO cursor.
    /// 
    /// (c) 2010 by Dominik Schmidt (schmidtd@comp.lancs.ac.uk)
    /// </summary>
    public class TuioCursor
    {
        #region properties

        public int Id { get; private set; }

        public PointF Location { get; set; }

        public PointF Speed { get; set; }

        public float MotionAcceleration { get; set; }

        #endregion

        #region constructors

        public TuioCursor(int id, PointF location)
        {
            Id = id;
            Location = location;
        }

        #endregion

    }
}
