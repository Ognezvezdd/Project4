using Spectre.Console;

namespace Lib
{
    /// <summary>
    /// Отвечает за отображение пользовательского интерфейса и вывод информации на консоль.
    /// </summary>
    public static class Display
    {
        /// <summary>
        /// Отображает главное меню для взаимодействия с пользователем.
        /// </summary>
        /// <param name="database">Объект базы данных для получения информации о словарях и переводах.</param>
        public static void ShowMainMenu(Database database)
        {
            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Выберите действие:[/]")
                    .AddChoices(
                        "Вывести все слова в текущем словаре",
                        "Показать словари",
                        "Перевести слово",
                        "История взаимодействия",
                        "Выход"));

            switch (choice)
            {
                case "Вывести все слова в текущем словаре":
                    ShowCurrentDictionary(database);
                    break;
                case "Показать словари":
                    ShowDictionaries(database);
                    break;
                case "Перевести слово":
                    TranslateWord(database);
                    break;
                case "История взаимодействия":
                    WriteHistory(database);
                    break;
                case "Выход":
                    return;
            }
        }

        /// <summary>
        /// Отображает список доступных словарей.
        /// </summary>
        /// <param name="database">Объект базы данных для получения списка языковых пар.</param>
        private static void ShowDictionaries(Database database)
        {
            Table table = new Table();
            table.AddColumn("[green]Языковая пара[/]").Centered();
            foreach (string pair in database.GetAllPairs())
            {
                table.AddRow(pair);
            }

            AnsiConsole.Write(table);
            Console.ReadKey();
        }

        /// <summary>
        /// Выводит текущий словарь в виде таблицы.
        /// </summary>
        /// <param name="database">Объект базы данных для получения текущего словаря.</param>
        private static void ShowCurrentDictionary(Database database)
        {
            Wordbook wordbook = database.GetCurrent();
            if (wordbook.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Словарь пуст![/]");
                Console.ReadKey();
                return;
            }

            Table table = new Table().Title($"[yellow]Словарь: {wordbook.LanguagePair}[/]");
            table.AddColumn("[green]Слово[/]").Centered();
            table.AddColumn("[blue]Перевод[/]").Centered();

            foreach (KeyValuePair<string, List<string>> entry in wordbook.GetTranslations())
            {
                string translationsJoined = string.Join(", ", entry.Value);
                table.AddRow(entry.Key, translationsJoined);
            }

            AnsiConsole.Write(table);
            Console.ReadKey();
        }

        /// <summary>
        /// Отображает историю переводов для текущего языкового сочетания.
        /// </summary>
        /// <param name="database">Объект базы данных для получения истории переводов.</param>
        private static void WriteHistory(Database database)
        {
            string languagePair = database.CurrentPair;
            if (string.IsNullOrEmpty(languagePair))
            {
                return;
            }

            string message = string.Join("\n", database.GetCurrent().History);
            ConsoleManager.WriteColor(message, "white");
            Console.ReadKey();
        }

        /// <summary>
        /// Выполняет перевод слова или фразы и отображает все варианты перевода.
        /// </summary>
        /// <param name="database">Объект базы данных для выполнения перевода и поиска слова.</param>
        private static void TranslateWord(Database database)
        {
            string languagePair = database.CurrentPair;
            if (string.IsNullOrEmpty(languagePair))
            {
                return;
            }

            string? word = MenuManager.FindWord(database);
            if (word == null)
            {
                ConsoleManager.WriteWarn("Слово не найдено");
                Console.ReadKey();
                return;
            }

            string translationsJoined = database.Translate(database.CurrentPair, word);
            AnsiConsole.MarkupLine($"[green]Перевод:[/] {translationsJoined}");

            Console.ReadKey();
        }
    }
}