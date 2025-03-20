using Spectre.Console;

namespace Lib
{
    /// <summary>
    /// Класс для контекстного перевода с базовым уровнем обработки неоднозначных слов.
    /// При наличии нескольких вариантов перевода для слова предлагает пользователю выбрать нужный.
    /// </summary>
    public class ContextTranslator
    {
        private readonly Database _database;

        /// <summary>
        /// Конструктор, принимающий экземпляр базы данных.
        /// </summary>
        /// <param name="database">Экземпляр базы данных, содержащий словари.</param>
        public ContextTranslator(Database database)
        {
            _database = database;
        }

        /// <summary>
        /// Переводит фразу с учетом контекста.
        /// Если для слова имеется несколько вариантов перевода, пользователь выбирает подходящий.
        /// </summary>
        /// <param name="phrase">Исходная фраза для перевода.</param>
        /// <returns>Переведённая фраза.</returns>
        public string TranslateWithContext(string phrase)
        {
            string trimmedPhrase = phrase.Trim().ToLower();
            string[] words = trimmedPhrase.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> resultWords = new List<string>();

            Wordbook wordbook = _database.GetCurrent();
            if (!words.Any(wordbook.IsIn))
            {
                ConsoleManager.WriteColor("В фразе нет знакомых слов", "yellow");
                return "";
            }

            foreach (string word in words)
            {
                if (wordbook.IsIn(word))
                {
                    List<string> translations = wordbook.MultiplyTranslate(word);
                    string chosenTranslation = string.Empty;
                    switch (translations.Count)
                    {
                        case 1:
                            chosenTranslation = translations[0];
                            break;
                        case > 1:
                            {
                                List<string> options = new List<string>();
                                foreach (string translation in translations)
                                {
                                    options.Add(translation);
                                }

                                chosenTranslation = AnsiConsole.Prompt(
                                    new SelectionPrompt<string>()
                                        .Title($"Выберите перевод для слова \"[yellow]{word}[/]\":")
                                        .AddChoices(options));
                                break;
                            }
                    }

                    resultWords.Add(chosenTranslation);
                }
                else
                {
                    resultWords.Add("|не найдено|");
                }
            }

            string finalTranslation = string.Join(" ", resultWords);
            return finalTranslation;
        }
    }
}