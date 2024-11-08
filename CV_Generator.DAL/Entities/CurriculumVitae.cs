using System;
using System.Collections.Generic;

namespace CV_Generator.DAL.Entities;

public partial class CurriculumVitae
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public DateTime? CreateAt { get; set; }

    public int? CreateBy { get; set; }

    public virtual Account? CreateByNavigation { get; set; }
}
