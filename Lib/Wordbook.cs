namespace Lib
{
    /// <summary>
    /// Класс, представляющий словарь для одной языковой пары.
    /// Поддерживает хранение до двух вариантов перевода для каждого слова.
    /// </summary>
    public class Wordbook
    {
        public string LanguagePair { get; private set; }
        private Dictionary<string, List<string>> Translations { get; set; }
        internal readonly List<string> History = new();

        /// <summary>
        /// Количество объектов в Translations
        /// </summary>
        internal int Count => Translations.Count;

        /// <summary>
        /// Используется ТОЛЬКО для вывода Display и FileManager
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, List<string>> GetTranslations()
        {
            return Translations;
        }

        /// <summary>
        /// ТОЛЬКО ДЛЯ ОДНОКРАТНОГО ИСПОЛЬЗОВАНИЯ В MenuManager
        /// </summary>
        /// <returns></returns>
        internal List<string> GetAllKeys()
        {
            return Translations.Keys.ToList();
        }

        /// <summary>
        /// Конструктор, принимающий словарь переводов и языковую пару.
        /// </summary>
        /// <param name="translations">Словарь переводов.</param>
        /// <param name="languagePair">Языковая пара.</param>
        public Wordbook(Dictionary<string, List<string>> translations, string languagePair)
        {
            LanguagePair = languagePair;
            Translations = translations;
        }

        internal bool IsIn(string? word)
        {
            if (word == null)
            {
                return false;
            }

            word = word.ToLower();
            if (Translations.ContainsKey(word))
            {
                return true;
            }

            foreach (List<string> curr in Translations.Values)
            {
                foreach (string wordNow in curr)
                {
                    if (word.Trim().ToLower() == wordNow.Trim().ToLower())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public List<string> MultiplyTranslate(string? word, string flag = "warn")
        {
            if (word == null || IsIn(word) == false)
            {
                return [];
            }

            word = word.Trim().ToLower();

            return Translations.ContainsKey(word) ? Translations[word] : [TranslateWord(word, flag)];
        }


        /// <summary>
        /// Переводит слово. Если для слова имеется несколько вариантов, возвращается первый.
        /// Также реализован обратный поиск по значению.
        /// </summary>
        /// <param name="word">Слово для перевода.</param>
        /// <param name="flag"></param>
        /// <returns>Перевод или пустая строка, если перевод не найден.</returns>
        public string TranslateWord(string? word, string flag = "warn")
        {
            word = word?.Trim().ToLower();

            // Поиск перевода по ключу
            if (word != null && Translations.ContainsKey(word))
            {
                List<string> translationsForWord = Translations[word];
                string chosenTranslationMultiply = translationsForWord.Count > 1
                    ? "|" + string.Join("/", translationsForWord) + "|"
                    : string.Join("/", translationsForWord);

                History.Add($"ПЕРЕВОД СЛОВА {word}: {chosenTranslationMultiply}");
                return chosenTranslationMultiply;
            }

            // Поиск обратного перевода: ищем, если слово присутствует среди вариантов перевода
            foreach ((string? key, List<string>? value) in Translations)
            {
                if (value.Any(translation => translation.ToLower() == word))
                {
                    History.Add($"ПЕРЕВОД ЗНАЧЕНИЯ {word}: {key}");
                    return key;
                }
            }

            History.Add($"{word}: ОШИБКА ПЕРЕВОДА");
            if (flag != "ignore")
            {
                ConsoleManager.WriteWarn(
                    $"Перевод для слова или значения '{word}' не найден в словаре {LanguagePair}.");
            }

            return string.Empty;
        }

        /// <summary>
        /// Переводит фразу, разбивая её на отдельные слова.
        /// </summary>
        /// <param name="phrase">Фраза для перевода.</param>
        /// <param name="flag">Доп опция. warn - будет выводиться warn. ignore - не будет выводиться warn</param>
        /// <returns>Переведённая фраза.</returns>
        public string TranslatePhrase(string? phrase, string flag = "none")
        {
            phrase = phrase?.Trim().ToLower();
            if (phrase != null && phrase.Contains(' '))
            {
                List<string?> translatedWords = new List<string?>();
                string?[] words = phrase.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                foreach (string? word in words)
                {
                    translatedWords.Add(TranslateWord(word, flag));
                }

                return string.Join(" ", translatedWords);
            }

            return TranslateWord(phrase, flag);
        }

        /// <summary>
        /// Добавляет или обновляет слово в словаре.
        /// Если слово уже существует и перевода нет, то перевод добавляется в список вариантов.
        /// Если вариантов уже два, новый перевод заменяет основной вариант.
        /// </summary>
        /// <param name="word">Слово для добавления или обновления.</param>
        /// <param name="translation">Перевод слова.</param>
        public void AddOrUpdateWord(string word, string translation)
        {
            word = word.ToLower();
            translation = translation.ToLower();
            History.Add($"ДОБАВЛЕНИЕ/ОБНОВЛЕНИЕ СЛОВА {word}: {translation}");
            if (Translations.ContainsKey(word))
            {
                List<string> existingTranslations = Translations[word];
                if (!existingTranslations.Contains(translation))
                {
                    if (existingTranslations.Count < 2)
                    {
                        existingTranslations.Add(translation);
                    }
                    else
                    {
                        // Если уже два варианта, заменяем основной вариант (первый)
                        existingTranslations[0] = translation;
                    }
                }
            }
            else
            {
                Translations[word] = [translation];
            }

            ConsoleManager.WriteMessage($"Слово '{word}' добавлено/обновлено в словаре {LanguagePair}.");
        }

        /// <summary>
        /// Удаляет слово из словаря.
        /// </summary>
        /// <param name="word">Слово для удаления.</param>
        public void RemoveWord(string? word)
        {
            word = word?.ToLower();
            if (word != null && Translations.Remove(word))
            {
                History.Add($"УДАЛЕНИЕ СЛОВА {word}");
                ConsoleManager.WriteMessage($"Слово '{word}' удалено из словаря {LanguagePair}.");
            }
            else
            {
                History.Add($"ПОПЫТКА УДАЛЕНИЯ СЛОВА {word}: СЛОВО НЕ НАЙДЕНО");
                ConsoleManager.WriteWarn($"Слово '{word}' не найдено в словаре {LanguagePair}.");
            }
        }
    }
}