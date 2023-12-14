using Forge.OpenAI.Interfaces.Services;
using Forge.OpenAI.Models.ChatCompletions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OpenAIApp.Models;
using System.Diagnostics;
using TranslateGPT.Constants;

namespace TranslateGPT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IOpenAIService _openAIService;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, HttpClient httpClient, IOpenAIService openAIService)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
            _openAIService = openAIService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TranslateMessage()
        {
            ViewBag.Languages = new SelectList(Languages.mostUsedLanguages);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TranslateMessage(string query, string translateFrom, string translateTo)
        {
            ChatCompletionRequest request = new ChatCompletionRequest(new List<ChatMessage>
            {
                ChatMessage.CreateFromAssistant($"Translate from {translateFrom} to {translateTo}"),
                ChatMessage.CreateFromUser(query)
            }, "gpt-3.5-turbo");

            var response = await _openAIService.ChatCompletionService.GetAsync(request, CancellationToken.None).ConfigureAwait(false);

            ViewBag.Result = response.Result!.Choices[0].Message.Content;
            ViewBag.Languages = new SelectList(Languages.mostUsedLanguages);

            return View();
        }

        [HttpGet]
        public IActionResult ChatBot()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChatBot(string inputMessage)
        {
            ChatCompletionRequest request = new ChatCompletionRequest(new List<ChatMessage>
            {
                ChatMessage.CreateFromAssistant(inputMessage)
            }, "gpt-3.5-turbo");

            var response = await _openAIService.ChatCompletionService.GetAsync(request, CancellationToken.None).ConfigureAwait(false);

            ViewBag.Result = response.Result!.Choices[0].Message.Content;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}