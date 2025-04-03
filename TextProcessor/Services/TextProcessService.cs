
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using TextProcessor.Data;
using TextProcessor.Helpers;
using TextProcessor.Models;

namespace TextProcessor.Services
{
    internal sealed class TextProcessService(
        ILogger<TextProcessService> logger,
        FileReaderByLine fileReaderByLine,
        DbGateway db) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(500);
            logger.LogInformation("Start service!");
            bool exit = false;
            ConcurrentDictionary<string, int> wordStatictics = new();
            try
            {
                while (!exit && !stoppingToken.IsCancellationRequested)
                {
                    int choose = await Client.MainMenu();
                    switch (choose)
                    {
                        case 1:
                            {
                                var filePath = Client.GetFilePathFromUser();
                                Task.Run(async () => await TextProcessTask(filePath, wordStatictics, stoppingToken));
                                break;
                            }
                        case 2:
                            await db.DeleteAllWordsAsync();
                            break;
                        case 3:
                            exit = true;
                            logger.LogInformation("Service finished");
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (OperationCanceledException) 
            {
                logger.LogInformation("Exit application...");
            }
        }

        private async Task TextProcessTask(string filePath, ConcurrentDictionary<string, int> wordStatictics, CancellationToken stoppingToken)
        {
            logger.LogInformation("Start text handle...");
            List<WordDetector> resultHandle = await FileHandle(filePath, wordStatictics, stoppingToken);

            logger.LogInformation($"Handle finish\nCount words = {resultHandle.Count}");
            logger.LogInformation("подготовка к записи в бд");
            await SaveToDb(resultHandle, stoppingToken);
        }

        private async Task SaveToDb(List<WordDetector> resultHandle, CancellationToken stoppingToken)
        {
            await Task.Delay(500);
            await db.AddOrUpdate(resultHandle, stoppingToken);
        }

        private async Task<List<WordDetector>> FileHandle(string filePath, ConcurrentDictionary<string, int> wordStatictics, CancellationToken stoppingToken)
        {
            //read to concurrent dictionary
            await fileReaderByLine.ReadAsync(filePath, wordStatictics, stoppingToken);

            var resultHandle = wordStatictics
                .Where(x => x.Value > 2)
                .Select(x => new WordDetector(x.Key, x.Value)).ToList();
            return resultHandle;
        }
    }
}
