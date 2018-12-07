using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Robot
{
    public class Utils
    {
        public class DataTrend
        {
            private List<double> _dataList = new List<double>();

            private int _maxItems = 2;
            public int MaxItems
            {
              get { return _maxItems; }
              set { _maxItems = value; }
            }

            public void addDataPoint( int data )
            {
                _dataList.Add( (double)data );
                if (_dataList.Count > _maxItems)
                {
                    _dataList.RemoveAt(0);
                }
            }

            public void addDataPoint( double data )
            {
                _dataList.Add( data );
            }

            // if data items are increasing, return positive, else return negative.
            public double getDataTrend()
            {
                double trend = 0;
                // 1 2 3 3 4  : 1 1 0 1 : 3
                // 3 3 3 2 2  : 0 0 -1 0 : -1
                for ( int i = 1; i < _dataList.Count; i++ )
                {
                    double slope = _dataList[i] - _dataList[i - 1];
                    
                    // more recent are more significant
                    //slope = slope * ((double)i / (double)_dataList.Count);

                    // all data points equally significant

                    
                    trend += slope;
                }
                return trend;
            }
        }


        static public byte lowByte(short arg)
        {
            return (byte)(arg & 0xff);
        }

        static public byte highByte(short arg)
        {
            return (byte)((arg >> 8) & 0xff);
        }

        static public void debugOut(string output)
        {
            System.Console.WriteLine(output);
        }

        static public void errorOut(string output)
        {
            System.Console.WriteLine(output);
        }

    }
}
