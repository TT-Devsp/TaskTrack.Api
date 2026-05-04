namespace TaskTrack.Application.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Gestor = "Gestor";
    public const string Tecnico = "Tecnico";
    public const string Solicitante = "Solicitante";
    public const string Visualizador = "Visualizador";

    public static readonly IReadOnlyList<string> All = new[] { Admin, Gestor, Tecnico, Solicitante, Visualizador };
}
