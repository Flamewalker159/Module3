using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using Module3.Data;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Module3;

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
            var user = await db.users.Include(user => user.role).FirstOrDefaultAsync(u => u.login == login);

            if (user == null)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка",
                    "Вы ввели неверный логин или пароль. Пожалуйста проверьте ещё раз введенные данные", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
                return;
            }
            
            if (user.password != password)
            {
                user.failedattempts++;

                if (user.failedattempts >= 3)
                {
                    user.isblocked = true;
                    await db.SaveChangesAsync();
                    await MessageBoxManager.GetMessageBoxStandard("Ошибка",
                        "Вы заблокированы из-за неверного пароля", ButtonEnum.Ok,
                        MsBox.Avalonia.Enums.Icon.Forbidden).ShowAsync();
                    return;
                }

                await db.SaveChangesAsync();
                await MessageBoxManager.GetMessageBoxStandard("Ошибка",
                    $"Неверный пароль. Попыток осталось: {3 - user.failedattempts}", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Forbidden).ShowAsync();
                return;
            }
            
            if (user.isblocked)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка",
                    "Вы заблокированы. Обратитесь к администратору.", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Forbidden).ShowAsync();
                return;
            }

            if (user.lastlogin.HasValue && (DateTime.Now - user.lastlogin!.Value).Days > 30)
            {
                user.isblocked = true;
                await db.SaveChangesAsync();
                await MessageBoxManager.GetMessageBoxStandard("Ошибка",
                    "Вы заблокированы из-за длительного отсутствия. Обратитесь к администратору.", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Forbidden).ShowAsync();
                return;
            }

            user.failedattempts = 0;
            user.lastlogin = DateTime.Now.ToUniversalTime();
            await db.SaveChangesAsync();

            await MessageBoxManager.GetMessageBoxStandard("Успех", "Вы успешно авторизовались", ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Success).ShowAsync();
            
            switch (user.role.name)
            {
                case "Администратор":
                    await new AdminWindow().ShowDialog(this);
                    break;
                case "Пользователь":
                    await new UserWindow(user).ShowDialog(this);
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