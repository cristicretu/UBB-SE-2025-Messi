using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Input;
using Windows.Graphics.Display;

public class SearchService
{
    public List<string> Search(string query, IEnumerable<string> strings, double threshold = 0.6)
    {
        if (string.IsNullOrEmpty(query))
            return new List<string>();

        query = query.ToLower();
        var matches = new List<(string Text, double Score)>();

        foreach (var str in strings)
        {
            string strLower = str.ToLower();

            if (strLower.Contains(query))
            {
                matches.Add((str, 0.9));
                continue;
            }

            if (query.Contains(strLower))
            {
                matches.Add((str, 0.8));
                continue;
            }

            if (str.Contains(" "))
            {
                string[] words = strLower.Split(' ');
                foreach (var word in words)
                {
                    if (word.Contains(query) || query.Contains(word))
                    {
                        double wordScore = 0.7;
                        matches.Add((str, wordScore));
                        continue;
                    }
                }
            }

            double levenScore = LevenshteinSimilarity(query, strLower);
            if (levenScore >= threshold)
            {
                matches.Add((str, levenScore));
            }
        }

        return matches
            .GroupBy(m => m.Text)
            .Select(g => g.OrderByDescending(m => m.Score).First())
            .OrderByDescending(m => m.Score)
            .Select(m => m.Text)
            .ToList();
    }

    public double LevenshteinSimilarity(string source, string target)
    {
        int [,] distance = new int[source.Length + 1, target.Length + 1];

        for(int i = 0; i <= source.Length; i++)
            distance[i, 0] = i;
        for(int j = 0; j <= target.Length; j++)
            distance[0, j] = j;

        for(int i = 1; i <= source.Length; i++)
        {
            for(int j = 1; j <= target.Length; j++)
            {
                int cost = (source[i - 1] == target[j - 1]) ? 0 : 1;

                distance[i, j] = Math.Min(
                    Math.Min(
                        distance[i - 1, j] + 1,
                        distance[i, j - 1] + 1),
                    distance[i - 1, j - 1] + cost);
            }
        }

        int maxLength = Math.Max(source.Length, target.Length);
        return maxLength == 0 ? 1.0 : 1.0 - ((double) distance[source.Length, target.Length] / maxLength);
    }
}