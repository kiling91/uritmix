using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers.Core.Extensions;

namespace Helpers.Pagination.Helpers;

public static class PaginatedListLoader
{
    /// <summary>
    ///     Loads all pages.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <param name="loadPageTask">Task, assepting page size and page index and returning one loaded page of items</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="maxThreads">The maximum threads that can be used to load pages.</param>
    /// <returns>
    ///     All items loaded
    /// </returns>
    public static async Task<List<TViewModel>> LoadAllPages<TViewModel>(
        Func<Paginator, Task<PaginatedList<TViewModel>>> loadPageTask, int pageSize, int maxThreads)
    {
        var firstPage = await loadPageTask(new Paginator(pageSize, 1));
        if (!firstPage.NextPageExists)
            return firstPage.ToList();

        var totalPages = firstPage.TotalPages;
        if (maxThreads <= 0)
            maxThreads = totalPages - 1;

        var loadedPageBlocks = new List<PaginatedList<TViewModel>[]>();
        var blockStartIndex = 2;
        while (blockStartIndex <= totalPages)
        {
            var blockSize = Math.Min(maxThreads, totalPages - blockStartIndex + 1);
            var pageLoadTasks = Enumerable.Range(blockStartIndex, blockSize)
                .Select(pageNumber => loadPageTask(new Paginator(pageSize, pageNumber))).ToList();
            var pagesBlock = await Task.WhenAll(pageLoadTasks);
            loadedPageBlocks.Add(pagesBlock);

            blockStartIndex += blockSize;
        }

        var pages = loadedPageBlocks.SelectMany(block => block, (block, list) => list).ToList();
        var items = new List<TViewModel>(firstPage.Count + pages.Select(p => p.Count).DefaultIfEmpty(0).Sum());
        items.AddRange(firstPage);
        foreach (var page in pages) items.AddRange(page);

        return items;
    }

    /// <summary>
    ///     Loads all pages by blocks of provided maxThreads size, applying selector after loading of each block
    /// </summary>
    /// <param name="loadPageTask">Task, assepting page size and page index and returning one loaded page of items</param>
    /// <param name="selector">Selector that is applied to each loaded page in block</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="maxThreads">The maximum threads that can be used to load pages.</param>
    /// <typeparam name="TViewModel">Type of the loaded page item</typeparam>
    /// <typeparam name="TResult">Type of the selector result</typeparam>
    /// <returns></returns>
    public static Task<List<TResult>> LoadAllPages<TViewModel, TResult>(
        Func<Paginator, Task<PaginatedList<TViewModel>>> loadPageTask,
        Func<PaginatedList<TViewModel>, TResult> selector,
        int pageSize, int maxThreads)
    {
        return LoadAllPages(loadPageTask, result => Task.FromResult(selector(result)), pageSize, maxThreads);
    }

    /// <summary>
    ///     Loads all pages by blocks of provided maxThreads size, applying action to pages after loading of each block
    /// </summary>
    /// <param name="loadPageTask">Task, assepting page size and page index and returning one loaded page of items</param>
    /// <param name="action">Action that is performed to each loaded page in block</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="maxThreads">The maximum threads that can be used to load pages.</param>
    /// <typeparam name="TViewModel">Type of the loaded page item</typeparam>
    /// <returns></returns>
    public static Task LoadAllPages<TViewModel>(Func<Paginator, Task<PaginatedList<TViewModel>>> loadPageTask,
        Func<PaginatedList<TViewModel>, Task> action,
        int pageSize, int maxThreads = 0)
    {
        return LoadAllPages(loadPageTask, result => action(result).AsTaskWithResult(), pageSize, maxThreads);
    }

    /// <summary>
    ///     Loads pages, applying action to each loaded page
    /// </summary>
    public static Task ForEachPage<TViewModel>(Func<Paginator, Task<PaginatedList<TViewModel>>> loadPageTask,
        Func<PaginatedList<TViewModel>, Task> action,
        int pageSize, int maxThreads = 0)
    {
        return LoadAllPages(loadPageTask, action, pageSize, maxThreads);
    }

    /// <summary>
    ///     Loads all pages by blocks of provided maxThreads size, applying selector after loading of each block
    /// </summary>
    /// <param name="loadPageTask">Task, assepting page size and page index and returning one loaded page of items</param>
    /// <param name="selector">Selector that is applied to each loaded page in block</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="maxThreads">The maximum threads that can be used to load pages.</param>
    /// <typeparam name="TViewModel">Type of the loaded page item</typeparam>
    /// <typeparam name="TResult">Type of the selector result</typeparam>
    /// <returns></returns>
    public static async Task<List<TResult>> LoadAllPages<TViewModel, TResult>(
        Func<Paginator, Task<PaginatedList<TViewModel>>> loadPageTask,
        Func<PaginatedList<TViewModel>, Task<TResult>> selector,
        int pageSize, int maxThreads)
    {
        var firstPage = await loadPageTask(new Paginator(pageSize, 1));
        var firstResult = await selector(firstPage);
        if (!firstPage.NextPageExists)
            return firstResult.AsList();

        var totalPages = firstPage.TotalPages;
        if (maxThreads <= 0)
            maxThreads = totalPages - 1;

        var loadedPageResultBlocks = new List<TResult[]>();
        var blockStartIndex = 2;
        while (blockStartIndex <= totalPages)
        {
            var blockSize = Math.Min(maxThreads, totalPages - blockStartIndex + 1);
            var pageLoadTasks = Enumerable.Range(blockStartIndex, blockSize)
                .Select(pageNumber => loadPageTask(new Paginator(pageSize, pageNumber)))
                .Select(task => task.WithResult(selector))
                .ToList();
            var pagesBlock = await Task.WhenAll(pageLoadTasks);
            loadedPageResultBlocks.Add(pagesBlock);

            blockStartIndex += blockSize;
        }

        var results = loadedPageResultBlocks.SelectMany(block => block, (block, list) => list).Prepend(firstResult)
            .ToList();

        return results;
    }

