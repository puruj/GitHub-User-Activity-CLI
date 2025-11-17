using GitHub_User_Activity_CLI;
using GitHub_User_Activity_CLI.Models;
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
}