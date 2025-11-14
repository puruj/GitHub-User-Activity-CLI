namespace GithubActivity;


public class  GithubActivity
{
    public static int main (string[] args)
    {
        var app = new GithubActivity();
        return app.Run(args);
    }

    public int Run (string[] args)
    {
        try
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Usage: GithubActivity <github-username>");
                return 1;
            }

            string cmd = args[0].ToLowerInvariant();

            switch(cmd)
            {
                case "github-username": return 0;                     

                default:
                    Console.WriteLine($"Unknown command: {cmd}");
                    return 1;
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

}
