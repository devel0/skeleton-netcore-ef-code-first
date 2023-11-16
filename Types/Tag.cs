namespace skeleton_netcore_ef_code_first;

public class Tag
{

    public ICollection<Post> Posts { get; set; }

    public long Id { get; set; }

    public required string Value { get; set; }

}