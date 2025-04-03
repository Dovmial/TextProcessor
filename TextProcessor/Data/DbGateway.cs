

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TextProcessor.Extensions;
using TextProcessor.Models;

namespace TextProcessor.Data
{
    public sealed class DbGateway(TextProcessDbContext db, ILogger<DbGateway> logger)
    {
        public async Task DeleteAllWordsAsync()
        {
            await db.Words.ExecuteDeleteAsync();
        }
        /// <summary>
        /// try many times to save.
        /// https://learn.microsoft.com/ru-ru/ef/core/saving/concurrency?tabs=data-annotations
        /// </summary>
        /// <param name="words"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task AddOrUpdate(ICollection<WordDetector> words, CancellationToken token, int direction = 1)
        {
            int attempt = 1;
            logger.LogInformation($"Start save {words.Count} records to db ({attempt})");

            try
            {
                foreach (var word in words)
                {
                    var wordEntity = await db.Words.FirstOrDefaultAsync(x => x.Word == word.Word);
                    if (wordEntity is not null)
                    {
                        wordEntity.Amount += word.Amount*direction;
                        db.Words.Update(wordEntity);
                    }
                    else
                    {
                        db.Words.Add(word);
                    }
                }
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                bool success = false;
                ++attempt;
                logger.LogWarning($"Attempt({attempt}) to write failed.");
                logger.LogInformation($"Start save {words.Count} records to db ({attempt})");
                while (!success && !token.IsCancellationRequested)
                {
                    try
                    {
                        foreach (var entry in ex.Entries)
                        {
                            //get actual data
                            var dbValues = entry.GetDatabaseValues();
                            if (dbValues != null)
                                entry.OriginalValues.SetValues(dbValues);
                        }
                        await db.SaveChangesAsync();
                        await Task.Delay(500);
                    }
                    catch (DbUpdateConcurrencyException retryEx)
                    {
                        logger.LogError(retryEx.MessageWithInnerExceptions());
                        await Task.Delay(500);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.MessageWithInnerExceptions());
                await Task.Delay(500);
            }
        }

        //not using in this version
        public async Task UndoFileStatistic(ICollection<WordDetector> words, CancellationToken token)
        {
            await AddOrUpdate(words, token, direction: - 1);
            await db.Words
                .Where(x=> x.Amount == 0)
                .ExecuteDeleteAsync();
        }
    }
}
