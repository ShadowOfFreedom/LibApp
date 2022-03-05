using AutoMapper;
using LibApp.Dtos;
using LibApp.Models;
using LibApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using HttpDeleteAttribute = Microsoft.AspNetCore.Mvc.HttpDeleteAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace LibApp.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase {
        private readonly IRepositoryAsync<Customer> _customersRepo;
        private readonly IMapper _mapper;

        public CustomersController(IRepositoryAsync<Customer> customersRepo, IMapper mapper) {
            _customersRepo = customersRepo;
            _mapper = mapper;
        }

        // GET /api/customers
        [HttpGet]
        [Authorize(Roles = "StoreManager")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetCustomers() {
            var customers = await _customersRepo.GetAll();
            return Ok(customers.Select(_mapper.Map<Customer, CustomerDto>));
        }


        // GET /api/customers/{id}
        [HttpGet("{id}", Name = "GetCustomer")]
        [Authorize(Roles = "StoreManager")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetCustomer(int id) {
            var customer = await _customersRepo.GetById(id);

            if (customer == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            return Ok(_mapper.Map<CustomerDto>(customer));
        }

        // POST /api/customers
        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> CreateCustomer(CustomerDto customerDto) {
            if (!ModelState.IsValid)
                return BadRequest();

            var customer = _mapper.Map<Customer>(customerDto);
            await _customersRepo.Add(customer);
            customerDto.Id = customer.Id;

            return CreatedAtRoute(nameof(GetCustomer), new {id = customerDto.Id}, customerDto);
        }

        // PUT /api/customers
        [HttpPut("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UpdateCustomer(int id, CustomerDto customerDto) {
            if (!ModelState.IsValid)
                throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest);
            
            var customerInDb = await _customersRepo.GetById(id);
            if (customerInDb == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            _mapper.Map(customerDto, customerInDb);
            await _customersRepo.Update(customerInDb);

            return CreatedAtRoute(nameof(GetCustomer), new {id = customerDto.Id}, customerDto);
        }

        // DELETE /api/customers
        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteCustomer(int id) {
            var customerInDb = await _customersRepo.GetById(id);
            if (customerInDb == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            
            await _customersRepo.Delete(id);
            return Ok(_mapper.Map<CustomerDto>(customerInDb));
        }
    }
}