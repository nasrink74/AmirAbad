using SpadStorePanel.Infrastructure.Repositories;
using SpadStorePanel.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpadStorePanel.Web.Controllers
{
    public class AboutUsController : Controller
    {
        private readonly StaticContentDetailsRepository _staticContentDetailsRepository;
        private readonly OurTeamRepository _ourTeamRepository;
        private readonly CertificatesRepository _certificatesRepository; 

        public AboutUsController(CertificatesRepository certificatesRepository, OurTeamRepository ourTeamRepository,StaticContentDetailsRepository staticContentDetailsRepository)
        {
            _staticContentDetailsRepository = staticContentDetailsRepository;
            _certificatesRepository = certificatesRepository;
            _ourTeamRepository = ourTeamRepository;
        }
        // GET: AboutUs
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult History()
        {
           var Histoty= _staticContentDetailsRepository.GetAll().Where(a => a.Id == 16 && a.IsDeleted==false).FirstOrDefault();
            return View(Histoty);
        }
        public ActionResult Image()
        {
            //var Image = _staticContentDetailsRepository.Get(17).Image;
            ViewBag.Image = _staticContentDetailsRepository.Get(17).Image;
            return View();
        }
        public ActionResult Text()
        {
           var Text= _staticContentDetailsRepository.GetAll().Where(a => a.StaticContentTypeId==11 && a.IsDeleted == false).ToList();
            return View(Text);
        }
        public ActionResult Statistics()
        {
            StatictisModel statictisModel = new StatictisModel();
            statictisModel.Satisfaction = _staticContentDetailsRepository.Get(21).Title;
            statictisModel.Exprence = _staticContentDetailsRepository.Get(22).Title;
            statictisModel.Reaction = _staticContentDetailsRepository.Get(23).Title;
            statictisModel.awards = _staticContentDetailsRepository.Get(24).Title;
            return View(statictisModel);
 
        }
        public ActionResult OurTeam()
        {
            var OrTeam = _ourTeamRepository.GetAll().Where(a => a.IsDeleted == false).ToList();

            return View(OrTeam);
        }
        public ActionResult CooperationList()
        {
            var CooperationList = _certificatesRepository.GetAll().Where(a => a.IsDeleted == false).ToList();
            return View(CooperationList);
        }
    }
}