using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDR.HDHomerun
{
    /// <summary>
    /// class to hold HDHomeRunPlus 
    /// </summary>
    public class cHDHomeRunPlus
    {
        private String _deviceID;
        public String DeviceID
        {
            get { return _deviceID; }
            set { _deviceID = value; }
        }
        private String _ip;
        public String IP
        {
            get { return _ip; }
            set { _ip = value; }
        }
        public cHDHomeRunPlus()
        {
            //needed for auto deserialization
        }
        public cHDHomeRunPlus(String iDev, String iIP)
        {
            _deviceID = iDev;
            _ip = iIP;
        }
    }
}
