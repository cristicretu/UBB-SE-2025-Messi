using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

public class CategoryRepository
{
    private readonly DataLink _dataLink;

    public CategoryRepository(DataLink dataLink)
    {
        _dataLink = dataLink;
    }

    public List<Category> GetCategories()
    {
        List<Category> categories = new List<Category>();
        var dataTable = _dataLink.ExecuteReader("GetCategories");

        foreach (DataRow row in dataTable.Rows)  
        {
            categories.Add(new Category(
                Convert.ToInt32(row["Id"]),  
                row["Name"] != DBNull.Value ? row["Name"].ToString() : ""  
            ));
        }

        return categories;
    }

    public Category GetCategoryByName(string name)
    {
        Category category = null;
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Name", name)
        };
        var dataTable = _dataLink.ExecuteReader("GetCategoryByName", parameters);

        DataRow row = dataTable.Rows[0];
        return new Category(
            Convert.ToInt32(row["Id"]),
            row["Name"] != DBNull.Value ? row["Name"].ToString() : ""
        );
    }

}