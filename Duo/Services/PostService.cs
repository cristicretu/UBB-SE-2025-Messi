using System.Collections.Generic;
using Microsoft.UI.Xaml.Input;

public class PostsService
{
    private PostRepository postRepository;
    private SearchService searchService;

    public PostsService(PostRepository postRepository) {
        this.postRepository = postRepository;
    }

    public List<Post> SearchPostsByKeyword(string keyword)
    {   
        if (string.IsNullOrEmpty(keyword))
            return new List<Post>();

        List<string> allTitles = postRepository.GetAllPostTitles();
        List<string> matchingTitles = searchService.Search(keyword, allTitles, 0.6);

        List<Post> results = new List<Post>();
        foreach (string title in matchingTitles)
        {
            List<Post> postsWithTitle = postRepository.GetByTitle(title);
            results.AddRange(postsWithTitle);
        }

        return results;
    }
}