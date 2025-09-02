using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramBot.Services;


namespace TelegramBot.Handlers
{
    /// Класс для обработки входящих сообщений от пользователей
    /// Определяет тип запроса и вызывает соответствующие методы
    public class MessageHandler
    {
        private readonly TextProcessor _textProcessor;

        // Словарь для отслеживания состояния пользователей
        // Ключ - ID пользователя, значение - текущее действие
        private readonly Dictionary<long, string> _userStates = new();

        public MessageHandler(TextProcessor textProcessor)
        {
            _textProcessor = textProcessor;
        }

        /// Основной метод обработки входящих сообщений
        public async Task HandleMessage(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == null) return;

            long userId = message.Chat.Id;
            string messageText = message.Text;

            // Проверяем, есть ли у пользователя активное состояние
            if (_userStates.TryGetValue(userId, out string? currentState))
            {
                // Если состояние есть - обрабатываем ввод как данные для текущего действия
                await ProcessUserInput(botClient, message, currentState);
                _userStates.Remove(userId); // Сбрасываем состояние после обработки
            }
            else
            {
                // Если состояния нет - обрабатываем как команду
                await ProcessCommand(botClient, message);
            }
        }

        /// Обработка команд от пользователя
        private async Task ProcessCommand(ITelegramBotClient botClient, Message message)
        {
            switch (message.Text.ToLower())
            {
                case "/start":
                    await SendWelcomeMessage(botClient, message);
                    break;

                case "анализ текста":
                case "/analyze":
                    _userStates[message.Chat.Id] = "analyze";
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        "Введите текст для анализа (подсчет слов, предложений и символов):");
                    break;

                case "сумма чисел":
                case "/sum":
                    _userStates[message.Chat.Id] = "sum";
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        "Введите текст с числами для подсчета суммы:");
                    break;

                default:
                    await SendWelcomeMessage(botClient, message);
                    break;
            }
        }

        /// Обработка пользовательского ввода (когда ожидаются данные)
        private async Task ProcessUserInput(ITelegramBotClient botClient, Message message, string state)
        {
            switch (state)
            {
                case "analyze":
                    var analysis = _textProcessor.AnalyzeText(message.Text);
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        $"Результаты анализа текста:\n" +
                        $"Слов: {analysis.words}\n" +
                        $"Предложений: {analysis.sentences}\n" +
                        $"Символов: {analysis.characters}");
                    break;

                case "sum":
                    int sum = _textProcessor.CalculateSum(message.Text);
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        $"Сумма чисел в тексте: {sum}");
                    break;
            }

            // После обработки показываем главное меню
            await SendWelcomeMessage(botClient, message);
        }

        /// Отправка приветственного сообщения с клавиатурой
        private async Task SendWelcomeMessage(ITelegramBotClient botClient, Message message)
        {
            // Создаем клавиатуру с кнопками
            var replyKeyboard = new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Анализ текста") },
                new[] { new KeyboardButton("Сумма чисел") }
            })
            {
                ResizeKeyboard = true // Кнопки подстраиваются под размер экрана
            };

            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Выберите действие:",
                replyMarkup: replyKeyboard);
        }
    }
}
