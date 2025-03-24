using System;
using System.Windows.Input;
using Duo.Models;
using Duo.Services;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Duo.Commands;
using System.Collections.Generic;
using System.Diagnostics;

namespace Duo.ViewModels
{
    public class CategoryViewModel : INotifyPropertyChanged
    {
        private readonly CategoryService _categoryService;
        private string _categoryName = string.Empty;
        private List<Category> _categories = new List<Category>();

        public event PropertyChangedEventHandler PropertyChanged;

        public List<Category> Categories
        {
            get => _categories;
            set
            {
                if (_categories != value)
                {
                    _categories = value;
                    OnPropertyChanged();
                }
            }
        }

        public CategoryViewModel(CategoryService categoryService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            LoadCategories();
        }

        public void LoadCategories()
        {
            try
            {
                Categories = _categoryService.GetAllCategories();
            }
            catch (Exception ex)
            {
                Categories = new List<Category>();
            }
        }

        public List<string> GetCategoryNames()
        {
            List<string> categoryNames = new List<string>();
            foreach (var category in Categories)
            {
                categoryNames.Add(category.Name);
            }
            return categoryNames;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
