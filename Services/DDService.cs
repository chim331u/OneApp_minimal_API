using System.Net;
using System.Text.RegularExpressions;
using fc_minimalApi.AppContext;
using fc_minimalApi.Contracts.DD;
using fc_minimalApi.Interfaces;
using fc_minimalApi.Models;
using Microsoft.EntityFrameworkCore;
using HtmlAgilityPack;

namespace fc_minimalApi.Services;

public class DDService : IDDService
{
    private readonly ApplicationContext _context; // Database context
    private readonly ILogger<DDService> _logger; // Logger for logging information and error
    private readonly IConfiguration _config;

    public DDService(ApplicationContext context, ILogger<DDService> logger, IConfiguration config)
    {
        _context = context;
        _logger = logger;
        _config = config;
    }

    #region DD

    public async Task<string> GetLinks(string link)
    {
        // var _setting = new DD_Settings()
        // {
        //     CreatedDate = DateTime.Now,
        //     IsActive = true,
        //     LastUpdatedDate = DateTime.Now,
        //     Id = 1, Dd_User = "chim", Dd_Password = "aneurysm"
        // };
        //
        // _setting = await AddSetting(_setting);

        var pageContent = await LoginDD(link);

        if (!string.IsNullOrEmpty(pageContent))
        {
            //crea o aggiorna thread
            var thread = await GetThread(pageContent);

            thread.Url = link;
            var oldThread = await _context.DDThreads.FirstOrDefaultAsync(x => x.Url.Equals(thread.Url));

            if (oldThread != null)
            {
                //already exists : update 

                oldThread.MainTitle = thread.MainTitle;
                oldThread.Type = thread.Type;
                oldThread.Note = thread.Note;
                thread = await UpdateThread(oldThread);
                _logger.LogInformation($"Thread already exists. Updated.");
            }
            else
            {
                //add
                thread = await AddThread(thread);
                _logger.LogInformation($"New thread: added.");
            }

            //cerca links
            List<DD_LinkEd2k>? links = await GetEd2kLinks(pageContent, thread);

            if (links != null)
            {
                //retrive all links for thread
                var savedLinks = await _context.DDLinkEd2.Where(x => x.Threads.Id == thread.Id).ToListAsync();

                List<DD_LinkEd2k> _linkToUpdate = new List<DD_LinkEd2k>();
                List<DD_LinkEd2k> _linkToAdd = new List<DD_LinkEd2k>();

                foreach (var _link in links)
                {
                    //check if exist
                    if (savedLinks.Any(x => x.Ed2kLink.Equals(_link.Ed2kLink)))
                    {
                        var oldLink = savedLinks.FirstOrDefault(x => x.Ed2kLink.Equals(_link.Ed2kLink));

                        // if (oldLink != null && oldLink.IsNew)
                        // {
                        //already exists : update isnew
                        oldLink.IsNew = false;
                        _linkToUpdate.Add(oldLink);
                        // await UpdateLinkEd2k(oldLink);
                        // }
                    }
                    else
                    {
                        //add
                        _link.IsNew = true;
                        _linkToAdd.Add(_link);
                        // await AddLinkEd2k(_link);
                        _logger.LogInformation($"New link: {_link.Title} added.");
                    }
                }

                if (_linkToUpdate.Count > 0)
                {
                    await UpdateListLinkEd2k(_linkToUpdate);
                }

                if (_linkToAdd.Count > 0)
                {
                    await AddListLinkEd2k(_linkToAdd);
                }
            }
        }

        return "ok";
    }

