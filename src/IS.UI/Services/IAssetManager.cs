﻿using IS.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IS.UI.Services
{
    public interface IAssetManager
    {
        string GetBookCoverUrl(string bookName);
        Task<IReadOnlyList<BookModel>> GetBooksAsync();
    }
}
