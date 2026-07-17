using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Honeypot.Models;

namespace Honeypot.ViewModels;

public partial class MainViewModel
{
    /// <summary>
    /// 添加分类
    /// </summary>
    /// <param name="title"></param>
    /// <param name="icon"></param>
    public void CreateCategory(string title, string icon)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader().GetString("UnknownCategoryTitle");
            }

            if (string.IsNullOrWhiteSpace(icon))
            {
                icon = "\uE72E";
            }

            PasswordsDataAccess.AddCategory(title, icon, DateTime.Now.Ticks);

            LoadCategoriesTable();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);

            var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
            ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongAddCategories")}: {ex.Message}");
        }
    }

    /// <summary>
    /// 编辑分类信息
    /// </summary>
    /// <param name="category"></param>
    /// <param name="title"></param>
    /// <param name="icon"></param>
    public void EditCategory(CategoryModel category, string title, string icon)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader().GetString("UnknownCategoryTitle");
            }

            if (string.IsNullOrWhiteSpace(icon))
            {
                icon = "\uE72E";
            }

            PasswordsDataAccess.UpdateCategory(category.Id, title, icon, category.Order);

            LoadPasswordsTable();
            LoadCategoriesTable();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);

            var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
            ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongEditCategories")}: {ex.Message}");
        }
    }

    /// <summary>
    /// 将分类移到最前
    /// </summary>
    /// <param name="category"></param>
    public void MoveCategory(CategoryModel category)
    {
        try
        {
            PasswordsDataAccess.UpdateCategory(category.Id, category.Title, category.Icon, DateTime.Now.Ticks);

            LoadCategoriesTable();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);

            var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
            ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongMoveCategories")}: {ex.Message}");
        }
    }

    /// <summary>
    /// 删除分类
    /// </summary>
    /// <param name="id"></param>
    public void DeleteCategory(int id)
    {
        try
        {
            PasswordsDataAccess.DeleteCategory(id);

            LoadPasswordsTable();
            LoadCategoriesTable();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);

            var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
            ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongDeleteCategories")}: {ex.Message}");
        }
    }
}
