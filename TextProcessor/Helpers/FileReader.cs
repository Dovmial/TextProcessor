
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text;
using TextProcessor.Extensions;

namespace TextProcessor.Helpers
{
    public class FileReaderByLine(ILogger<FileReaderByLine> logger)
    {
        public async Task ReadAsync(
            string fullFilePath,
            ConcurrentDictionary<string, int> wordsStatistic,
            CancellationToken token)
        {
            if (!File.Exists(fullFilePath))
            {
                logger.LogError($"File [{fullFilePath}] is not found");
                return;
            }
            int taskLimit = Environment.ProcessorCount;
            List<Task> tasks = new(taskLimit);
            try
            {
                await foreach (string line in File.ReadLinesAsync(fullFilePath, Encoding.UTF8, token))
                {
                    if (tasks.Count >= taskLimit)
                    {
                        var finishedTask = await Task.WhenAny(tasks);
                        tasks.Remove(finishedTask);
                    }
                    tasks.Add(Task.Run(() =>
                    {
                        var result = TextLineHandler.Handle(line, wordsStatistic);
                        if (result.IsSuccess == false)
                            logger.LogError(result.Error);
                    }, token));
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.MessageWithInnerExceptions());
                return;
            }
        }
    }
}
