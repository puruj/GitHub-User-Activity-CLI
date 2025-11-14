using System.Net;
using GitHub_User_Activity_CLI;
using GitHubActivity.Cli;

namespace GitHubActivity.Cli;
public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // Help / usage
        if (args.Length == 0)
        {
            PrintUsage();
            return 1; // missing username
        }

        if (args[0] is "--help" or "-h")
        {
            PrintUsage();
            return 0;
        }

        var username = args[0];

        try
        {
            using var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GitHubActivityCli/1.0");

            var gitHubClient = new GitHubClient(httpClient);

            var events = await gitHubClient.GetUserEventsAsync(username);
            var lines = EventFormatter.ToSummaryLines(events).Take(10);

            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }

            return 0;
        }
        catch (GitHubNotFoundException)
        {
            Console.Error.WriteLine($"User '{username}' not found on GitHub.");
            return 2;
        }
        catch (GitHubRateLimitException)
        {
            Console.Error.WriteLine("GitHub API rate limit exceeded. Try again later.");
            return 2;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Unexpected error: {ex.Message}");
            return 2;
        }
    }

    private static void PrintUsage()
    {
        Console.WriteLine("""
            github-activity <github-username>

            Fetches recent public activity for the given GitHub user
            and prints a summary to the terminal.

            Options:
              -h, --help    Show this help message
            """);
    }
}
