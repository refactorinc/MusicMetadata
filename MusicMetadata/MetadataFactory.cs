using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MusicMetadata
{
    static class MetadataFactory
    {
        static internal IEnumerable<Metadata> MaterializeFor(IEnumerable<string> filePaths)
        {
            filePaths = filePaths.ToList();
            if (filePaths.Any())
            {
                var tracksPerDisc = ParseTracksPerDisc(filePaths).ToList();
                var discCount = tracksPerDisc.Max(x => x.Item1);
                foreach (var filePath in filePaths)
                {
                    Metadata metadata = null;
                    try
                    {
                        metadata = MetadataStore.Read(filePath);
                    }
                    catch
                    {
                        metadata = new Metadata(filePath) { IsBroken = true };
                    }
                    if (metadata == null)
                    {
                        var discAndTrack = ParseDiscAndTrack(filePath);
                        metadata = new Metadata(filePath)
                        {
                            Track = String.Format("{0}/{1}", discAndTrack.Item2, tracksPerDisc.Single(x => x.Item1 == discAndTrack.Item1).Item2),
                            Disc = String.Format("{0}/{1}", discAndTrack.Item1, discCount),
                        };
                    }
                    yield return metadata;
                }
            }
        }

        static IEnumerable<Tuple<int, int>> ParseTracksPerDisc(IEnumerable<string> filePaths)
        {
            foreach (var tracksByDisc in filePaths.Select(ParseDiscAndTrack).ToLookup(x => x.Item1))
            {
                yield return tracksByDisc.OrderBy(x => x.Item2).Last();
            }
        }

        static Tuple<int, int> ParseDiscAndTrack(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var regexMatch = _regex.Match(fileName);
            var disc = regexMatch.Groups["disc"].Value;
            var discIndex = String.IsNullOrEmpty(disc) ? 1 : Convert.ToInt32(disc);
            var trackIndex = Convert.ToInt32(regexMatch.Groups["track"].Value);
            return Tuple.Create(discIndex, trackIndex);
        }

        static readonly Regex _regex = new Regex(REGEX_PATTERN, RegexOptions.Compiled);
        const string REGEX_PATTERN = @"^(?<disc>\d?)(?<track>\d{2}).*$";
    }
}
