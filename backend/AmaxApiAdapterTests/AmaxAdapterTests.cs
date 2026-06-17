using AmaxApiAdapter.Adapters;
using AmaxApiAdapter.Http;
using AmaxApiAdapter.Models.DTOs;
using NSubstitute;
using System.Text.Json;

namespace AmaxApiAdapterTests
{
    public class AmaxAdapterTests
    {
        private IAmaxHttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            _httpClient = Substitute.For<IAmaxHttpClient>();
        }

        [Test]
        public async Task GivenAmaxUserDataWithAnAccount_WhenGettingAmaxUsername_ReturnAmaxUsername()
        {
            //Arrange
            string response = @"{
  ""amax_account"": true,
  ""avatarUrl"": ""https://cdn.discordapp.com/avatars/XXXXXXXXXXXXXXXXXX/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX.png"",
  ""isGameBanned"": false,
  ""isDiscordBanned"": false,
  ""ban_data"": null,
  ""amax_player_data"": {
    ""stats"": {
      ""playerName"": ""Bobby"",
      ""statLevel"": 49,
      ""statFansCurrent"": 890000,
      ""statRaceTime"": 298613,
      ""statDriverScore"": 5582,
      ""statTop3"": 1091,
      ""statRaces"": 1413,
      ""statFirst"": 457,
      ""statHits"": 8622,
      ""statFired"": 29421,
      ""statWrecked"": 423,
      ""statLegend"": 0,
      ""statLegendTime"": 23663
    },
    ""leveling"": {
      ""level"": 49,
      ""legend"": 0,
      ""fans"": 6835313,
      ""fans_levelup_percent"": 100
    },
    ""friends"": [
      {
        ""name"": ""Shannon"",
        ""isOnline"": true,
        ""status"": """"
      }
    ],
    ""friends_purposes"": {
      ""outcoming"": [],
      ""incoming"": [
        ""NO.1"",
        ""Rhymer"",
        ""MC""
      ]
    }
  }
}";

            JsonDocument document = JsonDocument.Parse(response);
            string expectedResult = "Bobby";
            string token = "XXXXXXXXXXXXXXXXXXXXXXXX.XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            _httpClient.GetUserAmaxData(token).Returns(document);

            AmaxAdapter amaxAdapter = new(_httpClient);

            //Act
            string actualResult = await amaxAdapter.GetAmaxUsername(token);

            //Assert
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task GivenAmaxPlayerData_WhenGettingAmaxPlayerStats_ReturnAmaxPlayerStats()
        {
            //Arrange
            string response = @"{
  ""error"": false,
  ""error_msg"": """",
  ""data"": {
    ""amaxPlayerData"": {
      ""amaxPastGames"": """",
      ""amaxLevelingData"": {
        ""level"": 49,
        ""legend"": 0,
        ""fansTotal"": 6835313,
        ""fansCurrent"": 890000,
        ""fansNeeded"": 890000
      },
      ""amaxStatsData"": {
        ""statLevel"": 49,
        ""statFans"": 6835313,
        ""statRaceTime"": 298613072,
        ""statDriverScore"": 5582,
        ""statTop3"": 1091,
        ""statRaces"": 1413,
        ""statFirst"": 457,
        ""statHits"": 8622,
        ""statFired"": 29421,
        ""statWrecked"": 423,
        ""statLegend"": 0,
        ""statLegendTime"": 23663
      }
    },
    ""banData"": {
      ""ban_reason"": """",
      ""ban_start"": """",
      ""ban_end"": """"
    },
    ""player_name"": ""Bobby"",
    ""isOnline"": false,
    ""status"": 0,
    ""isGameBanned"": false,
    ""accountType"": 0,
    ""amaxPfpUrl"": ""https://aiwarehouse.fra1.cdn.digitaloceanspaces.com/amax-pfp/XXXXXXXX-XXXXXXXXX-XXXX-XXXXXXXXXXXX.jpg""
  }
}";

            JsonDocument document = JsonDocument.Parse(response);
            PlayerStats expectedResult = new PlayerStats
            {
                TotalFans = 6835313,
                DriverScore = 5582,
                RaceTime = 298613072,
                RaceStarts = 1413,
                Wins = 457,
                PodiumFinishes = 1091,
                PowerUpUses = 29421,
                PowerUpHits = 8622
            };

            string playerName = "Bobby;";
            string token = "XXXXXXXXXXXXXXXXXXXXXXXX.XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            _httpClient.GetPlayerStats(playerName).Returns(document);

            AmaxAdapter amaxAdapter = new(_httpClient);

            //Act
            PlayerStats actualResult = await amaxAdapter.GetPlayerStats(playerName);

            //Assert
            Assert.That(actualResult.TotalFans, Is.EqualTo(expectedResult.TotalFans));
            Assert.That(actualResult.DriverScore, Is.EqualTo(expectedResult.DriverScore));
            Assert.That(actualResult.RaceTime, Is.EqualTo(expectedResult.RaceTime));
            Assert.That(actualResult.RaceStarts, Is.EqualTo(expectedResult.RaceStarts));
            Assert.That(actualResult.Wins, Is.EqualTo(expectedResult.Wins));
            Assert.That(actualResult.PodiumFinishes, Is.EqualTo(expectedResult.PodiumFinishes));
            Assert.That(actualResult.PowerUpUses, Is.EqualTo(expectedResult.PowerUpUses));
            Assert.That(actualResult.PowerUpHits, Is.EqualTo(expectedResult.PowerUpHits));
        }

        [Test]
        public async Task GivenAmaxUserDataWithoutAnAccount_WhenGettingAmaxUsername_ReturnNull()
        {
            //Arrange
            string response = @"{
  ""amax_account"": false,
  ""avatarUrl"": ""https://cdn.discordapp.com/avatars/XXXXXXXXXXXXXXXXXX/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX.png"",
  ""isGameBanned"": false,
  ""isDiscordBanned"": false,
  ""ban_data"": null,
  ""amax_player_data"": {
    ""stats"": {
      ""playerName"": """",
      ""statLevel"": 0,
      ""statFansCurrent"": 0,
      ""statRaceTime"": 0,
      ""statDriverScore"": 0,
      ""statTop3"": 0,
      ""statRaces"": 0,
      ""statFirst"": 0,
      ""statHits"": 0,
      ""statFired"": 0,
      ""statWrecked"": 0,
      ""statLegend"": 0,
      ""statLegendTime"": 0
    },
    ""leveling"": {
      ""level"": 0,
      ""legend"": 0,
      ""fans"": 0,
      ""fans_levelup_percent"": 0
    },
    ""friends"": [],
    ""friends_purposes"": {
      ""outcoming"": [],
      ""incoming"": []
    }
  }
}";

            JsonDocument document = JsonDocument.Parse(response);
            string expectedResult = null!;
            string token = "XXXXXXXXXXXXXXXXXXXXXXXX.XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            _httpClient.GetUserAmaxData(token).Returns(document);

            AmaxAdapter amaxAdapter = new(_httpClient);

            //Act
            string actualResult = await amaxAdapter.GetAmaxUsername(token);

            //Assert
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }
    }
}