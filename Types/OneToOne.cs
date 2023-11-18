namespace skeleton_netcore_ef_code_first;

public class TableA_One
{
    [Key]
    public int Id { get; set; }
    /*
    Follow key required to avoid follow compile error
    "The dependent side could not be determined for the one-to-one relationship between 'TableA_One.BObject' and 'TableB_One.AObject'. To identify the dependent side of the relationship, configure the foreign key property. If these navigations should not be part of the same relationship, configure them independently via separate method chains in 'OnModelCreating'. See http://go.microsoft.com/fwlink/?LinkId=724062 for more details."
    */
    public int BObjectId { get; set; } // required foreign key
    public TableB_One BObject { get; set; } = null!; // one ( required )
    public string? Data { get; set; }
}

public class TableB_One
{
    [Key]
    public int Id { get; set; }

    public virtual TableA_One? AObject { get; set; } // one ( optional )
    public string? Data { get; set; }
}
