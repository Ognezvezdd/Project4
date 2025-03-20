using Spectre.Console;

namespace Lib
{
    /// <summary>
    /// Управляет операциями загрузки и сохранения словарей из файловой системы.
    /// </summary>
    public class FileManager
    {
        private readonly string _folderPath;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="FileManager"/>.
        /// </summary>
        /// <param name="debug">
        /// Если <c>true</c>, используется путь по умолчанию ("Languages"). 
        /// Если <c>false</c>, запрашивается путь у пользователя и проверяется его корректность.
        /// </param>
        /// <exception cref="DirectoryNotFoundException">
        /// Выбрасывается, если указанный путь некорректен или не существует.
        /// </exception>
        public FileManager(bool debug = false)
        {
            if (debug)
            {
                _folderPath = "Languages";
            }
            else
            {
                _folderPath = AnsiConsole.Ask<string>("[blue]Введите абсолютный путь к папке со словарями[/]");

                if (string.IsNullOrWhiteSpace(_folderPath) || !Directory.Exists(_folderPath))
                {
                    ConsoleManager.WriteWarn("Указан некорректный путь к папке.");
                    throw new DirectoryNotFoundException("Путь к папке со словарями не найден.");
                }
            }
        }

        /// <summary>
        /// Загружает словари из текстовых файлов, расположенных в указанной папке, и обновляет базу данных.
        /// </summary>
        /// <param name="database">
        /// Ссылка на объект базы данных, который будет обновлен загруженными словарями.
        /// </param>
        public void LoadDictionaries(ref Database database)
        {
            string[] files = Directory.GetFiles(_folderPath, "*.txt");
            List<Wordbook> res = new List<Wordbook>();
            string lastPair = "-1";
            foreach (string file in files)
            {
                try
                {
                    string[] lines = File.ReadAllLines(file);
                    if (lines.Length < 1)
                    {
                        ConsoleManager.WriteWarn($"Файл {file} пустой или некорректный.", false);
                        continue;
                    }

                    string pair = lines[0].Trim();
                    Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] parts = lines[i].Split(':', 2);
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim().ToLower();
                            string[] translation = parts[1].Trim().ToLower().Split(", ");
                            if (!dictionary.ContainsKey(key))
                            {
                                dictionary[key] = new List<string>();
                            }

                            foreach (string val in translation)
                            {
                                dictionary[key].Add(val);
                            }
                        }
                    }

                    if (lastPair == "-1")
                    {
                        lastPair = pair;
                    }

                    res.Add(new Wordbook(dictionary, pair));
                    ConsoleManager.WriteMessage($"Загружен словарь '{pair}' с {dictionary.Count} словами.");
                }
                catch (Exception ex)
                {
                    ConsoleManager.WriteWarn($"Ошибка при загрузке файла {file}: {ex.Message}");
                }
            }

            database = new Database(res, lastPair);
        }

        /// <summary>
        /// Сохраняет выбранный словарь в текстовый файл в указанной папке.
        /// </summary>
        /// <param name="database">Объект базы данных, содержащий словари.</param>
        /// <param name="pair">Языковая пара словаря, который необходимо сохранить.</param>
        public void SaveDictionary(Database database, string pair)
        {
            if (!database.GetAllPairs().Contains(pair))
            {
                ConsoleManager.WriteWarn($"Словарь для пары '{pair}' не найден.");
                return;
            }

            string filePath = Path.Combine(_folderPath, $"{pair}.txt");
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(pair);
                    foreach ((string? key, List<string>? value) in database.PrivateGet(pair).GetTranslations())
                    {
                        string translationsJoined = string.Join(", ", value);
                        writer.WriteLine($"{key}:{translationsJoined}");
                    }
                }

                ConsoleManager.WriteMessage($"Словарь '{pair}' успешно сохранён.");
            }
            catch (Exception ex)
            {
                ConsoleManager.WriteWarn($"Ошибка при сохранении словаря '{pair}': {ex.Message}");
            }
        }
    }
}