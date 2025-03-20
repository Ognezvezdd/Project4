using Nestor;
using Nestor.Models;

namespace Lib
{
    /// <summary>
    /// Класс для продвинутого перевода фраз с использованием морфологического анализа.
    /// </summary>
    public class AdvancedTranslator
    {
        private readonly Database _database;

        /// <summary>
        /// Конструктор, принимающий базу данных для перевода.
        /// </summary>
        /// <param name="database">Экземпляр базы данных со словарями.</param>
        public AdvancedTranslator(Database database)
        {
            _database = database;
        }

        /// <summary>
        /// Переводит фразу, разбивая её на слова, приводя каждое слово к начальной форме и выполняя поиск перевода Nestor.API.
        /// </summary>
        /// <param name="phrase">Исходная фраза для перевода.</param>
        /// <returns>Переведённая фраза.</returns>
        public string Translate(string phrase)
        {
            string trimmedPhrase = phrase.Trim().ToLower();
            string[] words = trimmedPhrase.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> translatedWords = new List<string>();

            foreach (string word in words)
            {
                MorphResult analysisResult = MorphAnalyzer.Analyze(word);
                string baseWord = analysisResult.Lemma ?? word;

                string translated = _database.Translate(_database.CurrentPair, baseWord, "ignore");

                if (!string.IsNullOrEmpty(translated))
                {
                    if (analysisResult.IsPlural && !translated.EndsWith("s"))
                    {
                        translated += "s";
                    }
                }
                else
                {
                    translated = "|не найдено|";
                }

                translatedWords.Add(translated);
            }

            return string.Join(" ", translatedWords);
        }
    }

    /// <summary>
    /// Упрощённый морфологический анализатор с использованием возможностей Nestor.
    /// Для каждого слова создаётся экземпляр NestorMorph, и вызывается его метод WordInfo.
    /// Если слово имеет несколько вариантов (например, "стали"), выбирается первый вариант.
    /// </summary>
    public static class MorphAnalyzer
    {
        public static MorphResult Analyze(string? word)
        {
            MorphResult result = new MorphResult();

            if (word == null)
            {
                return result;
            }

            NestorMorph nMorph = new NestorMorph();
            Word[] words = nMorph.WordInfo(word);

            if (words.Length > 0)
            {
                Word selectedWord = words[0];

                result.Lemma = selectedWord.Lemma.Word;

                result.IsPlural = selectedWord.Tag != null && selectedWord.Tag.ToString()!.Contains("мн");
            }
            else
            {
                result.Lemma = word;
                result.IsPlural = false;
            }

            return result;
        }
    }

    /// <summary>
    /// Результат морфологического анализа слова.
    /// </summary>
    public class MorphResult
    {
        /// <summary>
        /// Начальная форма слова (лемма).
        /// </summary>
        public string? Lemma { get; set; }

        /// <summary>
        /// Флаг, указывающий на множественное число.
        /// </summary>
        public bool IsPlural { get; set; }
    }
}