//using System;
//using System.Globalization;
//using System.Text.RegularExpressions;
//using System.Windows.Data;

//namespace MusicMetadata
//{
//    [ValueConversion(typeof(TrackMetadataViewModel), typeof(string))]
//    internal class TrackNumberConverter : IValueConverter
//    {
//        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            var trackMetadataViewModel = value as TrackMetadataViewModel;
//            if (trackMetadataViewModel == null || trackMetadataViewModel.IsBroken)
//            {
//                return null;
//            }
//            else
//            {
//                var discRegexMatches = _regex.Match(trackMetadataViewModel.Disc);
//                var discIndex = Convert.ToInt32(discRegexMatches.Groups["Index"].Value);
//                var discCount = Convert.ToInt32(discRegexMatches.Groups["Count"].Value);
//                var trackRegexMatches = _regex.Match(trackMetadataViewModel.Track);
//                var trackIndex = Convert.ToInt32(trackRegexMatches.Groups["Index"].Value);

//                return String.Format("{0}{1:00}",
//                    discCount > 1 ? discIndex.ToString() : String.Empty,
//                    trackIndex);
//            }
//        }

//        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }

//        static readonly Regex _regex = new Regex(REGEX_PATTERN, RegexOptions.Compiled);
//        const string REGEX_PATTERN = @"^(?<Index>\d+)/(?<Count>\d+)$";
//    }
//}
