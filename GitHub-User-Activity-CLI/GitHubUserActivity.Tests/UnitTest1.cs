using GitHub_User_Activity_CLI;
using GitHub_User_Activity_CLI.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace GitHubUserActivity.Tests
{
    public class EventFormatterTests
    {
        [Fact]
        public void ToSummaryLines_PushEventWithTwoCommits_FormatsCorrectly()
        {
            var payloadJson = """{ "commits": [ { "id": "1" }, { "id": "2" } ] } """;
            var payload = JsonDocument.Parse(payloadJson).RootElement;

            var events = new[]
            {
                new GitHubEvent
                {
                    Type = "PushEvent",
                    Repo = new GitHubRepo { Name = "user/repo"},
                    Payload = payload
                }
            };

            //Act
            var lines = EventFormatter.ToSummaryLines(events).ToArray();

            Assert.Single(lines);
            Assert.Equal("Pushed 2 commits to user/repo", lines[0]);

        }

        [Fact]
        public void ToSummaryLines_PushEventWithoutCommits_UsesFallbackMessage()
        {
            var payloadJson = "{}";
            var payload = JsonDocument.Parse(payloadJson).RootElement;

            var events = new[]
            {
                new GitHubEvent
                {
                    Type = "PushEvent",
                    Repo = new GitHubRepo { Name = "user/repo"},
                    Payload = payload
                }
            };

            //Act
            var lines = EventFormatter.ToSummaryLines(events).ToArray();

            Assert.Single(lines);
            Assert.Equal("Pushed commits to user/repo", lines[0]);

        }

        [Fact]
        public void ToSummaryLines_IssuesEventWithOpenedAction_FormatsOpened()
        {
            var payloadJson = """{ "action": "opened"}""";
            var payload = JsonDocument.Parse(payloadJson).RootElement;

            var events = new[]
            {
                new GitHubEvent
                {
                    Type = "IssuesEvent",
                    Repo = new GitHubRepo { Name = "user/repo"},
                    Payload = payload
                }
            };

            //Act 
            var lines = EventFormatter.ToSummaryLines(events).ToArray();

            Assert.Single(lines);
            Assert.Equal("Opened an issue in user/repo", lines[0]);
        }

        [Fact]
        public void ToSummaryLines_WatchEvent_FormatsStarred()
        {
            var events = new[]
{
                new GitHubEvent
                {
                    Type = "WatchEvent",
                    Repo = new GitHubRepo { Name = "user/repo"},
                    Payload = default
                }
            };

            //Act
            var lines = EventFormatter.ToSummaryLines(events).ToArray();

            Assert.Single(lines);
            Assert.Equal("Starred user/repo", lines[0]);
        }


        [Fact]
        public void ToSummaryLines_UnknownEventType_UsesFallbackFormat() 
        {
            // Arrange
            var events = new[]
            {
                new GitHubEvent
                {
                    Type = "ForkEvent",
                    Repo = new GitHubRepo { Name = "user/repo" },
                    Payload = default
                }
            };

            // Act
            var lines = EventFormatter.ToSummaryLines(events).ToArray();

            // Assert
            Assert.Single(lines);
            Assert.Equal("ForkEvent in user/repo", lines[0]);
        }
    }

    public class GitHubClientTests
    {
        [Fact]
        public async Task GetUserEventsAsync_Success_ReturnsEvents()
        {
            // Arrange
            var json = """
                [
                  {
                    "type": "PushEvent",
                    "repo": { "name": "user/repo" },
                    "payload": { }
                  }
                ]
                """;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var handler = new StubHttpMessageHandler(response);
            var httpClient = new HttpClient(handler);
            var client = new GitHubClient(httpClient);

            // Act
            var events = await client.GetUserEventsAsync("some-user");

            // Assert
            Assert.Single(events);
            Assert.Equal("PushEvent", events[0].Type);
            Assert.Equal("user/repo", events[0].Repo.Name);

            // optional: verify it called the right URL
            Assert.Equal("https://api.github.com/users/some-user/events", handler.LastRequest!.RequestUri!.ToString());
        }

        [Fact]
        public async Task GetUserEventsAsync_404_ThrowsGitHubNotFoundException()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            var handler = new StubHttpMessageHandler(response);
            var httpClient = new HttpClient(handler);
            var client = new GitHubClient(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<GitHubNotFoundException>(() => client.GetUserEventsAsync("missing-user"));
        }

        [Fact]
        public async Task GetUserEventsAsync_403_ThrowsGitHubRateLimitException()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            var handler = new StubHttpMessageHandler(response);
            var httpClient = new HttpClient(handler);
            var client = new GitHubClient(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<GitHubRateLimitException>(() => client.GetUserEventsAsync("any-user"));
        }
    }
}