namespace Lib
{
    /// <summary>
    /// Класс, представляющий базу данных всех словарей.
    /// </summary>
    public class Database
    {
        private List<Wordbook> Wordbooks { get; set; }
        internal string CurrentPair { get; private set; }

        /// <summary>
        /// Получает список всех языковых пар.
        /// </summary>
        /// <returns>Список языковых пар.</returns>
        public List<string> GetAllPairs()
        {
            return Wordbooks.Select(t => t.LanguagePair).ToList();
        }

        /// <summary>
        /// Метод, предназначенный только для сохранения в файл.
        /// </summary>
        /// <param name="pair">Языковая пара.</param>
        /// <returns>Словарь для указанной языковой пары.</returns>
        internal Wordbook PrivateGet(string pair)
        {
            return Wordbooks.First(t => t.LanguagePair == pair);
        }

        /// <summary>
        /// Возвращает текущий активный словарь.
        /// </summary>
        /// <returns>Текущий словарь.</returns>
        public Wordbook GetCurrent()
        {
            return Wordbooks.First(t => t.LanguagePair == CurrentPair);
        }

        /// <summary>
        /// Изменяет текущую языковую пару.
        /// </summary>
        /// <param name="pair">Новая языковая пара.</param>
        public void ChangePair(string pair)
        {
            if (Wordbooks.Exists(t => t.LanguagePair == pair) == false)
            {
                ConsoleManager.WriteWarn("Языковая пара не найдена");
                return;
            }

            CurrentPair = pair;
        }

        /// <summary>
        /// Конструктор, принимающий список словарей и текущую языковую пару.
        /// </summary>
        /// <param name="wordbooks">Список словарей.</param>
        /// <param name="currentPair">Текущая языковая пара (по умолчанию: English-Russian).</param>
        internal Database(List<Wordbook> wordbooks, string currentPair = "English-Russian")
        {
            CurrentPair = currentPair;
            Wordbooks = wordbooks;
        }

        /// <summary>
        /// Конструктор копирования с возможностью задать новую текущую языковую пару.
        /// </summary>
        /// <param name="database">Существующая база данных.</param>
        /// <param name="currentPair">Новая текущая языковая пара (по умолчанию: English-Russian).</param>
        public Database(Database database, string currentPair = "English-Russian")
        {
            CurrentPair = currentPair;
            Wordbooks = database.Wordbooks;
        }

        /// <summary>
        /// Инициализация базы данных с тестовыми данными.
        /// </summary>
        /// <returns>Экземпляр базы данных с тестовыми данными.</returns>
        public static Database InitializeSampleData()
        {
            List<Wordbook> data = new();
            Dictionary<string, List<string>> englishToRussian = new()
            {
                { "hello", ["привет"] }, { "world", ["мир"] }, { "cat", ["кот"] }
            };
            data.Add(new Wordbook(englishToRussian, "English-Russian"));

            Dictionary<string, List<string>> englishToFrench = new()
            {
                { "hello", ["bonjour"] }, { "world", ["monde"] }, { "cat", ["chat"] }
            };
            data.Add(new Wordbook(englishToFrench, "English-French"));

            return new Database(data);
        }

        /// <summary>
        /// Перевод слова ИЛИ фразы для указанной языковой пары.
        /// </summary>
        /// <param name="languagePair">Языковая пара.</param>
        /// <param name="word">Слово для перевода.</param>
        /// <param name="flag">Доп опция. warn - будет выводиться warn. ignore - не будет выводиться warn
        /// </param>
        /// <returns>Перевод слова или пустая строка, если перевод не найден.</returns>
        public string Translate(string languagePair, string? word, string flag = "none")
        {
            if (word == null)
            {
                if (flag != "ignore")
                {
                    ConsoleManager.WriteWarn($"Языковая пара '{languagePair}' не найдена.");
                }

                return string.Empty;
            }

            Wordbook? wordbook = Wordbooks.FirstOrDefault(wb => wb.LanguagePair == languagePair);
            if (wordbook != null)
            {
                if (word.Trim().ToLower().Split(" ").Length > 0)
                {
                    return wordbook.TranslatePhrase(word, flag);
                }

                return wordbook.TranslateWord(word, flag);
            }

            if (flag != "ignore")
            {
                ConsoleManager.WriteWarn($"Языковая пара '{languagePair}' не найдена.");
            }

            return string.Empty;
        }

        /// <summary>
        /// Добавляет или обновляет слово в указанной языковой паре.
        /// </summary>
        /// <param name="languagePair">Языковая пара.</param>
        /// <param name="word">Слово.</param>
        /// <param name="translation">Перевод слова.</param>
        public void AddOrUpdateWord(string languagePair, string word, string translation)
        {
            Wordbook? wordbook = Wordbooks.FirstOrDefault(wb => wb.LanguagePair == languagePair);
            if (wordbook != null)
            {
                wordbook.AddOrUpdateWord(word, translation);
            }
            else
            {
                ConsoleManager.WriteWarn($"Языковая пара '{languagePair}' не найдена.");
            }
        }

        /// <summary>
        /// Удаляет слово из указанной языковой пары.
        /// </summary>
        /// <param name="languagePair">Языковая пара.</param>
        /// <param name="word">Слово для удаления.</param>
        public void RemoveWord(string languagePair, string? word)
        {
            Wordbook? wordbook = Wordbooks.FirstOrDefault(wb => wb.LanguagePair == languagePair);
            if (wordbook != null)
            {
                wordbook.RemoveWord(word);
            }
            else
            {
                ConsoleManager.WriteWarn($"Языковая пара '{languagePair}' не найдена.");
            }
        }


        /// <summary>
        /// Перегрузка оператора + для добавления нового словаря в базу данных.
        /// </summary>
        /// <param name="a">Исходная база данных.</param>
        /// <param name="b">Добавляемый словарь.</param>
        /// <returns>Новая база данных с добавленным словарем.</returns>
        public static Database operator +(Database a, Wordbook b)
        {
            return new Database(a.Wordbooks.Append(b).ToList(), a.CurrentPair);
        }
    }
}