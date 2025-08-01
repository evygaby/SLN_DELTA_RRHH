namespace Api
{
    public class FuncionesVarias
    {
        public string SeccionesSeleccionadas(string[] secciones)
        {
            string codigos = "";

            string[] curso = secciones;
            int niveldesde = int.Parse(curso[0]);
            int nivelhasta = int.Parse(curso[1]);
            for (var dn = niveldesde; dn <= nivelhasta; dn++)
                codigos = codigos + dn.ToString() + ",";
            return codigos;
        }
    }
}
