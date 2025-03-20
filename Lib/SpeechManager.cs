using Microsoft.CognitiveServices.Speech;
using Spectre.Console;

namespace Lib
{
    /// <summary>
    /// Класс, реализующий голосовой ввод и вывод.
    /// Для обеих задач используется Microsoft Cognitive Services Speech,
    /// что обеспечивает одинаковую работу на Mac и Windows.
    /// </summary>
    public class SpeechManager
    {
        private readonly string _subscriptionKey;
        private readonly string _serviceRegion;


        /// <summary>
        /// Точка входа в распознавание и вывод текста
        /// </summary>
        /// <param name="database">Экземпляр базы данных</param>
        public static async Task Run(Database database)
        {
            string subscriptionKey;
            string region;
            try
            {
                subscriptionKey = File.ReadAllText("../../../../Lib/API_TOKENS/speech_key.txt").Trim();
                region = File.ReadAllText("../../../../Lib/API_TOKENS/speech_region.txt").Trim();
            }
            catch (FileNotFoundException fnfEx)
            {
                ConsoleManager.WriteWarn($"Файл не найден: {fnfEx.Message}");
                return;
            }
            catch (DirectoryNotFoundException)
            {
                ConsoleManager.WriteWarn("Сначала добавьте токен в папку [bold]API_TOKENS[/]");
                return;
            }
            catch (Exception ex)
            {
                ConsoleManager.WriteWarn($"Ошибка при чтении файлов с ключами: {ex.GetType()}");
                return;
            }

            try
            {
                SpeechManager speechManager = new SpeechManager(subscriptionKey, region);
                AnsiConsole.MarkupLine("[green]Начинается распознавание речи. Говорите...[/]");
                string recognizedText = await speechManager.RecognizeSpeechAsync();
                if (string.IsNullOrWhiteSpace(recognizedText))
                {
                    ConsoleManager.WriteWarn("Распознавание речи не удалось.");
                }
                else
                {
                    ConsoleManager.WriteMessage($"Распознано: {recognizedText}");
                    await speechManager.SpeakAsync(recognizedText);
                }
            }
            catch (Exception)
            {
                ConsoleManager.WriteWarn("Ошибка при работке с распознаванием речи. Проверьте токен");
                Console.ReadKey();
            }
        }


        /// <summary>
        /// Конструктор, принимающий ключ подписки и регион для Microsoft CognitiveServices Speech.
        /// </summary>
        /// <param name="subscriptionKey">Ключ подписки для службы распознавания речи.</param>
        /// <param name="serviceRegion">Регион службы (например, "west's").</param>
        private SpeechManager(string subscriptionKey, string serviceRegion)
        {
            _subscriptionKey = subscriptionKey;
            _serviceRegion = serviceRegion;
        }

        /// <summary>
        /// Асинхронно распознает речь с микрофона и возвращает распознанный текст.
        /// </summary>
        /// <returns>Распознанный текст или пустую строку в случае ошибки.</returns>
        private async Task<string> RecognizeSpeechAsync()
        {
            SpeechConfig config = SpeechConfig.FromSubscription(_subscriptionKey, _serviceRegion);
            // Установка языка распознавания
            config.SpeechRecognitionLanguage = "en-US";

            using SpeechRecognizer recognizer = new SpeechRecognizer(config);
            SpeechRecognitionResult result = await recognizer.RecognizeOnceAsync();
            switch (result.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    return result.Text;
                case ResultReason.NoMatch:
                    Console.WriteLine("Речь не распознана.");
                    break;
                case ResultReason.Canceled:
                    CancellationDetails cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"Распознавание отменено: {cancellation.Reason}");
                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"Ошибка: {cancellation.ErrorDetails}");
                    }

                    break;
            }

            return string.Empty;
        }

        /// <summary>
        /// Асинхронно озвучивает переданный текст с использованием синтеза речи Microsoft Cognitive Services Speech.
        /// </summary>
        /// <param name="text">Текст для озвучивания.</param>
        private async Task SpeakAsync(string text)
        {
            SpeechConfig config = SpeechConfig.FromSubscription(_subscriptionKey, _serviceRegion);
            using SpeechSynthesizer synthesizer = new(config);
            SpeechSynthesisResult result = await synthesizer.SpeakTextAsync(text);
            if (result.Reason != ResultReason.SynthesizingAudioCompleted)
            {
                Console.WriteLine($"Ошибка синтеза речи: {result.Reason}");
            }
        }
    }
}