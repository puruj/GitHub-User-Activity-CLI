using GitHub_User_Activity_CLI.Models;
using System.Text.Json;

namespace GitHub_User_Activity_CLI
{
    public class GithubClient
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public GithubClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<GitHubEvent>> GetUserEventsAsync(string username, CancellationToken cancellationToken = default)
        {
            var url = $"https://api.github.com/users/{username}/events";

            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new GitHubNotFoundException(username);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new GitHubRateLimitException();
            }

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            var events = JsonSerializer.Deserialize<List<GitHubEvent>>(json, _jsonOptions);

            return events ?? new List<GitHubEvent>();
        }
    }

    public sealed class GitHubNotFoundException : Exception
    {
        public string Username { get; }

        public GitHubNotFoundException(string username) : base($"GitHub user '{username}' not found.")
        {
            Username = username;
        }
    }

    public sealed class GitHubRateLimitException : Exception
    {
        public GitHubRateLimitException() : base("GitHub API rate limit exceeded.")
        {
        }
    }
}
