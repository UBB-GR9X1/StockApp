namespace StockApp.Views.Components
{
    using Microsoft.UI.Xaml.Controls;
    using Src.Model;

    public sealed partial class MessageHistoryComponent : Page
    {
        public Message Message;

        public MessageHistoryComponent()
        {
            this.InitializeComponent();
        }

        public void SetMessageData(Message givenMessage)
        {
            this.Message = givenMessage;
            this.MessageTypeTextBlock.Text = $"Type: {this.Message.Type}";
            this.MessageTextBlock.Text = $"{this.Message.MessageText}";
        }
    }
}
