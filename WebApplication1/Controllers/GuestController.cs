using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class GuestController : Controller
    {
        [HttpGet]
        public ActionResult Register()
        {
            return View(new GuestListViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(GuestListViewModel model)
        {
            model.EnterGuestList = true;
            if (ModelState.IsValid)
            {
                if (model.EnterGuestList == true && !string.IsNullOrWhiteSpace(model.GuestListText))
                {
                    model.Guests = ParseGuestList(model.GuestListText, model.VisitFrom, model.VisitTo, model.ProjectFirm);
                }
                else
                {
                    model.Guests =
                    [
                        new Guest
                        {
                            VisitFrom = model.VisitFrom,
                            VisitTo = model.VisitTo,
                            ProjectFirm = model.ProjectFirm,
                            FirstName = model.FirstName,
                            LastName = model.LastName
                        }
                    ];
                }

                // Save guests to the database or perform other actions here.

                return View("Success", model.Guests); // Redirect to a success page or show the list of guests.
            }

            return View(model);
        }

        private List<Guest> ParseGuestList(string input, DateTime visitFrom, DateTime visitTo, string projectFirm)
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

            // Map the nameList to a list of Guest objects
            List<Guest> guests = new List<Guest>();
            foreach (string name in nameList)
            {
                string[] nameParts = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (nameParts.Length > 0)
                {
                    string firstName = nameParts[0];
                    string lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty;
                    guests.Add(new Guest
                    {
                        VisitFrom = visitFrom,
                        VisitTo = visitTo,
                        ProjectFirm = projectFirm,
                        FirstName = firstName,
                        LastName = lastName
                    });
                }
            }

            return guests;
        }
    }
}
