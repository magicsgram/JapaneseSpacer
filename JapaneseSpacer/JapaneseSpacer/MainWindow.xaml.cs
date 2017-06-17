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
    private Int32 zoomLevelPercent = 130;
    private Int32 lineSpaceX10 = 20;

    private HashSet<Char> katakanaSet = new HashSet<Char>();

    public MainWindow()
    {
      String katakanaLetters = "ァアィイゥウェエォオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂッツヅテデトドナニヌネノハバパヒビピフブプヘベペホボポマミムメモャヤュユョヨラリルレロヮワヰヱヲンヴヵヶヷヸヹヺーヽヾヿ";
      foreach (Char c in katakanaLetters)
        katakanaSet.Add(c);

      DataContext = this;
      InitializeComponent();
      typeTimer = new DispatcherTimer();
      typeTimer.Interval = TimeSpan.FromSeconds(1.0);
      typeTimer.Tick += OnTypeTimer_Tick;
      resultBrowser.DocumentCompleted += (sender, e) =>
      {
        ApplyStyle();
      };
    }

    private void ApplyStyle()
    {
      if (resultBrowser.Document.Body != null)
        resultBrowser.Document.Body.Style = $"zoom:{zoomLevelPercent}%;line-height:{lineSpaceX10 / 10f};";
    }

    private void PerformSpacing()
    {
      if (!IsInitialized)
        return;

      Boolean shouldShowFurigana = furiganaCheckBox.IsChecked.Value;

      String inputText = inputTextBox.Text;
      String[] sentences = inputText.Split('。');

      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<meta charset=\"utf-8\"/>");
      for (Int32 i = 0; i < sentences.Length; ++i)
      {
        String sentence = sentences[i];
        if (i < sentence.Length - 1) // if not last sentence, append period
          sentence += "。";

        var phonemes = JapanesePhoneticAnalyzer.GetWords(sentence, false).ToList();
        foreach (var phoneme in phonemes)
        {
          if (phoneme.IsPhraseStart)
            if (Char.IsLetter(phoneme.DisplayText[0]))
              stringBuilder.Append("<ruby>　</ruby>");

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
      zoomLevelPercent += 10;
      ApplyStyle();
    }

    private void OnFontsizeDownButton_Click(Object sender, RoutedEventArgs e)
    {
      if (zoomLevelPercent > 100)
      {
        zoomLevelPercent -= 10;
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
  }
}
