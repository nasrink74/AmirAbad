using SpadStorePanel.Core.Models;
using SpadStorePanel.Infrastructure;
using SpadStorePanel.Infrastructure.Repositories;
using SpadStorePanel.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpadStorePanel.Web.Controllers
{
    public class CantactFormController : Controller
    {

        private readonly StaticContentDetailsRepository _staticContentDetailsRepository;
        private readonly ContactFormsRepository _contactFormsRepository;
        private readonly MyDbContext _context;
        public CantactFormController(MyDbContext context,ContactFormsRepository contactFormsRepository,StaticContentDetailsRepository staticContentDetailsRepository)
        {
            _staticContentDetailsRepository = staticContentDetailsRepository;
            _contactFormsRepository = contactFormsRepository;
            _context = context;
        }
        // GET: Cantact
        public ActionResult Index()
        {
            SocialViewModel socialViewModel = new SocialViewModel();
            FooterViewModel footerViewModel = new FooterViewModel(socialViewModel);
            if (_staticContentDetailsRepository.Get(5) != null) { footerViewModel._SocialLinks.Facebook = _staticContentDetailsRepository.Get(5).Link; }
            else { footerViewModel._SocialLinks.Facebook = "#"; }
            if (_staticContentDetailsRepository.Get(6) != null) { footerViewModel._SocialLinks.Linkdin = _staticContentDetailsRepository.Get(6).Link; }
            else { footerViewModel._SocialLinks.Linkdin = "#"; }

            if (_staticContentDetailsRepository.Get(12) != null) { footerViewModel._SocialLinks.twitter = _staticContentDetailsRepository.Get(12).Link; }
            else { footerViewModel._SocialLinks.twitter = "#"; }

            if (_staticContentDetailsRepository.Get(7) != null) { footerViewModel.Address = _staticContentDetailsRepository.Get(7).Description; }
            else { footerViewModel.Address = ""; }

            if (_staticContentDetailsRepository.Get(8) != null) { footerViewModel.Email = _staticContentDetailsRepository.Get(8).Description; }
            else { footerViewModel.Email = ""; }

            if (_staticContentDetailsRepository.Get(9) != null) { footerViewModel.Phone = _staticContentDetailsRepository.Get(9).Description; }
            else { footerViewModel.Phone = ""; }

            if (_staticContentDetailsRepository.Get(10) != null) { footerViewModel.AccountNumber = _staticContentDetailsRepository.Get(10).Description; }
            else { footerViewModel.AccountNumber = ""; }

            if (_staticContentDetailsRepository.Get(11) != null) { footerViewModel.Map = _staticContentDetailsRepository.Get(11).Description; }
            else { footerViewModel.Map = ""; }


            if (_staticContentDetailsRepository.Get(48) != null) { footerViewModel.WorkTime = _staticContentDetailsRepository.Get(48).Description; }
            else { footerViewModel.WorkTime = ""; }
            return View(footerViewModel);
        }


        public ActionResult FormCantact()
        {
            return View();
        }
   
        [HttpPost]
        public string ContactUS(ContactForm form)
        {
            try
            {
                _context.ContactForms.Add(form);
                _context.SaveChanges();
      return "success";
            }
            catch
            {
                return "fail";
            }

        }
    }
}