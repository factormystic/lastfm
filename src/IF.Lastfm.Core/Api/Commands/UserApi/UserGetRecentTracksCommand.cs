using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace IF.Lastfm.Core.Api.Commands.UserApi
{
    internal class UserGetRecentTracksCommand : GetAsyncCommandBase<PageResponse<LastTrack>>
    {
        public string Username { get; private set; }

        public DateTime? From { get; set; }

        public UserGetRecentTracksCommand(ILastAuth auth, string username) : base(auth)
        {
            Method = "user.getRecentTracks";

            Username = username;
        }

        public override void SetParameters()
        {
            Parameters.Add("user", Username);
            
            if (From.HasValue)
            {
                Parameters.Add("from", From.Value.ToUnixTimestamp().ToString());
            }

            AddPagingParameters();
            DisableCaching();
        }

        public async override Task<PageResponse<LastTrack>> HandleResponse(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();

            LastFmApiError error;
            if (LastFm.IsResponseValid(json, out error) && response.IsSuccessStatusCode)
            {
                var jtoken = JsonConvert.DeserializeObject<JToken>(json).SelectToken("recenttracks");
                var itemsToken = jtoken.SelectToken("track");
                var attrToken = jtoken.SelectToken("@attr");

                return PageResponse<LastTrack>.CreateSuccessResponse(itemsToken, attrToken, LastTrack.ParseJToken, LastPageResultsType.Attr);
            }
            else
            {
                return LastResponse.CreateErrorResponse<PageResponse<LastTrack>>(error);
            }
        }
    }
}