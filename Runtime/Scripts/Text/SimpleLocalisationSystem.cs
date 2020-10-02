using System;
using System.Collections.Generic;
using UnityEngine;
using m039.Common;

namespace m039.Common
{
	public class SimpleLocalisationSystem
	{
		const string LocalisationCsvFilename = "Localisation";

		readonly Dictionary<string, int> _languageToIndex = new Dictionary<string, int>();

		readonly Dictionary<string, int> _keyToIndex = new Dictionary<string, int>();

		List<List<string>> _csvData;

		readonly public List<string> Languages = new List<string>();

		public string CurrentLanguage { get; set; }

		public void LoadData(string filename = LocalisationCsvFilename)
		{
			var csvText = Resources.Load<TextAsset>(filename);
			if (csvText != null)
			{
				var csvData = new SimpleCSVReader().ParseCsv(csvText.text);
				InitData(csvData);
			}
		}

		private void InitData(List<List<string>> csvData)
		{
			_languageToIndex.Clear();
			_keyToIndex.Clear();
			Languages.Clear();
			CurrentLanguage = null;
			_csvData = csvData;

			for (int i = 1; i < _csvData[0].Count; i++)
			{
				var language = _csvData[0][i];
				_languageToIndex.Add(language, i);

				// Select first language as the current language.
				if (CurrentLanguage == null)
				{
					CurrentLanguage = language;
				}

				// Add an language to the list of all languages.
				Languages.Add(language);
			}

			for (int i = 1; i < _csvData.Count; i++)
			{
				_keyToIndex.Add(_csvData[i][0], i);
			}
		}

		public void SelectPrevLanguage()
		{
			var indexOfCurrentLanguage = Languages.IndexOf(CurrentLanguage);
			if (indexOfCurrentLanguage == -1)
				return;

			indexOfCurrentLanguage -= 1;
			if (indexOfCurrentLanguage == -1)
			{
				indexOfCurrentLanguage = Languages.Count - 1;
			}

			CurrentLanguage = Languages[indexOfCurrentLanguage];
		}

		public void SelectNextLanguage()
		{
			var indexOfCurrentLanguage = Languages.IndexOf(CurrentLanguage);
			if (indexOfCurrentLanguage == -1)
				return;

			indexOfCurrentLanguage += 1;
			if (indexOfCurrentLanguage == Languages.Count)
			{
				indexOfCurrentLanguage = 0;
			}

			CurrentLanguage = Languages[indexOfCurrentLanguage];
		}

		public string this[string key]
		{
			get
			{
				return GetString(CurrentLanguage, key);
			}
		}

		private string GetString(string language, string key)
		{
			if (_csvData == null)
				throw new ArgumentNullException();

			return _csvData[_keyToIndex[key]][_languageToIndex[language]];
		}
	}
}
