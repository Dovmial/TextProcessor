
namespace TextProcessor.Models
{
    public class WordDetector
    {
        public int Id { get; set; }
        public string Word { get; set; }
        public int Amount { get; set; }

        public WordDetector()
        {
            
        }

        public WordDetector(string word, int amount)
        {
            Word = word;
            Amount = amount;
        }
        /// <summary>
        /// Field for concurrency
        /// https://learn.microsoft.com/ru-ru/ef/core/saving/concurrency?tabs=data-annotations
        /// </summary>
        public byte[] Version { get; set; }

        public override string ToString()
        {
            return $"{Word}:   {Amount}";
        }
    }
}
