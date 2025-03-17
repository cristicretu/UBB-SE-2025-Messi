using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;

public class HashtagRepository
{
    private readonly DataLink _dataLink;

    public HashtagRepository(DataLink dataLink)
    {
        _dataLink = dataLink;
    }

    public Hashtag GetHashtagById(int id)
    {
        if (id <= 0) throw new Exception("Error - GetHashtagById: Id must be greater than 0");

        DataTable? dataTable = null;

        try
        {
            var sqlParameters = new SqlParameter[]
            {
                        new SqlParameter("@Id", id)
            };

            dataTable = _dataLink.ExecuteReader("ReadHashtagById", sqlParameters);

            if (dataTable.Rows.Count == 0) throw new Exception("Error - GetHashtagById: No records found");

            var tag = dataTable.Rows[0]["Tag"]?.ToString();

            if (tag == null)
            {
                throw new Exception("Error - GetHashtagById: Tag is null");
            }

            Hashtag hashtag = new Hashtag(
                Convert.ToInt32(dataTable.Rows[0]["Id"]),
                tag
            );

            return hashtag;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error - GetHashtagById: {ex.Message}");
        }
        finally
        {
            dataTable?.Dispose();
        }

    }

    public Hashtag GetHashtagByText(string text)
    {
        if(string.IsNullOrWhiteSpace(text)) throw new Exception("Error - GetHashtagByText: Text cannot be null or empty");

        DataTable? dataTable = null;

        try
        {
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@text", text)
            };

            dataTable = _dataLink.ExecuteReader("GetHashtagByText", sqlParameters);

            if (dataTable.Rows.Count == 0) throw new Exception("Error - GetHashtagByText: No records found");

            Hashtag hashtag = new Hashtag(
                Convert.ToInt32(dataTable.Rows[0]["Id"]),
                text
            );

            return hashtag;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error - GetHashtagByText: {ex.Message}");
        }
        finally
        {
            dataTable?.Dispose();
        }

    }

    public Hashtag CreateHashtag(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) throw new Exception("Error - CreateHashtag: Text cannot be null or empty");

        try
        {
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Tag", text)
            };
            var result = _dataLink.ExecuteScalar<int>("CreateHashtag", sqlParameters);

            if (result == 0) throw new Exception("Error - CreateHashtag: Hashtag could not be created!");

            Hashtag hashtag = new Hashtag(
                result,
                text
            );

            return hashtag;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error - CreateHashtag: {ex.Message}");
        }
    }

    public bool DeleteHashtag(int id)
    {
        if (id <= 0) throw new Exception("Error - DeleteHashtag: Id must be greater than 0");

        try
        {
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Id", id)
            };
            var result = _dataLink.ExecuteNonQuery("DeleteHashtag", sqlParameters);

            if (result == 0) throw new Exception("Error - DeleteHashtag: Hashtag could not be deleted!");

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error - DeleteHashtag: {ex.Message}");

        }
    }

    public List<Hashtag> GetHashtagsByPostId(int postId)
    {

    }

    public bool AddHashtagToPost(int postId, int hashtagId)
    {

    }
    public bool RemoveHashtagFromPost(int postId, int hashtagId)
    {

    }
}
