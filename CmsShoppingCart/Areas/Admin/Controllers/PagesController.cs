using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsShoppingCart.Models;
using CmsShoppingCart.Models.ViewModels.Pages;
using CmsShoppingCart.Models.Data;

namespace CmsShoppingCart.Areas.Admin.Controllers
{

    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {

            // Declare a list of PageVM
            List<PageVM> pagelist;
            // Init the list
            using (Db db = new Db())
            {
                pagelist = db.Pages.ToList().OrderBy(p => p.Sorting).Select(p => new PageVM(p)).ToList();
            }
            // return view with list
            return View(pagelist);
        }
        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }
        // Post: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            // Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                // Declare slug
                string slug;
                //init pageDto
                PageDto pageDto = new PageDto();
                //pageDto title
                pageDto.Title = model.Title;
                //check if and set slug if need be
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }
                // Make sure title and slug are unique

                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "That Title or Slug already exists.");
                    return View(model);
                }
                // pageDto the rest
                pageDto.Slug = model.Slug;
                pageDto.Body = model.Body;
                pageDto.HasSidebar = model.HasSidebar;
                pageDto.Sorting = 100;
                //save pageDto
                db.Pages.Add(pageDto);
                db.SaveChanges();
            }
            //set TempData message
            TempData["SM"] = "You have added a new page!";
            // Redirect
            return RedirectToAction("Index");
        }

        // Get: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            // Declare pagevm
            PageVM model;
            //Get the Page
            using (Db db = new Db())
            {
                PageDto pagedto = db.Pages.Find(id);
                //Confirm page exist
                if (pagedto == null)
                {
                    return Content("The page does not exist");
                }

                //Init pageVm
                model = new PageVM(pagedto);

            }

            //return the view model
            return View(model);
        }
        // Post: Admin/Pages/EditPage/id

        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            // Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                //Declare slug
                string slug;
                //Get the page
                PageDto pageDto = db.Pages.Find(model.Id);
                //check if and set slug if need be
                pageDto.Title = model.Title;

                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }
                // Make sure title and slug are unique
                if (db.Pages.Where(x => x.Id != model.Id).Any(x => x.Title == model.Title) ||
                    db.Pages.Where(x => x.Id != model.Id).Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "That Title or Slug already exists.");
                    return View(model);
                }
                // pageDto the rest
                pageDto.Slug = model.Slug;
                pageDto.Body = model.Body;
                pageDto.HasSidebar = model.HasSidebar;
                pageDto.Sorting = 100;
                //save pageDto
                db.SaveChanges();
            }
            //set TempData message
            TempData["SM"] = "You have Edited a new page!";
            // Redirect
            return RedirectToAction("Index");
        }

        // Get: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            //Declare PageVM
            PageVM page;
            using (Db db = new Db())
            {
                PageDto pageDto;
                pageDto = db.Pages.Find(id);
                //confirm page exist
                if (pageDto == null)
                {
                    return Content("the page does not exist.");
                }
                // init pagevm
                page = new PageVM(pageDto);

            }
            // return view with model
            return View(page);
        }

        // Get: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                PageDto page = db.Pages.Find(id);
                db.Pages.Remove(page);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Post: Admin/Pages/RecorderPages
        [HttpPost]
        public void RecorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                int count = 1;
                PageDto dto;
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }
            }
        }

        // Get: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            // Declare the model
            SidebarVM model;
            using (Db db = new Db())
            {            
                // get Dto
                SidebarDto dto = db.Sidebar.Find(1);
                //init the model
                model = new SidebarVM(dto);
            }
            // retutn view model

            return View(model);
        }
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                SidebarDto dto= db.Sidebar.Find(1);
                dto.Body = model.Body;
                db.SaveChanges();
            }
            TempData["SM"] = "You have Edited a Sidebar!";
            return RedirectToAction("Index");
        }

    }
}