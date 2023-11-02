namespace skeleton_netcore_ef_code_first;

public class PostTag
{

    public long Id { get; set; }

    public required Post Post { get; set; }

    public required Tag Tag { get; set; }

}