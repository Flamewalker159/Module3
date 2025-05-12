using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using Module3.Data;
using Module3.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Module3.Views;

public partial class UserEditWindow : Window
{
    private readonly User? _editUser;

    public UserEditWindow(User? user)
    {
        InitializeComponent();
        _editUser = user;

        if (_editUser != null)
        {
            LoginTextBox.Text = _editUser.Login;
            PasswordTextBox.Text = _editUser.Password;
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
                var exists = await db.Users.AnyAsync(u => u.Login == login);
                if (exists)
                {
                    await MessageBoxManager.GetMessageBoxStandard(
                        "Ошибка", "Пользователь с таким логином уже существует", ButtonEnum.Ok,
                        MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
                    return;
                }

                var newUser = new User
                {
                    Login = login,
                    Password = password
                };
                db.Users.Add(newUser);
            }
            else // Редактирование
            {
                var user = await db.Users.FindAsync(_editUser.Id);
                if (user != null)
                {
                    user.Login = login;
                    user.Password = password;
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