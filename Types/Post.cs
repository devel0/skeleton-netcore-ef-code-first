namespace skeleton_netcore_ef_code_first;

public class Post
{

    public long Id { get; set; }

    public required string Title { get; set; }    

    public ICollection<Tag> PostTags { get; set; }
    


}