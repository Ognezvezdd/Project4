using Spectre.Console;

namespace Lib
{
    /// <summary>
    /// Класс, управляющий запуском и завершением работы приложения.
    /// Загружает словари при запуске и сохраняет их при завершении.
    /// </summary>
    public class StartAndEndManager
    {
        private FileManager _fileManager = null!;

        /// <summary>
        /// Режим отладки (Debug), который влияет на выбор пути к словарям.
        /// </summary>
        public bool Debug = false;

        /// <summary>
        /// Запускает приложение: инициализирует FileManager и загружает словари.
        /// </summary>
        /// <param name="database">Ссылка на базу данных, в которую будут загружены словари.</param>
        public void StartApp(ref Database database)
        {
            while (true)
            {
                try
                {
                    _fileManager = new FileManager(Debug);
                    _fileManager.LoadDictionaries(ref database);
                    ConsoleManager.WriteMessage("Для продолжения нажмите любую клавишу...");
                    Console.ReadKey();
                    break;
                }
                catch (DirectoryNotFoundException)
                {
                    continue;
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Завершает работу приложения: сохраняет словари и выводит финальное сообщение.
        /// </summary>
        /// <param name="database">База данных, содержащая словари для сохранения.</param>
        public void EndApp(Database database)
        {
            foreach (string pair in database.GetAllPairs())
            {
                _fileManager.SaveDictionary(database, pair);
            }

            AnsiConsole.MarkupLine("[green]Спасибо за работу! Данные [bold]сохранены![/][/]");
        }
    }
}