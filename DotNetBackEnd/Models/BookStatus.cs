using System;
using System.Collections.Generic;

namespace LibraryMgmt.Models;

public partial class BookStatus
{
    public int BookStatusId { get; set; }

    public int? BookId { get; set; }

    public int? StudentId { get; set; }

    public DateTime DateCheckout { get; set; }

    public DateTime? DateReturned { get; set; }

    public virtual Book? Book { get; set; }

    public virtual Student? Student { get; set; }
}
