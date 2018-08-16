using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Common
{
    public class TimeRange
    {

  
        public const int MinutesOfTheDay = 1440;
        private bool _initialized = false;

        public TimeRange(TimeSpan start, TimeSpan end)
            : this()
        {
            Start = start;
            End = end;
        }
        public TimeRange(int startH, int startM, int endH, int endM)
            : this()
        {
            Start = new TimeSpan(startH, startM, 0);
            End = new TimeSpan(endH, endM, 0);
        }

        public TimeRange()
        {
            Start = new TimeSpan(0, 0, 0);
            End = new TimeSpan(24, 0, 0);
            _initialized = true;
        }

        public override string ToString()
        {
            return FormatTime(Start) + '-' + FormatTime(End);
        }

        public static string FormatTime(TimeSpan timefield)
        {
            if (timefield.Hours == 0 && timefield.Minutes == 0)
            {
                return "24:00";
            }
            else
            {
                return timefield.Hours.ToString().PadLeft(2, '0') + ":" + timefield.Minutes.ToString().PadLeft(2, '0');
            }
        }

        public void SetStart(int h, int m)
        {
            Start = new TimeSpan(h, m, 0);
        }

        public void SetEnd(int h, int m)
        {
            End = new TimeSpan(h, m, 0);
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;
            TimeRange tr = (TimeRange)obj;
            return ToString() == tr.ToString();
        }

        public bool Clashes(TimeRange other)
        {
            return Clashes(other, false);
        }

        public bool Clashes(TimeRange other, bool inclusive)
        {
            if (inclusive)
            {
                return (other.Start <= Start && other.End >= End) ||
                    (other.Start < Start && other.End >= Start) ||
                    (other.End > End && other.Start <= End) ||
                    (other.Start >= Start && other.End <= End);
            }
            else
            {
                return (other.Start < Start && other.End > End) ||
                    (other.Start < Start && other.End > Start) ||
                    (other.End > End && other.Start < End) ||
                    (other.Start >= Start && other.End <= End);
            }
        }

        public static bool operator ==(TimeRange left, TimeRange right)
        {
            if ((object)left == null && (object)right == null)
            {
                return true;
            }
            else if ((object)left != null && (object)right != null)
            {
                return left.ToString() == right.ToString();
            }
            return false;
        }

        public static bool operator !=(TimeRange left, TimeRange right)
        {
            return !(left == right);
        }

        public bool IsIn(TimeSpan timespan)
        {
            return (Start.Ticks <= timespan.Ticks && End.Ticks >= timespan.Ticks);
        }


        public bool IsIn(TimeRange timerange)
        {
            return IsIn(timerange.Start) && IsIn(timerange.End);
        }

        /// <summary>
        /// Tries to parse a given string into a TimeRange object
        /// Strings of the following format are parsed:
        /// - 1:00-12:20
        /// - 1:00:10-12:20:00
        /// - 01:00-12:30
        /// - 1:0-12:30
        /// - 100-1230
        /// - 0100-1230
        /// - 1-1230
        /// </summary>
        /// <param name="timeRangeString"></param>
        /// <returns>null if could not be parsed</returns>
        public static TimeRange Parse(string input)
        {
            string[] parts = input.Split('-');
            if (parts.Length == 2)
            {
                TimeSpan? start = ParseTimeSpan(parts[0]);
                TimeSpan? end = ParseTimeSpan(parts[1]);
                if (start == null || end == null || start.Value > end.Value) return null;
                return new TimeRange((start ?? new TimeSpan()), (end ?? new TimeSpan()));
            }
            return null;
        }

        public static TimeSpan? ParseTimeSpan(string input)
        {
            string[] parts = input.Split(':');
            int h = 0, m = 0;
            if (parts.Length >= 2)
            {
                if (int.TryParse(parts[0], out h) && int.TryParse(parts[1], out m))
                {
                    if (h > 24 || m > 60) return null;
                    return new TimeSpan(h, m, 0);
                }
                return null;
            }
            else if (input.Length > 0)
            {
                switch (input.Length)
                {
                    case 1:
                        if (int.TryParse(input, out h))
                        {
                            if (h > 24) return null;
                            return new TimeSpan(h, 0, 0);
                        }
                        break;
                    case 2:
                        goto case 1;
                    case 3:
                        goto case 4;
                    case 4:
                        string minutes = input.Substring(input.Length - 2, 2);
                        string hours = input.Substring(0, ((input.Length == 3) ? 1 : 2));
                        if (int.TryParse(hours, out h) && int.TryParse(minutes, out m))
                        {
                            if (h > 24 || m > 60) return null;
                            return new TimeSpan(h, m, 0);
                        }
                        break;
                }
            }
            return null;
        }

        #region Properties

        private TimeSpan _start;
        private TimeSpan _end;

        public TimeSpan Start
        {
            get { return _start; }
            set
            {
                if (_initialized)
                {
                    if (value.TotalMinutes > TimeRange.MinutesOfTheDay - 1) throw new ArgumentException("Min. 00:00, Max. 23:59");
                    if (value >= End) throw new ArgumentException("Start must be before End.");
                }
                _start = value;
            }
        }

        public TimeSpan End
        {
            get { return _end; }
            set
            {
                if (_initialized)
                {
                    if (value.TotalMinutes < 1 || value.TotalMinutes > TimeRange.MinutesOfTheDay) throw new ArgumentException("Min. 00:01, Max. 24:00");
                    if (value <= Start) throw new ArgumentException("End must be after Start.");
                }
                _end = value;
            }
        }

        #endregion
    }
}
