using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Nancy.Hosting.Self;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace HDR
{
    class Program
    {
        //HDHomeRunPlus device list
        public static List<HDHomerun.cHDHomeRunPlus> HDHRPs;

        //URI Host
        public static String webHost;

        //hdhomerun_config.exe path                  
        public static String hdhomerun_config;

        //ffmpeg.exe path         
        public static String ffmpeg;

        //reference for channel + PID (ffmpeg)                  
        public static Dictionary<String, Int32> channelNumberPID;

        //static virtual path (used by bootstrapper)
        public static String staticPath = "static";

        //static local directory (used by bootstrapper)
        //filesystem path is realative to location of executable
        //ex: c:\myapp\myapp.exe
        // c:\myapp\  + "static\" = c:\myapp\static\    
        public static String staticlDir = "static";  
         
        //filesystem stream directory
        public static String streamDir;

        //filesystem logos directory
        public static String logoDir;

        //web server stream path
        public static String streamPath;

        //web server logo path               
        public static String logolPath;

        public static String appTitle = "HDR version 0.1";

        static void Main(string[] args)
        {
            try
            {

                //check for arguments
                if (args.Length != 3)  { Console.WriteLine("Parameters Missing: hdhomerun_config ffmpeg Port"); Console.ReadLine(); return;}

                //set variables
                hdhomerun_config = args[0];                  
                ffmpeg = args[1];                            
                
                webHost = "http://" + getLocalIP() + ":" + Convert.ToInt32(args[2]);
                
                streamDir = getAppPath() + "\\" + staticlDir + "\\streams\\";
                logoDir = getAppPath() + "\\" + staticlDir + "\\logos\\";

                streamPath = webHost + "/" + staticPath + "/streams/";
                logolPath = webHost + "/" + staticPath + "/logos/";


                //init
                channelNumberPID = new Dictionary<String, Int32>();

                //display version
                Console.Title = appTitle;
                Console.WriteLine ("\r\nCurrent logo storage " + logoDir);
                Console.WriteLine ("Current stream storage " + streamDir + "\r\n");

                //Check for hdhomerun_config
                if (!File.Exists(hdhomerun_config)) { Console.WriteLine("Missing hdhomerun_config.exe"); Console.ReadLine(); return; }
                //Check for ffmpeg
                if (!File.Exists(ffmpeg)) { Console.WriteLine("Missing FFMPEG.exe"); Console.ReadLine(); return; }
                //folder check
                if(!Directory.Exists(streamDir)) { Console.WriteLine("Directory Not Found: " + streamDir);}
                if (!Directory.Exists(logoDir)) { Console.WriteLine("Directory Not Found: " + logoDir); }


                //load available HDHomeRunPlus'
                HDHRPs = HDHomerun.cConfig.getDevices(hdhomerun_config);
                if (HDHRPs.Count == 0) { Console.WriteLine("No HDHomerunPlus Found"); Console.ReadLine(); return; } 

                //start NancyFx web server
                using (var host = new NancyHost(new Uri(webHost)))
                {
                    Console.WriteLine("Web server running at: " + webHost);
                    host.Start();
                    Console.ReadLine();
                    host.Stop();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Program.Main, Error: {0}", ex.Message));
                Console.ReadLine();
            }
        }


        /// <summary>
        /// Returns channel status
        /// </summary>
        /// <param name="channel"></param>
        public static HDHomerun.cChannelStatus streamStatus(String channelNumber)
        {
            try
            {
                // Check if channel's m3u8 exists
                Boolean isReady = File.Exists(streamDir + channelNumber + ".m3u8");

                Int32 pid = 0;

                HDHomerun.cChannelStatus RTN = new HDHomerun.cChannelStatus();
                RTN.channel = channelNumber;
                RTN.ready = isReady;
                RTN.pid = pid;

                if (isReady == true)
                {
                    //last created datetime
                    RTN.last_read = File.GetCreationTime(streamDir + channelNumber + ".m3u8").ToLongDateString();
                    channelNumberPID.TryGetValue(channelNumber, out pid);
                    RTN.pid = 0;
                }

                return RTN;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("cStream.getStatus, Error: {0}", ex.Message));
            }
        }

        /// <summary>
        /// stop stream
        /// </summary>
        /// <param name="channel"></param>
        public static void streamStop(String channelNumber)
        {
            try
            {
                //get pid from dictionary
                Int32 pid = 0;
                channelNumberPID.TryGetValue(channelNumber, out pid);

                //Kill process (ffmpeg)
                if (pid !=0 ){ Program.execAppRead(@"C:\windows\system32\Taskkill.exe", " /PID " + pid + " /F"); }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("cStream.stopStream, Error: {0}", ex.Message));
            }
            finally
            {
                //remove from dictionary
                if (channelNumberPID.ContainsKey(channelNumber))
                {
                    channelNumberPID.Remove(channelNumber);
                }

                // Remove .m3u8 and .ts files
                string[] fileList = Directory.GetFiles(streamDir, channelNumber + "*");
                foreach (string file in fileList)
                {
                    System.IO.File.Delete(file);
                }
            }
        }

        /// <summary>
        /// start stream
        /// </summary>
        /// <param name="channelNumber"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        public static Int32 streamStart(String channelNumber, String quality)
        {
            try
            {
                //Check for available tuner
                String ip = HDHomerun.cConfig.getTunersAvailable(HDHRPs, hdhomerun_config);
                if (String.IsNullOrEmpty(ip))
                {
                    Console.WriteLine("No Tuners available for channel stream");
                    return 0;
                }

                //start the encoder (add HLS options - 10 minute cache ~500MB)
                //-hls_time 10 -hls_list_size 6 -hls_wrap 60 -start_number 1 -y
                String iParam = " -i http://" + ip + ":5004/auto/v" + channelNumber + "?transcode=" + quality + " -vcodec copy -hls_wrap 300 " + streamDir + channelNumber + ".m3u8";
                Console.WriteLine(iParam);
                Int32 pid = Program.execApp(ffmpeg, iParam);

                //add PID to dictionary
                channelNumberPID.Add(channelNumber, pid);

                return pid;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("cStream.startStream, Error: {0}", ex.Message));
            }
        }


        /// <summary>
        /// run process, read output, wait for exit
        /// </summary>
        /// <param name="iApp">executable</param>
        /// <param name="iParams">run parameters</param>
        /// <returns></returns>
        public static String execAppRead(String iApp, String iParams)
        {
            String RTN = String.Empty;
            String Err = String.Empty;
            String ConsOut = String.Empty;

            //Create Process Start information
            ProcessStartInfo psi = new ProcessStartInfo(iApp, iParams);
            psi.UseShellExecute = false;
            psi.ErrorDialog = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = false;
            psi.WorkingDirectory = @"c:\";

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(psi))
                {
                    exeProcess.WaitForExit();
                    StreamReader srOut = exeProcess.StandardOutput;
                    StreamReader srErr = exeProcess.StandardError;
                    //save the return
                    RTN = srOut.ReadToEnd() + "\r" + srErr.ReadToEnd();
                    Console.WriteLine(RTN + "\r");
                    srOut.Close();
                    srErr.Close();
                }
                return RTN;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }

        /// <summary>
        /// run process, do not wait for exit
        /// </summary>
        /// <param name="iApp">executable</param>
        /// <param name="iParams">run parameters</param>
        /// <returns></returns>
        public static Int32 execApp(String iApp, String iParams)
        {

            //Create Process Start information
            ProcessStartInfo psi = new ProcessStartInfo(iApp, iParams);
            psi.UseShellExecute = false;
            psi.ErrorDialog = false;
            psi.CreateNoWindow = false;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.WorkingDirectory = @"c:\";

            try
            {
                Process exeProcess = Process.Start(psi);
                return exeProcess.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// get local ip address from system
        /// </summary>
        /// <returns>IPAddress</returns>
        private static IPAddress getLocalIP()
        {
            IPAddress RTN = new IPAddress(0);
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    RTN = ip;
                }
            }
            return RTN;
        }

        /// <summary>
        /// get application running path
        /// </summary>
        /// <returns>String</returns>
        private static String getAppPath()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }


    }
}
