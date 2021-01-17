using System;
using System.ComponentModel.DataAnnotations.Schema;
using SearchAThing;

namespace skeleton_netcore_ef_code_first
{
    
    [Table("sample_table")]
    public class SampleTable : IRecord
    {

        DateTime _create_timestamp;
        /// <summary>
        /// create timestamp ( server:UTC js:LOCAL )
        /// </summary>        
        public DateTime create_timestamp
        {
            get { return _create_timestamp.UnspecifiedAsUTCDateTime(); }
            set { _create_timestamp = value; }
        }

        public int some_data { get; set; }

    }

}