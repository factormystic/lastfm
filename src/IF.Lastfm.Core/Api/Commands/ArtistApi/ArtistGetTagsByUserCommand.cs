﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IF.Lastfm.Core.Api.Commands.ArtistApi
{
    internal class ArtistGetTagsByUserCommand : GetAsyncCommandBase<PageResponse<LastTag>>
    {
        public string ArtistName { get; set; }

        public string Username { get; set; }

        public bool Autocorrect { get; set; }

        public ArtistGetTagsByUserCommand(ILastAuth auth, string artist, string username)
            : base(auth)
        {
            Method = "artist.getTags";

            ArtistName = artist;
            Username = username;
        }

        public override void SetParameters()
        {
            Parameters.Add("artist", ArtistName);
            Parameters.Add("user", Username);
            Parameters.Add("autocorrect", Convert.ToInt32(Autocorrect).ToString());

            AddPagingParameters();
            DisableCaching();
        }

        public async override Task<PageResponse<LastTag>> HandleResponse(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();

            LastFmApiError error;
            if (LastFm.IsResponseValid(json, out error) && response.IsSuccessStatusCode)
            {
                var jtoken = JsonConvert.DeserializeObject<JToken>(json);
                var resultsToken = jtoken.SelectToken("tags");
                var itemsToken = resultsToken.SelectToken("tag");

                return PageResponse<LastTag>.CreateSuccessResponse(itemsToken, LastTag.ParseJToken);
            }
            else
            {
                return PageResponse<LastTag>.CreateErrorResponse(error);
            }
        }
    }
}
