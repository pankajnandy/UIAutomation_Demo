using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specflow_UIAutomation_Demo.Config
{
    public class AppConfig
    {
        // Gets or sets the environment the application is running in (ex. QA, UAT, Prod)
        public string Environment { get; set; }
        // Gets or sets the browser to be used for automation (ex. Chrome, Firefox
        public string Browser { get; set; }
    }
}
