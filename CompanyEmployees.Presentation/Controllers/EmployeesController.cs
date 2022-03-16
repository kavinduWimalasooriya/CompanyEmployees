using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController :ControllerBase
    {
        private readonly IServiceManager _service;
        public EmployeesController(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeesFromCompany(Guid companyId, [FromQuery]EmployeeParameters employeeParameters)
        {
            var employees = await _service.EmployeeService.GetEmployeesAsync(companyId,employeeParameters, trackChanges:false);
            return Ok(employees); 
        }

        [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeFromCompany(Guid companyId,Guid id)
        {
            var employee = await _service.EmployeeService.GetEmployeeAsync(companyId, id, trackChanges:false);
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
        {
            if (employee is null) return BadRequest("EmployeeForCreationDto object is null");
            if (!ModelState.IsValid) return UnprocessableEntity(ModelState);
            var employeeToReturn = await _service.EmployeeService.CreateEmployeeForCompanyAsync(companyId, employee, trackChanges: false);
            return CreatedAtRoute("GetEmployeeForCompany", new { companyId,Id = employeeToReturn.Id},employeeToReturn);
        }
        [HttpDelete("{Id:guid}")]
        public async Task<IActionResult> DeleteEmployee(Guid companyId,Guid id)
        {
            await _service.EmployeeService.DeleteEmployeeForCompanyAsync(companyId,id,trackChanges:false);
            return NoContent();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId,Guid id,[FromBody]EmployeeForUpdateDto employee)
        {
            if (employee is null) return BadRequest("EmployeeForUpdateDto object is null");
            await _service.EmployeeService.UpdateEmployeeForCompanyAsync(companyId,id,employee,compTrackChanges:false,empTrackChanges:true);
            return NoContent();
        }
    }
}