    public async Task<string> GetLinks(int threadId)
    {
        // var _setting = new DD_Settings()
        // {
        //     CreatedDate = DateTime.Now,
        //     IsActive = true,
        //     LastUpdatedDate = DateTime.Now,
        //     Id = 1, Dd_User = "chim", Dd_Password = "aneurysm"
        // };
        //
        // _setting = await AddSetting(_setting);

        var link = await _context.DDThreads.Where(x => x.Id == threadId).Select(x => x.Url).FirstOrDefaultAsync();


        if (link != null)
        {
            var pageContent = await LoginDD(link);

            if (!string.IsNullOrEmpty(pageContent))
            {
                //crea o aggiorna thread
                var thread = await GetThread(pageContent);

                thread.Url = link;
                var oldThread = await _context.DDThreads.FirstOrDefaultAsync(x => x.Url.Equals(thread.Url));

                if (oldThread != null)
                {
                    //already exists : update 

                    oldThread.MainTitle = thread.MainTitle;
                    oldThread.Type = thread.Type;
                    oldThread.Note = thread.Note;
                    thread = await UpdateThread(oldThread);
                    _logger.LogInformation($"Thread already exists. Updated.");
                }
                else
                {
                    //add
                    thread = await AddThread(thread);
                    _logger.LogInformation($"New thread: added.");
                }

                //cerca links
                List<DD_LinkEd2k>? links = await GetEd2kLinks(pageContent, thread);

                if (links != null)
                {
                    //retrive all links for thread
                    var savedLinks = await _context.DDLinkEd2.Where(x => x.Threads.Id == thread.Id).ToListAsync();

                    List<DD_LinkEd2k> _linkToUpdate = new List<DD_LinkEd2k>();
                    List<DD_LinkEd2k> _linkToAdd = new List<DD_LinkEd2k>();

                    foreach (var _link in links)
                    {
                        //check if exist
                        if (savedLinks.Any(x => x.Ed2kLink.Equals(_link.Ed2kLink)))
                        {
                            var oldLink = savedLinks.FirstOrDefault(x => x.Ed2kLink.Equals(_link.Ed2kLink));

                            // if (oldLink != null && oldLink.IsNew)
                            // {
                            //already exists : update isnew
                            oldLink.IsNew = false;
                            _linkToUpdate.Add(oldLink);
                            // await UpdateLinkEd2k(oldLink);
                            // }
                        }
                        else
                        {
                            //add
                            _link.IsNew = true;
                            _linkToAdd.Add(_link);
                            // await AddLinkEd2k(_link);
                            _logger.LogInformation($"New link: {_link.Title} added.");
                        }
                    }

                    if (_linkToUpdate.Count > 0)
                    {
                        await UpdateListLinkEd2k(_linkToUpdate);
                    }

                    if (_linkToAdd.Count > 0)
                    {
                        await AddListLinkEd2k(_linkToAdd);
                    }
                }
            }

            return "ok";
        }

        _logger.LogWarning("Thread not found.");
        return "Unable to find thread";
    }

    public async Task<string> UseLink(int linkId)
    {
        var link = await _context.DDLinkEd2.FirstOrDefaultAsync(x => x.Id == linkId);

        if (link != null)
        {
            link.IsUsed = true;
            link.LastUpdatedDate = DateTime.Now;
            link.IsNew = false;

            var linkUpdated = await UpdateLinkEd2k(link);

            if (linkUpdated != null)
            {
                return "ok";
            }
        }

        return "error";
    }

