﻿using IF.Lastfm.Core.Api.Commands.AlbumApi;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Tests.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IF.Lastfm.Core.Tests.Api.Commands
{
    [TestClass]
    public class AlbumGetTagsByUserCommandTests : CommandTestsBase
    {
        private AlbumGetTagsByUserCommand _command;

        [TestInitialize]
        public void Initialise()
        {
            _command = new AlbumGetTagsByUserCommand(MAuth.Object, "", "", "");
        }

        [TestMethod]
        public async Task HandleResponseSingle()
        {
            var expectedTags = new List<LastTag>
            {
                new LastTag("Test Tag", "http://www.last.fm/tag/test%20tag")
            };

            var response = CreateResponseMessage(Encoding.UTF8.GetString(AlbumApiResponses.AlbumGetTagsSingle));
            var parsed = await _command.HandleResponse(response);

            var expectedJson = expectedTags.TestSerialise();
            var actualJson = parsed.Content.TestSerialise();

            parsed.AssertValues(true, 1, 1, 1, 1);
            Assert.AreEqual(expectedJson, actualJson, expectedJson.DifferencesTo(actualJson));
        }

        [TestMethod]
        public async Task HandleResponseMultiple()
        {
            var expectedTags = new List<LastTag>
            {
                new LastTag("test tag 2: electric boogaloo", "http://www.last.fm/tag/test%20tag%202%3A%20electric%20boogaloo"),
                new LastTag("Test Tag", "http://www.last.fm/tag/test%20tag")
            };

            var response = CreateResponseMessage(Encoding.UTF8.GetString(AlbumApiResponses.AlbumGetTagsMultiple));
            var parsed = await _command.HandleResponse(response);

            var expectedJson = expectedTags.TestSerialise();
            var actualJson = parsed.Content.TestSerialise();

            parsed.AssertValues(true, 2, 2, 1, 1);
            Assert.AreEqual(expectedJson, actualJson, expectedJson.DifferencesTo(actualJson));
        }

        [TestMethod]
        public async Task HandleResponseEmpty()
        {
            var response = CreateResponseMessage(Encoding.UTF8.GetString(AlbumApiResponses.AlbumGetTagsEmpty));
            var parsed = await _command.HandleResponse(response);

            parsed.AssertValues(true, 0, 0, 1, 1);
            Assert.IsTrue(!parsed.Content.Any());
        }

        [TestMethod]
        public async Task HandleResponseError()
        {
            var response = CreateResponseMessage(Encoding.UTF8.GetString(AlbumApiResponses.AlbumGetTagsError));
            var parsed = await _command.HandleResponse(response);

            parsed.AssertValues(false, 0, 0, 1, 1);
            Assert.IsFalse(parsed.Success);
            Assert.IsTrue(parsed.Error == LastFmApiError.MissingParameters);
            Assert.IsTrue(!parsed.Content.Any());
        }
    }
}