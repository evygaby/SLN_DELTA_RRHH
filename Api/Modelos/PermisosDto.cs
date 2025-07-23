using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Api.Modelos
{
    public partial class QRY_ObtenerVentanasXRolesResult
    {
        public int Idventana { get; set; }
        public string? codigo { get; set; }
        public string? descripcion { get; set; } = default!;
        public string? url { get; set; } = default!;
        public string? icono { get; set; } = default!;
        public string? urlpadre { get; set; }
        public int? IdPadre { get; set; }
        public int? permiso { get; set; }
    }
    public class PermisosDto
    {
        public int id { get; set; }
        public string? fullName { get; set; }
        public bool? expanded { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? selected { get; set; }

        public List<PermisosDto>? items { get; set; }
    }


    public class PreguntaDto
    {
        [Key]
        public long IdPregunta { get; set; }
        public int? IdPadre { get; set; }
        public string? alias { get; set; }
        public bool? Isnivel { get; set; }
        public int? relacion { get; set; }
        public string? descripcion { get; set; }
        public string? explicacion { get; set; }
        public bool? expanded { get; set; }
        public bool? selected { get; set; }
        public List<PreguntaDto>? items { get; set; }
        public byte? index { get; set; }
        public byte? IdTipo { get; set; }
        public byte? Estado { get; set; }
        public int? IdRepetitivo { get; set; }
        public int? IdEscala { get; set; }
        public int? orden { get; set; }
        public bool? noaplica { get; set; }
        public bool? isopcional { get; set; }


        public bool? Has_Items { get; set; }

    }

    public class State
    {
        public bool selected { get; set; }
        public bool? opened { get; set; }
        public bool? disabled { get; set; }
    }

    public class Child

    {
        public string? label { get; set; }
        public bool? isTitle { get; set; }
        public string? id { get; set; }
        public string? parentId { get; set; }
        public string? link { get; set; }
        public string? icon { get; set; }
        public bool? isLayout { get; set; }
        public iconComponent? badge { get; set; }

    }
    public class iconComponent

    {

        public string? name { get; set; }


    }
    public class MenuDto
    {

        public string? label { get; set; }
        public bool? isTitle { get; set; }
        public string? id { get; set; }
        public string? parentId { get; set; }
        public string? link { get; set; }
        public string? icon { get; set; }
        public bool? isLayout { get; set; }
        public iconComponent? badge { get; set; }
        public List<Child>? subItems { get; set; }
    }
}
