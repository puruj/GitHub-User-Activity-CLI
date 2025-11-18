# GitHub User Activity CLI

A .NET 8 command-line tool that fetches recent public events for a given GitHub user and prints concise summaries to the terminal. It uses the GitHub events API and includes basic error handling for missing users and rate limiting.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/) installed
- Network access to `api.github.com`

## Getting Started

Clone the repository and restore dependencies:

```bash
cd GitHub-User-Activity-CLI
```

To run the CLI without installing it globally, execute:

```bash
dotnet run --project GitHub-User-Activity-CLI/GitHub-User-Activity-CLI -- <github-username>
```

The tool prints up to ten recent public events in a readable format, such as:

```
Pushed 2 commits to octocat/Hello-World
Opened an issue in octocat/example-repo
Starred octocat/another-repo
```

Use `-h` or `--help` to see usage details.

## Testing

Run the test suite with:

```bash
dotnet test
```

The tests cover formatting logic and the GitHub client’s handling of common API responses.

## Notes

- The GitHub API enforces rate limits for unauthenticated requests. If you hit a rate limit, try again later or add authentication headers in `GithubClient`.
- HTTP requests set a 10-second timeout and include a custom `User-Agent` header as required by the GitHub API.
