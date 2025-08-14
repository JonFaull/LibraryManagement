using System;
using System.Collections.Generic;

namespace LibraryMgmt.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public string? Author { get; set; }

    public string? Isbn { get; set; }

    public int NoCopies { get; set; }

    public virtual ICollection<BookStatus> BookStatuses { get; set; } = new List<BookStatus>();
}
