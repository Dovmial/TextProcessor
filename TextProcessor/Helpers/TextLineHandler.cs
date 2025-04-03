
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using TextProcessor.Extensions;

namespace TextProcessor.Helpers
{
    public static class TextLineHandler
    {
        /// <summary>
        /// заполняет конкурентную очередь статистикой по словам
        /// </summary>
        /// <param name="str"></param>
        /// <param name="wordsStatistic"></param>
        public static OperationResult Handle(string str, ConcurrentDictionary<string, int> wordsStatistic)
        {
            try
            {
                string lineWithoutPunctuationMark = Regex.Replace(str, @"[^\w\s-]", "");
                string[] words = lineWithoutPunctuationMark
                    .ToLower()
                    .Replace('ё', 'е')
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Where(x => x.Length < 21 && x.Length > 3)
                    .ToArray();

                var tempDictionary = words
                    .GroupBy(x => x)
                    .ToDictionary(g => g.Key, g => g.Count());

                foreach (var el in words.GroupBy(x => x))
                {
                    int count = el.Count();
                    wordsStatistic.AddOrUpdate(el.Key, count, (key, oldValue) => oldValue + count);
                }
                return OperationResultCreator.Success;
            }
            catch (Exception ex) 
            {
                return OperationResultCreator.Failure(ex.MessageWithInnerExceptions());
            }
        }
    }
}
