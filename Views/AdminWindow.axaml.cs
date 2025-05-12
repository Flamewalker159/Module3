using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using Module3.Data;
using Module3.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Module3.Views;

public partial class AdminWindow : Window
{
    public AdminWindow()
    {
        InitializeComponent();
        LoadUsers();
    }

    private async Task LoadUsers()
    {
        await using var db = new AppDbContext();
        UsersListBox.ItemsSource = await db.Users.ToListAsync();
    }

    private async void AddUser_Click(object? sender, RoutedEventArgs e)
    {
        var window = new UserEditWindow(null); // null -> режим добавления
        await window.ShowDialog(this);
        await LoadUsers();
    }

    private async void EditUser_Click(object? sender, RoutedEventArgs e)
    {
        if (UsersListBox.SelectedItem is User selectedUser)
        {
            var window = new UserEditWindow(selectedUser);
            await window.ShowDialog(this);
            await LoadUsers();
        }
    }

    private async void UnblockUser_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (UsersListBox.SelectedItem is User selectedUser)
            {
                selectedUser.IsBlocked = false;
                selectedUser.CounterFailed = 0;

                await using var db = new AppDbContext();
                db.Users.Update(selectedUser);
                await db.SaveChangesAsync();

                await MessageBoxManager.GetMessageBoxStandard(
                    "Успех", "Блокировка снята", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Success).ShowAsync();

                await LoadUsers();
            }
            else
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка",
                    "Выберите пользователя", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Warning).ShowAsync();
            }
        }
        catch (Exception exception)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка",
                exception.Message, ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
        }
    }
}