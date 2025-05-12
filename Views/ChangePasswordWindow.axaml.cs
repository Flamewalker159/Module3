using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Module3.Data;
using Module3.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Module3.Views;

public partial class ChangePasswordWindow : Window
{
    private readonly User _user;

    public ChangePasswordWindow(User user)
    {
        InitializeComponent();
        _user = user;
    }

    private async void ChangePassword_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(OldPasswordTextBox.Text) || string.IsNullOrWhiteSpace(NewPasswordTextBox.Text)
                                                                   || string.IsNullOrWhiteSpace(RepeatPasswordTextBox
                                                                       .Text))
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Заполните все поля", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Warning).ShowAsync();
                return;
            }

            if (OldPasswordTextBox.Text != _user.Password)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Не верный старый пароль", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
                return;
            }

            if (NewPasswordTextBox.Text != RepeatPasswordTextBox.Text)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Пароли не совпадают", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Warning).ShowAsync();
                return;
            }

            _user.Password = NewPasswordTextBox.Text.Trim();
            _user.IsFirstLogin = false;
            await using var db = new AppDbContext();
            db.Users.Update(_user);
            await db.SaveChangesAsync();

            await MessageBoxManager.GetMessageBoxStandard("Успех", "Пароль успешно изменен", ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Success).ShowAsync();

            Close();
        }
        catch (Exception exception)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", exception.Message, ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
        }
    }
}