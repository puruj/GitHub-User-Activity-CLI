using GitHub_User_Activity_CLI.Models;
using System.Text.Json;

namespace GitHub_User_Activity_CLI
{
    public static class EventFormatter
    {
        public static IEnumerable<string> ToSummaryLines(IEnumerable<GitHubEvent> events)
        {
            foreach (var ev in events)
            {
                yield return ev.Type switch
                {
                    "PushEvent" => FormatPushEvent(ev),
                    "IssuesEvent" => FormatIssuesEvent(ev),
                    "IssueCommentEvent" => $"Commented on an issue in {ev.Repo.Name}",
                    "WatchEvent" => $"Starred {ev.Repo.Name}",
                    _ => $"{ev.Type} in {ev.Repo.Name}"
                };
            }
        }

        private static string FormatPushEvent(GitHubEvent ev)
        {
            // payload.commits is an array
            if (ev.Payload.TryGetProperty("commits", out JsonElement commits) && commits.ValueKind == JsonValueKind.Array)
            {
                var count = commits.GetArrayLength();
                var plural = count == 1 ? "commit" : "commits";
                return $"Pushed {count} {plural} to {ev.Repo.Name}";
            }

            return $"Pushed commits to {ev.Repo.Name}";
        }

        private static string FormatIssuesEvent(GitHubEvent ev)
        {
            // payload.action is usually "opened", "closed", "reopened"
            string action = "acted on";
            if (ev.Payload.TryGetProperty("action", out JsonElement actionElement) && actionElement.ValueKind == JsonValueKind.String)
            {
                action = actionElement.GetString() ?? "acted on";
            }

            var verb = action switch
            {
                "opened" => "Opened",
                "closed" => "Closed",
                "reopened" => "Reopened",
                _ => char.ToUpper(action[0]) + action[1..]
            };

            return $"{verb} an issue in {ev.Repo.Name}";
        }
    }
}
