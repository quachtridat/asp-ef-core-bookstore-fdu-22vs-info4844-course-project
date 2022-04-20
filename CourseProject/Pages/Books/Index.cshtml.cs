using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CourseProject.Models;
using CourseProject.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseProject.Pages.Books {
    public class IndexModel : PageModel {
        protected readonly SimpleMemoryCache _cache;
        protected readonly Data.CourseProjectAzureSqlContext _context;
        protected readonly ILogger<IndexModel> _logger;

        public IndexModel(Data.CourseProjectAzureSqlContext context, ILogger<IndexModel> logger, SimpleMemoryCache simpleMemoryCache) {
            _context = context;
            _logger = logger;
            _cache = simpleMemoryCache;
        }

        protected SimpleMemoryCache.Options DefaultCacheOptions { get; set; } = new SimpleMemoryCache.Options {
            AbsoluteExpiration = TimeSpan.FromMinutes(1),
            AutoExpiration = TimeSpan.FromMinutes(1),
            SlidingExpiration = TimeSpan.FromMinutes(1)
        };

        public enum BooksViewType {
            None = 0,
            List = 1,
            Tiles = 2
        }

        public BooksViewType CurrentBooksViewType { get; set; } = BooksViewType.Tiles;

        public int PerPage { get; protected set; } = 10;
        public int MaxPagination { get; protected set; } = 5;

        public int NumPages { get; set; } = 1;
        public int CurrentPage { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public bool ForceClearCache { get; set; } = false;

        public IList<Book> Books { get; set; } = new List<Book>();

        public ISet<string> AllCategories { get; set; } = new SortedSet<string>();
        public ISet<string> AllCountries { get; set; } = new SortedSet<string>();
        public ISet<string> AllLanguages { get; set; } = new SortedSet<string>();

        public IEnumerable<SelectListItem> FilterCategories { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> FilterCountries { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> FilterLanguages { get; set; } = new List<SelectListItem>();

        public string FilterWildcardText { get; protected set; } = "(Any)";

        [BindProperty]
        public string? FilteredTitle { get; set; } = null;
        [BindProperty]
        public string? FilteredAuthor { get; set; } = null;
        [BindProperty]
        public string? FilteredDescription { get; set; } = null;
        [BindProperty]
        public IEnumerable<string>? FilteredCategories { get; set; } = null;
        [BindProperty]
        public IEnumerable<string>? FilteredCountries { get; set; } = null;
        [BindProperty]
        public IEnumerable<string>? FilteredLanguages { get; set; } = null;

        protected Dictionary<string, dynamic> GetFilteredBookPropsSerializedDictionary() {
            const string nil = "(null)";
            return new Dictionary<string, dynamic> {
                { nameof(FilteredTitle), FilteredTitle is null ? nil : $"\"{FilteredTitle}\"" },
                { nameof(FilteredAuthor), FilteredAuthor is null ? nil : $"{{{string.Join(',', FilteredAuthor.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(str => $"\"{str}\""))}}}" },
                { nameof(FilteredDescription), FilteredDescription is null ? nil : $"\"{FilteredDescription}\"" },
                { nameof(FilteredCategories), FilteredCategories is null ? nil : $"{{{string.Join(',', FilteredCategories.Select(str => $"\"{str}\""))}}}" },
                { nameof(FilteredCountries), FilteredCountries is null ? nil : $"{{{string.Join(',', FilteredCountries.Select(str => $"\"{str}\""))}}}" },
                { nameof(FilteredLanguages), FilteredLanguages is null ? nil : $"{{{string.Join(',', FilteredLanguages.Select(str => $"\"{str}\""))}}}" }
            };
        }

        protected object GetCacheKeyByProps(Dictionary<string, dynamic>? props = null, string? customKeyName = null) {
            if (customKeyName is null) customKeyName = string.Empty;
            string propString =
                props is null ?
                string.Empty :
                string.Join(';', props.Select(kv => $"{kv.Key}={kv.Value}"));
            return $"{nameof(CourseProject.Pages.Books.IndexModel)}{{{propString}}}_\"{customKeyName}\"".GetHashCode();
        }

        public async Task OnGetAsync(int? pageNumber, string? booksViewType) {
            CurrentPage = Math.Max(1, pageNumber.GetValueOrDefault(1));
            if (Enum.TryParse(booksViewType, true, out BooksViewType parsedBookViewType))
                CurrentBooksViewType = parsedBookViewType;

            await ObtainCachedNumPagesCachedFilteredBooksClientSideAsync(ForceClearCache);
            await ObtainCachedFilteredBooksClientSideAsyncForPageNumber(CurrentPage, ForceClearCache);

            //await ObtainCachedNumPagesCachedFilteredBooksServerSideAsync();
            //await ObtainCachedFilteredBooksServerSideAsyncForPageNumber(CurrentPage);

            await ObtainCachedAllCategoriesAsync(ForceClearCache);
            await ObtainCachedAllCountriesAsync(ForceClearCache);
            await ObtainCachedAllLanguagesAsync(ForceClearCache);
            ObtainFilterCategories();
            ObtainFilterCountries();
            ObtainFilterLanguages();
        }

        public async Task<IActionResult> OnPostAsync() {
            await ObtainCachedNumPagesCachedFilteredBooksClientSideAsync(ForceClearCache);
            await ObtainCachedFilteredBooksClientSideAsyncForPageNumber(1, ForceClearCache);

            //await ObtainCachedNumPagesCachedFilteredBooksServerSideAsync();
            //await ObtainCachedFilteredBooksServerSideAsyncForPageNumber();

            await ObtainCachedAllCategoriesAsync(ForceClearCache);
            await ObtainCachedAllCountriesAsync(ForceClearCache);
            await ObtainCachedAllLanguagesAsync(ForceClearCache);
            ObtainFilterCategories();
            ObtainFilterCountries();
            ObtainFilterLanguages();

            return Page();
        }

        private void ObtainFilterLanguages() {
            FilterLanguages = AllLanguages.Select(
                language => new SelectListItem {
                    Value = language,
                    Text = language
                }
            ).ToList();
        }

        private void ObtainFilterCountries() {
            FilterCountries = AllCountries.Select(
                country => new SelectListItem {
                    Value = country,
                    Text = country
                }
                ).ToList();
        }

        public void ObtainFilterCategories() {
            FilterCategories = AllCategories.Select(
                category => new SelectListItem {
                    Value = category,
                    Text = category
                }
            ).ToList();
        }

        public async ValueTask ObtainCachedAllLanguagesAsync(bool forceClearCache = false) {
            AllLanguages = await _cache.GetSetAsync(
                GetCacheKeyByProps(customKeyName: nameof(AllLanguages)),
                async () => {
                    var allLanguagesArray = await _context.Books.Select(book => book.Languages).ToArrayAsync();
                    var resultSet = new SortedSet<string>();
                    foreach (var languages in allLanguagesArray) {
                        foreach (var language in languages) {
                            resultSet.Add(language);
                        }
                    }
                    return resultSet;
                },
                DefaultCacheOptions,
                forceClearCache
                );
        }

        public async ValueTask ObtainCachedAllCountriesAsync(bool forceClearCache = false) {
            AllCountries = await _cache.GetSetAsync(
                GetCacheKeyByProps(customKeyName: nameof(AllCountries)),
                async () => {
                    var allCountriesArray = await _context.Books.Select(book => book.Countries).ToArrayAsync();
                    var resultSet = new SortedSet<string>();
                    foreach (var countries in allCountriesArray) {
                        foreach (var country in countries) {
                            resultSet.Add(country);
                        }
                    }
                    return resultSet;
                },
                DefaultCacheOptions,
                forceClearCache
                );
        }
        
        public async ValueTask ObtainCachedAllCategoriesAsync(bool forceClearCache = false) {
            AllCategories = await _cache.GetSetAsync(
                GetCacheKeyByProps(customKeyName: nameof(AllCategories)),
                async () => {
                    var allCategoriesArray = await _context.Books.Select(book => book.Categories).ToArrayAsync();
                    var resultSet = new SortedSet<string>();
                    foreach (var categories in allCategoriesArray) {
                        foreach (var category in categories) {
                            resultSet.Add(category);
                        }
                    }
                    return resultSet;
                },
                DefaultCacheOptions,
                forceClearCache
                );
        }

        public async ValueTask ObtainCachedFilteredBooksClientSideAsync(bool forceClearCache = false) {
            Books = (await GetCachedFilteredBooksClientSideAsync(forceClearCache)).ToList();
        }

        public async ValueTask ObtainCachedFilteredBooksServerSideAsync(bool forceClearCache = false) {
            Books = (await GetCachedFilteredBooksServerSideAsync(forceClearCache)).ToList();
        }

        public async ValueTask ObtainCachedFilteredBooksClientSideAsyncForPageNumber(int pageNumber = 1, bool forceClearCache = false) {
            Books = (await GetCachedFilteredBooksClientSideAsyncForPageNumber(pageNumber, forceClearCache)).ToList();
        }

        public async ValueTask ObtainCachedFilteredBooksServerSideAsyncForPageNumber(int pageNumber = 1, bool forceClearCache = false) {
            Books = (await GetCachedFilteredBooksServerSideAsyncForPageNumber(pageNumber, forceClearCache)).ToList();
        }

        public async ValueTask ObtainCachedNumPagesCachedFilteredBooksClientSideAsync(bool forceClearCache = false) {
            NumPages = await GetCachedNumPagesCachedFilteredBooksClientSideAsync(forceClearCache);
        }

        public async ValueTask ObtainCachedNumPagesCachedFilteredBooksServerSideAsync(bool forceClearCache = false) {
            NumPages = await GetCachedNumPagesCachedFilteredBooksServerSideAsync(forceClearCache);
        }

        public async ValueTask<int> GetCachedNumPagesCachedFilteredBooksClientSideAsync(bool forceClearCache = false) {
            var propsDict = GetFilteredBookPropsSerializedDictionary();

            propsDict.Add(nameof(PerPage), PerPage);

            return await _cache.GetSetAsync(
                GetCacheKeyByProps(propsDict, nameof(GetCachedNumPagesCachedFilteredBooksClientSideAsync)),
                async () => Convert.ToInt32(Math.Ceiling((await GetCachedFilteredBooksClientSideAsync(forceClearCache)).Count() / Convert.ToDouble(PerPage))),
                DefaultCacheOptions,
                forceClearCache
            );
        }

        public async ValueTask<int> GetCachedNumPagesCachedFilteredBooksServerSideAsync(bool forceClearCache = false) {
            var propsDict = GetFilteredBookPropsSerializedDictionary();

            propsDict.Add(nameof(PerPage), PerPage);

            return await _cache.GetSetAsync(
                GetCacheKeyByProps(propsDict, nameof(GetCachedNumPagesCachedFilteredBooksServerSideAsync)),
                async () => Convert.ToInt32(Math.Ceiling((await GetCachedFilteredBooksServerSideAsync(forceClearCache)).Count() / Convert.ToDouble(PerPage))),
                DefaultCacheOptions,
                forceClearCache
            );
        }

        public async ValueTask<IEnumerable<Book>> GetCachedFilteredBooksClientSideAsyncForPageNumber(int pageNumber = 1, bool forceClearCache = false) {
            //var propsDict = GetFilteredBookPropsSerializedDictionary();

            //propsDict.Add(nameof(PerPage), PerPage);
            //propsDict.Add(nameof(pageNumber), pageNumber);

            //return await _cache.GetSetAsync(
            //    GetCacheKeyByProps(propsDict, nameof(GetCachedFilteredBooksClientSideAsyncForPageNumber)),
            //    async () => (await GetCachedFilteredBooksClientSideAsync(forceClearCache)).Skip((pageNumber - 1) * PerPage).Take(PerPage),
            //    DefaultCacheOptions,
            //    forceClearCache
            //);

            return (await GetCachedFilteredBooksClientSideAsync(forceClearCache)).Skip((pageNumber - 1) * PerPage).Take(PerPage);
        }

        public async ValueTask<IEnumerable<Book>> GetCachedFilteredBooksServerSideAsyncForPageNumber(int pageNumber = 1, bool forceClearCache = false) {
            //var propsDict = GetFilteredBookPropsSerializedDictionary();

            //propsDict.Add(nameof(PerPage), PerPage);
            //propsDict.Add(nameof(pageNumber), pageNumber);

            //return await _cache.GetSetAsync(
            //    GetCacheKeyByProps(propsDict, nameof(GetCachedFilteredBooksServerSideAsyncForPageNumber)),
            //    async () => (await GetCachedFilteredBooksServerSideAsync(forceClearCache)).Skip((pageNumber - 1) * PerPage).Take(PerPage),
            //    DefaultCacheOptions,
            //    forceClearCache
            //);

            return (await GetCachedFilteredBooksServerSideAsync(forceClearCache)).Skip((pageNumber - 1) * PerPage).Take(PerPage);
        }

        public async ValueTask<IEnumerable<Book>> GetCachedFilteredBooksClientSideAsync(bool forceClearCache = false) {
            return await _cache.GetSetAsync(
                GetCacheKeyByProps(
                    GetFilteredBookPropsSerializedDictionary(),
                    nameof(GetCachedFilteredBooksClientSideAsync)
                ),
                async () => await GetFilteredBooksClientSideAsync(),
                DefaultCacheOptions,
                forceClearCache
            );
        }

        public async ValueTask<IEnumerable<Book>> GetCachedFilteredBooksServerSideAsync(bool forceClearCache = false) {
            return await _cache.GetSetAsync(
                GetCacheKeyByProps(
                    GetFilteredBookPropsSerializedDictionary(),
                    nameof(GetCachedFilteredBooksServerSideAsync)
                ),
                async () => await GetFilteredBooksServerSideAsync(),
                DefaultCacheOptions,
                forceClearCache
            );
        }

        public async ValueTask<IEnumerable<Book>> GetFilteredBooksClientSideAsync() {
            var allBooks = _context.Books;

            // order by book ID
            var orderedAllBooks = await allBooks.OrderBy(book => book.BookId).ToArrayAsync();

            var resultBooks = orderedAllBooks.Take(0);

            resultBooks = resultBooks.UnionBy(
                orderedAllBooks.Where(book =>
                    (string.IsNullOrWhiteSpace(FilteredTitle) || book.Title.Contains(FilteredTitle)) &&
                    (string.IsNullOrWhiteSpace(FilteredAuthor) || ((string)book.Authors).Contains(FilteredAuthor)) &&
                    (string.IsNullOrWhiteSpace(FilteredDescription) || ((string)book.Descriptions).Contains(FilteredDescription))),
                book => book.BookId
            );

            // match filtered categories
            if (FilteredCategories is not null) {
                if (!FilteredCategories.Contains(FilterWildcardText)) {
                    var filteredCategorySet = new HashSet<string>(FilteredCategories);
                    resultBooks = resultBooks.IntersectBy(
                        resultBooks.Where(book => ((string)book.Categories).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Any(category => filteredCategorySet.Contains(category))).Select(book => book.BookId),
                        book => book.BookId
                    );
                }
            }

            // match filtered countries
            if (FilteredCountries is not null) {
                if (!FilteredCountries.Contains(FilterWildcardText)) {
                    var filteredCountrySet = new HashSet<string>(FilteredCountries);
                    resultBooks = resultBooks.IntersectBy(
                        resultBooks.Where(book => ((string)book.Countries).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Any(country => filteredCountrySet.Contains(country))).Select(book => book.BookId),
                        book => book.BookId
                    );
                }
            }

            // match filtered languages
            if (FilteredLanguages is not null) {
                if (!FilteredLanguages.Contains(FilterWildcardText)) {
                    var filteredLanguageSet = new HashSet<string>(FilteredLanguages);
                    resultBooks = resultBooks.IntersectBy(
                        resultBooks.Where(book => ((string)book.Languages).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Any(language => filteredLanguageSet.Contains(language))).Select(book => book.BookId),
                        book => book.BookId
                    );
                }
            }

            return resultBooks;
        }

        public async ValueTask<IEnumerable<Book>> GetFilteredBooksServerSideAsync() {
            var allBooksQuery = _context.Books.AsQueryable<Book>();

            // order by book ID
            var orderedAllBooksQuery = allBooksQuery.OrderBy(book => book.BookId);

            var resultBooksQuery = Enumerable.Empty<Book>().AsQueryable();

            resultBooksQuery = resultBooksQuery.UnionBy(
                orderedAllBooksQuery.Where(book =>
                    (string.IsNullOrWhiteSpace(FilteredTitle) || book.Title.Contains(FilteredTitle)) &&
                    (string.IsNullOrWhiteSpace(FilteredAuthor) || ((string)book.Authors).Contains(FilteredAuthor)) &&
                    (string.IsNullOrWhiteSpace(FilteredDescription) || ((string)book.Descriptions).Contains(FilteredDescription))),
                book => book.BookId
            );

            // match filtered categories
            if (FilteredCategories is not null) {
                if (!FilteredCategories.Contains(FilterWildcardText)) {
                    var filteredCategorySet = new HashSet<string>(FilteredCategories);
                    resultBooksQuery = resultBooksQuery.IntersectBy(
                        resultBooksQuery.Where(book => ((string)book.Categories).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Any(category => filteredCategorySet.Contains(category))).Select(book => book.BookId),
                        book => book.BookId
                    );
                }
            }

            // match filtered countries
            if (FilteredCountries is not null) {
                if (!FilteredCountries.Contains(FilterWildcardText)) {
                    var filteredCountrySet = new HashSet<string>(FilteredCountries);
                    resultBooksQuery = resultBooksQuery.IntersectBy(
                        resultBooksQuery.Where(book => ((string)book.Countries).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Any(country => filteredCountrySet.Contains(country))).Select(book => book.BookId),
                        book => book.BookId
                    );
                }
            }

            // match filtered languages
            if (FilteredLanguages is not null) {
                if (!FilteredLanguages.Contains(FilterWildcardText)) {
                    var filteredLanguageSet = new HashSet<string>(FilteredLanguages);
                    resultBooksQuery = resultBooksQuery.IntersectBy(
                        resultBooksQuery.Where(book => ((string)book.Languages).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Any(language => filteredLanguageSet.Contains(language))).Select(book => book.BookId),
                        book => book.BookId
                    );
                }
            }

            return await resultBooksQuery.ToListAsync();
        }
    }
}
