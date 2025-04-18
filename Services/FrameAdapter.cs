using System;
using Microsoft.UI.Xaml.Controls;
using StockApp.Services;

public class FrameAdapter : INavigationFrame
{
    private readonly Frame _frame;

    public FrameAdapter(Frame frame)
    {
        _frame = frame;
    }

    public bool Navigate(Type pageType, object parameter) => _frame.Navigate(pageType, parameter);

    public void GoBack() => _frame.GoBack();

    public bool CanGoBack => _frame.CanGoBack;
}
