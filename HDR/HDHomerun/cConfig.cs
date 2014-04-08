using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HDR.HDHomerun
{
    class cConfig
    {

        /// <summary>
        /// Finds HDHomeRunsPlus on network
        /// </summary>
        /// <param name="hdhomerun_config">hdhomerun_config executable</param>
        /// <returns></returns>
        public static List<cHDHomeRunPlus> getDevices(String hdhomerun_config)
        {
            try
            {
                String iRtn = Program.execAppRead(hdhomerun_config, "discover");
                if (iRtn.Contains("no devices found")) { return null; }
                iRtn = iRtn.Replace('\r', ' ').Trim();

                List<cHDHomeRunPlus> RTN = new List<cHDHomeRunPlus>();
                String[] readlines;
                readlines = iRtn.Split('\n');
                

                //get device list (HDHOMERUNS)
                foreach (String readline in readlines)
                {
                    String hddev = readline.Split(' ')[2];
                    String ip = readline.Split(' ')[5];

                    //get hdhomerun model
                    String status = Program.execAppRead(hdhomerun_config, " " + hddev + " get /sys/hwmodel");
                    if (status.Contains("HDTC-2US"))
                    {
                        //ADD only HDHomerunPlus
                        RTN.Add(new cHDHomeRunPlus(hddev, ip));
                    }
                    else
                    {
                        Console.WriteLine("getDevices: Non HDTC-2US Found. Ignoring");
                    }
                }
                return RTN;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("cConfig.getDevices, Error: {0}", ex.Message));
            }
        }


        /// <summary>
        /// Check status of tuner (availability)
        /// </summary>
        /// <param name="HDHDRPs">list of HDHomeRunPlus </param>
        /// <param name="hdhomerun_config">hdhomerun_config executable path</param>
        /// <returns></returns>
        public static String getTunersAvailable(List<HDHomerun.cHDHomeRunPlus> HDHDRPs, String hdhomerun_config)
        {
            try
            {
                String rtn = string.Empty;
                //look for available tuner in HDHomeRunPlus'
                foreach (cHDHomeRunPlus HDHDRP in HDHDRPs)
                {
                    //assume 2 tuners because its an HDHomeRunPlus
                    for (int i = 0; i < 1; i++)
                    {
                        String status = Program.execAppRead(hdhomerun_config, " " + HDHDRP.IP + " get /tuner" + i + "/status");
                        if (status.Contains("lock=none"))
                        {
                            rtn = HDHDRP.IP;
                            break;
                        }
                    }
                }
                return rtn;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("cConfig.getTunersAvailable, Error: {0}", ex.Message));
            }
        }
    }
}
