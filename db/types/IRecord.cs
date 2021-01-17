using System;
using System.ComponentModel.DataAnnotations;
using SearchAThing;

namespace skeleton_netcore_ef_code_first
{
    
    public interface IRecordBase
    {
        int id { get; set; }

        DateTime? update_timestamp { get; set; }
    }
    
    public abstract class IRecord : IRecordBase
    {
        [Key]
        public int id { get; set; }

        DateTime? _update_timestamp;
        /// <summary>
        /// user timestamp ( serveR:UTC js:LOCAL )
        /// </summary>
        public DateTime? update_timestamp
        {
            get
            {
                return _update_timestamp != null ? _update_timestamp.Value.UnspecifiedAsUTCDateTime() : new DateTime?();
            }
            set
            {
                _update_timestamp = value;
            }
        }
    }

}