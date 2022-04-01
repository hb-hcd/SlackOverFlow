using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NotQuora.Data;
using NotQuora.Models;
using System.Diagnostics;
namespace NotQuora.Controllers
{


    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext Db, UserManager<ApplicationUser> manager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _db = Db;
            _userManager = manager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        //public async Task<IActionResult> QuestionsIndex(int? pageNumber)
        //{
            //  var allQuestions = _db.Questions.Include(q => q.Answers).ThenInclude(q => q.User).ToList();
            //var questions = from question
            //                 in _db.Questions
            //                 select question;
            //var allQuestions = _db.Questions.Include(q => q.Answers).Include(q => q.User).Include(q => q.Tag);
            //int pageSize = 3;
            //var tags = _db.Tags.ToList();
            //ViewBag.taglist = new SelectList(tags, "Id", "Name");
            //if ( pageNumber < 1 )
            //{
            //    pageNumber = 1;
            //}
            //return View(await PaginatedList<Question>.CreateAsync(allQuestions.AsNoTracking(),pageNumber ??1,pageSize));
        //}

        public async Task<IActionResult> allQuestions(int? pageNumber)
        {

            var allQuestions = _db.Questions.Include(q => q.Answers).Include(q => q.User).Include(q => q.Tag);
            int pageSize = 3;
            var tags = _db.Tags.ToList();
            ViewBag.taglist = new SelectList(tags, "Id", "Name");
            if ( pageNumber < 1 )
            {
                pageNumber = 1;
            }
            //int totalPage = (int)Math.Ceiling( allQuestions.Count() / (double)pageSize);
            //if ( pageNumber >= totalPage )
            //{
            //    pageNumber = totalPage;
            //}
            return View(await PaginatedList<Question>.CreateAsync(allQuestions.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [Authorize]
        public async Task<IActionResult> UserCenter()
        {
            string currUsername = User.Identity.Name;
            if ( currUsername != null )
            {
                try
                {
                    ApplicationUser currUser = await _userManager.FindByEmailAsync(currUsername);
                    ViewBag.userId = currUser.Id;
                    return View();
                }
                catch ( Exception ex )
                {
                    return RedirectToAction("Error", new
                    {
                        message = ex.Message
                    });
                }
            }
            else
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> userQuestions(string uid)
        {
          ApplicationUser currUser =  await _userManager.FindByIdAsync(uid);
          var questions = _db.Questions.Include(q=>q.User).Where(q=>q.User.Id== currUser.Id).ToList();
          ViewBag.currUserId = currUser.Id;
          return View(questions);
        }

        public IActionResult editQuestion(int qid,string uid)
        {
            var currQuestion = _db.Questions.Include(q=>q.User).First(q=>q.Id==qid);
             if ( currQuestion.User.Id == uid )
            {
                ViewBag.uid = uid;
                return View(currQuestion);
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult editQuestion(string Title, string Description,string uid, Question question)
        {
            question.Title = Title;
            question.Description = Description; 
            _db.Update(question);
            _db.SaveChanges();
            return RedirectToAction("userQuestions", new
            {
                uid = uid
            } );
        }

        public async Task<IActionResult> userAnswers(string uid)
        {
            ApplicationUser currUser = await _userManager.FindByIdAsync(uid);
            try
            {
                if ( currUser != null )
                {
                    var answers = _db.Answers.Include(a => a.User).Where(a => a.User.Id == currUser.Id).ToList();
                    ViewBag.currUserId = currUser.Id;
                    return View(answers);
                }
                else
                {
                    return NotFound();
                }
            }catch (Exception ex)
            {
                return BadRequest();
            }
        }

        public IActionResult editAnswer(int aid, string uid)
        {
            var currAnswer = _db.Answers.Include(a=>a.User).First(a=>a.Id == aid);
            if ( currAnswer.User.Id == uid )
            {
                ViewBag.uid = uid;
                return View(currAnswer);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult editAnswer(int aid, string uid, string Content)
        {
            if ( aid != null && Content != null )
            {
                try
                {

                    Answer currAnswer = _db.Answers.First(a => a.Id == aid);
                    currAnswer.Content = Content;
                    _db.SaveChanges();
                    return RedirectToAction("userAnswers", new
                    {
                        uid = uid
                    });
                    ;
                }catch (Exception ex )
                {
                    return RedirectToAction("Error", new
                    {
                        Message = ex.Message
                    });
                }

            }
            else
            {
                return BadRequest();
            }

        }


        public async Task<IActionResult> markCorrectAnswer(string uid)
        {
            ApplicationUser currUser = await _userManager.FindByIdAsync(uid);

            var questions = _db.Questions.Where(q => q.User.Id == currUser.Id).ToList();
            return View(questions);

        }
        public IActionResult sortByTag(int TagId)
        {
            if ( TagId != null )
            {
                Tag tag = _db.Tags.FirstOrDefault(t => t.Id == TagId);
                if ( tag != null )
                {
                    ViewBag.tagName = tag.Name;
                    var sortedQuestions = _db.Questions.Include(q => q.Answers).Include(q => q.User).Where(q => q.TagId == TagId).ToList();
                    var tags = _db.Tags.ToList();
                    ViewBag.taglist = new SelectList(tags, "Id", "Name");
                    return View(sortedQuestions);
                }
                else
                {
                    return RedirectToAction("allQuestions");
                }
            }
            else
            {
                return RedirectToAction("allQuestions");
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ManageUserRole()
        {
            List<ApplicationUser> users = _db.Users.ToList();
            List<IdentityRole> roles = _roleManager.Roles.ToList();
            ViewBag.adminName = User.Identity.Name;
            ViewBag.userList = new SelectList(users);
            ViewBag.roleList = new SelectList(roles);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageUserRole(string userEmail, string roleName)
        {
            try
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(userEmail);
                IdentityRole role = await _roleManager.FindByNameAsync(roleName);
                if ( user != null && role != null )
                {
                    bool userAreadyInRole = await _userManager.IsInRoleAsync(user, role.Name);
                    if ( !userAreadyInRole )
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                        _db.SaveChanges();

                        TempData["message"] = $"Assigned {user.Email} as {role.Name}";
                    }
                    else
                    {
                        TempData["message"] = $"{user.Email} already in role {role.Name}";
                    }

                }
                else
                {
                    TempData["message"] = "Select an user and a role";

                }
                return RedirectToAction("ManageUserRole");
            }
            catch ( Exception ex )
            {
                return RedirectToAction("Error", new
                {
                    message = ex.Message
                });
            }
        }

        [Authorize]
        public IActionResult CreateQuestion()
        {
            string currUsername = User.Identity.Name;
            try
            {
                ApplicationUser user = _db.Users.First(u => u.UserName == currUsername);
                if ( user != null )
                {
                    var tags = _db.Tags.ToList();
                    ViewBag.taglist = new SelectList(tags, "Id", "Name");
                    return View();
                }
                else
                {
                    ViewBag["msg"] = "Please log in before post a question.";
                    return RedirectToAction("Index");
                }
            }
            catch ( Exception ex )
            {
                return NotFound(ex.Message);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuestion(string Title, string Description, int TagId)
        {
            ApplicationUser currUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            if ( Title != null && Description != null && TagId != null )
            {
                try
                {
                    Question question = new Question { Title = Title, Description = Description, TagId = TagId };
                    question.User = currUser;

                    currUser.Questions.Add(question);
                    _db.Tags.First(t => t.Id == TagId).Questions.Add(question);
                    await _userManager.UpdateAsync(currUser);
                    return RedirectToAction("allQuestions");
                }
                catch ( Exception ex )
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        [Authorize]
        public IActionResult questionDetail(int qid)
        {
            var currQuestion = _db.Questions.
                 Include(q => q.User).
                 Include(q => q.Tag).
                 Where(q => q.Id == qid).FirstOrDefault();

            ViewBag.allAnswers = _db.Answers.Include(a => a.User).Where(a => a.QuestionId == qid).ToList();


            return View(currQuestion);
        }

        public IActionResult postAnswer(int qid)
        {
            ViewBag.questionId = qid;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> postAnswer(string Content, int questionId)
        {
            ApplicationUser currUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            Question question = _db.Questions.First(q => q.Id == questionId);
            if ( currUser != null )
            {
                try
                {
                    Answer answer = new Answer(Content);
                    question.Answers.Add(answer);
                    currUser.Answers.Add(answer);
                    answer.User = currUser;
                    _db.Answers.Add(answer);
                    await _userManager.UpdateAsync(currUser);
                    return RedirectToAction("questionDetail", new
                    {
                        qid = questionId
                    });
                }
                catch ( Exception ex )
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return NotFound();
            }
        }


        [Authorize]
        public async Task<IActionResult> Upvote(int aid, int qid)
        {
            Answer answer = _db.Answers.Include(a => a.User).Where(a => a.Id == aid).First();
            ApplicationUser currUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if ( currUser != null )
            {
                try
                {
                    if ( currUser.Id != answer.User.Id )
                    {

                        answer.Upvote++;
                        answer.User.Reputation += 5;
                        await _userManager.UpdateAsync(answer.User);
                        return RedirectToAction("questionDetail", new
                        {
                            qid = qid
                        });
                    }
                    else
                    {
                        return RedirectToAction("allQuestions");
                    }

                }
                catch ( Exception ex )
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return RedirectToAction("Index");
            }

        }



        [Authorize]
        public async Task<IActionResult> Downvote(int aid, int qid)
        {
            Answer answer = _db.Answers.Include(a => a.User).Where(a => a.Id == aid).First();
            ApplicationUser currUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if ( currUser != null )
            {
                try
                {
                    if ( currUser.Id != answer.User.Id )
                    {

                        answer.Downvote++;
                        answer.User.Reputation -= 5;
                        await _userManager.UpdateAsync(answer.User);
                        return RedirectToAction("questionDetail", new
                        {
                            qid = qid
                        });
                    }
                    else
                    {
                        return RedirectToAction("allQuestions");
                    }

                }
                catch ( Exception ex )
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}