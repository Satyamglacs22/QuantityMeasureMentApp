using System;
using System.Collections.Generic;
using System.IO;

namespace QuantityMeasurementAppRepository.Utilities
{
    // Purpose:
    // Reads appsettings.json and provides all configuration values.
    // Correctly handles connection strings that contain multiple colons
    // (e.g. Trusted_Connection=True;TrustServerCertificate=True)
    // by reading the ENTIRE value after the first colon on each line,
    // not just up to the next colon.

    public class ApplicationConfig
    {
        private Dictionary<string, string> settings;

        // Default constructor - looks for appsettings.json next to the .exe
        public ApplicationConfig()
        {
            settings = new Dictionary<string, string>();

            string defaultPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "appsettings.json");

            LoadConfig(defaultPath);
        }

        // Constructor that accepts a custom path - used in unit tests
        public ApplicationConfig(string configFilePath)
        {
            settings = new Dictionary<string, string>();
            LoadConfig(configFilePath);
        }

        // Reads appsettings.json line by line.
        // Correctly handles values that contain colons (connection strings).
        private void LoadConfig(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("[ApplicationConfig] Warning: config file not found: "
                    + filePath + ". Using defaults.");
                return;
            }

            string[] lines = File.ReadAllLines(filePath);
            string currentSection = "";

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();

                // Skip blank lines and bare braces
                if (line == "" || line == "{" || line == "}" || line == "},")
                {
                    continue;
                }

                // Detect a section opener like  "Database": {
                // These lines end with { after the colon
                if (line.EndsWith("{") || line.EndsWith(": {"))
                {
                    int fq = line.IndexOf('"');
                    int sq = line.IndexOf('"', fq + 1);
                    if (fq >= 0 && sq > fq)
                    {
                        currentSection = line.Substring(fq + 1, sq - fq - 1);
                    }
                    continue;
                }

                // Detect a key:value line
                // KEY is between the first two quote marks
                // VALUE is between the LAST two quote marks
                // This correctly handles connection strings with multiple colons
                if (line.Contains(":") && line.Contains("\""))
                {
                    // Extract the key - always between the first pair of quotes
                    int firstQuote  = line.IndexOf('"');
                    int secondQuote = line.IndexOf('"', firstQuote + 1);

                    if (firstQuote < 0 || secondQuote < 0)
                    {
                        continue;
                    }

                    string keyPart = line.Substring(
                        firstQuote + 1, secondQuote - firstQuote - 1);

                    // Extract the value - between the LAST pair of quotes
                    // This correctly gets the full connection string
                    // even when it contains colons
                    int lastQuote       = line.LastIndexOf('"');
                    int secondLastQuote = line.LastIndexOf('"', lastQuote - 1);

                    string valuePart = "";

                    if (lastQuote > secondQuote && secondLastQuote > secondQuote)
                    {
                        // Value is quoted - extract between last pair of quotes
                        valuePart = line.Substring(
                            secondLastQuote + 1,
                            lastQuote - secondLastQuote - 1);
                    }
                    else
                    {
                        // Value is not quoted (e.g. numbers like 5, 30)
                        // Split on first colon and take everything after it
                        int colonIdx = line.IndexOf(':', secondQuote);
                        if (colonIdx >= 0)
                        {
                            valuePart = line.Substring(colonIdx + 1).Trim();
                            valuePart = valuePart.TrimEnd(',').Trim();
                        }
                    }

                    // Build the full key:  Section:Key  or just  Key
                    string fullKey = (currentSection != "")
                        ? currentSection + ":" + keyPart
                        : keyPart;

                    if (!settings.ContainsKey(fullKey))
                    {
                        settings.Add(fullKey, valuePart);
                    }
                }
            }

            Console.WriteLine("[ApplicationConfig] Loaded " + settings.Count
                + " settings from " + filePath);

            // Print all loaded settings for verification
            Console.WriteLine("[ApplicationConfig] Settings loaded:");
            foreach (string key in settings.Keys)
            {
                Console.WriteLine("  " + key + " = " + settings[key]);
            }
        }

        // Returns a setting value or the default if key is missing
        public string GetSetting(string key, string defaultValue)
        {
            if (settings.ContainsKey(key))
            {
                return settings[key];
            }
            Console.WriteLine("[ApplicationConfig] Key not found: " + key
                + ". Using default: " + defaultValue);
            return defaultValue;
        }

        // Returns the SQL Server connection string
        public string GetConnectionString()
        {
            return GetSetting("Database:ConnectionString",
                "Server=localhost\\SQLEXPRESS;Database=QuantityMeasurementAppDB;" +
                "Trusted_Connection=True;TrustServerCertificate=True;");
        }

        // Returns maximum pool size
        public int GetMaxPoolSize()
        {
            string val = GetSetting("Database:PoolSize", "5");
            int result = 5;
            int.TryParse(val, out result);
            return result;
        }

        // Returns connection timeout in seconds
        public int GetConnectionTimeout()
        {
            string val = GetSetting("Database:ConnectionTimeout", "30");
            int result = 30;
            int.TryParse(val, out result);
            return result;
        }

        // Returns "database" or "cache"
        public string GetRepositoryType()
        {
            return GetSetting("Repository:Type", "cache");
        }

        // Returns database provider (sqlserver by default)
        public string GetDatabaseProvider()
        {
            return GetSetting("Database:Provider", "sqlserver");
        }
    }
}