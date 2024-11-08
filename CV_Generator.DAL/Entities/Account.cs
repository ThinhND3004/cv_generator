using System;
using System.Collections.Generic;

namespace CV_Generator.DAL.Entities;

public partial class Account
{
    public int Id { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? FullName { get; set; }

    public int? Role { get; set; }

    public virtual ICollection<CurriculumVitae> CurriculumVitaes { get; set; } = new List<CurriculumVitae>();
}
