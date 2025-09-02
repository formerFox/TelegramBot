using TelegramBot.Services;

Console.WriteLine("=== Telegram Text Bot ===");

// Токен бота 
string botToken = "8360825158:AAF4xAP9vkADj0YGGnCZptdF0j82-TmuioU";

try
{
    // Создаем и запускаем бота
    var botService = new BotService(botToken);
    botService.Start();

    // Ждем бесконечно, чтобы приложение не закрылось
    Console.WriteLine("Нажмите Ctrl+C для остановки бота");
    await Task.Delay(Timeout.Infinite);
}
catch (Exception ex)
{
    Console.WriteLine($"Критическая ошибка: {ex.Message}");
}