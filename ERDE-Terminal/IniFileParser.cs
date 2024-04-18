using System;
using System.Collections.Generic;
using System.IO;

namespace ERDE_Terminal
{
    public class IniFileParser
    {
        public Dictionary<string, Dictionary<string, string>> ReadIniFile(string path)
        {
            var iniData = new Dictionary<string, Dictionary<string, string>>();
            string currentSection = null;

            foreach (var line in File.ReadAllLines(path))
            {
                string trimmedLine = line.Trim();
                if (trimmedLine.StartsWith(";") || string.IsNullOrEmpty(trimmedLine))
                    continue; // Skip comments and empty lines

                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                    iniData[currentSection] = new Dictionary<string, string>();
                }
                else if (currentSection != null)
                {
                    var keyValue = trimmedLine.Split(['='], 2);
                    if (keyValue.Length == 2)
                    {
                        iniData[currentSection][keyValue[0].Trim()] = keyValue[1].Trim();
                    }
                }
            }

            return iniData;
        }
    } 
}