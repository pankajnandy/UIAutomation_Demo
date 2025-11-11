using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specflow_UIAutomation_Demo.Config
{
    public class EnvironmentInfo
    {// Gets or sets the data for a specific environment configuration.
        public string Name { get; set; }
        public string Url { get; set; }
        public string ConnectionString { get; set; }
        public string ResultUrl { get; set; }
    }

    public class EnvironmentConfig
    {// Represents the configuration for multiple environments.
        public List<EnvironmentInfo> Environments { get; set; }
    }
}
