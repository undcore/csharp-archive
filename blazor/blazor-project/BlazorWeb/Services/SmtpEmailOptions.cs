namespace BlazorWeb.Services;

public sealed class SmtpEmailOptions
{
    public string Host { get; set; } = "";

    public int Port { get; set; } = 587;

    public string UserName { get; set; } = "";

    public string Password { get; set; } = "";

    public string FromEmail { get; set; } = "no-reply@localhost";

    public string FromName { get; set; } = "Dev Archive";

    public bool EnableSsl { get; set; } = true;
}
