// See https://aka.ms/new-console-template for more information

using OllamaSharp;

if (args.Length == 0)
{
    Console.WriteLine("Please specify the target directory.");
    Console.ReadLine();
}
else
{
    string dir = args[0];
    if (!Directory.Exists(dir))
    {
        Console.WriteLine("The specified directory does not exist.");
        Console.ReadLine();
        return;
    }

    var uri = new Uri("http://localhost:11434");
    var client = new OllamaApiClient(uri)
    {
        SelectedModel = "phi3"
    };

    var models = await client.ListLocalModelsAsync();
    Console.WriteLine($"Connecting to {uri} ...");

    await foreach (var status in client.PullModelAsync("phi3"))
    {
        Console.WriteLine($"{status.Percent}% {status.Status}");
    }

    Console.WriteLine();

    string[] csvFiles = Directory.GetFiles(dir, "*.csv", SearchOption.AllDirectories);
    string converted = string.Empty;
    foreach (var csvFile in csvFiles)
    {
        var result = await client.EmbedAsync(csvFile);
        List<float[]> vectors = result.Embeddings;

        foreach (var embedding in result.Embeddings)
        {
            foreach (var value in embedding)
            {
                Console.Write($"{value:F6} ");
                converted += $"{value:F6} ";
            }
        }

        Console.WriteLine();
    }

    File.WriteAllText($"{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.txt", converted);
    Console.WriteLine("\r\nFinished.");
}