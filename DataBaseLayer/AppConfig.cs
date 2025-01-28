using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DatabaseLayer
{
    public class AppConfig
    {
        public static string ConnectionString
        {
            get
            {
                ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                string path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                configurationBuilder.AddJsonFile(path, false);
                var root = configurationBuilder.Build();
                return root.GetSection("AppConfig").GetSection("ConnectionString").Value;
            }
        }
    }
}
