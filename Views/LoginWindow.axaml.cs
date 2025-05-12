using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using Module3.Data;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Module3.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private async void Login_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(LoginTextBox.Text) || string.IsNullOrWhiteSpace(PasswordTextBox.Text))
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Введите логин или пароль", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Warning).ShowAsync();
                return;
            }

            var login = LoginTextBox.Text.Trim();
            var password = PasswordTextBox.Text.Trim();

            await using var db = new AppDbContext();
            var user = await db.Users.FirstOrDefaultAsync(u => u.Login == login);

            if (user == null)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка",
                    "Вы ввели неверный логин или пароль. Пожалуйста проверьте ещё раз введенные данные", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
                return;
            }

            if (user.Password != password)
            {
                user.CounterFailed++;

                if (user.CounterFailed >= 3)
                {
                    user.IsBlocked = true;
                    await db.SaveChangesAsync();
                    await MessageBoxManager.GetMessageBoxStandard("Ошибка",
                        "Вы заблокированы из-за неверного пароля", ButtonEnum.Ok,
                        MsBox.Avalonia.Enums.Icon.Forbidden).ShowAsync();
                    return;
                }

                await db.SaveChangesAsync();
                await MessageBoxManager.GetMessageBoxStandard("Ошибка",
                    $"Неверный пароль. Попыток осталось: {3 - user.CounterFailed}", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Forbidden).ShowAsync();
                return;
            }

            if (user.IsBlocked)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка",
                    "Вы заблокированы. Обратитесь к администратору.", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Forbidden).ShowAsync();
                return;
            }

            if (user.LastLogin.HasValue && (DateTime.Now - user.LastLogin!.Value).Days > 30)
            {
                user.IsBlocked = true;
                await db.SaveChangesAsync();
                await MessageBoxManager.GetMessageBoxStandard("Ошибка",
                    "Вы заблокированы из-за длительного отсутствия. Обратитесь к администратору.", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Forbidden).ShowAsync();
                return;
            }

            user.CounterFailed = 0;
            if (user.LastLogin == null)
                user.IsFirstLogin = true;

            user.LastLogin = DateTime.Now.ToUniversalTime();

            await db.SaveChangesAsync();

            await MessageBoxManager.GetMessageBoxStandard("Успех", "Вы успешно авторизовались", ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Success).ShowAsync();

            switch (user.IsAdmin)
            {
                case true:
                    await new AdminWindow().ShowDialog(this);
                    break;
                case false:
                    if (user.IsFirstLogin)
                        await new ChangePasswordWindow(user).ShowDialog(this);
                    break;
            }
        }
        catch (Exception exception)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", exception.Message, ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
        }
    }
}