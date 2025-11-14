using System.Text.Json;

namespace GitHub_User_Activity_CLI.Models
{
    public class GitHubEvent
    {
        public string Type { get; set; } = string.Empty;
        public GitHubRepo Repo { get; set; } = new GitHubRepo();
        public JsonElement Payload { get; set; }
    }
}
