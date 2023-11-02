using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using skeleton_netcore_ef_code_first;

var dbContext = new LocalDbContext(readonlyMode: false);

if (dbContext.Posts.Count() == 0)
{
    var tag_asp_net_core = new Tag { Value = "asp-net-core" };
    var tag_ef_core = new Tag { Value = "ef-core" };
    var tag_code_first = new Tag { Value = "code-first" };

    var post1 = new Post { Title = "post1" };
    var post2 = new Post { Title = "post2" };

    dbContext.Posts.Add(post1);
    dbContext.Posts.Add(post2);

    dbContext.PostTags.Add(new PostTag { Post = post1, Tag = tag_asp_net_core });

    foreach (var tag in new[] { tag_asp_net_core, tag_ef_core, tag_code_first })
        dbContext.PostTags.Add(new PostTag { Post = post2, Tag = tag });
}

dbContext.SaveChanges();

{

    var q = dbContext.Posts
        .Include(post => post.PostTags)
        .ThenInclude(postTag => postTag.Tag)
        .ToList()
        .Select(post => new
        {
            postTitle = post.Title,
            tags = post.PostTags != null ? post.PostTags.Select(postTag => postTag.Tag.Value) : null
        });

    foreach (var x in q)
        Console.WriteLine(JsonSerializer.Serialize(x, new JsonSerializerOptions { WriteIndented = true }));
}