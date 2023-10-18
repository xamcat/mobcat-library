using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Microsoft.Maui;

namespace Microsoft.MobCAT.Forms.Controls
{
    internal enum MarkdownFormatType
    {
        Bold,
        Italic,
        Hyperlink,
        BulletList,
        NumberList,
        NewLineFormatting
    }

    internal class MarkdownParseResult
    {
        internal MarkdownFormatType Type { get; set; }
        internal Match RegexMatch { get; set; }
    }

    /// <summary>
    /// A <see cref="Label"/> that supports a limited subset of Markdown for rendering short markdown snippets.
    /// </summary>
    /// <remarks>This uses a very simple and basic technique for rendering the markdown as a <see cref="FormattedString"/>. It it intended for limited use.</remarks>
    public class MarkdownLabel : Label
    {
        const string BoldMarkdownRegexFormat = @"\*\*[^*]*\*\*";
        const string ItalicMarkdownRegexFormat = @"_[^*]_";
        const string HyperlinkMarkdownRegexFormat = @"(\[(.*?)\])(\((.*?)\))"; // Get group 2 for link text, 4 for the link itself
        const string BulletListMarkdownRegexFormat = @"((?<![\*-+]|[-*]|[+*]|[\**])([\*+-])\s)(.*)";
        const string NumberedListRegexFormat = @"((?<!\*)\d\d?\d?\.\s)(.*)"; // Get group 0 for actual number
        const string NewLineRegexFormat = @"(\n)";

        Dictionary<MarkdownFormatType, Func<Match, Span>> _markdownFormatters;

        /// <summary>
        /// Backing store for the <see cref="Text"/> bindable property.
        /// </summary>
        public new static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(MarkdownLabel), propertyChanged: TextPropertyChanged);

