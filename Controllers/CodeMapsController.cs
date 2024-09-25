using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shortly_API.Models;

namespace Shortly_API.Controllers
{
    [ApiController]
    public class CodeMapsController : ControllerBase
    {
        private readonly CodeMapContext _context;

        public CodeMapsController(CodeMapContext context)
        { 
            _context = context;
        }

        [HttpGet("/redirect/{code}")]
        public async Task<IActionResult> RedirectCodeMap(string code)
        {
            var foundCodeMap = await _context.CodeMaps.SingleOrDefaultAsync(codeMap => codeMap.Code == code);
            if (foundCodeMap == null)
            {
                return NotFound();
            }

            var link = foundCodeMap.Link;

            if (link == null)
            {
                return NotFound();
            }

            return Redirect(link);

        }

        [HttpPost("/create")]
        public async Task<IActionResult> CreateCodeMap([FromQuery] string link)
        {
            var code = RandomString(6);

            var uri = Uri.IsWellFormedUriString(link, UriKind.Absolute) ? new Uri(link) : new UriBuilder(link).Uri;
            
            var newCodeMap = new CodeMap {
                Link = uri.ToString(),
                Code = code,
            };

            Console.WriteLine($"Created link for {code} to {uri}");

            _context.CodeMaps.Add(newCodeMap);
            await _context.SaveChangesAsync();

            return Ok(newCodeMap);
        }

        private bool CodeMapExists(long id)
        {
            return _context.CodeMaps.Any(e => e.Id == id);
        }

        private static string RandomString(int length)
        {
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, length)
                .Select(x => pool[random.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }

        private static readonly Random random = new Random();

    }
}
