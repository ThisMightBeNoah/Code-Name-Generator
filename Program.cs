using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;

public class CodeNameGenerator
{
    private static readonly string[] Adjectives;
    private static readonly string[] Nouns;
    private static readonly string[] Animals;
    private static readonly Random Random = new Random();

    static CodeNameGenerator()
    {
        try
        {
            Adjectives = LoadEmbeddedResource("en-adj.txt");
            Animals = LoadEmbeddedResource("en-animal.txt");
            Nouns = LoadEmbeddedResource("en-noun.txt");

            if (Adjectives.Length == 0 || Nouns.Length == 0)
            {
                throw new InvalidOperationException("Word files are empty");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Failed to load word files. Make sure en-adj.txt and en-noun.txt " +
                "are set as Embedded Resources in your project.", ex);
        }
    }

    private static string[] LoadEmbeddedResource(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var allResources = assembly.GetManifestResourceNames();
        foreach (var res in allResources)
        {
            // Console.WriteLine($"  {res}");
        }

        // Look for resources that contain the filename (more flexible)
        var resourceName = allResources
            .FirstOrDefault(name => name.Contains(fileName));

        if (resourceName == null)
        {
            throw new FileNotFoundException($"Embedded resource containing '{fileName}' not found. Available resources: {string.Join(", ", allResources)}");
        }

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new FileNotFoundException($"Could not load stream for resource: {resourceName}");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd()
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrEmpty(line))
            .ToArray();
    }
    /*
        public static string GenerateCodeName()
        {
            var adjective = Adjectives[Random.Next(Adjectives.Length)];
            var noun = Nouns[Random.Next(Nouns.Length)];
            return $"{adjective}-{noun}";
        }
    */
    public static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Animal Or Noun (1 for Animal, 2 for Noun):");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input cannot be empty");
            }
            if (input.Equals("1", StringComparison.OrdinalIgnoreCase))
            {
                // Generate an animal-themed code name
                var adjective = Adjectives[Random.Next(Adjectives.Length)];
                var animalNoun = Animals[Random.Next(Animals.Length)];
                Console.WriteLine($"{adjective}-{animalNoun}");
            }
            else if (input.Equals("2", StringComparison.OrdinalIgnoreCase))
            {
                // Generate a noun-themed code name
                var nounAdjective = Adjectives[Random.Next(Adjectives.Length)];
                var nounNoun = Nouns[Random.Next(Nouns.Length)];
                Console.WriteLine($"{nounAdjective}-{nounNoun}");
            }
            else
            {
                throw new ArgumentException("Input must be 'animal' or 'noun'");
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating code name: {ex.Message}");
        }
    }
}