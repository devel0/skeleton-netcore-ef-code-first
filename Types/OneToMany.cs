namespace skeleton_netcore_ef_code_first;

public class TableC_Many
{
    [Key]
    public int Id { get; set; }    
    public TableD_One? DObject { get; set; } // one
    public string? Data { get; set; }
}

public class TableD_One
{
    [Key]
    public int Id { get; set; }
    public ICollection<TableC_Many> CObjects { get; set; } // many
    public string? Data { get; set; }
}
