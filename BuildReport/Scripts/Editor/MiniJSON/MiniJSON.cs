/*
 * Copyright (c) 2013 Calvin Rien
 *
 * Based on the JSON parser by Patrick van Bergen
 * http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html
 *
 * Simplified it so that it doesn't throw exceptions
 * and can be used in Unity iPhone with maximum code stripping.
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MiniJSON
{
    // Example usage:
    //
    //  using UnityEngine;
    //  using System.Collections;
    //  using System.Collections.Generic;
    //  using MiniJSON;
    //
    //  public class MiniJSONTest : MonoBehaviour {
    //      void Start () {
    //          var jsonString = "{ \"array\": [1.44,2,3], " +
    //                          "\"object\": {\"key1\":\"value1\", \"key2\":256}, " +
    //                          "\"string\": \"The quick brown fox \\\"jumps\\\" over the lazy dog \", " +
    //                          "\"unicode\": \"\\u3041 Men\u00fa sesi\u00f3n\", " +
    //                          "\"int\": 65536, " +
    //                          "\"float\": 3.1415926, " +
    //                          "\"bool\": true, " +
    //                          "\"null\": null }";
    //
    //          var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;
    //
    //          Debug.Log("deserialized: " + dict.GetType());
    //          Debug.Log("dict['array'][0]: " + ((List<object>) dict["array"])[0]);
    //          Debug.Log("dict['string']: " + (string) dict["string"]);
    //          Debug.Log("dict['float']: " + (double) dict["float"]); // floats come out as doubles
    //          Debug.Log("dict['int']: " + (long) dict["int"]); // ints come out as longs
    //          Debug.Log("dict['unicode']: " + (string) dict["unicode"]);
    //
    //          var str = Json.Serialize(dict);
    //
    //          Debug.Log("serialized: " + str);
    //      }
    //  }

    /// <summary>
    /// This class encodes and decodes JSON strings.
    /// Spec. details, see http://www.json.org/
    ///
    /// JSON uses Arrays and Objects. These correspond here to the datatypes IList and IDictionary.
    /// All numbers are parsed to doubles.
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>An List&lt;object&gt;, a Dictionary&lt;string, object&gt;, a double, an integer,a string, null, true, or false</returns>
        public static object Deserialize(string json)
        {
            // save the string for debug information
            if (string.IsNullOrEmpty(json)) return null;

            return Parser.Parse(json);
        }

        private sealed class Parser : IDisposable
        {
            private const string WORD_BREAK = "{}[],:\"";

            public static bool IsWordBreak(char c)
            {
                return char.IsWhiteSpace(c) || WORD_BREAK.IndexOf(c) != -1;
            }

            private const string HEX_DIGIT = "0123456789ABCDEFabcdef";

            public static bool IsHexDigit(char c)
            {
                return HEX_DIGIT.IndexOf(c) != -1;
            }

            private enum TOKEN
            {
                NONE,
                CURLY_OPEN,
                CURLY_CLOSE,
                SQUARED_OPEN,
                SQUARED_CLOSE,
                COLON,
                COMMA,
                STRING,
                NUMBER,
                TRUE,
                FALSE,
                NULL,
            };

            private StringReader json;

            private Parser(string jsonString)
            {
                this.json = new(jsonString);
            }

            public static object Parse(string jsonString)
            {
                using (var instance = new Parser(jsonString)) return instance.ParseValue();
            }

            public void Dispose()
            {
                this.json.Dispose();
                this.json = null;
            }

            private Dictionary<string, object> ParseObject()
            {
                var table = new Dictionary<string, object>();

                // ditch opening brace
                this.json.Read();

                // {
                while (true)
                {
                    switch (this.NextToken)
                    {
                        case TOKEN.NONE:        return null;
                        case TOKEN.COMMA:       continue;
                        case TOKEN.CURLY_CLOSE: return table;
                        case TOKEN.STRING:
                            // name
                            var name = this.ParseString();
                            if (name == null) return null;

                            // :
                            if (this.NextToken != TOKEN.COLON) return null;

                            // ditch the colon
                            this.json.Read();

                            // value
                            var valueToken = this.NextToken;
                            var value      = this.ParseByToken(valueToken);
                            if (value == null && valueToken != TOKEN.NULL) return null;
                            table[name] = value;
                            break;
                        default: return null;
                    }
                }
            }

            private List<object> ParseArray()
            {
                List<object> array = null;

                // ditch opening bracket
                this.json.Read();

                // [
                var parsing = true;
                while (parsing)
                {
                    var nextToken = this.NextToken;

                    switch (nextToken)
                    {
                        case TOKEN.NONE:  return null;
                        case TOKEN.COMMA: continue;
                        case TOKEN.SQUARED_CLOSE:
                            parsing = false;
                            break;
                        default:
                            var value = this.ParseByToken(nextToken);
                            if (value == null && nextToken != TOKEN.NULL) return null;
                            if (array == null) array = new();
                            array.Add(value);
                            break;
                    }
                }

                if (array == null) array = new();
                return array;
            }

            private object ParseValue()
            {
                var nextToken = this.NextToken;
                return this.ParseByToken(nextToken);
            }

            private object ParseByToken(TOKEN token)
            {
                switch (token)
                {
                    case TOKEN.STRING:       return this.ParseString();
                    case TOKEN.NUMBER:       return this.ParseNumber();
                    case TOKEN.CURLY_OPEN:   return this.ParseObject();
                    case TOKEN.SQUARED_OPEN: return this.ParseArray();
                    case TOKEN.TRUE:         return true;
                    case TOKEN.FALSE:        return false;
                    case TOKEN.NULL:         return null;
                    default:                 return null;
                }
            }

            private string ParseString()
            {
                var  s = new StringBuilder();
                char c;

                // ditch opening quote
                this.json.Read();

                var parsing = true;
                while (parsing)
                {
                    if (this.json.Peek() == -1)
                    {
                        parsing = false;
                        break;
                    }

                    c = this.NextChar;
                    switch (c)
                    {
                        case '"':
                            parsing = false;
                            break;
                        case '\\':
                            if (this.json.Peek() == -1)
                            {
                                parsing = false;
                                break;
                            }

                            c = this.NextChar;
                            switch (c)
                            {
                                case '"':
                                case '\\':
                                case '/':
                                    s.Append(c);
                                    break;
                                case 'b':
                                    s.Append('\b');
                                    break;
                                case 'f':
                                    s.Append('\f');
                                    break;
                                case 'n':
                                    s.Append('\n');
                                    break;
                                case 'r':
                                    s.Append('\r');
                                    break;
                                case 't':
                                    s.Append('\t');
                                    break;
                                case 'u':
                                    var hex = new char[4];

                                    for (var i = 0; i < 4; i++)
                                    {
                                        hex[i] = this.NextChar;
                                        if (!IsHexDigit(hex[i])) return null;
                                    }

                                    s.Append((char)Convert.ToInt32(new(hex), 16));
                                    break;
                            }
                            break;
                        default:
                            s.Append(c);
                            break;
                    }
                }

                return s.ToString();
            }

            private object ParseNumber()
            {
                var number = this.NextWord;

                if (number.IndexOf('.') == -1 && number.IndexOf('E') == -1 && number.IndexOf('e') == -1)
                {
                    long parsedInt;
                    long.TryParse(number, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out parsedInt);
                    if (parsedInt == 0)
                    {
                        ulong parsedUInt;
                        ulong.TryParse(number, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out parsedUInt);
                        return parsedUInt;
                    }
                    return parsedInt;
                }

                double parsedDouble;
                double.TryParse(number, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out parsedDouble);
                return parsedDouble;
            }

            private void EatWhitespace()
            {
                while (char.IsWhiteSpace(this.PeekChar))
                {
                    this.json.Read();

                    if (this.json.Peek() == -1) break;
                }
            }

            private char PeekChar => Convert.ToChar(this.json.Peek());

            private char NextChar => Convert.ToChar(this.json.Read());

            private string NextWord
            {
                get
                {
                    var word = new StringBuilder();

                    while (!IsWordBreak(this.PeekChar))
                    {
                        word.Append(this.NextChar);

                        if (this.json.Peek() == -1) break;
                    }

                    return word.ToString();
                }
            }

            private TOKEN NextToken
            {
                get
                {
                    this.EatWhitespace();

                    if (this.json.Peek() == -1) return TOKEN.NONE;

                    switch (this.PeekChar)
                    {
                        case '{': return TOKEN.CURLY_OPEN;
                        case '}':
                            this.json.Read();
                            return TOKEN.CURLY_CLOSE;
                        case '[': return TOKEN.SQUARED_OPEN;
                        case ']':
                            this.json.Read();
                            return TOKEN.SQUARED_CLOSE;
                        case ',':
                            this.json.Read();
                            return TOKEN.COMMA;
                        case '"': return TOKEN.STRING;
                        case ':': return TOKEN.COLON;
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case '-':
                            return TOKEN.NUMBER;
                    }

                    switch (this.NextWord)
                    {
                        case "false": return TOKEN.FALSE;
                        case "true":  return TOKEN.TRUE;
                        case "null":  return TOKEN.NULL;
                    }

                    return TOKEN.NONE;
                }
            }
        }

        /// <summary>
        /// Converts a IDictionary / IList object or a simple type (string, int, etc.) into a JSON string
        /// </summary>
        /// <param name="obj">A Dictionary&lt;string, object&gt; / List&lt;object&gt;</param>
        /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
        public static string Serialize(object obj)
        {
            return Serializer.Serialize(obj);
        }

        private sealed class Serializer
        {
            private StringBuilder builder;

            private Serializer()
            {
                this.builder = new();
            }

            public static string Serialize(object obj)
            {
                var instance = new Serializer();

                instance.SerializeValue(obj);

                return instance.builder.ToString();
            }

            private void SerializeValue(object value)
            {
                IList       asList;
                IDictionary asDict;
                string      asStr;

                if (value == null)
                    this.builder.Append("null");
                else if ((asStr = value as string) != null)
                    this.SerializeString(asStr);
                else if (value is bool)
                    this.builder.Append((bool)value ? "true" : "false");
                else if ((asList = value as IList) != null)
                    this.SerializeArray(asList);
                else if ((asDict = value as IDictionary) != null)
                    this.SerializeObject(asDict);
                else if (value is char)
                    this.SerializeString(new((char)value, 1));
                else
                    this.SerializeOther(value);
            }

            private void SerializeObject(IDictionary obj)
            {
                var first = true;

                this.builder.Append('{');

                foreach (var e in obj.Keys)
                {
                    if (!first) this.builder.Append(',');

                    this.SerializeString(e.ToString());
                    this.builder.Append(':');

                    this.SerializeValue(obj[e]);

                    first = false;
                }

                this.builder.Append('}');
            }

            private void SerializeArray(IList anArray)
            {
                this.builder.Append('[');

                var first = true;

                for (var i = 0; i < anArray.Count; i++)
                {
                    var obj = anArray[i];
                    if (!first) this.builder.Append(',');

                    this.SerializeValue(obj);

                    first = false;
                }

                this.builder.Append(']');
            }

            private void SerializeString(string str)
            {
                this.builder.Append('\"');

                var charArray = str.ToCharArray();
                for (var i = 0; i < charArray.Length; i++)
                {
                    var c = charArray[i];
                    switch (c)
                    {
                        case '"':
                            this.builder.Append("\\\"");
                            break;
                        case '\\':
                            this.builder.Append("\\\\");
                            break;
                        case '\b':
                            this.builder.Append("\\b");
                            break;
                        case '\f':
                            this.builder.Append("\\f");
                            break;
                        case '\n':
                            this.builder.Append("\\n");
                            break;
                        case '\r':
                            this.builder.Append("\\r");
                            break;
                        case '\t':
                            this.builder.Append("\\t");
                            break;
                        default:
                            var codepoint = Convert.ToInt32(c);
                            if (codepoint >= 32 && codepoint <= 126)
                            {
                                this.builder.Append(c);
                            }
                            else
                            {
                                this.builder.Append("\\u");
                                this.builder.Append(codepoint.ToString("x4"));
                            }
                            break;
                    }
                }

                this.builder.Append('\"');
            }

            private void SerializeOther(object value)
            {
                // NOTE: decimals lose precision during serialization.
                // They always have, I'm just letting you know.
                // Previously floats and doubles lost precision too.
                if (value is float)
                {
                    var numberToString = ((float)value).ToString("R", System.Globalization.CultureInfo.InvariantCulture);
                    if (numberToString.IndexOf('.') < 0 && numberToString.IndexOf("E", StringComparison.OrdinalIgnoreCase) < 0)
                        // if whole number, add a ".0" at the end
                        numberToString = string.Format("{0}.0", numberToString);
                    this.builder.Append(numberToString);
                }
                else if (value is int
                    || value is uint
                    || value is long
                    || value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is ulong)
                {
                    this.builder.Append(value);
                }
                else if (value is double
                    || value is decimal)
                {
                    var numberToString = Convert.ToDouble(value).ToString("R", System.Globalization.CultureInfo.InvariantCulture);
                    if (numberToString.IndexOf('.') < 0 && numberToString.IndexOf("E", StringComparison.OrdinalIgnoreCase) < 0)
                        // if whole number, add a ".0" at the end
                        numberToString = string.Format("{0}.0", numberToString);
                    this.builder.Append(numberToString);
                }
                else
                {
                    #if UNITY_5_3_OR_NEWER
                    this.builder.Append(UnityEngine.JsonUtility.ToJson(value));
                    #endif
                }
            }
        }
    }
}