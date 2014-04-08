using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDR.HDHomerun
{
    class cChannelStatus
    {
        private string _channel;
        public string channel
        {
            get { return _channel; }
            set { _channel = value; } 
        }
        private Boolean _ready;
        public Boolean ready
        {
            get { return _ready; }
            set { _ready = value; }
        }
        private string _last_read;
        public string last_read
        {
            get { return _last_read; }
            set { _last_read = value; }
        }
        private Int32 _pid;
        public Int32 pid
        {
            get { return _pid; }
            set { _pid = value; }
        }

        public cChannelStatus()
        {
        }

        public cChannelStatus(String channel, Boolean ready, String lastRead, Int32 pID)
        {
            _channel = channel;
            _ready = ready;
            _last_read = lastRead;
            _pid =pID;
        }
    }
}
