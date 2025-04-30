using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Module3.Data;
using Module3.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Module3;

public partial class AdminWindow : Window
{
    public AdminWindow()
    {
        InitializeComponent();
        LoadUsers();
    }
    
    private async void LoadUsers()
    {
        await using var db = new AppDbContext();
        var users = await db.users.ToListAsync();
        UsersDataGrid.ItemsSource = users;
    }

    private async void AddUser_Click(object? sender, RoutedEventArgs e)
    {
        var window = new UserEditWindow(null); // null → режим добавления
        await window.ShowDialog(this);
        LoadUsers();
    }

    private async void EditUser_Click(object? sender, RoutedEventArgs e)
    {
        if (UsersDataGrid.SelectedItem is User selectedUser)
        {
            var window = new UserEditWindow(selectedUser);
            await window.ShowDialog(this);
            LoadUsers();
        }
    }

    private async void UnblockUser_Click(object? sender, RoutedEventArgs e)
    {
        if (UsersDataGrid.SelectedItem is User selectedUser)
        {
            await using var db = new AppDbContext();
            var user = await db.users.FindAsync(selectedUser.id);
            if (user is not null)
            {
                user.isblocked = false;
                user.failedattempts = 0;
                await db.SaveChangesAsync();
                await MessageBoxManager.GetMessageBoxStandard(
                    "Успех", "Блокировка снята", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Success).ShowAsync();
                LoadUsers();
            }
        }
    }
}