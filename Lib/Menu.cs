using Spectre.Console;

namespace Lib
{
    public static class Menu
    {
        /// <summary>
        /// Редактирование словаря: добавление, изменение или удаление перевода.
        /// </summary>
        /// <param name="database">Экземпляр базы данных.</param>
        public static void EditDatabase(Database database)
        {
            string option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[#90EE90]Выберете желаемое действие:[/]")
                    .AddChoices("Добавить перевод",
                        "Редактировать перевод",
                        "Удалить перевод",
                        "Отмена"));

            switch (option)
            {
                case "Добавить перевод":
                    string wordToAdd = AnsiConsole.Ask<string>("[green]Введите слово для перевода[/]");
                    string translationToAdd = AnsiConsole.Ask<string>("[green]Введите перевод[/]");
                    database.AddOrUpdateWord(database.CurrentPair, wordToAdd, translationToAdd);
                    break;
                case "Редактировать перевод":
                    string? wordToEdit = MenuManager.FindWord(database);
                    if (wordToEdit != null)
                    {
                        string translationToEdit = AnsiConsole.Ask<string>("[green]Введите новый перевод[/]");
                        database.AddOrUpdateWord(database.CurrentPair, wordToEdit, translationToEdit);
                    }
                    else
                    {
                        ConsoleManager.WriteWarn("Слово не найдено для редактирования.");
                    }

                    break;
                case "Удалить перевод":
                    string? wordToDelete = MenuManager.FindWord(database);
                    if (wordToDelete == null)
                    {
                        ConsoleManager.WriteWarn("Операция не выполнена: слово не найдено.");
                    }
                    else
                    {
                        database.RemoveWord(database.CurrentPair, wordToDelete);
                    }

                    break;
                default:
                    ConsoleManager.WriteMessage("Операция отменена.");
                    break;
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Отображение главного меню.
        /// </summary>
        /// <param name="database">Экземпляр базы данных.</param>
        public static void Display(Database database)
        {
            Lib.Display.ShowMainMenu(database);
        }

        /// <summary>
        /// Дополнительные задачи: контекстный перевод и обучение словаря.
        /// </summary>
        /// <param name="database">Экземпляр базы данных.</param>
        public static void AdditionalTask(Database database)
        {
            string option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Выберите дополнительную задачу[/]")
                    .AddChoices(
                        "Контекстный перевод",
                        "Обучение словаря",
                        "Отмена"));

            switch (option)
            {
                case "Контекстный перевод":
                    ContextTranslation(database);
                    break;
                case "Обучение словаря":
                    TrainDictionary(database);
                    break;
                default:
                    ConsoleManager.WriteMessage("Операция отменена.");
                    break;
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Выбор языковой пары для работы со словарем.
        /// </summary>
        /// <param name="database">Экземпляр базы данных.</param>
        public static void ChooseLanguage(Database database)
        {
            ConsoleManager.WriteMessage($"Сейчас установлена пара [red]{database.CurrentPair}[/]");
            string lang = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Выберите языковую пару[/]")
                    .AddChoices(database.GetAllPairs()));
            database.ChangePair(lang);
            ConsoleManager.WriteMessage($"Языковая пара успешно изменена на {lang}");
            Console.ReadKey();
        }

        /// <summary>
        /// Голосовой ввод и вывод перевода с использованием SpeechManager.
        /// </summary>
        /// <param name="database">Экземпляр базы данных.</param>
        public static async Task Sound(Database database)
        {
            await SpeechManager.Run(database);
            Console.ReadKey();
        }

        /// <summary>
        /// Продвинутый перевод фраз с использованием AdvancedTranslator.
        /// </summary>
        /// <param name="database">Экземпляр базы данных.</param>
        public static async Task AdvancedTranslation(Database database)
        {
            AdvancedTranslator translator = new(database);
            string inputPhrase = AnsiConsole.Ask<string>("[green]Введите фразу для перевода:[/]");
            string result = translator.Translate(inputPhrase);
            ConsoleManager.WriteColor(result, "white");
            Console.ReadKey();
            await Task.CompletedTask;
        }

        /// <summary>
        /// Контекстный перевод фразы с выбором перевода для неоднозначных слов.
        /// </summary>
        /// <param name="database">Экземпляр базы данных.</param>
        private static void ContextTranslation(Database database)
        {
            string phrase = AnsiConsole.Ask<string>("[green]Введите фразу для контекстного перевода:[/]");
            ContextTranslator translator = new ContextTranslator(database);
            string translatedPhrase = translator.TranslateWithContext(phrase);
            ConsoleManager.WriteColor(translatedPhrase, "white");
        }

        /// <summary>
        /// Обучение словаря через интерактивное подтверждение перевода.
        /// Если перевод неверный, пользователь может внести корректировку.
        /// </summary>
        /// <param name="database">Экземпляр базы данных.</param>
        private static void TrainDictionary(Database database)
        {
            string word = AnsiConsole.Ask<string>("[green]Введите слово, перевод которого хотите проверить:[/]");
            DictionaryTrainer trainer = new DictionaryTrainer(database);
            trainer.ReviewTranslation(database, word);
        }
    }
}