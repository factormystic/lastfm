﻿using System;
using System.Collections.Generic;
using System.Linq;
using IF.Lastfm.Core.Api.Commands.AlbumApi;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Tests.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IF.Lastfm.Core.Tests.Api.Commands.AlbumApi
{
    [TestClass]
    public class GetAlbumInfoCommandTests : CommandTestsBase
    {
        private GetAlbumInfoCommand _command;

        public GetAlbumInfoCommandTests()
        {
            _command = new GetAlbumInfoCommand(MAuth.Object)
            {
                AlbumName = "Ray of Light",
                ArtistName = "Madonna"
            };

            _command.SetParameters();
        }
        

        [TestMethod]
        public async Task HandleSuccessResponse()
        {
            var expectedAlbum = new LastAlbum
            {
                ArtistName = "Madonna",
                ListenerCount = 509271,
                TotalPlayCount = 7341494,
                Mbid = "ddb3168d-66a9-4b2d-af02-05278da8a23d",
                Url = new Uri("http://www.last.fm/music/Madonna/Ray+of+Light", UriKind.Absolute),
                Name = "Ray of Light",
                ReleaseDateUtc = new DateTime(2005, 09, 13, 0, 0, 0),
                Id = "1934",
                Images = new LastImageSet(
                    "http://userserve-ak.last.fm/serve/34s/37498173.png",
                    "http://userserve-ak.last.fm/serve/64s/37498173.png",
                    "http://userserve-ak.last.fm/serve/174s/37498173.png",
                    "http://userserve-ak.last.fm/serve/300x300/37498173.png",
                    "http://userserve-ak.last.fm/serve/_/37498173/Ray+of+Light.png"),
                TopTags = new List<LastTag>
                {
                    new LastTag("albums i own", "http://www.last.fm/tag/albums%20i%20own"),
                    new LastTag("pop", "http://www.last.fm/tag/pop"),
                    new LastTag("electronic", "http://www.last.fm/tag/electronic"),
                    new LastTag("dance", "http://www.last.fm/tag/dance"),
                    new LastTag("madonna", "http://www.last.fm/tag/madonna")
                }
            };

            var response = CreateResponseMessage(Encoding.UTF8.GetString(AlbumApiResponses.AlbumGetInfoSuccess));
            var parsed = await _command.HandleResponse(response);

            Assert.IsTrue(parsed.Success);

            var actual = parsed.Content;
            Assert.IsTrue(actual.Tracks.Count() == 13);
            actual.Tracks = null;

            var expectedJson = JsonConvert.SerializeObject(expectedAlbum, Formatting.Indented);
            var actualJson = JsonConvert.SerializeObject(parsed.Content, Formatting.Indented);

            Assert.AreEqual(expectedJson, actualJson, expectedJson.DifferencesTo(actualJson));
        }

        [TestMethod]
        public async Task HandleErrorResponse()
        {
            var response = CreateResponseMessage(Encoding.UTF8.GetString(AlbumApiResponses.AlbumGetInfoMissing));

            var parsed = await _command.HandleResponse(response);

            Assert.IsFalse(parsed.Success);
            Assert.IsTrue(parsed.Error == LastFmApiError.MissingParameters);
        }
    }
}
