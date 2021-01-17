using System;
using System.IO;
using System.IO.Compression;
using Npgsql;
using SearchAThing;

namespace skeleton_netcore_ef_code_first
{

    public static class MigrationsTools
    {

        public static void CheckMigrationsToolCmdline(string[] args)
        {                        
            if (args.Length > 0)
            {
                #region --backup-migrations
                if (args[0] == "--backup-migrations")
                {
                    if (args.Length != 2)
                    {
                        Console.WriteLine($"args: --backup-migrations <migrations_folder_path>");
                        Environment.Exit(1);
                    }

                    var migrations_folder = args[1];
                    if (!Directory.Exists(migrations_folder))
                    {
                        Console.WriteLine($"migrations folder [{migrations_folder}] doesn't exists");
                        Environment.Exit(2);
                    }

                    Console.WriteLine($"---> backup migrations from [{migrations_folder}] to db");

                    var g0 = Global.Instance;
                    var connString = g0.ConnectionString;

                    Console.WriteLine($"  - zipping");

                    var migrations_zip = g0.MigrationsBackupPathfilename;
                    if (File.Exists(migrations_zip)) File.Delete(migrations_zip);
                    ZipFile.CreateFromDirectory(migrations_folder, migrations_zip);

                    Console.WriteLine($"  - copying to db");

                    using (var conn = new NpgsqlConnection(connString))
                    {
                        conn.Open();
                        var zip_bytes = File.ReadAllBytes(migrations_zip);

                        using (var cmd = new NpgsqlCommand(@"
                            delete from migrations_backup;
                            insert into migrations_backup (migration_id, migrations_folder) values (1, @zip)"))
                        {
                            var param = cmd.CreateParameter();
                            param.ParameterName = "@zip";
                            param.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bytea;
                            param.Value = zip_bytes;
                            cmd.Parameters.Add(param);
                            cmd.Connection = conn;                            
                            cmd.ExecuteNonQuery();
                        }

                        conn.Close();
                    }

                    Environment.Exit(0);
                }
                #endregion

                #region --restore-migrations
                if (args[0] == "--restore-migrations")
                {
                    if (args.Length != 2)
                    {
                        Console.WriteLine($"args: --restore-migrations <migrations_folder_path>");
                        Environment.Exit(1);
                    }

                    var migrations_folder = args[1];
                    if (Directory.Exists(migrations_folder))
                    {
                        Directory.Delete(migrations_folder, true);
                    }

                    Console.WriteLine($"---> restore migrations from db to [{migrations_folder}]");

                    var g0 = Global.Instance;
                    var connString = g0.ConnectionString;

                    var migrations_zipfile = g0.MigrationsBackupPathfilename;
                    if (File.Exists(migrations_zipfile)) File.Delete(migrations_zipfile);

                    Console.WriteLine($"  - copying from db");

                    using (var conn = new NpgsqlConnection(connString))
                    {
                        conn.Open();
                        byte[] zip_bytes = null;

                        using (var cmd = new NpgsqlCommand(@"
                            select migrations_folder from migrations_backup"))
                        {                            
                            cmd.Connection = conn;
                            var rdr = cmd.ExecuteReader();
                            if (rdr.Read())
                            {
                                zip_bytes = (byte[])rdr[0];
                            }
                            rdr.Close();

                            if (zip_bytes != null)
                            {
                                Console.WriteLine($"  - found {((long)zip_bytes.Length).HumanReadable()} size migrations zip");
                                File.WriteAllBytes(migrations_zipfile, zip_bytes);
                                Console.WriteLine($"  - extracting to [{migrations_folder}]");
                                ZipFile.ExtractToDirectory(migrations_zipfile, migrations_folder);
                            }                            
                        }

                        conn.Close();
                    }                    
                    Environment.Exit(0);
                }
                #endregion
            }

        }

    }

}