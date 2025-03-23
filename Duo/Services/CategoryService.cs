using System;
using System.Collections.Generic;
using Duo.Models;
using Duo.Repositories;

namespace Duo.Services
{
    public class CategoryService
    {
        private readonly CategoryRepository _categoryRepository;

        public CategoryService(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public List<Category> GetAllCategories()
        {
            try
            {
                return _categoryRepository.GetCategories();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching categories: {ex.Message}");
                return new List<Category>(); // Return empty list if error occurs
            }
        }

        public Category GetCategoryByName(string name)
        {
            try
            {
                return _categoryRepository.GetCategoryByName(name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
