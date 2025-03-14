﻿using Microsoft.Data.SqlClient;
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
        try
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@Name", name)
            };
            var dataTable = _dataLink.ExecuteReader("GetCategoryByName", parameters);

            if (dataTable.Rows.Count == 0)
            {
                throw new Exception($"Category '{name}' not found.");
            }

            DataRow row = dataTable.Rows[0];
            return new Category(
                Convert.ToInt32(row["Id"]),
                row["Name"] != DBNull.Value ? row["Name"].ToString() : ""
            );
        }
        catch (Exception ex)
        {
            throw new Exception($"Error fetching category '{name}': {ex.Message}");
        }
    }


}