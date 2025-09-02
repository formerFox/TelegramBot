using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelegramBot.Services
{
    /// Класс для обработки текстовых данных
    /// Выполняет две основные функции: подсчет суммы чисел и анализ текста
    public class TextProcessor
    {

        /// Вычисляет сумму всех чисел в тексте
        /// Ищет числа с помощью регулярного выражения и суммирует их
        public int CalculateSum(string text)
        {
            // Регулярное выражение для поиска чисел (включая отрицательные и десятичные)
            var matches = Regex.Matches(text, @"-?\d+\.?\d*");

            int sum = 0;
            foreach (Match match in matches)
            {
                // Парсим каждое найденное число и добавляем к сумме
                if (double.TryParse(match.Value, out double number))
                {
                    sum += (int)number;
                }
            }
            return sum;
        }

        /// Анализирует текст и возвращает статистику
        /// Подсчитывает слова, предложения и общее количество символов
        public (int words, int sentences, int characters) AnalyzeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return (0, 0, 0);

            // Подсчет слов: разделяем текст по пробелам и удаляем пустые элементы
            int words = text.Split(new[] { ' ', '\t', '\n', '\r' },
                                StringSplitOptions.RemoveEmptyEntries).Length;

            // Подсчет предложений: ищем точки, восклицательные и вопросительные знаки
            int sentences = Regex.Matches(text, @"[.!?]+").Count;

            // Подсчет символов: просто длина строки
            int characters = text.Length;

            return (words, sentences, characters);
        }
    }
}
