using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ParseController : Controller
    {
        // GET: Parse
        public ActionResult Index()
        {
            return View(new ParseViewModel());
        }

        [HttpPost]
        public ActionResult Index(ParseViewModel model)
        {

            if (!string.IsNullOrWhiteSpace(model.InputText))
            {
                model.Users = ParseInput(model.InputText);
            }


            return View(model);
        }

        public static List<User> ParseInput(string input)
        {
            // Define tags to be ignored and remove them along with their content
            string[] tagsToIgnore = { "title" };
            foreach (var tag in tagsToIgnore)
            {
                input = Regex.Replace(input, $@"<\s*{tag}[^>]*>.*?<\s*/\s*{tag}\s*>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            }

            // Replace HTML line breaks with newlines
            input = input.Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n");

            // Sanitize input by removing remaining HTML tags, except for <ul>, <ol>, and <li> tags
            input = Regex.Replace(input, @"<(?!/?(ul|ol|li)\b)[^>]+>", string.Empty, RegexOptions.IgnoreCase);

            // Replace <li> tags with newlines
            input = input.Replace("</li>", "\n").Replace("<li>", "");

            // Replace <ul> and <ol> tags with newlines
            input = input.Replace("</ul>", "\n").Replace("<ul>", "\n").Replace("</ol>", "\n").Replace("<ol>", "\n");

            // Define all delimiters in a single regex pattern
            string[] delimiters = new[] { "\r\n", "\n", "\r", ",", ";" };
            string delimiterPattern = string.Join("|", delimiters.Select(Regex.Escape));

            // Replace all delimiters with a single common delimiter (semicolon in this case)
            string normalizedInput = Regex.Replace(input, delimiterPattern, ";");

            // Replace multiple spaces with a single space and trim leading/trailing whitespace
            normalizedInput = Regex.Replace(normalizedInput, @"\s+", " ").Trim();

            // Split input by the common delimiter
            string[] nameList = normalizedInput.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(name => name.Trim())
                                               .ToArray();

            // Map the nameList to a list of User objects
            List<User> userObjects = new List<User>();
            foreach (string name in nameList)
            {
                string[] nameParts = name.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (nameParts.Length > 0)
                {
                    string firstName = nameParts[0];
                    string lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty;
                    userObjects.Add(new User { FirstName = firstName, LastName = lastName });
                }
            }

            return userObjects;
        }
    }
}


