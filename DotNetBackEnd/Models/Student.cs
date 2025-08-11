using System;
using System.Collections.Generic;

namespace LibraryMgmt.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int? CourseId { get; set; }

    public string EmailAddress { get; set; } = null!;

    public virtual ICollection<BookStatus> BookStatuses { get; set; } = new List<BookStatus>();

    public virtual Course? Course { get; set; }
}
