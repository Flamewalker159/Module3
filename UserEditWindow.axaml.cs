using System;
using System.Linq;
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

public partial class UserEditWindow : Window
{
    private readonly User? _editUser;

    public UserEditWindow(User? user)
    {
        InitializeComponent();
        _editUser = user;
        
        if (_editUser != null)
        {
            LoginTextBox.Text = _editUser.login;
            PasswordTextBox.Text = _editUser.password;
        }
    }
    
    private async void Save_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(LoginTextBox.Text) || string.IsNullOrWhiteSpace(PasswordTextBox.Text))
            {
                await MessageBoxManager.GetMessageBoxStandard(
                    "Ошибка", "Заполните все поля", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Warning).ShowAsync();
                return;
            }
        
            var login = LoginTextBox.Text.Trim();
            var password = PasswordTextBox.Text.Trim();

            await using var db = new AppDbContext();

            if (_editUser == null) // Добавление
            {
                bool exists = await db.users.AnyAsync(u => u.login == login);
                if (exists)
                {
                    await MessageBoxManager.GetMessageBoxStandard(
                        "Ошибка", "Пользователь с таким логином уже существует", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
                    return;
                }
                
                var newUser = new User
                {
                    login = login,
                    password = password,
                    role = (await db.roles.FirstOrDefaultAsync(r => r.name == "Пользователь"))!
                };
                db.users.Add(newUser);
            }
            else // Редактирование
            {
                var user = await db.users.FindAsync(_editUser.id);
                if (user != null)
                {
                    user.login = login;
                    user.password = password;
                }
            }

            await db.SaveChangesAsync();
            Close();
        }
        catch (Exception exception)
        {
            await MessageBoxManager.GetMessageBoxStandard(
                "Ошибка", exception.Message, ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
        }
    }
}