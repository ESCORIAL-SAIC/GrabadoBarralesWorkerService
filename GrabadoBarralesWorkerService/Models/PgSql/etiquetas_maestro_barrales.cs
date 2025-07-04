﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GrabadoBarralesWorkerService.Models.PgSql;

[Table("etiquetas_maestro_barrales", Schema = "web")]
public partial class etiquetas_maestro_barrales
{
    [Key]
    public int id { get; set; }

    [Required]
    [StringLength(50)]
    public string serie { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime fecha_alta { get; set; }

    [Required]
    [StringLength(50)]
    public string tipo_barral_codigo { get; set; }

    [Required]
    [StringLength(50)]
    public string operador { get; set; }
    public int? puesto { get; set; }
}