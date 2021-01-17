using System;
using System.IO;
using Newtonsoft.Json;

namespace skeleton_netcore_ef_code_first
{

    public class Config
    {         

        public string DBHostname { get; set; }
        public int DBPort { get; set; }
        public string DBName { get; set; }
        public string DBUsername { get; set; }
        public string DBPassword { get; set; }

        [JsonIgnore]
        public string ConnectionString
        {
            get
            {
                return $"Server={DBHostname};Database={DBName};Username={DBUsername};Port={DBPort};Password={DBPassword}";
            }
        }

        public void Save(Global global)
        {
            try
            {
                if (File.Exists(global.AppConfigPathfilename))
                    File.Copy(global.AppConfigPathfilename, global.AppConfigPathfilenameBackup, true);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"unable to backup config file [{global.AppConfigPathfilename}] to [{global.AppConfigPathfilenameBackup}] : {ex.Message}");
            }
            File.WriteAllText(global.AppConfigPathfilename, JsonConvert.SerializeObject(this, Formatting.None));
            // save with mode 600
            LinuxHelper.SetFilePermission(global.AppConfigPathfilename, 384);
        }

    }

}