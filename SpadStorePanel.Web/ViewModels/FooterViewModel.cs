using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpadStorePanel.Web.ViewModels
{
    public class FooterViewModel
    {
        public FooterViewModel(SocialViewModel socialViewModel)
        {
            _SocialLinks = socialViewModel;   
        }
        public FooterViewModel()
        {
        
        }
        public SocialViewModel _SocialLinks { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
        public string AccountNumber { get; set; }

        public string Map { get; set; }
        public string WorkTime { get; set; }
        public string ConstText { get; set; }


    }
}