        /// <summary>
        /// Backing store for the <see cref="TextColor"/> bindable property.
        /// </summary>
        public new static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(MarkdownLabel), propertyChanged: TextColorPropertyChanged);

        /// <summary>
        /// Backing store for the <see cref="HyperlinkActionCommand"/> bindable property. 
        /// </summary>
        public static readonly BindableProperty HyperlinkActionCommandProperty = BindableProperty.Create(nameof(HyperlinkActionCommand), typeof(ICommand), typeof(MarkdownLabel), propertyChanged: HyperlinkActionCommandChanged);

        /// <summary>
        /// Initializes a new instances of the <see cref="MarkdownLabel"/> class.
        /// </summary>
        public MarkdownLabel()
        {
            _markdownFormatters = new Dictionary<MarkdownFormatType, Func<Match, Span>>
            {
                { MarkdownFormatType.Bold, FormatBoldText },
                { MarkdownFormatType.Italic, FormatItalicText },
                { MarkdownFormatType.Hyperlink, FormatHyperlinkText },
                { MarkdownFormatType.BulletList, FormatBulletListText },
                { MarkdownFormatType.NumberList, FormatNumberListText },
                { MarkdownFormatType.NewLineFormatting, FormatNewLineText }
            };
        }

        /// <summary>
        /// Gets or sets the text for the Label.
        /// </summary>
        /// <remarks>The value can be plain text, markdown or both.</remarks>
        public new string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="Color"/> for the text of this Label.
        /// </summary>
        public new Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the command to invoke when a hyperlink is activated.
        /// </summary>
        public ICommand HyperlinkActionCommand
        {
            get { return (ICommand)GetValue(HyperlinkActionCommandProperty); }
            set { SetValue(HyperlinkActionCommandProperty, value); }
        }

        void RenderMarkdownText()
        {
            if (!TextContainsMarkdown(Text))
            {
                FormattedText = new FormattedString();
                FormattedText.Spans.Add(new Span { Text = Text, TextColor = TextColor });
                return;
            }

            FormattedText = ParseMarkdownText(Text);
        }

        Span FormatPlainText(string text) => new Span { Text = text, TextColor = TextColor };

        Span FormatNewLineText(Match match) => new Span { Text = Environment.NewLine };

        Span FormatBoldText(Match match) => new Span { FontAttributes = FontAttributes.Bold, TextColor = TextColor, Text = match.Value.Replace("**", string.Empty) };

        Span FormatItalicText(Match match) => new Span { FontAttributes = FontAttributes.Italic, TextColor = TextColor, Text = match.Value.Replace("__", string.Empty) };

        Span FormatBulletListText(Match match) => new Span { Text = "\u2022", TextColor = TextColor };

        Span FormatNumberListText(Match match) => new Span { Text = match.Groups.Count == 2 ? match.Groups[0].Value : match.Value, TextColor = TextColor }; // TODO: Consider handling situations where the numbering specified is not in correct sequence!

        Span FormatHyperlinkText(Match match)
        {
            Span span = new Span
            {
                TextColor = Colors.Blue,
                TextDecorations = TextDecorations.Underline,
                Text = match.Groups.Count >= 5 ? match.Groups[2].Value : match.Value
            };

            if (HyperlinkActionCommand != null)
            {
                span.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = HyperlinkActionCommand,
                    CommandParameter = match.Groups.Count >= 5 ? match.Groups[4].Value : match.Value
                });
            }

            return span;
        }

        FormattedString ParseMarkdownText(string markdown)
        {
            FormattedString formattedString = new FormattedString();

            // Clear escape characters
            var unescapatedMarkdown = markdown.Replace("\\\"", "\"");

            // Iterate over string to build up FormattedString.Spans
            var matches = GetMarkdownMatches(unescapatedMarkdown);
            var lastMatch = matches.LastOrDefault();
            int currentIndex = 0;

            foreach (var match in matches)
            {
                var startIndex = match.RegexMatch.Index;
                var endIndex = startIndex + match.RegexMatch.Length;

                var currentMatchIndex = (matches.IndexOf(match));
                var nextMatchIndex = currentMatchIndex++;

                if (match.Type == MarkdownFormatType.BulletList || match.Type == MarkdownFormatType.NumberList)
                    endIndex = startIndex + match.RegexMatch.Value.IndexOfAny(new char[] { '*', '+', '-' }) + (match.Type == MarkdownFormatType.BulletList ? 1 : 2);

                if (startIndex > currentIndex)
                {
                    var previousText = unescapatedMarkdown.Substring(currentIndex, startIndex - currentIndex);
                    formattedString.Spans.Add(FormatPlainText(previousText));
                }

                // TODO: Look ahead to see if there is further formatting (cross-over). If so, set endIndex to next start
                if (_markdownFormatters.ContainsKey(match.Type))
                    formattedString.Spans.Add(_markdownFormatters[match.Type](match.RegexMatch));
                else
                    formattedString.Spans.Add(FormatPlainText(match.RegexMatch.Value));

                currentIndex = endIndex;

                if (match == lastMatch)
                {
                    var trailingTextStartIndex = currentIndex;
                    var trailingTextLength = unescapatedMarkdown.Length - trailingTextStartIndex;
                    var trailingText = unescapatedMarkdown.Substring(trailingTextStartIndex, trailingTextLength);
                    formattedString.Spans.Add(FormatPlainText(trailingText));
                }
            }

            return formattedString;
        }

        bool TextContainsMarkdown(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            var boldFormatting = Regex.IsMatch(text, BoldMarkdownRegexFormat);
            var italicFormatting = Regex.IsMatch(text, ItalicMarkdownRegexFormat);
            var hyperlinkFormatting = Regex.IsMatch(text, HyperlinkMarkdownRegexFormat);
            var bulletListFormatting = Regex.IsMatch(text, BulletListMarkdownRegexFormat);
            var numberListFormatting = Regex.IsMatch(text, NumberedListRegexFormat);
            var newLineFormatting = Regex.IsMatch(text, NewLineRegexFormat);

            return boldFormatting ||
                   italicFormatting ||
                   hyperlinkFormatting ||
                   bulletListFormatting ||
                   numberListFormatting ||
                   newLineFormatting;
        }

        IList<MarkdownParseResult> GetMarkdownMatches(string markdown)
        {
            var boldFormatting = Regex.Matches(markdown, BoldMarkdownRegexFormat);
            var italicFormatting = Regex.Matches(markdown, ItalicMarkdownRegexFormat);
            var hyperlinkFormatting = Regex.Matches(markdown, HyperlinkMarkdownRegexFormat);
            var bulletListFormatting = Regex.Matches(markdown, BulletListMarkdownRegexFormat);
            var numberListFormatting = Regex.Matches(markdown, NumberedListRegexFormat);
            var newLineFormatting = Regex.Matches(markdown, NewLineRegexFormat);

            var results = new List<MarkdownParseResult>();

            ProcessMarkdownMatch(results, boldFormatting, MarkdownFormatType.Bold);
            ProcessMarkdownMatch(results, italicFormatting, MarkdownFormatType.Italic);
            ProcessMarkdownMatch(results, hyperlinkFormatting, MarkdownFormatType.Hyperlink);
            ProcessMarkdownMatch(results, bulletListFormatting, MarkdownFormatType.BulletList);
            ProcessMarkdownMatch(results, numberListFormatting, MarkdownFormatType.NumberList);
            ProcessMarkdownMatch(results, newLineFormatting, MarkdownFormatType.NewLineFormatting);

            return results.OrderBy(i => i.RegexMatch.Index).ToList();
        }

        void ProcessMarkdownMatch(IList<MarkdownParseResult> results, MatchCollection matches, MarkdownFormatType formatType)
        {
            foreach (Match match in matches)
                results.Add(new MarkdownParseResult { Type = formatType, RegexMatch = match });
        }

        static void TextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
            => (bindable as MarkdownLabel).RenderMarkdownText();

        static void TextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
            => (bindable as MarkdownLabel).RenderMarkdownText();

        static void HyperlinkActionCommandChanged(BindableObject bindable, object oldValue, object newValue)
            => (bindable as MarkdownLabel).RenderMarkdownText();
    }
}