using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.Conventions;

namespace HDR.NancyFX
{
        public class cCustomBoostrapper : DefaultNancyBootstrapper
        {
            /// <summary>
            /// custom bootstrapper used to map static url to filesystem
            /// </summary>
            /// <param name="conventions"></param>
            protected override void ConfigureConventions(NancyConventions conventions)
            {
                base.ConfigureConventions(conventions);

                //map a static folder for logos and streams
                //filesystem path is realative to location of executable
                //ex: c:\myapp\myapp.exe + "static\" = c:\myapp\static\

                conventions.StaticContentsConventions.Add(
                    StaticContentConventionBuilder.AddDirectory(Program.staticPath, Program.staticlDir)
                );
            }
        }

}
