using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace m039.Common
{
	public class SimpleCSVReader
	{
		// The code is havily borrowed from the https://github.com/agens-no/PolyglotUnity project.

		class Reader
		{
			enum ParsingMode
			{
				InQuote,
				OutQuote
			}

			readonly List<List<string>> _result = new List<List<string>>();

			readonly List<string> _line = new List<string>();

			readonly StringBuilder _buffer = new StringBuilder();

			public List<List<string>> Parse(string input)
			{
				Init();

				int length = input.Length;
				char c1;
				char c2;
				ParsingMode mode = ParsingMode.OutQuote;
				bool requireTrimLineHead = false;
				var isBlank = new Regex(@"\s");

				for (int i = 0; i < length - 1; i++)
				{
					c1 = input[i];
					c2 = input[i + 1];

					// Remove whitespace at beginning of a line
					if (requireTrimLineHead)
					{
						if (isBlank.IsMatch(c1.ToString()))
						{
							continue;
						}

						requireTrimLineHead = false;
					}

					switch (mode)
					{
						case ParsingMode.OutQuote:
							if (c1 == '"')
							{
								mode = ParsingMode.InQuote;
								continue;
							}
							else if (c1 == ',')
							{
								_line.Add(_buffer.ToString());
								_buffer.Clear();
							}
							else if (c1 == '\r' && c2 == '\n')
							{
								// New line (CR+LF)

								_line.Add(_buffer.ToString());
								_result.Add(_line);
                                _line.Clear();
								_buffer.Clear();
								i++; // Skip next iteration.

								requireTrimLineHead = true;
							}
							else if (c1 == '\n' || c1 == '\r')
							{
								// New line

								_line.Add(_buffer.ToString());
								_result.Add(_line);
                                _line.Clear();
								_buffer.Clear();

								requireTrimLineHead = true;
							}
							else
							{
								_buffer.Append(c1);
							}
							break;

						case ParsingMode.InQuote:
							if (c1 == '"' && c2 != '"')
							{
								mode = ParsingMode.OutQuote;
							}
							else if (c1 == '"' && c2 == '"')
							{
								_buffer.Append('"');
								i++;
							}
							else
							{
								_buffer.Append(c1);
							}
							break;
					}
				}

				// The final step.

				c1 = input[length - 1];

				switch (mode)
				{
					case ParsingMode.OutQuote:
						if (c1 == ',')
						{
							_line.Add(_buffer.ToString());
							_line.Add(string.Empty);
							_result.Add(_line);
							return _result;
						}

						if (_line.Count <= 0 && string.Empty.Equals(c1.ToString().Trim()))
						{
							return _result;
						}

						_buffer.Append(c1);
						_line.Add(_buffer.ToString());
						_result.Add(_line);
						break;

					case ParsingMode.InQuote:
						if (c1 != '"')
						{
							// Closing quote is missing.
							_buffer.Append(c1);
						}

						_line.Add(_buffer.ToString());
						_result.Add(_line);
						break;
				}

				return _result;
			}

			void Init()
			{
				_result.Clear();
                _line.Clear();
				_buffer.Clear();
			}
		}

		public List<List<string>> ParseCsv(string input)
		{
			return new Reader().Parse(input);
		}

	}
}
