
namespace TextProcessor.Helpers
{
    public static class Client
    {
        public static async Task<int> MainMenu()
        {
            await Task.Delay(200);
            int result = 0;
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("""
                    1 - Import file for handle;
                    2 - Delete all records in db;
                    3 - Exit
                    >>>
                    """);
                Console.ForegroundColor = ConsoleColor.White;

                string? choose = Console.ReadLine();
                if (int.TryParse(choose, out result))
                    return result;
                else
                    Console.WriteLine($"Incorrect answear: {result}");
            }
        }

        public static string GetFilePathFromUser()
        {
            Console.Write("FullPath by file: ");
            return Console.ReadLine()!;
        }
    }
}
