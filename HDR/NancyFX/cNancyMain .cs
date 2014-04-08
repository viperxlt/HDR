using System;
using System.Collections.Generic;
using System.Text;
using Nancy;
using System.IO;

namespace HDR.Web
{
    public class NancyMain  : NancyModule
    {
        public NancyMain()
        {
            //default url = application comments
            Get["/"] = x =>
            {
                Console.WriteLine("NancyMain: /");

                String rtn = "<strong>CHANNELS</strong></br></br>";
                rtn += "Get /channels</br>";
                rtn += "Return a list of ALL channels in HDHomerunPlus</br></br>";
                rtn += "Get /channels?type=favorites</br>";
                rtn += "Return a list of favorite channels in HDHomerunPlus</br></br>";
                rtn += "<i>Note:</i> Channel logo names are automatically generated according to the channel name.</br>";
                rtn += "If the channel name is \"FOX\" then the logo name will be \"FOX.png\"</br></br>";
                rtn += "<strong>STREAMING</strong></br></br>";
                rtn += "Post /channels/{number}/tune</br>";
                rtn += "Tune to channel (start)</br></br>";
                rtn += "Get /channels/{number}/status</br>";
                rtn += "Tune channel status (status)</br></br>";
                rtn += "Post /channels/{number}/stop</br>";
                rtn += "DeTune from channel (Stop)</br></br>";
                rtn += "Get /channels/{number}.m3u8</br>";
                rtn += "HLS Channel stream</br></br>";
                rtn += "<strong>STORAGE</strong></br></br>";
                rtn += "Current logo storage " + Program.logoDir + ".</br>";
                rtn += "Current stream storage " + Program.streamDir + ".</br></br>";
                return (rtn);
            };

            //CHANNELS

            Get["/channels"] = parameters =>
            {
                try
                {
                    //get "favorites" parameter if available
                    Boolean onlyFavorites = false;
                    Nancy.DynamicDictionary rawStart = Request.Query;
                    if (rawStart.ContainsKey("type"))
                    {
                        if (rawStart["type"] == "favorites")
                        {
                            onlyFavorites = true;
                            Console.WriteLine("Get /channels?type=favorites");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Get /channels");
                    }

                    //get RokuChannel list
                    List<Roku.cChannel> rokuChannels = Roku.cChannel.getRokuChannels(
                                                        HDHomerun.cChannels.getChannels(Program.HDHRPs),
                                                        onlyFavorites,
                                                        Program.logolPath,
                                                        Program.logoDir);

                    //create dictionary for json formatting
                    Dictionary<String, List<Roku.cChannel>> dic = new Dictionary<string,List<Roku.cChannel>>();
                    dic.Add("channels", rokuChannels);

                    //return JSON
                    return Response.AsJson(dic);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(String.Format("NancyMain.channels, Error: {0}", ex.Message));
                    return "";
                }
            };


            //STREAMING


            //Tune to channel (Post)
            Post["/channels/{number}/tune"] = parameters =>
            {
                Console.WriteLine("Post: /channels/" + parameters.number + "/tune");

                //start channel stream
                Program.streamStart(parameters.number,"heavy");
                //return not necessary
                return "";
            };


            //Tune channel status
            Get["/channels/{number}/status"] = parameters =>
            {
                Console.WriteLine("Get /channels/" + parameters.number + "/status");

                //get channel status
                HDHomerun.cChannelStatus rtn = Program.streamStatus(parameters.number);
                //return in JSON format
                return Response.AsJson(rtn);
            };
            

            //DeTune from channel (Post)
            Post["/channels/{number}/stop"] = parameters =>
            {
                Console.WriteLine("Post /channels/" + parameters.number + "/stop");

                //stop stream
                 Program.streamStop(parameters.number);
                 //return not necessary
                 return "";
            };


            //route to stream location
            Get["/channels/{number}"] = parameters =>
            {
                Console.WriteLine("Get /channels/" + parameters.number);

                String rtn = Program.streamPath + parameters.number;
                return Response.AsRedirect(rtn);
            };

        }
    }
}
