namespace Api.Modelos
{
    public class LoginUsuarios
    {
        public Login usuarioLogueado { get; set; }
        public List<MenuDto>? Menu { get; set; }
    }
}