    public async Task<List<ThreadsDto>> GetActiveThreads()
    {
        try
        {
            var threads = await _context.DDThreads.Where(x => x.IsActive).ToListAsync();

            List<ThreadsDto> threadsDtoList = new List<ThreadsDto>();


            foreach (var _thread in threads)
            {
                var links = await _context.DDLinkEd2.Where(x => x.Threads.Id == _thread.Id).ToListAsync();
                var dto = new ThreadsDto();
                dto.Id = _thread.Id;
                dto.MainTitle = _thread.MainTitle;
                dto.LinksNumber = links.Count;
                dto.NewLinks = links.Any(x => x.IsNew);

                threadsDtoList.Add(dto);
            }

            return threadsDtoList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    public async Task<List<Ed2kLinkDto>> GetActiveLinks(int threadId)
    {
        try
        {
            var links = await _context.DDLinkEd2.Where(x => x.IsActive && x.Threads.Id == threadId).ToListAsync();

            return links.Select(x=> new Ed2kLinkDto
            {
                Id = x.Id, Ed2kLink = x.Ed2kLink, IsNew = x.IsNew, IsUsed = x.IsUsed, ThreadId = x.Threads.Id, Title = x.Title
            }).ToList();
            //return _mapper.Map<List<Ed2kLinkDto>>(links);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    private async Task<string> LoginDD(string threadLink)
    {
        var _setting = await _context.DDSettings.FirstOrDefaultAsync(x => x.IsActive);

        if (_setting != null)
        {
            var _ddUserName = _setting.Dd_User;
            var _ddPassword = _setting.Dd_Password;

            var clientHandler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };

            using var client = new HttpClient(clientHandler);

            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

            // 1. Verifica se l'utente è già loggato
            var checkResponse = await client.GetAsync(threadLink);
            var checkContent = await checkResponse.Content.ReadAsStringAsync();

            if (checkContent
                .Contains(_ddUserName)) // Controlla se il testo della pagina indica la necessità di login
            {
                _logger.LogInformation("Sei già loggato! Accedendo alla pagina protetta...");
            }
            else
            {
                _logger.LogInformation("Non sei loggato. Procedo con il login...");

                // Parametri per il login
                var loginData = new Dictionary<string, string>
                {
                    { "username", _ddUserName }, // Nome utente
                    { "password", _ddPassword }, // Password
                    { "redirect", "index.php" },
                    { "login", "Login" } // Nome del pulsante di login (può variare)
                };

                var loginContent = new FormUrlEncodedContent(loginData);

                // Eseguire la richiesta POST per il login
                var response = await client.PostAsync(threadLink, loginContent);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Errore durante il login.");
                    return string.Empty;
                }

                _logger.LogInformation("Login effettuato con successo!");
            }

            // Accedere alla pagina richiesta
            var pageResponse = await client.GetAsync(threadLink);
            var pageContent = await pageResponse.Content.ReadAsStringAsync();

            return pageContent;
        }
        else
        {
            _logger.LogWarning($"No settings found.");
            return string.Empty;
        }
    }

    private async Task<DD_Threads> GetThread(string pageContent)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(pageContent);

        //TROVA TITOLO PAGINA
        // Cerca il tag <h3> con class="first"

        string linkText = "";
        var h3Node = htmlDoc.DocumentNode.SelectSingleNode("//h3[@class='first']");
        if (h3Node == null)
        {
            _logger.LogWarning($"Nessun tag 'titolo' trovato.");
        }
        else
        {
            // All'interno di <h3 class="first">, cerca il tag <a>
            var aNode = h3Node.SelectSingleNode("a");
            if (aNode == null)
            {
                _logger.LogWarning($"Nessun tag 'titolo' trovato.");
                // return $"Nessun tag <a> trovato dentro <h3 class='first'>.";
            }
            else
            {
                // Estrae il testo e il valore dell'attributo href del tag <a>
                linkText = aNode.InnerText.Trim();
            }
        }

        //cread dd_thread
        var _thread = new DD_Threads
        {
            MainTitle = linkText,
            CreatedDate = DateTime.Now,
            IsActive = true
        };
        // Stampa i risultati
        _logger.LogInformation($"Titolo Pagina= {_thread.MainTitle}");
        return _thread;
    }

    private async Task<List<DD_LinkEd2k>> GetEd2kLinks(string pageContent, DD_Threads thread)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(pageContent);
        // Definisce il pattern per catturare i link che iniziano per "ed2k://" oppure "file://"
        string pattern = @"(?:ed2k:\/\/|file:\/\/)[^""'\s]+";

        // Esegue la ricerca con Regex
        var matches = Regex.Matches(htmlDoc.Text, pattern);

        if (matches.Count == 0)
        {
            _logger.LogWarning("Nessun link trovato che inizi per 'ed2k://' o 'file://'.");
            return null;
        }

        // Usa un HashSet per evitare duplicati
        List<string> allLinks = new List<string>();
        foreach (Match match in matches)
        {
            if (match.Success)
            {
                allLinks.Add(match.Value);
            }
        }

        var uniqueLinks = allLinks.Distinct().ToList();
        var _ed2kLinksList = new List<DD_LinkEd2k>();

        Regex regex = new Regex(@"\|file\|([^|]+)\|");

        foreach (var _link in uniqueLinks)
        {
            Match match = regex.Match(_link);

            if (match.Success)
            {
                var _title = match.Groups[1].Value;
                _title = WebUtility.UrlDecode(_title);
                _title = WebUtility.HtmlDecode(_title);
                var link = _link;
                link = WebUtility.UrlDecode(link);
                link = WebUtility.HtmlDecode(link);

                var dd_link = new DD_LinkEd2k
                {
                    Title = _title,
                    Ed2kLink = link,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsUsed = false,
                    Threads = thread
                };

                _ed2kLinksList.Add(dd_link);
            }
            else
            {
                _logger.LogInformation("Nome del file non trovato.");
            }
        }

        foreach (var ddLink in _ed2kLinksList)
        {
            _logger.LogInformation($"Titolo: {ddLink.Title} - Link: {ddLink.Ed2kLink}");
        }

        return _ed2kLinksList;
    }

