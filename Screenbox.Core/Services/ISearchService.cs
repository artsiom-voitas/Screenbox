﻿using Screenbox.Core;

namespace Screenbox.Core.Services
{
    public interface ISearchService
    {
        SearchResult SearchLocalLibrary(string query);
    }
}