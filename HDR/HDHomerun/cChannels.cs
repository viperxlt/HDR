using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace HDR.HDHomerun
{
    class cChannels
    {
        public string GuideNumber { get; set; }
        public string GuideName { get; set; }
        public string Tags { get; set; }
        public string URL { get; set; }

        public cChannels()
        {
        }

        public cChannels(String guideNumber, String guideName, String tags, String url)
        {
            GuideNumber = guideNumber;
            GuideName = guideName;
            Tags = tags;
            URL = url;
        }

        /// <summary>
        /// get channel list from HDHomeRunPlus (from first device in list)
        /// </summary>
        /// <param name="ipaddr"></param>
        /// <returns>List of cChannels</returns>
        public static List<cChannels> getChannels(List<cHDHomeRunPlus> HDHDRPs)
        {
            try 
	        {	
                List<cChannels> channels = new List<cChannels>();
                String json = string.Empty;

                //use webclient to request channel JSON from HDHomeRunPLus
                using (var client = new WebClient())
                {
                    //get channels using first HDHomeRunPlus device
                    json = client.DownloadString("http://" + HDHDRPs[0].IP + "/lineup.json");
                }
                //deserialize JSON into cChannel
                var serializer = new Nancy.Json.JavaScriptSerializer();
                channels = serializer.Deserialize<List<cChannels>>(json);
                
                return channels;
	        }
	        catch (Exception ex)
	        {
                throw new Exception(String.Format("cChannel.getChannels, Error: {0}", ex.Message));
	        }
        }
    }
}
