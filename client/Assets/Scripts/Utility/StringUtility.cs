using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

public class StringUtility
{
    // Convert underscore naming to camel case naming
    public static string UnderscoreToCamelCase(string input)
    {
        // If the input is null or empty, return it directly
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Split the input by underscore into a string array
        string[] words = input.Split('_');

        // Create a string builder to concatenate the result
        StringBuilder result = new StringBuilder();

        // Iterate through the string array, capitalize the first letter of each word, lowercase the rest, and add them to the result
        foreach (string word in words)
        {
            // If the word is empty, skip it
            if (string.IsNullOrEmpty(word))
            {
                continue;
            }

            // Capitalize the first letter of the word, lowercase the rest, and add them to the result
            result.Append(char.ToUpper(word[0]) + word.Substring(1).ToLower());
        }

        // Return the result string
        return result.ToString();
    }

    // Convert camel case naming to underscore naming
    public static string CamelCaseToUnderscore(string input)
    {
        // If the input is null or empty, return it directly
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Create a string builder to concatenate the result
        StringBuilder result = new StringBuilder();

        // Iterate through the input string, add an underscore before each uppercase letter, and add them to the result
        for (int i = 0; i < input.Length; i++)
        {
            // If the current character is an uppercase letter, and it is not the first character, add an underscore before it
            if (char.IsUpper(input[i]) && i > 0)
            {
                result.Append('_');
            }

            // Convert the current character to lowercase, and add it to the result
            result.Append(char.ToLower(input[i]));
        }

        // Return the result string
        return result.ToString();
    }
}
