namespace skeleton_netcore_ef_code_first;

public class TableC_One
{
    [Key]
    public int Id { get; set; }
    public ICollection<TableD_Many> DObjects { get; set; } // many D
    public string? Data { get; set; }
}

public class TableD_Many
{
    [Key]
    public int Id { get; set; }    
    public TableC_One? CObject { get; set; } // one C
    public string? Data { get; set; }
}
