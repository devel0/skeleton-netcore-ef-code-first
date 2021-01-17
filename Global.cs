using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using static System.Environment;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace skeleton_netcore_ef_code_first
{

    public class Global
    {

        #region instance
        static Global instance = null;
        static object lckInstance = new object();
        public static Global Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lckInstance)
                    {
                        if (instance == null)
                        {
                            instance = new Global();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion

        internal static bool MainStarted = false;

        public static readonly string AppName = "skeleton-netcore-ef-code-first";

        /// <summary>
        /// ~/.config/appname
        /// </summary>        
        public string AppFolder
        {
            get
            {
                var pathname = Path.Combine(Path.Combine(Environment.GetFolderPath(SpecialFolder.UserProfile), ".config"), AppName);

                if (!Directory.Exists(pathname)) Directory.CreateDirectory(pathname);

                return pathname;
            }
        }

        public string AppConfigPathfilename
        {
            get { return Path.Combine(AppFolder, "config.json"); }
        }

        public string AppConfigPathfilenameBackup
        {
            get { return Path.Combine(AppFolder, "config.json.bak"); }
        }

        public string MigrationsBackupPathfilename
        {
            get { return Path.Combine(AppFolder, "Migrations.zip"); }
        }

        public Config Config { get; private set; }

        public string ConnectionString => Config.ConnectionString;

        public Global()
        {
            if (!File.Exists(AppConfigPathfilename))
            {
                Config = new Config();
                Config.Save(this);
            }
            else
            {
                // check mode 600
                if (!LinuxHelper.IsFilePermissionSafe(AppConfigPathfilename, Convert.ToInt32("600", 8)))
                {
                    throw new Exception($"invalid file permission [{AppConfigPathfilename}] must set to 600");
                }

                var attrs = File.GetAttributes(AppConfigPathfilename);
                Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(AppConfigPathfilename));
            }

            if (string.IsNullOrEmpty(Config.DBHostname) || Config.DBPassword == "pass")
            {
                Config.DBHostname = "hostname";
                Config.DBPort = 5432;
                Config.DBName = "srvdb";
                Config.DBUsername = "postgres";
                Config.DBPassword = "pass";
                Config.Save(this);

                Task.Run(async () =>
                {
                    var res = await SearchAThing.UtilToolkit.ExecBashRedirect($"ls -la {AppFolder}", CancellationToken.None, false, false);
                    System.Console.WriteLine(res.Output);
                });

                throw new Exception($"please configure [{AppConfigPathfilename}] setting DBHostname, DBPort, DBName, DBUsername, DBPassword (see README.md)");
            }
        }

    }

}
