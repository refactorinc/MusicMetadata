using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MusicMetadata.Persistence;

namespace MusicMetadata.ViewModels
{
    static class AlbumViewModelFactory
    {
        public static AlbumViewModel FromObjects(IEnumerable<MetadataDto> objects)
        {
            if (objects.Any())
            {
                var first = objects.First();
                var album = new AlbumViewModel
                {
                    Artist = first.AlbumArtist,
                    Title = first.Album,
                    Year = first.Year,
                    Genre = first.Genre,
                    Compilation = first.Compilation.HasValue && first.Compilation.Value == 1
                };
                foreach (var discWithTracks in objects.GroupBy(x => x.Disc).OrderBy(x => x.Key))
                {
                    var regexMatch = _regex.Match(discWithTracks.First().AccurateRipDiscId ?? string.Empty);
                    var disc = new DiscViewModel
                    {
                        DiscNumber = Convert.ToInt32(discWithTracks.Key.Split('/')[0]),
                        AccurateRipDiscId = regexMatch.Success ? regexMatch.Groups[1].Captures[0].Value : string.Empty
                    };
                    foreach (var trackFromDisc in discWithTracks)
                    {
                        var track = new TrackViewModel
                        {
                            Id = trackFromDisc.Id,
                            TrackNumber = Convert.ToInt32(trackFromDisc.Track.Split('/')[0]),
                            Artist = trackFromDisc.Artist,
                            Title = trackFromDisc.Title
                        };
                        disc.Tracks.Add(track);
                    }
                    album.Discs.Add(disc);
                }
                return album;
            }
            else
            {
                return new AlbumViewModel();
            }
        }

        public static IEnumerable<MetadataDto> FromViewModel(AlbumViewModel album)
        {
            foreach (var disc in album.Discs)
            foreach (var track in disc.Tracks)
            {
                var dto = IdentityMap.Get(track.Id);
                dto.Artist = track.Artist;
                dto.Title = track.Title;
                dto.Album = album.Title;
                dto.Year = album.Year;
                dto.Genre = album.Genre;
                dto.AccurateRipDiscId = string.IsNullOrEmpty(disc.AccurateRipDiscId)
                    ? null
                    : string.Format("{0}-{1}", disc.AccurateRipDiscId, Convert.ToInt32(dto.Track.Split('/')[0]));
                dto.AlbumArtist = album.Artist;
                dto.Compilation = album.Compilation ? 1 : 0;
                yield return dto;
            }
        }

        static readonly Regex _regex = new Regex(REGEX_PATTERN, RegexOptions.Compiled);
        const string REGEX_PATTERN = @"^(.*)-.*";
    }
}
