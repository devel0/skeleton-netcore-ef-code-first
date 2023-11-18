namespace skeleton_netcore_ef_code_first;

public class TableB_One
{
    [Key]
    public int Id { get; set; }

    public virtual TableA_One? AObject { get; set; } // one ( optional )
    public string? Data { get; set; }
}

public class TableA_One
{
    [Key]
    public int Id { get; set; }    
    public int BObjectId { get; set; } // required foreign key
    public TableB_One BObject { get; set; } = null!; // one ( required )
    public string? Data { get; set; }
}