    public async Task<DD_Threads> AddThread(DD_Threads thread)
    {
        try
        {
            thread.CreatedDate = DateTime.Now;
            thread.IsActive = true;

            var threadAdded = await _context.DDThreads.AddAsync(thread);
            await _context.SaveChangesAsync();

            return threadAdded.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    public async Task<DD_Threads> UpdateThread(DD_Threads thread)
    {
        try
        {
            thread.LastUpdatedDate = DateTime.Now;

            var threadUpdated = _context.DDThreads.Update(thread);
            await _context.SaveChangesAsync();

            return threadUpdated.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    public async Task<DD_Threads> DeleteThread(DD_Threads thread)
    {
        try
        {
            thread.IsActive = false;
            thread.LastUpdatedDate = DateTime.Now;
            var threadDeleted = _context.DDThreads.Update(thread);
            await _context.SaveChangesAsync();

            return threadDeleted.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    public async Task<DD_LinkEd2k> AddLinkEd2k(DD_LinkEd2k linkEd2K)
    {
        try
        {
            linkEd2K.CreatedDate = DateTime.Now;
            linkEd2K.IsActive = true;
            var linkAdded = await _context.DDLinkEd2.AddAsync(linkEd2K);
            await _context.SaveChangesAsync();

            return linkAdded.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    public async Task<List<DD_LinkEd2k>> AddListLinkEd2k(List<DD_LinkEd2k> linkEd2KList)
    {
        foreach (var linkEd2K in linkEd2KList)
        {
            try
            {
                linkEd2K.CreatedDate = DateTime.Now;
                linkEd2K.IsActive = true;
                var linkAdded = await _context.DDLinkEd2.AddAsync(linkEd2K);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error add link {linkEd2K.Title} - {ex.Message}");
                //return null;
            }
        }

        await _context.SaveChangesAsync();
        return linkEd2KList;
    }

    public async Task<DD_LinkEd2k> UpdateLinkEd2k(DD_LinkEd2k linkEd2K)
    {
        try
        {
            linkEd2K.LastUpdatedDate = DateTime.Now;
            var linkUpdated = _context.DDLinkEd2.Update(linkEd2K);
            await _context.SaveChangesAsync();

            return linkUpdated.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    public async Task<List<DD_LinkEd2k>> UpdateListLinkEd2k(List<DD_LinkEd2k> linkEd2KList)
    {
        foreach (var linkEd2K in linkEd2KList)
        {
            try
            {
                linkEd2K.LastUpdatedDate = DateTime.Now;
                var linkUpdated = _context.DDLinkEd2.Update(linkEd2K);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error update link {linkEd2K.Title} - {ex.Message}");
                //return null;
            }
        }

        await _context.SaveChangesAsync();
        return linkEd2KList;
    }

    public async Task<DD_LinkEd2k> DeleteLinkEd2k(DD_LinkEd2k linkEd2K)
    {
        try
        {
            linkEd2K.IsActive = false;
            linkEd2K.LastUpdatedDate = DateTime.Now;
            var linkDeleted = _context.DDLinkEd2.Update(linkEd2K);
            await _context.SaveChangesAsync();

            return linkDeleted.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    public async Task<DD_Settings> AddSetting(DD_Settings setting)
    {
        try
        {
            setting.CreatedDate = DateTime.Now;
            setting.IsActive = true;
            var settingAdded = await _context.DDSettings.AddAsync(setting);
            await _context.SaveChangesAsync();

            return settingAdded.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    public async Task<DD_Settings> UpdateSetting(DD_Settings setting)
    {
        try
        {
            setting.LastUpdatedDate = DateTime.Now;
            var settingUpdated = _context.DDSettings.Update(setting);
            await _context.SaveChangesAsync();

            return settingUpdated.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    public async Task<DD_Settings> DeleteSetting(DD_Settings setting)
    {
        try
        {
            setting.IsActive = false;
            setting.LastUpdatedDate = DateTime.Now;
            var settingDeleted = _context.DDSettings.Update(setting);
            await _context.SaveChangesAsync();

            return settingDeleted.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }

        return null;
    }
    #endregion
}

