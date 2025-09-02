using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramBot.Handlers;

namespace TelegramBot.Services
{
    /// Основной сервис для работы с Telegram Bot API
    /// Запускает получение сообщений и управляет жизненным циклом бота
    public class BotService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly MessageHandler _messageHandler;

        public BotService(string botToken)
        {
            // Создаем клиент для работы с Telegram API
            _botClient = new TelegramBotClient(botToken);

            // Создаем обработчик сообщений
            _messageHandler = new MessageHandler(new TextProcessor());
        }

        /// Запуск бота и начало прослушивания сообщений
        public void Start()
        {
            Console.WriteLine("Запуск бота...");

            // Настройки для получения обновлений
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>(), // Типы получаемых обновлений
                ThrowPendingUpdates = true // Игнорировать старые сообщения при запуске
            };

            // Запускаем получение сообщений
            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions
            );

            Console.WriteLine("Бот запущен и готов к работе!");
        }

        /// Обработка входящих обновлений (сообщений)
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Обрабатываем только текстовые сообщения
            if (update.Message is { } message && message.Text is not null)
            {
                Console.WriteLine($"Получено сообщение: {message.Text}");

                try
                {
                    // Передаем сообщение в обработчик
                    await _messageHandler.HandleMessage(botClient, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка обработки сообщения: {ex.Message}");
                }
            }
        }

        /// Обработка ошибок при получении сообщений
        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Ошибка Telegram API: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}
