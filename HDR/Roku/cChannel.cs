using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HDR.Roku
{
    class cChannel
    {
        private string _guideNumber;
        public string GuideNumber
        {
            get { return _guideNumber; }
            set { _guideNumber = value; } 
        }
        private string _guideName;
        public string GuideName
        {
            get { return _guideName; }
            set { _guideName = value; }
        }
        private string _logoUrl;
        public string LogoUrl
        {
            get { return _logoUrl; }
            set { _logoUrl = value; }
        }

        public cChannel()
        {
        }

        public cChannel(String guideNumber, String guideName, String logoPath, String logoDir)
        {
            _guideNumber = guideNumber;
            _guideName = guideName;

            //logo exists? may slow down a bit, but look good on screen
            if (File.Exists(logoDir + GuideName + ".png"))
            {
                _logoUrl = logoPath + GuideName + ".png";
            }
            else
            {
                _logoUrl = logoPath + "default.png";
            }
            
        }

        /// <summary>
        /// get list of roku channels
        /// </summary>
        /// <param name="HDHomeRunChannels">list of HDHomeRun.cChannels</param>
        /// <returns>list of Roku.cChannel</returns>
        public static List<cChannel> getRokuChannels(List<HDHomerun.cChannels> HDRChannels, Boolean onlyFavorites, String logoPath, String logoDir)
        {
            try
            {
                List<Roku.cChannel> rokuChannels = new List<Roku.cChannel>();

                //transform hdhomerun list channel to roku channel list
                foreach (HDHomerun.cChannels HDRChannel in HDRChannels)
                {
                    if (onlyFavorites == false)
                    {
                        rokuChannels.Add(new Roku.cChannel(HDRChannel.GuideNumber, HDRChannel.GuideName,logoPath,logoDir));
                    }
                    else
                    {
                        if (HDRChannel.Tags.Contains("favorite"))
                        {
                            rokuChannels.Add(new Roku.cChannel(HDRChannel.GuideNumber, HDRChannel.GuideName, logoPath,logoDir));
                        }
                    }
                }
                return rokuChannels;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("cChannel.getRokuChannels, Error: {0}", ex.Message));
            }
        }

    }
}
