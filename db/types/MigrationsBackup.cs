using System.ComponentModel.DataAnnotations.Schema;

namespace skeleton_netcore_ef_code_first
{
    
    [Table("migrations_backup")]
    public class MigrationsBackup : IRecord
    {

        public string migration_id { get; set; }
        public byte[] migrations_folder { get; set; }

    }

}