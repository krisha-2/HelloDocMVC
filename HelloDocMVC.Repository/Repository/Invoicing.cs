using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Repository.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository
{
    public class Invoicing : IInvoicing
    {
        private readonly HelloDocDbContext _context;
        public Invoicing(HelloDocDbContext context, EmailConfiguration emailConfig)
        {
            _context = context;
        }
    }
}
