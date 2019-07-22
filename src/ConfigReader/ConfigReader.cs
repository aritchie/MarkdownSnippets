﻿using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using MarkdownSnippets;

public static class ConfigReader
{
    public static ConfigInput Read(string directory)
    {
        var path = Path.Combine(directory, "mdsnippets.json");
        if (!File.Exists(path))
        {
            return null;
        }

        return Parse(File.ReadAllText(path));
    }

    public static ConfigInput Parse(string contents)
    {
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
        {
            stream.Position = 0;
            var serializer = new DataContractJsonSerializer(typeof(ConfigSerialization));
            var configSerialization = (ConfigSerialization) serializer.ReadObject(stream);
            return new ConfigInput
            {
                WriteHeader = configSerialization.WriteHeader,
                ReadOnly = configSerialization.ReadOnly,
                LinkFormat = GetLinkFormat(configSerialization.LinkFormat),
            };
        }
    }
    static LinkFormat? GetLinkFormat(string value)
    {
        if (value == null)
        {
            return null;
        }

        if (!Enum.TryParse<LinkFormat>(value, out var linkFormat))
        {
            throw new ConfigurationException("Failed to parse LinkFormat:" + linkFormat);
        }

        return linkFormat;
    }
}