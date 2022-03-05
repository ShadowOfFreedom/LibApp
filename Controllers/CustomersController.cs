using LibApp.Models;
using LibApp.Repositories.Interfaces;
using LibApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace LibApp.Controllers {
    public class CustomersController : Controller {
        private readonly IRepositoryAsync<Customer> _customersRepo;
        private readonly IRepositoryAsync<MembershipType> _membershipTypesRepo;

        public CustomersController(IRepositoryAsync<Customer> customersRepo,
            IRepositoryAsync<MembershipType> membershipTypesRepo) {
            _customersRepo = customersRepo;
            _membershipTypesRepo = membershipTypesRepo;
        }

        [Authorize(Roles = "StoreManager")]
        [Authorize(Roles = "Owner")]
        public async Task<ViewResult> Index() => View(await _customersRepo.GetAll());


        [Authorize(Roles = "StoreManager")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Details(int id) {
            var customer = await _customersRepo.GetById(id);

            if (customer == null)
                return Content("User not found");

            return View(customer);
        }

        
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> New() {
            var viewModel = new CustomerFormViewModel() {
                MembershipTypes = await _membershipTypesRepo.GetAll()
            };

            return View("CustomerForm", viewModel);
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Edit(int id) {
            var customer = await _customersRepo.GetById(id);
            if (customer == null)
                return NotFound();

            var viewModel = new CustomerFormViewModel(customer) {
                MembershipTypes = await _membershipTypesRepo.GetAll()
            };

            return View("CustomerForm", viewModel);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Save(Customer customer) {
            if (!ModelState.IsValid) {
                var viewModel = new CustomerFormViewModel(customer) {
                    MembershipTypes = await _membershipTypesRepo.GetAll()
                };

                return View("CustomerForm", viewModel);
            }

            try {
                if (customer.Id == 0)
                    await _customersRepo.Add(customer);
                else {
                    var customerInDb = await _customersRepo.GetById(customer.Id);
                    customerInDb.Name = customer.Name;
                    customerInDb.Birthdate = customer.Birthdate;
                    customerInDb.MembershipTypeId = customer.MembershipTypeId;
                    customerInDb.HasNewsletterSubscribed = customer.HasNewsletterSubscribed;
                }
            }
            catch (DbUpdateException e) {
                Console.WriteLine(e);
            }

            return RedirectToAction("Index", "Customers");
        }

        public ViewResult Login() => View("Login");
    }
}