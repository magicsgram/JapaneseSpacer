using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Windows.Globalization;

namespace JapaneseSpacer
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private DispatcherTimer typeTimer;
    private Int32 fontSize = 20;
    private Int32 lineSpaceX10 = 20;
    private String fontFamilyName = "Yu Gothic UI";

    public List<FontFamily> SortedFontList
    {
      get
      {
        var fontFamiliesList = Fonts.SystemFontFamilies.ToList();
        fontFamiliesList.Sort((x, y) => x.Source.CompareTo(y.Source));
        return fontFamiliesList;
      }
    }

    private HashSet<Char> katakanaSet = new HashSet<Char>();

    public MainWindow()
    {
      DataContext = this;
      InitializeComponent();

      foreach (FontFamily font in fontComboBox.Items)
      {
        if (font.FamilyNames.Values.Contains("Yu Gothic UI"))
        {
          fontComboBox.SelectedItem = font;
          break;
        }
      }

      BuildKatakanaSet();

      // Timer triggers when the user does not edit the textbox for 1 second
      typeTimer = new DispatcherTimer();
      typeTimer.Interval = TimeSpan.FromSeconds(1.0);
      typeTimer.Tick += OnTypeTimer_Tick;
      resultBrowser.DocumentCompleted += (sender, e) =>
      {
        ApplyStyle();
      };
    }

    private void BuildKatakanaSet()
    {
      String katakanaLetters = "ァアィイゥウェエォオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂッツヅテデトドナニヌネノハバパヒビピフブプヘベペホボポマミムメモャヤュユョヨラリルレロヮワヰヱヲンヴヵヶヷヸヹヺーヽヾヿ";
      foreach (Char c in katakanaLetters)
        katakanaSet.Add(c);
    }

    private void ApplyStyle()
    {
      if (resultBrowser.Document != null)
        if (resultBrowser.Document.Body != null)
        {
          resultBrowser.Document.Body.Style = $"font-size: {fontSize}pt;" +
                                              $"line-height:{lineSpaceX10 / 10f};" +
                                              $"font-family:{fontFamilyName};" +
                                              $"text-shadow: 0 0 1px rgba(0,0,0,0.3);";
        }
    }

    private void PerformSpacing()
    {
      if (!IsInitialized)
        return;

      Boolean shouldShowFurigana = furiganaCheckBox.IsChecked.Value;

      String inputText = inputTextBox.Text.Trim();
      // Perform very rigorous sentence splitter,
      // because JapanesePhoneticAnalyzer doesn't accept a sentence w/ length greater than 100.
      List<String> sentences = new List<String>();
      Int32 sentenceStartingIndex = 0;
      for (Int32 i = 0; i < inputText.Length; ++i)
      {
        if (!Char.IsLetterOrDigit(inputText[i])) // Sentence split detected
        {
          String newSentence = inputText.Substring(sentenceStartingIndex, i - sentenceStartingIndex + 1);
          sentences.Add(newSentence);
          sentenceStartingIndex = i + 1;
        }
      }
      if (sentenceStartingIndex < inputText.Length) // Clean up needed
      {
        String newSentence = inputText.Substring(sentenceStartingIndex, inputText.Length - sentenceStartingIndex);
        sentences.Add(newSentence);
      }

      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<meta charset=\"utf-8\"/>");
      foreach (String sentence in sentences)
      {
        var phonemes = JapanesePhoneticAnalyzer.GetWords(sentence, false).ToList();
        JapanesePhoneme previousPhoneme = null;
        foreach (var phoneme in phonemes)
        {
          if (phoneme.IsPhraseStart)
            if (Char.IsLetter(phoneme.DisplayText[0])
              && Char.IsLetter(previousPhoneme == null? '-' : previousPhoneme.DisplayText.Last())) // Make sure we don't add spaces after non-letter characters.
              stringBuilder.Append("<ruby>　</ruby>");
          previousPhoneme = phoneme;

          // Original text part
          stringBuilder.Append($"<ruby><rb>{phoneme.DisplayText}</rb>");
          // Furigana part
          if (shouldShowFurigana && phoneme.DisplayText != phoneme.YomiText
            && !katakanaSet.Contains(phoneme.DisplayText[0]) && Char.IsLetter(phoneme.DisplayText[0])) // Furigana needs to be displayed
            stringBuilder.Append($"<rt>{phoneme.YomiText}</rt>");
          stringBuilder.Append("</ruby>");
        }
      }
      stringBuilder.Append("</p>");
      String resultString = stringBuilder.ToString();
      resultBrowser.DocumentText = resultString;
    }

    private void OnTypeTimer_Tick(Object sender, Object e)
    {
      typeTimer.Stop();
      PerformSpacing();
    }

    private void OnTextBox_TextChanged(Object sender, TextChangedEventArgs e)
    {
      // Restart typeTimer
      typeTimer.Stop();
      typeTimer.Start();
    }

    private void OnShowFuriganaCheckBox_CheckedChanged(Object sender, RoutedEventArgs e)
    {
      PerformSpacing();
    }

    private void OnFontSizeUpButton_Click(Object sender, RoutedEventArgs e)
    {
      fontSize += 1;
      ApplyStyle();
    }

    private void OnFontsizeDownButton_Click(Object sender, RoutedEventArgs e)
    {
      if (fontSize > 8)
      {
        fontSize -= 1;
        ApplyStyle();
      }
    }

    private void OnLineUpButton_Click(Object sender, RoutedEventArgs e)
    {
      lineSpaceX10 += 1;
      Trace.WriteLine(lineSpaceX10);
      ApplyStyle();
    }

    private void OnLineDownButton_Click(Object sender, RoutedEventArgs e)
    {
      if (lineSpaceX10 > 20)
      {
        lineSpaceX10 -= 1;
        ApplyStyle();
      }
    }

    private void OnFontComboBox_SelectionChanged(Object sender, SelectionChangedEventArgs e)
    {
      var fontFamily = e.AddedItems[0] as FontFamily;
      fontFamilyName = fontFamily.Source;
      ApplyStyle();
    }
  }
}