    /// <summary>
    ///     Loads all pages.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <param name="loadPageTask">Task, assepting page size and page index and returning one loaded page of items</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="maxThreads">The maximum threads that can be used to load pages.</param>
    /// <returns>
    ///     All items loaded
    /// </returns>
    public static Task<List<TViewModel>> LoadAllPages<TViewModel>(
        Func<int, int, Task<PaginatedList<TViewModel>>> loadPageTask, int pageSize, int maxThreads)
    {
        return LoadAllPages(paginator => loadPageTask(paginator.PageSize, paginator.PageNumber), pageSize, maxThreads);
    }

    /// <summary>
    ///     Loads the pages one by one, until specified condition for all loaded pages will not be met.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <param name="loadPageTask">The load page task.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="stopLoadingPagesCondition">The stop loading pages condition.</param>
    /// <returns>All loaded pages</returns>
    public static Task<List<TViewModel>> LoadPagesUntil<TViewModel>(
        Func<Paginator, Task<PaginatedList<TViewModel>>> loadPageTask,
        int pageSize,
        Predicate<List<TViewModel>> stopLoadingPagesCondition)
    {
        return LoadPagesUntil(loadPageTask, pageSize, list => Task.FromResult(stopLoadingPagesCondition(list)));
    }

    /// <summary>
    ///     Loads the pages one by one, until specified condition for all loaded pages will not be met.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <param name="loadPageTask">The load page task.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="stopLoadingPagesCondition">The stop loading pages condition.</param>
    /// <returns>All loaded pages</returns>
    public static async Task<List<TViewModel>> LoadPagesUntil<TViewModel>(
        Func<Paginator, Task<PaginatedList<TViewModel>>> loadPageTask,
        int pageSize,
        Func<List<TViewModel>, Task<bool>> stopLoadingPagesCondition)
    {
        var firstPage = await loadPageTask(new Paginator(pageSize, 1));
        var loadedPages = new List<TViewModel>(firstPage);
        var nextPageExists = firstPage.NextPageExists;

        var pageNumber = 2;
        while (nextPageExists && !await stopLoadingPagesCondition(loadedPages))
        {
            var page = await loadPageTask(new Paginator(pageSize, pageNumber++));
            nextPageExists = page.NextPageExists;
            loadedPages.AddRange(page);
        }

        return loadedPages;
    }

    /// <summary>
    ///     Loads the pages one by one, until specified condition for all loaded pages will not be met.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <param name="loadPageTask">The load page task.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="stopLoadingPagesCondition">The stop loading pages condition.</param>
    /// <returns>All loaded pages</returns>
    public static async Task<List<TViewModel>> LoadPagesUntil<TViewModel>(
        Func<Paginator, Task<PaginatedList<TViewModel>>> loadPageTask,
        int pageSize,
        Func<List<TViewModel>, PaginatedList<TViewModel>, Task<bool>> stopLoadingPagesCondition)
    {
        var page = await loadPageTask(new Paginator(pageSize, 1));
        var loadedPages = new List<TViewModel>(page);
        var nextPageExists = page.NextPageExists;

        var pageNumber = 2;
        while (nextPageExists && !await stopLoadingPagesCondition(loadedPages, page))
        {
            page = await loadPageTask(new Paginator(pageSize, pageNumber++));
            nextPageExists = page.NextPageExists;
            loadedPages.AddRange(page);
        }

        return loadedPages;
    }

    /// <summary>
    ///     Loads the pages one by one, until specified condition for all loaded pages will not be met.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="loadPageTask">The load page task.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="selector"></param>
    /// <param name="stopLoadingPagesCondition">The stop loading pages condition.</param>
    /// <returns>All loaded pages</returns>
    public static async Task<List<TResult>> LoadPagesUntil<TViewModel, TResult>(
        Func<Paginator, Task<PaginatedList<TViewModel>>> loadPageTask,
        int pageSize,
        Func<PaginatedList<TViewModel>, Task<TResult>> selector,
        Func<List<TResult>, TResult, Task<bool>> stopLoadingPagesCondition)
    {
        var page = await loadPageTask(new Paginator(pageSize, 1));
        var pageResult = await selector(page);
        var loadedResults = pageResult.AsList();
        var nextPageExists = page.NextPageExists;

        var pageNumber = 2;
        while (nextPageExists && !await stopLoadingPagesCondition(loadedResults, pageResult))
        {
            page = await loadPageTask(new Paginator(pageSize, pageNumber++));
            pageResult = await selector(page);
            nextPageExists = page.NextPageExists;
            loadedResults.Add(pageResult);
        }

        return loadedResults;
    }
}