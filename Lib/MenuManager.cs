using Spectre.Console;

namespace Lib
{
    /// <summary>
    /// Предоставляет методы для управления меню поиска перевода.
    /// </summary>
    public static class MenuManager
    {
        /// <summary>
        /// Отображает меню выбора метода поиска перевода и возвращает выбранное слово или фразу.
        /// </summary>
        /// <param name="database">Объект базы данных, содержащий текущий словарь для поиска.</param>
        /// <returns>
        /// Возвращает выбранное слово или фразу для перевода, либо <c>null</c>, если поиск отменён.
        /// </returns>
        public static string? FindWord(Database database)
        {
            string option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[#6CB36C]Выберите метод поиска перевода[/]")
                    .AddChoices("Выбрать из списка",
                        "Ввести слово/фразу",
                        "Отмена"));
            switch (option)
            {
                case "Выбрать из списка":
                    string? word = null;
                    try
                    {
                        word = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .AddChoices(database.GetCurrent().GetAllKeys())
                        );
                    }
                    catch (InvalidOperationException)
                    {
                        ConsoleManager.WriteWarn("Словарь пуст");
                    }
                    return word;
                case "Ввести слово/фразу":
                    return AnsiConsole.Ask<string>("[green]Введите слово для перевода[/]");
                default:
                    return null;
            }
        }
    }
}