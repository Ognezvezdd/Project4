using Lib;
using Spectre.Console;

namespace Project4
{
    internal static class Program
    {
        private static void Main()
        {
            // Инициализация базы данных с тестовыми данными
            Database database = new(Database.InitializeSampleData());

            // Инициализация менеджера запуска и завершения приложения с включенным режимом отладки
            StartAndEndManager startAndEndManager = new() { Debug = false };
            startAndEndManager.StartApp(ref database);

            // Главный цикл приложения
            while (true)
            {
                try
                {
                    Console.Clear();

                    // Отображение главного меню и выбор действия
                    string option = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[green]Выберите действие[/]")
                            .AddChoices(
                                "Просмотр [bold]текущего[/] словаря",
                                "Редактировать базу данных",
                                "Выбрать языковую пару",
                                "Продвинутый перевод предложения с [bold]Nestor.API[/]",
                                "Голосовой ввод/вывод",
                                "Дополнительная задача",
                                "Выход"));

                    // Обработка выбранного пункта меню
                    switch (option)
                    {
                        case "Выбрать языковую пару":
                            Menu.ChooseLanguage(database);
                            break;
                        case "Редактировать базу данных":
                            Menu.EditDatabase(database);
                            break;
                        case "Просмотр [bold]текущего[/] словаря":
                            Menu.Display(database);
                            break;
                        case "Продвинутый перевод предложения с [bold]Nestor.API[/]":
                            _ = Menu.AdvancedTranslation(database);
                            break;
                        case "Голосовой ввод/вывод":
                            _ = Menu.Sound(database);
                            break;
                        case "Дополнительная задача":
                            Menu.AdditionalTask(database);
                            break;
                        case "Выход":
                            startAndEndManager.EndApp(database);
                            return;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleManager.WriteWarn($"Произошла ошибка: {ex.Message}");
                    Console.ReadKey();
                }
            }
        }
    }
}