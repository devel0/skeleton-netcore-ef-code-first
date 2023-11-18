namespace skeleton_netcore_ef_code_first;

public class TableE_Many
{
    [Key]
    public int Id { get; set; }    
    public List<TableF_Many>? FObjects { get; set; } = new(); // many
    public string? Data { get; set; }
}

public class TableF_Many
{
    [Key]
    public int Id { get; set; }
    public List<TableE_Many> EObjects { get; set; } = new(); // many
    public string? Data { get; set; }
}
