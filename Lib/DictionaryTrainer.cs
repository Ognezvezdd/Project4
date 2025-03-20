using Spectre.Console;

namespace Lib
{
    /// <summary>
    /// Класс для обучения словаря.
    /// Позволяет пользователю оценивать переводы и вносить корректировки, чтобы словарь становился лучше.
    /// </summary>
    public class DictionaryTrainer
    {
        private readonly Database _database;

        /// <summary>
        /// Конструктор, принимающий экземпляр базы данных.
        /// </summary>
        /// <param name="database">Экземпляр базы данных, содержащий словари.</param>
        public DictionaryTrainer(Database database)
        {
            _database = database;
        }

        /// <summary>
        /// Проверяет перевод для указанного слова и запрашивает обратную связь у пользователя.
        /// При отрицательной оценке запрашивает правильный перевод и обновляет словарь.
        /// </summary>
        /// <param name="database">Экземпляр базы данных</param>
        /// <param name="word">Слово для проверки перевода.</param>
        public void ReviewTranslation(Database database, string word)
        {
            string languagePair = _database.CurrentPair;
            string currentTranslation = _database.Translate(languagePair, word, "ignored");

            if (string.IsNullOrEmpty(currentTranslation))
            {
                ConsoleManager.WriteWarn(
                    $"Перевод для слова '{word}' не найден. [green]Согласно задаче нужно добавить перевод[/]");
                string translationToAdd = AnsiConsole.Ask<string>("[green]Введите перевод[/]");
                database.AddOrUpdateWord(database.CurrentPair, word, translationToAdd);
                return;
            }

            string feedback = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Перевод слова \"[yellow]{word}[/]\" - \"[green]{currentTranslation}[/]\". Верно ли это?")
                    .AddChoices(
                        "Верно",
                        "Неверно"
                    ));

            if (feedback == "Неверно")
            {
                string correctTranslation = AnsiConsole.Ask<string>("[green]Введите правильный перевод:[/]");
                _database.AddOrUpdateWord(languagePair, word, correctTranslation);
                ConsoleManager.WriteMessage($"Слово '{word}' обновлено с новым переводом: {correctTranslation}");
            }
            else
            {
                ConsoleManager.WriteMessage("Спасибо за подтверждение!");
            }
        }
    }
}